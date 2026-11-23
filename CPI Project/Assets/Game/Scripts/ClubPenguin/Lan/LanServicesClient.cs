#if !UNITY_WEBGL
using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ClubPenguin.Net.Lan
{
	public static class LanServicesClient
	{
		private sealed class Link
		{
			public readonly string Address;

			public readonly string Swid;

			public readonly System.Collections.Concurrent.ConcurrentQueue<string> Incoming = new System.Collections.Concurrent.ConcurrentQueue<string>();

			public readonly BlockingCollection<string> Outgoing = new BlockingCollection<string>();

			public volatile bool Closed;

			public bool Welcomed;

			public bool Synced;

			public TcpClient Tcp;

			public Link(string address, string swid)
			{
				Address = address;
				Swid = swid;
			}

			public void Close()
			{
				if (Closed)
				{
					return;
				}
				Closed = true;
				Outgoing.CompleteAdding();
				try
				{
					if (Tcp != null)
					{
						Tcp.Close();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private sealed class Pending
		{
			public Action<string> Success;

			public Action<string> Failure;

			public float Deadline;
		}

		private const int Port = 9936;

		private const int MaxLineBytes = 4194304;

		private const float RequestTimeoutSeconds = 10f;

		private const float LayoutCheckSeconds = 5f;

		private const float RetrySeconds = 3f;

		private static readonly object owner = new object();

		private static readonly Dictionary<long, Pending> pending = new Dictionary<long, Pending>();

		private sealed class FriendsTransport : ILanFriendsTransport
		{
			public void Find(string name, Action<string> done)
			{
				Dictionary<string, object> args = new Dictionary<string, object>();
				args["name"] = name;
				SendRequest("find", args, delegate(string json)
				{
					object parsed;
					string found = LanJson.TryParse(json, out parsed) ? (LanJson.Get(parsed, "DisplayName") as string) : null;
					done(found);
				}, delegate
				{
					done(null);
				});
			}

			public void Invite(string name, Action<LanFriendsData.Entry?, string> done)
			{
				Dictionary<string, object> args = new Dictionary<string, object>();
				args["name"] = name;
				SendRequest("invite", args, delegate(string json)
				{
					object parsed;
					long id;
					if (!LanJson.TryParse(json, out parsed) || !LanJson.TryLong(LanJson.Path(parsed, "Notification", "Invitation", "FriendshipInvitationId"), out id))
					{
						done(null, "FAILED");
						return;
					}
					LanFriendsData.Entry entry = new LanFriendsData.Entry
					{
						Id = id,
						Name = (LanJson.Path(parsed, "Notification", "Friend", "DisplayName") as string) ?? name,
						Swid = LanJson.Path(parsed, "Notification", "Friend", "FirstName") as string
					};
					done(entry, null);
				}, delegate(string status)
				{
					done(null, status);
				});
			}

			public void Accept(long id, Action<bool> done)
			{
				Dictionary<string, object> args = new Dictionary<string, object>();
				args["id"] = id;
				SendRequest("accept", args, delegate
				{
					done(true);
				}, delegate
				{
					done(false);
				});
			}

			public void Reject(long id, Action<bool> done)
			{
				Dictionary<string, object> args = new Dictionary<string, object>();
				args["id"] = id;
				SendRequest("reject", args, delegate
				{
					done(true);
				}, delegate
				{
					done(false);
				});
			}

			public void Unfriend(string swid, Action<bool> done)
			{
				Dictionary<string, object> args = new Dictionary<string, object>();
				args["swid"] = swid;
				SendRequest("unfriend", args, delegate
				{
					done(true);
				}, delegate
				{
					done(false);
				});
			}
		}

		private const float FriendsCheckSeconds = 1f;

		private static readonly FriendsTransport transport = new FriendsTransport();

		private static readonly List<KeyValuePair<string, LanFriendsData.Entry>> heldNotes = new List<KeyValuePair<string, LanFriendsData.Entry>>();

		private static float nextFriendsCheck;

		private static Link link;

		private static bool running;

		private static long nextRequestId;

		private static string sentLayout;

		private static string sentVisibility;

		private static string sentIglooName;

		private static float nextLayoutCheck;

		private static float nextConnect;

		public static void Ensure()
		{
			if (running)
			{
				return;
			}
			running = true;
			CoroutineRunner.StartPersistent(run(), owner, "LanServices");
		}

		private static bool IsWelcomed()
		{
			return link != null && link.Welcomed && !link.Closed && LanGameServerHost.Address != null;
		}

		private static IEnumerator run()
		{
			while (true)
			{
				try
				{
					Tick();
				}
				catch (Exception)
				{
				}
				yield return null;
			}
		}

		private static void Tick()
		{
			string address = LanGameServerHost.Address;
			OfflineDatabase database = Service.IsSet<OfflineDatabase>() ? Service.Get<OfflineDatabase>() : null;
			string swid = (database != null) ? database.AccessToken : null;
			if (link != null && (link.Closed || link.Address != address || link.Swid != swid))
			{
				Teardown();
			}
			if (link == null)
			{
				if (address != null && !string.IsNullOrEmpty(swid) && Time.realtimeSinceStartup >= nextConnect)
				{
					nextConnect = Time.realtimeSinceStartup + RetrySeconds;
					Open(address, swid, database);
				}
				return;
			}
			string line;
			while (link != null && link.Incoming.TryDequeue(out line))
			{
				Handle(line);
			}
			if (link == null || !link.Welcomed)
			{
				return;
			}
			if (!link.Synced)
			{
				link.Synced = true;
				LanFriends.Transport = transport;
				LanCatalog.Send = SendCatalog;
				LanCfc.Send = SendCatalog;
				Write(SyncMessage(LanFriends.Load(link.Swid)));
				if (LanServer.IsLoopback(link.Address))
				{
					Dictionary<string, object> seed = new Dictionary<string, object>();
					seed["total"] = LanCfc.DeviceTotal();
					SendCatalog("cfc_seed", seed);
				}
			}
			if (heldNotes.Count > 0)
			{
				List<KeyValuePair<string, LanFriendsData.Entry>> notes = new List<KeyValuePair<string, LanFriendsData.Entry>>(heldNotes);
				heldNotes.Clear();
				foreach (KeyValuePair<string, LanFriendsData.Entry> note in notes)
				{
					if (!LanFriends.Deliver(link.Swid, note.Key, note.Value))
					{
						heldNotes.Add(note);
					}
				}
			}
			if (Time.realtimeSinceStartup >= nextFriendsCheck)
			{
				nextFriendsCheck = Time.realtimeSinceStartup + FriendsCheckSeconds;
				List<string> swids = new List<string>();
				foreach (LanFriendsData.Entry friend in LanFriends.Load(link.Swid).Friends)
				{
					swids.Add(friend.Swid);
				}
				LanDirectory.SetFriends(swids);
				CacheFriendProfiles(LanDirectory.KnownEntries());
			}
			if (Time.realtimeSinceStartup >= nextLayoutCheck)
			{
				nextLayoutCheck = Time.realtimeSinceStartup + LayoutCheckSeconds;
				SendLayout(database);
			}
			ExpireRequests();
		}

		private static void Open(string address, string swid, OfflineDatabase database)
		{
			Link opened = new Link(address, swid);
			link = opened;
			sentLayout = null;
			sentVisibility = null;
			sentIglooName = null;
			nextLayoutCheck = 0f;
			ZoneId igloo = OwnIgloo(database);
			Dictionary<string, object> iglooJson = new Dictionary<string, object>();
			iglooJson["name"] = igloo.name;
			iglooJson["instanceId"] = igloo.instanceId;
			Dictionary<string, object> hello = new Dictionary<string, object>();
			hello["cmd"] = "hello";
			hello["swid"] = swid;
			RegistrationProfile registration = database.Read<RegistrationProfile>();
			string name = !string.IsNullOrEmpty(registration.userName) ? registration.userName : (!string.IsNullOrEmpty(registration.displayName) ? registration.displayName : swid);
			hello["name"] = name;
			hello["igloo"] = iglooJson;
			hello["visibility"] = Visibility(database);
			opened.Outgoing.Add(LanJson.Serialize(hello));
			Thread thread = new Thread((ThreadStart)delegate
			{
				Pump(opened);
			});
			thread.IsBackground = true;
			thread.Start();
		}

		private static void Pump(Link l)
		{
			try
			{
				TcpClient tcp = new TcpClient();
				l.Tcp = tcp;
				tcp.NoDelay = true;
				tcp.Connect(l.Address, Port);
				NetworkStream stream = tcp.GetStream();
				Thread writer = new Thread((ThreadStart)delegate
				{
					try
					{
						foreach (string item in l.Outgoing.GetConsumingEnumerable())
						{
							byte[] bytes = Encoding.UTF8.GetBytes(item + "\n");
							stream.Write(bytes, 0, bytes.Length);
						}
					}
					catch (Exception)
					{
					}
					l.Close();
				});
				writer.IsBackground = true;
				writer.Start();
				MemoryStream buffer = new MemoryStream();
				byte[] chunk = new byte[16384];
				while (!l.Closed)
				{
					int read = stream.Read(chunk, 0, chunk.Length);
					if (read <= 0)
					{
						break;
					}
					int start = 0;
					for (int i = 0; i < read; i++)
					{
						if (chunk[i] == 10)
						{
							buffer.Write(chunk, start, i - start);
							l.Incoming.Enqueue(Encoding.UTF8.GetString(buffer.GetBuffer(), 0, (int)buffer.Length));
							buffer.SetLength(0);
							start = i + 1;
						}
					}
					buffer.Write(chunk, start, read - start);
					if (buffer.Length > MaxLineBytes)
					{
						break;
					}
				}
			}
			catch (Exception)
			{
			}
			l.Close();
		}

		private static void Teardown()
		{
			Link closing = link;
			link = null;
			if (closing != null)
			{
				closing.Close();
			}
			heldNotes.Clear();
			LanFriends.Transport = null;
			LanCatalog.Reset();
			LanCfc.Reset();
			LanDirectory.SetConnected(false, null);
			List<Pending> failed = new List<Pending>(pending.Values);
			pending.Clear();
			foreach (Pending p in failed)
			{
				p.Failure("LAN_DISCONNECTED");
			}
		}

		private static void Write(string line)
		{
			if (link != null && !link.Closed)
			{
				try
				{
					link.Outgoing.Add(line);
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		private static void Handle(string line)
		{
			object msg;
			if (!LanJson.TryParse(line, out msg))
			{
				return;
			}
			switch (LanJson.Get(msg, "cmd") as string)
			{
			case "welcome":
				link.Welcomed = true;
				LanDirectory.SetConnected(true, link.Swid);
				break;
			case "dir":
				HandleDirectory(msg);
				break;
			case "layout":
				LanDirectory.SetLayout(LanJson.Get(msg, "swid") as string, LanJson.Get(msg, "json") as string);
				break;
			case "note":
				HandleNote(LanJson.Get(msg, "kind") as string, LanJson.Get(msg, "json") as string);
				break;
			case "res":
				HandleResult(msg);
				break;
			case "cat_schedule":
				HandleCatalogSchedule(msg);
				break;
			case "cat_item":
				HandleCatalogItem(msg);
				break;
			case "cat_sold":
			{
				long id;
				long sold;
				if (LanJson.TryLong(LanJson.Get(msg, "id"), out id) && LanJson.TryLong(LanJson.Get(msg, "sold"), out sold))
				{
					LanCatalog.SetSold(id, sold);
				}
				break;
			}
			case "cat_ready":
				LanCatalog.SetReady();
				break;
			case "cfc_total":
			{
				long total;
				if (LanJson.TryLong(LanJson.Get(msg, "total"), out total))
				{
					LanCfc.SetTotal(total, LanServer.IsLoopback(link.Address));
					PushCfcTotal();
				}
				break;
			}
			}
		}

		private static void PushCfcTotal()
		{
			if (!Service.IsSet<EventDispatcher>())
			{
				return;
			}
			OfflineDatabase database = Service.IsSet<OfflineDatabase>() ? Service.Get<OfflineDatabase>() : null;
			global::ClubPenguin.Net.Domain.ScheduledEvent.CFCDonations donations = new global::ClubPenguin.Net.Domain.ScheduledEvent.CFCDonations();
			donations.cfcTotal = LanCfc.Total;
			donations.contribution = (database != null) ? database.Read<CFCDonationData>().personalContribution : 0L;
			Service.Get<EventDispatcher>().DispatchEvent(new ScheduledEventServiceEvents.CFCDonationsLoaded(donations));
		}

		private static void SendCatalog(string command, Dictionary<string, object> args)
		{
			if (!IsWelcomed())
			{
				return;
			}
			Dictionary<string, object> m = new Dictionary<string, object>(args);
			m["cmd"] = command;
			Write(LanJson.Serialize(m));
		}

		private static void HandleCatalogSchedule(object msg)
		{
			Dictionary<string, object> days = LanJson.Get(msg, "days") as Dictionary<string, object>;
			if (days == null)
			{
				return;
			}
			foreach (KeyValuePair<string, object> kv in days)
			{
				int day;
				long id;
				if (int.TryParse(kv.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out day) && LanJson.TryLong(kv.Value, out id))
				{
					LanCatalog.SetScheduleDay(day, id);
				}
			}
		}

		private static void HandleCatalogItem(object msg)
		{
			long id;
			long challenge;
			long sold;
			long created;
			string json = LanJson.Get(msg, "json") as string;
			if (json == null || !LanJson.TryLong(LanJson.Get(msg, "id"), out id) || !LanJson.TryLong(LanJson.Get(msg, "challenge"), out challenge))
			{
				return;
			}
			LanJson.TryLong(LanJson.Get(msg, "sold"), out sold);
			LanJson.TryLong(LanJson.Get(msg, "created"), out created);
			LanCatalog.AddItem(id, challenge, LanJson.Get(msg, "swid") as string, sold, created, json);
		}

		private static void HandleDirectory(object msg)
		{
			List<object> players = LanJson.Get(msg, "players") as List<object>;
			List<LanDirectory.Entry> entries = new List<LanDirectory.Entry>();
			List<LanDirectory.Entry> botEntries = new List<LanDirectory.Entry>();
			List<object> botProfiles = LanJson.Get(msg, "botProfiles") as List<object>;
			int playerCount = (players != null) ? players.Count : 0;
			List<object> profiles = new List<object>();
			if (players != null)
			{
				profiles.AddRange(players);
			}
			if (botProfiles != null)
			{
				profiles.AddRange(botProfiles);
			}
			int index = 0;
			{
				foreach (object p in profiles)
				{
					bool isBot = index++ >= playerCount;
					LanDirectory.Entry e = new LanDirectory.Entry();
					e.Swid = LanJson.Get(p, "swid") as string;
					e.Name = LanJson.Get(p, "name") as string;
					if (e.Swid == null || e.Name == null)
					{
						continue;
					}
					e.Room = LanJson.Get(p, "room") as string;
					long session;
					if (long.TryParse(LanJson.Get(p, "session") as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out session))
					{
						e.SessionId = session;
					}
					e.OutfitJson = LanJson.Get(p, "outfit") as string;
					int colour;
					if (LanJson.TryInt(LanJson.Get(p, "colour"), out colour))
					{
						e.Colour = colour;
					}
					e.IglooName = LanJson.Get(p, "iglooName") as string;
					e.IglooInstance = LanJson.Get(p, "iglooInstance") as string;
					e.Visibility = LanJson.Get(p, "visibility") as string;
					int daysOld;
					if (LanJson.TryInt(LanJson.Get(p, "daysOld"), out daysOld))
					{
						e.DaysOld = daysOld;
					}
					Dictionary<string, object> xp = LanJson.Get(p, "mascotXP") as Dictionary<string, object>;
					if (xp != null)
					{
						e.MascotXP = new Dictionary<string, long>();
						foreach (KeyValuePair<string, object> kv in xp)
						{
							long value;
							if (LanJson.TryLong(kv.Value, out value))
							{
								e.MascotXP[kv.Key] = value;
							}
						}
					}
					if (isBot)
					{
						botEntries.Add(e);
					}
					else
					{
						entries.Add(e);
					}
				}
			}
			LanDirectory.SetBotEntries(botEntries);
			List<string> botRooms = new List<string>();
			List<object> bots = LanJson.Get(msg, "bots") as List<object>;
			if (bots != null)
			{
				foreach (object b in bots)
				{
					string room = b as string;
					if (!string.IsNullOrEmpty(room))
					{
						botRooms.Add(room);
					}
				}
			}
			LanDirectory.SetBotRooms(botRooms);
			LanDirectory.Replace(entries);
			CacheFriendProfiles(entries);
			RefreshFriends();
		}

		private static void CacheFriendProfiles(List<LanDirectory.Entry> entries)
		{
			if (link == null)
			{
				return;
			}
			LanFriendsData data = LanFriends.Load(link.Swid);
			HashSet<string> friends = new HashSet<string>();
			foreach (LanFriendsData.Entry friend in data.Friends)
			{
				friends.Add(friend.Swid);
			}
			bool changed = data.Profiles.RemoveAll((LanFriendsData.CachedProfile p) => !friends.Contains(p.Swid)) > 0;
			foreach (LanDirectory.Entry e in entries)
			{
				if (!friends.Contains(e.Swid))
				{
					continue;
				}
				int index = data.Profiles.FindIndex((LanFriendsData.CachedProfile p) => p.Swid == e.Swid);
				LanFriendsData.CachedProfile cached = (index >= 0) ? data.Profiles[index] : new LanFriendsData.CachedProfile
				{
					Swid = e.Swid
				};
				LanFriendsData.CachedProfile next = cached;
				next.Name = e.Name;
				if (e.OutfitJson != null)
				{
					next.OutfitJson = e.OutfitJson;
				}
				if (e.Colour.HasValue)
				{
					next.Colour = e.Colour;
				}
				if (e.DaysOld.HasValue)
				{
					next.DaysOld = e.DaysOld;
				}
				if (e.MascotXP != null)
				{
					next.MascotXP = e.MascotXP;
				}
				if (e.IglooInstance != null)
				{
					next.IglooName = e.IglooName;
					next.IglooInstance = e.IglooInstance;
					next.Visibility = e.Visibility;
				}
				if (index >= 0 && SameProfile(cached, next))
				{
					continue;
				}
				if (index >= 0)
				{
					data.Profiles[index] = next;
				}
				else
				{
					data.Profiles.Add(next);
				}
				changed = true;
			}
			if (changed)
			{
				LanFriends.Save(data, link.Swid);
			}
		}

		private static bool SameProfile(LanFriendsData.CachedProfile a, LanFriendsData.CachedProfile b)
		{
			if (a.Name != b.Name || a.OutfitJson != b.OutfitJson || a.Colour != b.Colour || a.DaysOld != b.DaysOld || a.IglooName != b.IglooName || a.IglooInstance != b.IglooInstance || a.Visibility != b.Visibility)
			{
				return false;
			}
			if (a.MascotXP == null || b.MascotXP == null)
			{
				return a.MascotXP == b.MascotXP;
			}
			if (a.MascotXP.Count != b.MascotXP.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, long> kv in a.MascotXP)
			{
				long value;
				if (!b.MascotXP.TryGetValue(kv.Key, out value) || value != kv.Value)
				{
					return false;
				}
			}
			return true;
		}

		private static void RefreshFriends()
		{
			if (!Service.IsSet<global::ClubPenguin.Core.CPDataEntityCollection>() || !Service.IsSet<global::ClubPenguin.UI.OtherPlayerDetailsRequestBatcher>())
			{
				return;
			}
			global::ClubPenguin.UI.OtherPlayerDetailsRequestBatcher batcher = Service.Get<global::ClubPenguin.UI.OtherPlayerDetailsRequestBatcher>();
			foreach (Disney.Kelowna.Common.DataModel.DataEntityHandle handle in global::ClubPenguin.FriendsDataModelService.FriendsList)
			{
				batcher.RequestOtherPlayerDetails(handle);
			}
		}

		private static void HandleNote(string kind, string json)
		{
			if (kind == null || json == null)
			{
				return;
			}
			object parsed;
			if (!LanJson.TryParse(json, out parsed))
			{
				return;
			}
			LanFriendsData.Entry entry = default(LanFriendsData.Entry);
			long id;
			switch (kind)
			{
			case "invitation":
				if (LanJson.TryLong(LanJson.Path(parsed, "Invitation", "FriendshipInvitationId"), out id))
				{
					entry.Id = id;
				}
				entry.Name = LanJson.Path(parsed, "Friend", "DisplayName") as string;
				entry.Swid = LanJson.Path(parsed, "Friend", "FirstName") as string;
				if (LanJson.Path(parsed, "Invitation", "IsInviter") is bool && (bool)LanJson.Path(parsed, "Invitation", "IsInviter"))
				{
					kind = "outgoing";
				}
				break;
			case "friend":
				if (LanJson.TryLong(LanJson.Get(parsed, "FriendshipInvitationId"), out id))
				{
					entry.Id = id;
				}
				entry.Name = LanJson.Path(parsed, "Friend", "DisplayName") as string;
				entry.Swid = LanJson.Path(parsed, "Friend", "UserId") as string;
				break;
			case "uninvitation":
				if (LanJson.TryLong(LanJson.Get(parsed, "InvitationId"), out id))
				{
					entry.Id = id;
				}
				break;
			case "unfriend":
				entry.Swid = LanJson.Get(parsed, "FriendUserId") as string;
				break;
			default:
				return;
			}
			if (link == null || !LanFriends.Deliver(link.Swid, kind, entry))
			{
				heldNotes.Add(new KeyValuePair<string, LanFriendsData.Entry>(kind, entry));
			}
		}

		private static string SyncMessage(LanFriendsData data)
		{
			List<object> friends = new List<object>();
			foreach (LanFriendsData.Entry e in data.Friends)
			{
				Dictionary<string, object> d = new Dictionary<string, object>();
				d["swid"] = e.Swid;
				d["name"] = e.Name;
				friends.Add(d);
			}
			Dictionary<string, object> m = new Dictionary<string, object>();
			m["cmd"] = "sync";
			m["friends"] = friends;
			m["out"] = SyncInvitations(data.Outgoing);
			m["in"] = SyncInvitations(data.Incoming);
			return LanJson.Serialize(m);
		}

		private static List<object> SyncInvitations(List<LanFriendsData.Entry> list)
		{
			List<object> result = new List<object>();
			foreach (LanFriendsData.Entry e in list)
			{
				Dictionary<string, object> d = new Dictionary<string, object>();
				d["id"] = e.Id;
				d["name"] = e.Name;
				d["swid"] = e.Swid;
				result.Add(d);
			}
			return result;
		}

		private static void HandleResult(object msg)
		{
			long rid;
			Pending p;
			if (!LanJson.TryLong(LanJson.Get(msg, "rid"), out rid) || !pending.TryGetValue(rid, out p))
			{
				return;
			}
			pending.Remove(rid);
			string json = LanJson.Get(msg, "json") as string;
			if (LanJson.Get(msg, "ok") is bool && (bool)LanJson.Get(msg, "ok") && json != null)
			{
				p.Success(json);
			}
			else
			{
				p.Failure((LanJson.Get(msg, "status") as string) ?? "FAILED");
			}
		}

		private static void SendRequest(string command, Dictionary<string, object> args, Action<string> success, Action<string> failure)
		{
			if (!IsWelcomed())
			{
				failure("LAN_UNAVAILABLE");
				return;
			}
			nextRequestId++;
			pending[nextRequestId] = new Pending
			{
				Success = success,
				Failure = failure,
				Deadline = Time.realtimeSinceStartup + RequestTimeoutSeconds
			};
			Dictionary<string, object> m = new Dictionary<string, object>(args);
			m["cmd"] = command;
			m["rid"] = nextRequestId;
			Write(LanJson.Serialize(m));
		}

		private static void ExpireRequests()
		{
			if (pending.Count == 0)
			{
				return;
			}
			float now = Time.realtimeSinceStartup;
			List<long> expired = new List<long>();
			foreach (KeyValuePair<long, Pending> item in pending)
			{
				if (item.Value.Deadline <= now)
				{
					expired.Add(item.Key);
				}
			}
			foreach (long id in expired)
			{
				Pending p = pending[id];
				pending.Remove(id);
				p.Failure("TIMEOUT");
			}
		}

		private static ZoneId OwnIgloo(OfflineDatabase database)
		{
			SceneLayout layout = GetActiveIglooLayoutOperation.ReadActiveLayout(database);
			return new ZoneId
			{
				name = (layout != null && !string.IsNullOrEmpty(layout.zoneId)) ? layout.zoneId : "DefaultIgloo",
				instanceId = database.AccessToken
			};
		}

		private static string Visibility(OfflineDatabase database)
		{
			IglooEntity igloo = database.Read<IglooEntity>();
			if (igloo.Data == null || !igloo.Data.visibility.HasValue)
			{
				return null;
			}
			return igloo.Data.visibility.Value.ToString();
		}

		private static void SendLayout(OfflineDatabase database)
		{
			if (database == null)
			{
				return;
			}
			SceneLayout layout = GetActiveIglooLayoutOperation.ReadActiveLayout(database);
			string json = (layout != null) ? Service.Get<JsonService>().Serialize(layout) : null;
			string visibility = Visibility(database);
			string iglooName = OwnIgloo(database).name;
			if (json == sentLayout && visibility == sentVisibility && iglooName == sentIglooName)
			{
				return;
			}
			sentLayout = json;
			sentVisibility = visibility;
			sentIglooName = iglooName;
			Dictionary<string, object> igloo = new Dictionary<string, object>();
			igloo["name"] = iglooName;
			Dictionary<string, object> m = new Dictionary<string, object>();
			m["cmd"] = "layout";
			m["json"] = json;
			m["visibility"] = visibility;
			m["igloo"] = igloo;
			Write(LanJson.Serialize(m));
		}
	}
}
#endif
