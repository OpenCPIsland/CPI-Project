using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using DevonLocalization.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Net.Client
{
	public static class LanDirectory
	{
		public sealed class Entry
		{
			public string Swid;

			public string Name;

			public string Room;

			public long SessionId;

			public string OutfitJson;

			public int? Colour;

			public string IglooName;

			public string IglooInstance;

			public string Visibility;

			public int? DaysOld;

			public Dictionary<string, long> MascotXP;
		}

		private static readonly object sync = new object();

		private static List<Entry> entries = new List<Entry>();

		private static readonly Dictionary<string, string> layouts = new Dictionary<string, string>();

		private static HashSet<string> friendSwids = new HashSet<string>();

		private static string localSwid;

		private static readonly Dictionary<string, Entry> known = new Dictionary<string, Entry>();

		public static string LocalContentIdentifier;

		private static bool connected;

		public static bool Active
		{
			get
			{
				return connected && LanGameServerHost.Address != null;
			}
		}

		public static void SetConnected(bool value, string swid)
		{
			lock (sync)
			{
				connected = value;
				localSwid = swid;
				if (!value)
				{
					entries = new List<Entry>();
					layouts.Clear();
					known.Clear();
					friendSwids = new HashSet<string>();
				}
			}
		}

		public static void Replace(List<Entry> list)
		{
			lock (sync)
			{
				entries = list ?? new List<Entry>();
				foreach (Entry e in entries)
				{
					Entry old;
					if (known.TryGetValue(e.Swid, out old))
					{
						Fill(e, old);
					}
					known[e.Swid] = e;
				}
			}
		}

		public static void SetLayout(string swid, string json)
		{
			if (string.IsNullOrEmpty(swid))
			{
				return;
			}
			lock (sync)
			{
				if (json == null)
				{
					layouts.Remove(swid);
				}
				else
				{
					layouts[swid] = json;
				}
			}
		}

		public static bool IsFriend(string swid)
		{
			lock (sync)
			{
				return swid != null && friendSwids.Contains(swid);
			}
		}

		public static void SetFriends(IEnumerable<string> swids)
		{
			lock (sync)
			{
				friendSwids = new HashSet<string>(swids ?? Enumerable.Empty<string>());
			}
		}

		private static List<Entry> Snapshot()
		{
			lock (sync)
			{
				return entries.ToList();
			}
		}

		private static Entry Find(Func<Entry, bool> match)
		{
			return Snapshot().FirstOrDefault(match);
		}

		private static void Fill(Entry e, Entry from)
		{
			if (from == null)
			{
				return;
			}
			if (e.OutfitJson == null)
			{
				e.OutfitJson = from.OutfitJson;
			}
			if (!e.Colour.HasValue)
			{
				e.Colour = from.Colour;
			}
			if (!e.DaysOld.HasValue)
			{
				e.DaysOld = from.DaysOld;
			}
			if (e.MascotXP == null)
			{
				e.MascotXP = from.MascotXP;
			}
			if (e.IglooInstance == null)
			{
				e.IglooName = from.IglooName;
				e.IglooInstance = from.IglooInstance;
				e.Visibility = from.Visibility;
			}
		}

		public static List<Entry> KnownEntries()
		{
			lock (sync)
			{
				return known.Values.ToList();
			}
		}

		private static Entry Departed(Func<Entry, bool> match)
		{
			lock (sync)
			{
				foreach (Entry e in known.Values)
				{
					if (match(e))
					{
						return new Entry
						{
							Swid = e.Swid,
							Name = e.Name,
							OutfitJson = e.OutfitJson,
							Colour = e.Colour,
							IglooName = e.IglooName,
							IglooInstance = e.IglooInstance,
							Visibility = e.Visibility,
							DaysOld = e.DaysOld,
							MascotXP = e.MascotXP
						};
					}
				}
			}
			return null;
		}

		private static Entry Copy(Entry e)
		{
			return new Entry
			{
				Swid = e.Swid,
				Name = e.Name,
				Room = e.Room,
				SessionId = e.SessionId,
				OutfitJson = e.OutfitJson,
				Colour = e.Colour,
				IglooName = e.IglooName,
				IglooInstance = e.IglooInstance,
				Visibility = e.Visibility,
				DaysOld = e.DaysOld,
				MascotXP = e.MascotXP
			};
		}

		private static Entry Cached(string token, Func<ClubPenguin.Net.Offline.LanFriendsData.CachedProfile, bool> match)
		{
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}
			foreach (ClubPenguin.Net.Offline.LanFriendsData.CachedProfile p in LanFriends.Load(token).Profiles)
			{
				if (match(p))
				{
					return new Entry
					{
						Swid = p.Swid,
						Name = p.Name,
						OutfitJson = p.OutfitJson,
						Colour = p.Colour,
						IglooName = p.IglooName,
						IglooInstance = p.IglooInstance,
						Visibility = p.Visibility,
						DaysOld = p.DaysOld,
						MascotXP = p.MascotXP
					};
				}
			}
			return null;
		}

		public static OtherPlayerData BySwid(string swid, string token = null)
		{
			Entry e = Find((Entry x) => x.Swid == swid) ?? Departed((Entry x) => x.Swid == swid);
			Entry cached = Cached(token, (ClubPenguin.Net.Offline.LanFriendsData.CachedProfile x) => x.Swid == swid);
			if (e == null)
			{
				e = cached;
			}
			else if (cached != null)
			{
				e = Copy(e);
				Fill(e, cached);
			}
			return (e != null) ? ToPlayerData(e) : null;
		}

		public static OtherPlayerData ByName(string name, string token = null)
		{
			Entry e = Find((Entry x) => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Departed((Entry x) => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			Entry cached = Cached(token, (ClubPenguin.Net.Offline.LanFriendsData.CachedProfile x) => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
			if (e == null)
			{
				e = cached;
			}
			else if (cached != null)
			{
				e = Copy(e);
				Fill(e, cached);
			}
			return (e != null) ? ToPlayerData(e) : null;
		}

		public static OtherPlayerData BySession(long sessionId)
		{
			Entry e = Find((Entry x) => x.SessionId == sessionId && sessionId != 0);
			return (e != null) ? ToPlayerData(e) : null;
		}

		public static List<string> OnlineSwids(IEnumerable<string> swids)
		{
			List<string> result = new List<string>();
			if (swids == null)
			{
				return result;
			}
			HashSet<string> online = new HashSet<string>(from x in Snapshot()
				where !string.IsNullOrEmpty(x.Room) && x.Room.Split(':').Length >= 5
				select x.Swid);
			foreach (string swid in swids)
			{
				if (swid != null && online.Contains(swid))
				{
					result.Add(swid);
				}
			}
			return result;
		}

		public static List<OtherPlayerData> BySwids(IEnumerable<string> swids, string token = null)
		{
			List<OtherPlayerData> result = new List<OtherPlayerData>();
			if (swids == null)
			{
				return result;
			}
			foreach (string swid in swids)
			{
				OtherPlayerData d = BySwid(swid, token);
				if (d != null)
				{
					result.Add(d);
				}
			}
			return result;
		}

		public static List<OtherPlayerData> ByNames(IEnumerable<string> names, string token = null)
		{
			List<OtherPlayerData> result = new List<OtherPlayerData>();
			if (names == null)
			{
				return result;
			}
			foreach (string name in names)
			{
				OtherPlayerData d = ByName(name, token);
				if (d != null)
				{
					result.Add(d);
				}
			}
			return result;
		}

		private static OtherPlayerData ToPlayerData(Entry e)
		{
			OtherPlayerData d = new OtherPlayerData();
			d.id = new PlayerId
			{
				id = e.Swid,
				type = PlayerId.PlayerIdType.SWID
			};
			d.name = e.Name;
			d.member = true;
			d.mascotXP = (e.MascotXP != null) ? new Dictionary<string, long>(e.MascotXP) : new Dictionary<string, long>();
			d.outfit = new List<CustomEquipment>();
			if (!string.IsNullOrEmpty(e.OutfitJson))
			{
				try
				{
					PlayerOutfitDetails outfit = Service.Get<JsonService>().Deserialize<PlayerOutfitDetails>(e.OutfitJson);
					if (outfit.parts != null)
					{
						d.outfit.AddRange(outfit.parts);
					}
				}
				catch (Exception)
				{
				}
			}
			d.profile = new Profile
			{
				colour = e.Colour.GetValueOrDefault(),
				daysOld = e.DaysOld.GetValueOrDefault()
			};
			if (!string.IsNullOrEmpty(e.Room) && e.Room.Split(':').Length >= 5)
			{
				d.onlineLocation = new RoomIdentifier(e.Room);
				if (!string.IsNullOrEmpty(LocalContentIdentifier))
				{
					d.onlineLocation.contentIdentifier = LocalContentIdentifier;
				}
			}
			if (!string.IsNullOrEmpty(e.IglooInstance))
			{
				d.zoneId = new ZoneId
				{
					name = string.IsNullOrEmpty(e.IglooName) ? "DefaultIgloo" : e.IglooName,
					instanceId = e.IglooInstance
				};
			}
			return d;
		}

		private static Entry Owner(ZoneId igloo)
		{
			if (igloo == null || string.IsNullOrEmpty(igloo.instanceId))
			{
				return null;
			}
			return Find((Entry x) => x.IglooInstance == igloo.instanceId);
		}

		public static string OwnerName(ZoneId igloo)
		{
			Entry e = Owner(igloo);
			return (e != null) ? e.Name : null;
		}

		public static SceneLayout Layout(ZoneId igloo)
		{
			Entry e = Owner(igloo);
			if (e == null)
			{
				return null;
			}
			string json;
			lock (sync)
			{
				if (!layouts.TryGetValue(e.Swid, out json))
				{
					return null;
				}
			}
			try
			{
				return Service.Get<JsonService>().Deserialize<SceneLayout>(json);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static bool InRoom(Entry e, string zoneName, string instanceId)
		{
			if (string.IsNullOrEmpty(e.Room))
			{
				return false;
			}
			string[] parts = e.Room.Split(':');
			return parts.Length >= 5 && parts[2] == zoneName && (instanceId == null || parts[3] == instanceId);
		}

		private static RoomPopulationScale Scale(int count)
		{
			if (count <= 0)
			{
				return RoomPopulationScale.ZERO;
			}
			if (count < 4)
			{
				return RoomPopulationScale.ONE;
			}
			if (count < 8)
			{
				return RoomPopulationScale.TWO;
			}
			if (count < 15)
			{
				return RoomPopulationScale.THREE;
			}
			if (count < 30)
			{
				return RoomPopulationScale.FOUR;
			}
			return RoomPopulationScale.FIVE;
		}

		private static bool Visible(Entry e, bool friendsOnly)
		{
			IglooVisibility visibility = IglooVisibility.PUBLIC;
			if (!string.IsNullOrEmpty(e.Visibility))
			{
				try
				{
					visibility = (IglooVisibility)Enum.Parse(typeof(IglooVisibility), e.Visibility);
				}
				catch (Exception)
				{
				}
			}
			bool isFriend;
			lock (sync)
			{
				isFriend = friendSwids.Contains(e.Swid);
			}
			if (friendsOnly)
			{
				return isFriend && visibility != IglooVisibility.PRIVATE;
			}
			return visibility == IglooVisibility.PUBLIC || (visibility == IglooVisibility.FRIENDS_ONLY && isFriend);
		}

		public static List<IglooListItem> Igloos(bool friendsOnly)
		{
			List<Entry> all = Snapshot();
			List<IglooListItem> result = new List<IglooListItem>();
			foreach (Entry e in all)
			{
				if (e.Swid == localSwid || string.IsNullOrEmpty(e.IglooInstance) || !Visible(e, friendsOnly))
				{
					continue;
				}
				string zoneName = string.IsNullOrEmpty(e.IglooName) ? "DefaultIgloo" : e.IglooName;
				int count = all.Count((Entry x) => InRoom(x, zoneName, e.IglooInstance));
				result.Add(new IglooListItem
				{
					ownerData = ToPlayerData(e),
					iglooPopulation = Scale(count)
				});
			}
			return result;
		}

		public static List<RoomPopulation> IglooPopulations(IList<ZoneId> zoneIds)
		{
			List<Entry> all = Snapshot();
			List<RoomPopulation> result = new List<RoomPopulation>();
			if (zoneIds == null)
			{
				return result;
			}
			foreach (ZoneId zoneId in zoneIds)
			{
				if (zoneId == null)
				{
					continue;
				}
				Entry sample = all.FirstOrDefault((Entry x) => InRoom(x, zoneId.name, zoneId.instanceId));
				int count = all.Count((Entry x) => InRoom(x, zoneId.name, zoneId.instanceId));
				RoomIdentifier identifier = (sample != null) ? new RoomIdentifier(sample.Room) : new RoomIdentifier("", Language.none, zoneId, "");
				identifier.zoneId = zoneId;
				result.Add(new RoomPopulation(identifier, Scale(count)));
			}
			return result;
		}

		public static List<RoomPopulation> RoomPopulations(string world)
		{
			List<Entry> all = Snapshot();
			List<RoomPopulation> result = new List<RoomPopulation>();
			foreach (IGrouping<string, Entry> group in from x in all
				where !string.IsNullOrEmpty(x.Room) && x.Room.Split(':').Length >= 5 && string.IsNullOrEmpty(x.Room.Split(':')[3])
				group x by x.Room.Split(':')[2])
			{
				RoomIdentifier identifier = new RoomIdentifier(group.First().Room);
				if (!string.IsNullOrEmpty(world))
				{
					identifier.world = world;
				}
				result.Add(new RoomPopulation(identifier, Scale(group.Count())));
			}
			return result;
		}

		public static List<WorldRoomPopulation> WorldPopulations(string room)
		{
			List<Entry> all = Snapshot();
			List<WorldRoomPopulation> result = new List<WorldRoomPopulation>();
			foreach (IGrouping<string, Entry> group in from x in all
				where !string.IsNullOrEmpty(x.Room) && x.Room.Split(':').Length >= 5
				group x by x.Room.Split(':')[0])
			{
				int count = group.Count((Entry x) => string.IsNullOrEmpty(room) || x.Room.Split(':')[2] == room);
				result.Add(new WorldRoomPopulation(group.Key, Scale(count)));
			}
			return result;
		}
	}
}
