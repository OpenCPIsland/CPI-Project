#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanReward
	{
		public long Coins;

		public readonly Dictionary<string, long> MascotXP = new Dictionary<string, long>();

		public static LanReward From(object o)
		{
			LanReward r = new LanReward();
			long coins;
			if (LanJson.TryLong(LanJson.Get(o, "coins"), out coins))
			{
				r.Coins = coins;
			}
			Dictionary<string, object> xp = LanJson.Get(o, "mascotXP") as Dictionary<string, object>;
			if (xp != null)
			{
				foreach (KeyValuePair<string, object> kv in xp)
				{
					long v;
					if (LanJson.TryLong(kv.Value, out v))
					{
						r.MascotXP[kv.Key] = v;
					}
				}
			}
			return r;
		}

		public string ToJson()
		{
			Dictionary<string, object> d = new Dictionary<string, object>();
			d["coins"] = Coins;
			Dictionary<string, object> xp = new Dictionary<string, object>();
			foreach (KeyValuePair<string, long> kv in MascotXP)
			{
				xp[kv.Key] = kv.Value;
			}
			d["mascotXP"] = xp;
			return LanJson.Serialize(d);
		}
	}

	public sealed class LanGroupData
	{
		public string Path;

		public string Behaviour;

		public double CycleSeconds;

		public readonly List<KeyValuePair<string, float[]>> Children = new List<KeyValuePair<string, float[]>>();

		public string Start;

		public string End;
	}

	public sealed class LanTargetData
	{
		public string Path;

		public float[] Position;

		public double MinHits;

		public double PopulationMultiplier;

		public float[] HitCenter;

		public double HitRadius;
	}

	public sealed class LanPhaseData
	{
		public string Path;

		public float[] Position;

		public double SessionSeconds;

		public readonly List<LanTargetData> Targets = new List<LanTargetData>();
	}

	public sealed class LanSupersetData
	{
		public string Path;

		public LanReward TotalReward;

		public LanReward MinPerPlayerReward;

		public readonly List<LanPhaseData> Phases = new List<LanPhaseData>();
	}

	public sealed class LanPlaygroundData
	{
		public string Path;

		public float[] Position;

		public double EveryXMinutes;

		public string Behaviour;

		public float[] RewardsCenter;

		public double RewardsRadius;

		public double RoundDelaySeconds;

		public readonly List<LanSupersetData> Supersets = new List<LanSupersetData>();

		public string Start;

		public string End;
	}

	public sealed class LanWorldData
	{
		private readonly Dictionary<string, List<LanGroupData>> groups = new Dictionary<string, List<LanGroupData>>();

		private readonly Dictionary<string, List<LanPlaygroundData>> playgrounds = new Dictionary<string, List<LanPlaygroundData>>();

		public static float[] Vec3(object o)
		{
			Dictionary<string, object> m = o as Dictionary<string, object>;
			if (m != null)
			{
				o = new List<object> { LanJson.Get(m, "x"), LanJson.Get(m, "y"), LanJson.Get(m, "z") };
			}
			List<object> l = o as List<object>;
			if (l == null || l.Count != 3)
			{
				return null;
			}
			float[] v = new float[3];
			for (int i = 0; i < 3; i++)
			{
				double d;
				if (!LanJson.TryNumber(l[i], out d))
				{
					return null;
				}
				v[i] = (float)d;
			}
			return v;
		}

		private static void Dates(object e, out string start, out string end)
		{
			start = LanJson.Path(e, "activeDates", "start") as string;
			end = LanJson.Path(e, "activeDates", "end") as string;
		}

		public LanWorldData(string json)
		{
			object root;
			if (string.IsNullOrEmpty(json) || !LanJson.TryParse(json, out root))
			{
				return;
			}
			Dictionary<string, object> swo = LanJson.Get(root, "scheduledWorldObjects") as Dictionary<string, object>;
			if (swo != null)
			{
				foreach (KeyValuePair<string, object> zone in swo)
				{
					List<LanGroupData> list = new List<LanGroupData>();
					foreach (object g in (zone.Value as List<object>) ?? new List<object>())
					{
						try
						{
							if (LanJson.Get(g, "parties") != null)
							{
								continue;
							}
							LanGroupData gd = new LanGroupData();
							gd.Path = (string)LanJson.Get(g, "path");
							gd.Behaviour = (string)LanJson.Get(g, "behaviour");
							gd.CycleSeconds = LanJson.Number(LanJson.Get(g, "cycleSeconds"), 0.0);
							foreach (object c in (List<object>)LanJson.Get(g, "children"))
							{
								float[] pos = Vec3(LanJson.Get(c, "position"));
								string path = LanJson.Get(c, "path") as string;
								if (pos != null && path != null)
								{
									gd.Children.Add(new KeyValuePair<string, float[]>(path, pos));
								}
							}
							Dates(g, out gd.Start, out gd.End);
							if (gd.Path != null && gd.CycleSeconds > 0.0 && gd.Children.Count > 0)
							{
								list.Add(gd);
							}
						}
						catch (Exception)
						{
						}
					}
					groups[zone.Key] = list;
				}
			}
			Dictionary<string, object> it = LanJson.Get(root, "islandTargets") as Dictionary<string, object>;
			if (it == null)
			{
				return;
			}
			foreach (KeyValuePair<string, object> zone in it)
			{
				List<LanPlaygroundData> list = new List<LanPlaygroundData>();
				foreach (object p in (zone.Value as List<object>) ?? new List<object>())
				{
					try
					{
						if (LanJson.Get(p, "parties") != null)
						{
							continue;
						}
						LanPlaygroundData pd = new LanPlaygroundData();
						pd.Path = (string)LanJson.Get(p, "path");
						pd.Position = Vec3(LanJson.Get(p, "position"));
						pd.EveryXMinutes = LanJson.Number(LanJson.Get(p, "everyXMinutes"), 0.0);
						pd.Behaviour = (string)LanJson.Get(p, "behaviour");
						pd.RewardsCenter = Vec3(LanJson.Path(p, "rewardsVolume", "center"));
						pd.RewardsRadius = LanJson.Number(LanJson.Path(p, "rewardsVolume", "radius"), 0.0);
						pd.RoundDelaySeconds = LanJson.Number(LanJson.Get(p, "roundDelaySeconds"), 3.0);
						Dates(p, out pd.Start, out pd.End);
						foreach (object ss in (List<object>)LanJson.Get(p, "supersets"))
						{
							LanSupersetData sd = new LanSupersetData();
							sd.Path = (string)LanJson.Get(ss, "path");
							sd.TotalReward = LanReward.From(LanJson.Get(ss, "totalReward"));
							sd.MinPerPlayerReward = LanReward.From(LanJson.Get(ss, "minPerPlayerReward"));
							foreach (object ph in (List<object>)LanJson.Get(ss, "phases"))
							{
								LanPhaseData phd = new LanPhaseData();
								phd.Path = (string)LanJson.Get(ph, "path");
								phd.Position = Vec3(LanJson.Get(ph, "position"));
								phd.SessionSeconds = LanJson.Number(LanJson.Get(ph, "sessionSeconds"), 0.0);
								foreach (object t in (List<object>)LanJson.Get(ph, "targets"))
								{
									LanTargetData td = new LanTargetData();
									td.Path = (string)LanJson.Get(t, "path");
									td.Position = Vec3(LanJson.Get(t, "position"));
									td.MinHits = LanJson.Number(LanJson.Get(t, "minHits"), 1.0);
									td.PopulationMultiplier = LanJson.Number(LanJson.Get(t, "populationMultiplier"), 0.0);
									td.HitCenter = Vec3(LanJson.Path(t, "hitSphere", "center"));
									td.HitRadius = LanJson.Number(LanJson.Path(t, "hitSphere", "radius"), 0.0);
									if (td.Path != null && td.Position != null && td.HitCenter != null && td.HitRadius > 0.0)
									{
										phd.Targets.Add(td);
									}
								}
								if (phd.Path != null && phd.Position != null && phd.SessionSeconds > 0.0 && phd.Targets.Count > 0)
								{
									sd.Phases.Add(phd);
								}
							}
							if (sd.Path != null && sd.Phases.Count > 0)
							{
								pd.Supersets.Add(sd);
							}
						}
						if (pd.Path != null && pd.Position != null && pd.RewardsCenter != null && pd.EveryXMinutes > 0.0 && pd.EveryXMinutes <= 60.0 && pd.RoundDelaySeconds >= 0.0 && pd.RoundDelaySeconds <= 60.0 && pd.Supersets.Count > 0)
						{
							list.Add(pd);
						}
					}
					catch (Exception)
					{
					}
				}
				playgrounds[zone.Key] = list;
			}
		}

		private static bool Active(string start, string end, string date)
		{
			if (start == null || end == null)
			{
				return true;
			}
			return date != null && string.CompareOrdinal(start, date) <= 0 && string.CompareOrdinal(date, end) <= 0;
		}

		public List<LanGroupData> ScheduledGroups(string zone, string date)
		{
			List<LanGroupData> list;
			return groups.TryGetValue(zone, out list) ? list.Where((LanGroupData g) => Active(g.Start, g.End, date)).ToList() : new List<LanGroupData>();
		}

		public List<LanPlaygroundData> Playgrounds(string zone, string date)
		{
			List<LanPlaygroundData> list;
			return playgrounds.TryGetValue(zone, out list) ? list.Where((LanPlaygroundData p) => Active(p.Start, p.End, date)).ToList() : new List<LanPlaygroundData>();
		}
	}

	public static class LanPicker
	{
		public static Func<int> Create(string behaviour, int count, Random random)
		{
			if (behaviour == "ordered")
			{
				int i = -1;
				return delegate
				{
					i = (i + 1) % count;
					return i;
				};
			}
			if (behaviour == "random")
			{
				return () => random.Next(count);
			}
			List<int> pass = new List<int>();
			int last = -1;
			return delegate
			{
				if (pass.Count == 0)
				{
					for (int j = 0; j < count; j++)
					{
						pass.Add(j);
					}
					for (int k = pass.Count - 1; k > 0; k--)
					{
						int r = random.Next(k + 1);
						int tmp = pass[k];
						pass[k] = pass[r];
						pass[r] = tmp;
					}
					if (count > 1 && pass[0] == last)
					{
						int tmp2 = pass[0];
						pass[0] = pass[count - 1];
						pass[count - 1] = tmp2;
					}
				}
				last = pass[0];
				pass.RemoveAt(0);
				return last;
			};
		}
	}

	public sealed class LanWorldObjects
	{
		private sealed class LiveItem
		{
			public int ItemId;

			public string Path;

			public float[] Position;

			public int State;

			public long StateChangedMs;
		}

		private sealed class RoomState
		{
			public readonly Dictionary<string, LiveItem> Items = new Dictionary<string, LiveItem>();

			public readonly List<LanTimer[]> Timers = new List<LanTimer[]>();
		}

		private const int CategoryScheduledRoomObject = 2;

		private readonly LanServer server;

		private readonly LanWorldData data;

		private readonly Dictionary<string, RoomState> rooms = new Dictionary<string, RoomState>();

		public LanWorldObjects(LanServer server, LanWorldData data)
		{
			this.server = server;
			this.data = data;
		}

		private static SfsArray ItemVar(string name, byte kind, SfsValue v)
		{
			return new SfsArray().AddString(name).AddByte(kind).Add(v);
		}

		private static SfsArray Encode(LiveItem item)
		{
			SfsArray vars = new SfsArray().AddArray(ItemVar("category", SfsVar.Int, new SfsValue(SfsType.Int, CategoryScheduledRoomObject))).AddArray(ItemVar("creator", SfsVar.Int, new SfsValue(SfsType.Int, 0))).AddArray(ItemVar("path", SfsVar.String, new SfsValue(SfsType.UtfString, item.Path)))
				.AddArray(ItemVar("statetimestamp", SfsVar.String, new SfsValue(SfsType.UtfString, item.StateChangedMs.ToString(CultureInfo.InvariantCulture))))
				.AddArray(ItemVar("scheduledworldobjectstate", SfsVar.Int, new SfsValue(SfsType.Int, item.State)));
			return new SfsArray().AddInt(item.ItemId).AddArray(vars).AddFloats(item.Position);
		}

		public void SendSnapshot(LanPlayer player)
		{
			RoomState state = Ensure(player.RoomKey, player.RoomName);
			if (state.Items.Count == 0)
			{
				return;
			}
			SfsArray q = new SfsArray();
			foreach (LiveItem item in state.Items.Values)
			{
				q.AddArray(Encode(item));
			}
			player.Send(LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutArray("q", q)));
		}

		private void SetState(string roomKey, LiveItem item, int state)
		{
			item.State = state;
			item.StateChangedMs = LanServer.EpochMs;
			SfsArray vars = new SfsArray().AddArray(ItemVar("scheduledworldobjectstate", SfsVar.Int, new SfsValue(SfsType.Int, item.State))).AddArray(ItemVar("statetimestamp", SfsVar.String, new SfsValue(SfsType.UtfString, item.StateChangedMs.ToString(CultureInfo.InvariantCulture))));
			server.SendToRoom(roomKey, LanServer.Sys(1007, new SfsObject().PutInt("r", LanServer.RoomId).PutInt("i", item.ItemId).PutArray("v", vars)));
		}

		private void StartGroup(string roomKey, RoomState state, LanGroupData group)
		{
			long now = LanServer.EpochMs;
			List<LiveItem> children = new List<LiveItem>();
			foreach (KeyValuePair<string, float[]> child in group.Children)
			{
				LiveItem item = new LiveItem();
				item.ItemId = server.AllocateItemId();
				item.Path = child.Key;
				item.Position = child.Value;
				item.State = 0;
				item.StateChangedMs = now;
				state.Items[child.Key] = item;
				children.Add(item);
			}
			Func<int> pick = LanPicker.Create(group.Behaviour, children.Count, server.Random);
			LiveItem current = null;
			LanTimer[] holder = new LanTimer[1];
			state.Timers.Add(holder);
			Action cycle = null;
			cycle = delegate
			{
				if (current != null)
				{
					SetState(roomKey, current, 0);
				}
				current = children[pick()];
				SetState(roomKey, current, 1);
				holder[0] = server.Loop.After(group.CycleSeconds * 1000.0, cycle);
			};
			cycle();
		}

		private RoomState Ensure(string roomKey, string roomName)
		{
			RoomState state;
			if (rooms.TryGetValue(roomKey, out state))
			{
				return state;
			}
			state = new RoomState();
			rooms[roomKey] = state;
			foreach (LanGroupData g in data.ScheduledGroups(LanServer.ZoneOfRoom(roomKey), LanServer.ContentDateOf(roomName)))
			{
				StartGroup(roomKey, state, g);
			}
			return state;
		}

		public void ReleaseRoomIfEmpty(string roomKey)
		{
			RoomState state;
			if (!rooms.TryGetValue(roomKey, out state) || server.RoomOccupancy(roomKey) > 0)
			{
				return;
			}
			foreach (LanTimer[] t in state.Timers)
			{
				if (t[0] != null)
				{
					t[0].Cancel();
				}
			}
			rooms.Remove(roomKey);
		}
	}

	public sealed class LanRoomItem
	{
		public int ItemId;

		public string RoomKey;

		public float[] Position;

		public readonly List<KeyValuePair<string, SfsValue>> Vars = new List<KeyValuePair<string, SfsValue>>();

		public void Set(string name, SfsValue v)
		{
			for (int i = 0; i < Vars.Count; i++)
			{
				if (Vars[i].Key == name)
				{
					Vars[i] = new KeyValuePair<string, SfsValue>(name, v);
					return;
				}
			}
			Vars.Add(new KeyValuePair<string, SfsValue>(name, v));
		}
	}

	public sealed class LanRoomItems
	{
		private readonly LanServer server;

		private readonly Dictionary<int, LanRoomItem> items = new Dictionary<int, LanRoomItem>();

		public LanRoomItems(LanServer server)
		{
			this.server = server;
		}

		public static SfsValue Int(int v)
		{
			return new SfsValue(SfsType.Int, v);
		}

		public static SfsValue Str(string v)
		{
			return new SfsValue(SfsType.UtfString, v);
		}

		public static SfsValue Dbl(double v)
		{
			return new SfsValue(SfsType.Double, v);
		}

		private static byte KindOf(SfsValue v)
		{
			switch (v.Type)
			{
			case SfsType.Int:
				return SfsVar.Int;
			case SfsType.Double:
				return SfsVar.Double;
			default:
				return SfsVar.String;
			}
		}

		private static SfsArray EncodeVar(string name, SfsValue v)
		{
			return new SfsArray().AddString(name).AddByte(KindOf(v)).Add(v);
		}

		private static SfsArray EncodeTuple(LanRoomItem item)
		{
			SfsArray vars = new SfsArray();
			foreach (KeyValuePair<string, SfsValue> kv in item.Vars)
			{
				vars.AddArray(EncodeVar(kv.Key, kv.Value));
			}
			return new SfsArray().AddInt(item.ItemId).AddArray(vars).AddFloats(item.Position);
		}

		public LanRoomItem Add(string roomKey, float[] position, params KeyValuePair<string, SfsValue>[] vars)
		{
			LanRoomItem item = new LanRoomItem();
			item.ItemId = server.AllocateItemId();
			item.RoomKey = roomKey;
			item.Position = position;
			foreach (KeyValuePair<string, SfsValue> kv in vars)
			{
				item.Vars.Add(kv);
			}
			items[item.ItemId] = item;
			server.SendToRoom(roomKey, LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutArray("q", new SfsArray().AddArray(EncodeTuple(item)))));
			return item;
		}

		public static KeyValuePair<string, SfsValue> V(string name, SfsValue value)
		{
			return new KeyValuePair<string, SfsValue>(name, value);
		}

		public void SetVar(LanRoomItem item, string name, SfsValue v)
		{
			if (!items.ContainsKey(item.ItemId))
			{
				return;
			}
			item.Set(name, v);
			server.SendToRoom(item.RoomKey, LanServer.Sys(1007, new SfsObject().PutInt("r", LanServer.RoomId).PutInt("i", item.ItemId).PutArray("v", new SfsArray().AddArray(EncodeVar(name, v)))));
		}

		public void Remove(LanRoomItem item)
		{
			if (!items.Remove(item.ItemId))
			{
				return;
			}
			server.SendToRoom(item.RoomKey, LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutInts("n", new int[1]
			{
				item.ItemId
			})));
		}

		public void SendSnapshot(LanPlayer player)
		{
			List<LanRoomItem> live = items.Values.Where((LanRoomItem i) => i.RoomKey == player.RoomKey).ToList();
			if (live.Count == 0)
			{
				return;
			}
			SfsArray q = new SfsArray();
			foreach (LanRoomItem item in live)
			{
				q.AddArray(EncodeTuple(item));
			}
			player.Send(LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutArray("q", q)));
		}
	}
}
#endif
