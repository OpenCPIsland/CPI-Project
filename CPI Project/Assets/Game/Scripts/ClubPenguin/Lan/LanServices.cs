#if !UNITY_WEBGL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanServiceConnection
	{
		public readonly Socket Socket;

		public string Swid;

		public string Name;

		public string IglooName;

		public string IglooInstance;

		public string IglooVisibility;

		public string Layout;

		public bool Synced;

		public readonly Dictionary<string, string> Friends = new Dictionary<string, string>();

		public readonly Dictionary<long, LanInvite> Outgoing = new Dictionary<long, LanInvite>();

		public readonly Dictionary<long, LanInvite> Incoming = new Dictionary<long, LanInvite>();

		private readonly BlockingCollection<string> outbox = new BlockingCollection<string>();

		private volatile bool closed;

		public LanServiceConnection(Socket socket)
		{
			Socket = socket;
		}

		public bool Closed
		{
			get
			{
				return closed;
			}
		}

		public void Send(string line)
		{
			if (!closed)
			{
				outbox.Add(line);
			}
		}

		public void Close()
		{
			if (closed)
			{
				return;
			}
			closed = true;
			outbox.CompleteAdding();
			try
			{
				Socket.Close();
			}
			catch (Exception)
			{
			}
		}

		public void WriteLoop()
		{
			try
			{
				using (NetworkStream stream = new NetworkStream(Socket, false))
				{
					foreach (string line in outbox.GetConsumingEnumerable())
					{
						byte[] bytes = Encoding.UTF8.GetBytes(line + "\n");
						stream.Write(bytes, 0, bytes.Length);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public sealed class LanInvite
	{
		public long Id;

		public string InviterSwid;

		public string InviterName;

		public string InviteeSwid;

		public string InviteeName;
	}

	public sealed class LanServices
	{
		public const int Port = 9936;

		private const int MaxLineBytes = 4 * 1024 * 1024;

		private const double DirectoryIntervalMs = 2000.0;

		private readonly LanServer server;

		private readonly List<LanServiceConnection> clients = new List<LanServiceConnection>();

		private TcpListener listener;

		private volatile bool running;

		private string lastDirectory = "";

		private long sequence;

		private readonly LanCatalogStore catalog;

		public LanServices(LanServer server, string catalogPath)
		{
			this.server = server;
			catalog = new LanCatalogStore(server, catalogPath);
		}

		public bool Start()
		{
			try
			{
				listener = new TcpListener(IPAddress.Any, Port);
				listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				listener.Start();
			}
			catch (Exception)
			{
				listener = null;
				return false;
			}
			running = true;
			Thread t = new Thread(AcceptLoop);
			t.IsBackground = true;
			t.Name = "CPI LAN Services Accept";
			t.Start();
			server.Loop.After(DirectoryIntervalMs, DirectoryTick);
			return true;
		}

		public void DisconnectRemote()
		{
			remoteClosed = true;
			foreach (LanServiceConnection c in clients.ToList())
			{
				IPEndPoint ep = null;
				try
				{
					ep = c.Socket.RemoteEndPoint as IPEndPoint;
				}
				catch (Exception)
				{
				}
				if (ep == null || !IPAddress.IsLoopback(ep.Address))
				{
					c.Close();
					clients.Remove(c);
				}
			}
		}

		private bool remoteClosed;

		public void Stop()
		{
			running = false;
			if (listener != null)
			{
				try
				{
					listener.Stop();
				}
				catch (Exception)
				{
				}
				listener = null;
			}
			foreach (LanServiceConnection c in clients.ToList())
			{
				c.Close();
			}
			clients.Clear();
			catalog.Save();
		}

		private void AcceptLoop()
		{
			TcpListener l = listener;
			while (running && l != null)
			{
				Socket socket;
				try
				{
					socket = l.AcceptSocket();
				}
				catch (Exception)
				{
					return;
				}
				socket.NoDelay = true;
				LanServiceConnection conn = new LanServiceConnection(socket);
				server.Loop.Post(delegate
				{
					if (!running)
					{
						conn.Close();
						return;
					}
					IPEndPoint remote = null;
					try
					{
						remote = conn.Socket.RemoteEndPoint as IPEndPoint;
					}
					catch (Exception)
					{
					}
					if (remoteClosed && (remote == null || !IPAddress.IsLoopback(remote.Address)))
					{
						conn.Close();
						return;
					}
					clients.Add(conn);
				});
				Thread writer = new Thread(conn.WriteLoop);
				writer.IsBackground = true;
				writer.Name = "CPI LAN Services Write";
				writer.Start();
				Thread reader = new Thread(ReadLoop);
				reader.IsBackground = true;
				reader.Name = "CPI LAN Services Read";
				reader.Start(conn);
			}
		}

		private void ReadLoop(object state)
		{
			LanServiceConnection conn = (LanServiceConnection)state;
			MemoryStream line = new MemoryStream();
			byte[] buffer = new byte[16384];
			try
			{
				using (NetworkStream stream = new NetworkStream(conn.Socket, false))
				{
					while (running && !conn.Closed)
					{
						int n = stream.Read(buffer, 0, buffer.Length);
						if (n <= 0)
						{
							break;
						}
						for (int i = 0; i < n; i++)
						{
							if (buffer[i] == (byte)'\n')
							{
								string text = Encoding.UTF8.GetString(line.GetBuffer(), 0, (int)line.Length);
								line.SetLength(0);
								server.Loop.Post(delegate
								{
									Handle(conn, text);
								});
							}
							else
							{
								line.WriteByte(buffer[i]);
								if (line.Length > MaxLineBytes)
								{
									throw new IOException();
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
			server.Loop.Post(delegate
			{
				Closed(conn);
			});
		}

		private void Closed(LanServiceConnection conn)
		{
			conn.Close();
			clients.Remove(conn);
			if (conn.Swid != null && conn.Layout != null && !clients.Any((LanServiceConnection c) => c.Swid == conn.Swid))
			{
				Broadcast(Message("layout", "swid", conn.Swid, "json", null), null);
			}
		}

		private void Handle(LanServiceConnection conn, string text)
		{
			object msg;
			if (conn.Closed || !LanJson.TryParse(text, out msg) || !(msg is Dictionary<string, object>))
			{
				return;
			}
			string cmd = LanJson.Get(msg, "cmd") as string;
			if (cmd == null)
			{
				return;
			}
			if (cmd == "hello")
			{
				HandleHello(conn, msg);
				return;
			}
			if (conn.Swid == null)
			{
				return;
			}
			long rid;
			LanJson.TryLong(LanJson.Get(msg, "rid"), out rid);
			switch (cmd)
			{
			case "layout":
				HandleLayout(conn, msg);
				break;
			case "sync":
				HandleSync(conn, msg);
				break;
			case "find":
				HandleFind(conn, rid, msg);
				break;
			case "invite":
				HandleInvite(conn, rid, msg);
				break;
			case "accept":
				HandleAccept(conn, rid, msg);
				break;
			case "reject":
				HandleReject(conn, rid, msg);
				break;
			case "unfriend":
				HandleUnfriend(conn, rid, msg);
				break;
			case "cat_theme":
			case "cat_submit":
			case "cat_buy":
				catalog.Handle(conn, cmd, msg, delegate(string line)
				{
					Broadcast(line, null);
				});
				break;
			}
		}

		private static string Text(object msg, string key, int maxBytes)
		{
			string s = LanJson.Get(msg, key) as string;
			if (s == null || Encoding.UTF8.GetByteCount(s) > maxBytes)
			{
				return null;
			}
			return s;
		}

		private void HandleHello(LanServiceConnection conn, object msg)
		{
			string swid = Text(msg, "swid", 128);
			string name = Text(msg, "name", 255);
			if (string.IsNullOrEmpty(swid) || string.IsNullOrEmpty(name) || conn.Swid != null)
			{
				return;
			}
			foreach (LanServiceConnection other in clients.Where((LanServiceConnection c) => c != conn && c.Swid == swid).ToList())
			{
				other.Close();
				clients.Remove(other);
			}
			conn.Swid = swid;
			conn.Name = name;
			conn.IglooName = Text(LanJson.Get(msg, "igloo"), "name", 255);
			conn.IglooInstance = Text(LanJson.Get(msg, "igloo"), "instanceId", 255);
			conn.IglooVisibility = Text(msg, "visibility", 64);
			conn.Send(Message("welcome"));
			lastDirectory = "";
			SendDirectory(conn);
			foreach (LanServiceConnection other in clients)
			{
				if (other != conn && other.Swid != null && other.Layout != null)
				{
					conn.Send(Message("layout", "swid", other.Swid, "json", other.Layout));
				}
			}
			catalog.SendState(conn);
		}

		private void HandleLayout(LanServiceConnection conn, object msg)
		{
			string json = Text(msg, "json", MaxLineBytes);
			string visibility = Text(msg, "visibility", 64);
			if (visibility != null)
			{
				conn.IglooVisibility = visibility;
			}
			string iglooName = Text(LanJson.Get(msg, "igloo"), "name", 255);
			if (iglooName != null)
			{
				conn.IglooName = iglooName;
			}
			if (json == null || json == conn.Layout)
			{
				return;
			}
			conn.Layout = json;
			Broadcast(Message("layout", "swid", conn.Swid, "json", json), conn);
		}

		private void DirectoryTick()
		{
			if (!running)
			{
				return;
			}
			string dir = DirectoryJson();
			if (dir != lastDirectory)
			{
				lastDirectory = dir;
				foreach (LanServiceConnection c in clients)
				{
					if (c.Swid != null)
					{
						c.Send(dir);
					}
				}
			}
			server.Loop.After(DirectoryIntervalMs, DirectoryTick);
		}

		private void SendDirectory(LanServiceConnection conn)
		{
			conn.Send(DirectoryJson());
		}

		private string DirectoryJson()
		{
			List<object> list = new List<object>();
			HashSet<string> seen = new HashSet<string>();
			foreach (LanServiceConnection c in clients.OrderBy((LanServiceConnection x) => x.Swid, StringComparer.Ordinal))
			{
				if (c.Swid == null || !seen.Add(c.Swid))
				{
					continue;
				}
				LanPlayer p = server.BySwid(c.Swid);
				Dictionary<string, object> e = new Dictionary<string, object>();
				e["swid"] = c.Swid;
				e["name"] = (p != null && p.UserName != "guest") ? p.UserName : c.Name;
				e["room"] = (p != null && p.RoomName.Split(':').Length >= 5) ? p.RoomName : null;
				e["session"] = (p != null) ? p.PlayerSessionId : null;
				e["outfit"] = (p != null) ? p.OutfitJson : null;
				e["colour"] = (p != null && p.Colour.HasValue) ? (object)p.Colour.Value : null;
				e["iglooName"] = c.IglooName;
				e["iglooInstance"] = c.IglooInstance;
				e["visibility"] = c.IglooVisibility;
				e["daysOld"] = (p != null && p.DaysOld.HasValue) ? (object)p.DaysOld.Value : null;
				e["mascotXP"] = (p != null && p.MascotXP != null) ? p.MascotXP.ToDictionary((KeyValuePair<string, long> kv) => kv.Key, (KeyValuePair<string, long> kv) => (object)kv.Value) : null;
				list.Add(e);
			}
			List<object> bots = server.All().Where((LanPlayer b) => b.IsBot && b.Registered && b.RoomName != null && b.RoomName.Split(':').Length >= 5).Select((LanPlayer b) => (object)b.RoomName).ToList();
			List<object> botProfiles = new List<object>();
			foreach (LanPlayer b in server.All().Where((LanPlayer x) => x.IsBot && x.Registered && x.Swid != null))
			{
				Dictionary<string, object> e = new Dictionary<string, object>();
				e["swid"] = b.Swid;
				e["name"] = b.UserName;
				e["room"] = (b.RoomName != null && b.RoomName.Split(':').Length >= 5) ? b.RoomName : null;
				e["session"] = b.PlayerSessionId;
				e["outfit"] = b.OutfitJson;
				e["colour"] = b.Colour.HasValue ? (object)b.Colour.Value : null;
				e["daysOld"] = b.DaysOld.HasValue ? (object)b.DaysOld.Value : null;
				e["mascotXP"] = (b.MascotXP != null) ? b.MascotXP.ToDictionary((KeyValuePair<string, long> kv) => kv.Key, (KeyValuePair<string, long> kv) => (object)kv.Value) : null;
				botProfiles.Add(e);
			}
			return Message("dir", "players", list, "bots", bots, "botProfiles", botProfiles);
		}

		internal static string Message(string cmd, params object[] pairs)
		{
			Dictionary<string, object> m = new Dictionary<string, object>();
			m["cmd"] = cmd;
			for (int i = 0; i + 1 < pairs.Length; i += 2)
			{
				m[(string)pairs[i]] = pairs[i + 1];
			}
			return LanJson.Serialize(m);
		}

		private void Broadcast(string line, LanServiceConnection except)
		{
			foreach (LanServiceConnection c in clients)
			{
				if (c != except && c.Swid != null)
				{
					c.Send(line);
				}
			}
		}

		private void Reply(LanServiceConnection conn, long rid, bool ok, object json, string status)
		{
			conn.Send(Message("res", "rid", rid, "ok", ok, "json", (json != null) ? LanJson.Serialize(json) : null, "status", status));
		}

		private void Note(LanServiceConnection conn, string kind, object notification)
		{
			conn.Send(Message("note", "kind", kind, "json", LanJson.Serialize(notification)));
		}

		private IEnumerable<LanServiceConnection> Online(string swid)
		{
			return clients.Where((LanServiceConnection c) => c.Swid == swid && !c.Closed);
		}

		private LanServiceConnection OnlineByName(string name, LanServiceConnection except)
		{
			return clients.FirstOrDefault((LanServiceConnection c) => c != except && c.Swid != null && c.Swid != except.Swid && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		private long NextSequence()
		{
			sequence++;
			return sequence;
		}

		private long NewInviteId()
		{
			long id;
			do
			{
				id = server.Random.Next(1, int.MaxValue);
			}
			while (clients.Any((LanServiceConnection c) => c.Outgoing.ContainsKey(id) || c.Incoming.ContainsKey(id)));
			return id;
		}

		private static Dictionary<string, object> User(string swid, string name)
		{
			Dictionary<string, object> u = new Dictionary<string, object>();
			u["UserId"] = swid;
			u["HashedUserId"] = swid;
			u["DisplayName"] = name;
			u["FirstName"] = "";
			u["Status"] = "ACTIVE";
			return u;
		}

		private Dictionary<string, object> InvitationNote(long id, string otherSwid, string otherName, bool isInviter)
		{
			Dictionary<string, object> inv = new Dictionary<string, object>();
			inv["FriendshipInvitationId"] = id;
			inv["FriendDisplayName"] = otherName;
			inv["IsInviter"] = isInviter;
			inv["IsTrusted"] = false;
			Dictionary<string, object> friend = User(null, otherName);
			friend["FirstName"] = otherSwid;
			Dictionary<string, object> n = new Dictionary<string, object>();
			n["SequenceNumber"] = NextSequence();
			n["Invitation"] = inv;
			n["Friend"] = friend;
			return n;
		}

		private Dictionary<string, object> FriendNote(long id, string otherSwid, string otherName)
		{
			Dictionary<string, object> n = new Dictionary<string, object>();
			n["SequenceNumber"] = NextSequence();
			n["Friend"] = User(otherSwid, otherName);
			n["FriendshipInvitationId"] = id;
			n["IsTrusted"] = false;
			return n;
		}

		private Dictionary<string, object> RemoveInvitationNote(long id)
		{
			Dictionary<string, object> n = new Dictionary<string, object>();
			n["SequenceNumber"] = NextSequence();
			n["InvitationId"] = id;
			return n;
		}

		private Dictionary<string, object> RemoveFriendNote(string otherSwid)
		{
			Dictionary<string, object> n = new Dictionary<string, object>();
			n["SequenceNumber"] = NextSequence();
			n["FriendUserId"] = otherSwid;
			return n;
		}

		private static Dictionary<string, object> Wrap(object notification)
		{
			Dictionary<string, object> r = new Dictionary<string, object>();
			r["Status"] = "OK";
			r["Notification"] = notification;
			return r;
		}

		private void HandleFind(LanServiceConnection conn, long rid, object msg)
		{
			string name = Text(msg, "name", 255);
			LanServiceConnection target = (name != null) ? OnlineByName(name, conn) : null;
			if (target == null)
			{
				Reply(conn, rid, false, null, "NOT_FOUND");
				return;
			}
			Dictionary<string, object> r = new Dictionary<string, object>();
			r["Status"] = "OK";
			r["DisplayName"] = target.Name;
			r["FirstName"] = "";
			Reply(conn, rid, true, r, null);
		}

		private void HandleInvite(LanServiceConnection conn, long rid, object msg)
		{
			string name = Text(msg, "name", 255);
			LanServiceConnection target = (name != null) ? OnlineByName(name, conn) : null;
			if (target == null)
			{
				Reply(conn, rid, false, null, "NOT_FOUND");
				return;
			}
			if (conn.Friends.ContainsKey(target.Swid))
			{
				Reply(conn, rid, false, null, "ALREADY_FRIENDS");
				return;
			}
			if (conn.Outgoing.Values.Any((LanInvite i) => i.InviteeSwid == target.Swid))
			{
				Reply(conn, rid, false, null, "INVITATION_ALREADY_EXISTS");
				return;
			}
			LanInvite reverse = conn.Incoming.Values.FirstOrDefault((LanInvite i) => i.InviterSwid == target.Swid);
			if (reverse != null)
			{
				Reply(conn, rid, false, null, "INVITATION_ALREADY_EXISTS");
				return;
			}
			LanInvite invite = new LanInvite
			{
				Id = NewInviteId(),
				InviterSwid = conn.Swid,
				InviterName = conn.Name,
				InviteeSwid = target.Swid,
				InviteeName = target.Name
			};
			foreach (LanServiceConnection c in Online(conn.Swid))
			{
				c.Outgoing[invite.Id] = invite;
			}
			foreach (LanServiceConnection c in Online(target.Swid))
			{
				c.Incoming[invite.Id] = invite;
				Note(c, "invitation", InvitationNote(invite.Id, conn.Swid, conn.Name, false));
			}
			Reply(conn, rid, true, Wrap(InvitationNote(invite.Id, target.Swid, target.Name, true)), null);
		}

		private void HandleAccept(LanServiceConnection conn, long rid, object msg)
		{
			long id;
			LanInvite invite;
			if (!LanJson.TryLong(LanJson.Get(msg, "id"), out id) || !conn.Incoming.TryGetValue(id, out invite) || string.IsNullOrEmpty(invite.InviterSwid))
			{
				Reply(conn, rid, false, null, "NOT_FOUND");
				return;
			}
			foreach (LanServiceConnection c in Online(conn.Swid))
			{
				c.Incoming.Remove(id);
				c.Friends[invite.InviterSwid] = invite.InviterName;
			}
			foreach (LanServiceConnection c in Online(invite.InviterSwid))
			{
				c.Outgoing.Remove(id);
				c.Friends[conn.Swid] = conn.Name;
				Note(c, "friend", FriendNote(id, conn.Swid, conn.Name));
			}
			Reply(conn, rid, true, Wrap(FriendNote(id, invite.InviterSwid, invite.InviterName)), null);
		}

		private void HandleReject(LanServiceConnection conn, long rid, object msg)
		{
			long id;
			LanInvite invite;
			if (!LanJson.TryLong(LanJson.Get(msg, "id"), out id) || !conn.Incoming.TryGetValue(id, out invite))
			{
				Reply(conn, rid, false, null, "NOT_FOUND");
				return;
			}
			foreach (LanServiceConnection c in Online(conn.Swid))
			{
				c.Incoming.Remove(id);
			}
			if (!string.IsNullOrEmpty(invite.InviterSwid))
			{
				foreach (LanServiceConnection c in Online(invite.InviterSwid))
				{
					c.Outgoing.Remove(id);
					Note(c, "uninvitation", RemoveInvitationNote(id));
				}
			}
			Reply(conn, rid, true, Wrap(RemoveInvitationNote(id)), null);
		}

		private void HandleUnfriend(LanServiceConnection conn, long rid, object msg)
		{
			string swid = Text(msg, "swid", 128);
			if (string.IsNullOrEmpty(swid) || !conn.Friends.ContainsKey(swid))
			{
				Reply(conn, rid, false, null, "NOT_FOUND");
				return;
			}
			foreach (LanServiceConnection c in Online(conn.Swid))
			{
				c.Friends.Remove(swid);
			}
			foreach (LanServiceConnection c in Online(swid))
			{
				c.Friends.Remove(conn.Swid);
				Note(c, "unfriend", RemoveFriendNote(conn.Swid));
			}
			Reply(conn, rid, true, Wrap(RemoveFriendNote(swid)), null);
		}

		private void HandleSync(LanServiceConnection conn, object msg)
		{
			if (conn.Synced)
			{
				return;
			}
			conn.Synced = true;
			List<object> friends = LanJson.Get(msg, "friends") as List<object>;
			if (friends != null)
			{
				foreach (object f in friends.Take(1000))
				{
					string swid = Text(f, "swid", 128);
					string name = Text(f, "name", 255);
					if (!string.IsNullOrEmpty(swid))
					{
						conn.Friends[swid] = name ?? "";
					}
				}
			}
			ReadInvites(LanJson.Get(msg, "out") as List<object>, conn, true);
			ReadInvites(LanJson.Get(msg, "in") as List<object>, conn, false);
			foreach (LanServiceConnection other in clients.ToList())
			{
				if (other != conn && other.Synced && other.Swid != null && other.Swid != conn.Swid)
				{
					Reconcile(conn, other);
					Reconcile(other, conn);
				}
			}
		}

		private static void ReadInvites(List<object> list, LanServiceConnection conn, bool outgoing)
		{
			if (list == null)
			{
				return;
			}
			foreach (object o in list.Take(1000))
			{
				long id;
				string name = Text(o, "name", 255);
				string swid = Text(o, "swid", 128);
				if (!LanJson.TryLong(LanJson.Get(o, "id"), out id) || string.IsNullOrEmpty(name))
				{
					continue;
				}
				LanInvite invite = outgoing ? new LanInvite
				{
					Id = id,
					InviterSwid = conn.Swid,
					InviterName = conn.Name,
					InviteeSwid = swid,
					InviteeName = name
				} : new LanInvite
				{
					Id = id,
					InviterSwid = swid,
					InviterName = name,
					InviteeSwid = conn.Swid,
					InviteeName = conn.Name
				};
				if (outgoing)
				{
					conn.Outgoing[id] = invite;
				}
				else
				{
					conn.Incoming[id] = invite;
				}
			}
		}

		private static bool Same(string name, string swid, LanServiceConnection other)
		{
			if (!string.IsNullOrEmpty(swid))
			{
				return swid == other.Swid;
			}
			return string.Equals(name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		private void Reconcile(LanServiceConnection a, LanServiceConnection b)
		{
			foreach (LanInvite invite in a.Outgoing.Values.ToList())
			{
				if (!Same(invite.InviteeName, invite.InviteeSwid, b) || b.Incoming.ContainsKey(invite.Id))
				{
					continue;
				}
				a.Outgoing.Remove(invite.Id);
				if (b.Friends.ContainsKey(a.Swid))
				{
					a.Friends[b.Swid] = b.Name;
					Note(a, "friend", FriendNote(invite.Id, b.Swid, b.Name));
				}
				else
				{
					Note(a, "uninvitation", RemoveInvitationNote(invite.Id));
				}
			}
			foreach (LanInvite invite in a.Incoming.Values.ToList())
			{
				if (!Same(invite.InviterName, invite.InviterSwid, b) || b.Outgoing.ContainsKey(invite.Id))
				{
					continue;
				}
				a.Incoming.Remove(invite.Id);
				Note(a, "uninvitation", RemoveInvitationNote(invite.Id));
			}
			if (a.Friends.ContainsKey(b.Swid) && !b.Friends.ContainsKey(a.Swid) && !b.Outgoing.Values.Any((LanInvite i) => i.InviteeSwid == a.Swid || string.Equals(i.InviteeName, a.Name, StringComparison.OrdinalIgnoreCase)))
			{
				a.Friends.Remove(b.Swid);
				Note(a, "unfriend", RemoveFriendNote(b.Swid));
			}
		}
	}
}
#endif
