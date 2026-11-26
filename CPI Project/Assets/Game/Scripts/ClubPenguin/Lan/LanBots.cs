#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed partial class LanServer
	{
		internal void BotEnter(LanPlayer bot, string roomName, float[] position)
		{
			bot.RoomName = roomName;
			bot.RoomKey = RoomKeyOf(roomName);
			bot.LastPosition = LanMotion.Copy(position);
			bot.Placed = true;
			bot.Announced = false;
			Register(bot);
			Targets.EnsureRoom(bot.RoomKey, bot.RoomName);
			Lobbies.EnsureRoom(bot.RoomKey);
			BroadcastNewJoiner(bot);
		}

		internal void BotLeave(LanPlayer bot)
		{
			if (!bot.Registered)
			{
				return;
			}
			RemovePlayer(bot);
			bot.Placed = false;
			bot.Announced = false;
			bot.EquippedConsumable = null;
			bot.EquippedItemName = null;
			bot.HeldObject = null;
			bot.HeldActionCount = null;
			bot.LocomotionState = 0;
			bot.PendingLobbyItem = null;
			bot.ThrowChargingSince = null;
		}

		internal void BotLocomotion(LanPlayer bot, byte action, float[] position, float[] direction, float[] velocity, string obj)
		{
			if (!bot.Registered)
			{
				return;
			}
			SfsObject p = new SfsObject().PutFloats("p", position).PutByte("a", action);
			if (direction != null)
			{
				p.PutFloats("d", direction);
			}
			if (velocity != null)
			{
				p.PutFloats("v", velocity);
			}
			if (obj != null)
			{
				p.PutString("o", obj);
			}
			HandleLocomotion(bot, p);
		}

		internal void BotChatActivity(LanPlayer bot)
		{
			if (bot.Registered)
			{
				SendToRoom(bot.RoomKey, Ext("chat.activity", new SfsObject().PutInt("senderId", bot.UserId)));
			}
		}

		internal void BotChat(LanPlayer bot, string message)
		{
			BotChat(bot, message, 0);
		}

		internal void BotChat(LanPlayer bot, string message, int sizzleClip)
		{
			if (!bot.Registered)
			{
				return;
			}
			string received = "{\"senderSessionId\":" + bot.PlayerSessionId + ",\"message\":" + LanJson.Quote(message ?? "") + ",\"emotion\":" + sizzleClip.ToString(CultureInfo.InvariantCulture) + "}";
			SendToRoom(bot.RoomKey, Ext("chat.msg", new SfsObject().PutString("msg", received).PutInt("senderId", bot.UserId)));
		}

		internal void BotGameMessage(LanPlayer bot, int gameId, int type, string json)
		{
			PartyGames.HandleMessage(bot, new SfsObject().PutInt("id", gameId).PutInt("type", type).PutString("message", json));
		}
	}

	public sealed class LanBotGrid
	{
		private const uint Magic = 0x3247424Cu;

		public string Zone;

		public float MinX;

		public float MinZ;

		public float Cell;

		public int W;

		public int H;

		public byte[] Flags;

		public short[] Heights;

		private bool Index(float x, float z, out int i)
		{
			int cx = (int)Math.Floor((x - MinX) / Cell);
			int cz = (int)Math.Floor((z - MinZ) / Cell);
			if (cx < 0 || cz < 0 || cx >= W || cz >= H)
			{
				i = -1;
				return false;
			}
			i = cz * W + cx;
			return true;
		}

		public bool Walkable(float[] p)
		{
			int i;
			return Index(p[0], p[2], out i) && Flags[i] == 1;
		}

		public bool Ground(float[] p, out float y)
		{
			int i;
			if (Index(p[0], p[2], out i) && Flags[i] == 1)
			{
				y = Heights[i] / 20f;
				return true;
			}
			y = 0f;
			return false;
		}

		public bool Clear(float[] a, float[] b)
		{
			double dx = b[0] - a[0];
			double dz = b[2] - a[2];
			double d = Math.Sqrt(dx * dx + dz * dz);
			int steps = Math.Max(1, (int)Math.Ceiling(d / 0.25));
			for (int k = 1; k <= steps; k++)
			{
				double t = (double)k / steps;
				int i;
				if (!Index((float)(a[0] + dx * t), (float)(a[2] + dz * t), out i) || Flags[i] != 1)
				{
					return false;
				}
			}
			return true;
		}

		public byte[] Serialize()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter w = new BinaryWriter(ms))
				{
					w.Write(Magic);
					w.Write(Zone ?? "");
					w.Write(MinX);
					w.Write(MinZ);
					w.Write(Cell);
					w.Write(W);
					w.Write(H);
					w.Write(Flags);
					foreach (short h in Heights)
					{
						w.Write(h);
					}
				}
				return ms.ToArray();
			}
		}

		public static LanBotGrid Deserialize(byte[] data)
		{
			try
			{
				using (BinaryReader r = new BinaryReader(new MemoryStream(data)))
				{
					if (r.ReadUInt32() != Magic)
					{
						return null;
					}
					LanBotGrid g = new LanBotGrid();
					g.Zone = r.ReadString();
					g.MinX = r.ReadSingle();
					g.MinZ = r.ReadSingle();
					g.Cell = r.ReadSingle();
					g.W = r.ReadInt32();
					g.H = r.ReadInt32();
					if (g.W <= 0 || g.H <= 0 || g.Cell <= 0f || (long)g.W * g.H > 4000000)
					{
						return null;
					}
					g.Flags = r.ReadBytes(g.W * g.H);
					g.Heights = new short[g.W * g.H];
					for (int i = 0; i < g.Heights.Length; i++)
					{
						g.Heights[i] = r.ReadInt16();
					}
					return (g.Flags.Length == g.W * g.H) ? g : null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public sealed class LanBotMap
	{
		public sealed class Node
		{
			public float[] P;

			public readonly List<int> Links = new List<int>();
		}

		private sealed class Heap
		{
			private readonly List<KeyValuePair<double, int>> items = new List<KeyValuePair<double, int>>();

			public int Count
			{
				get
				{
					return items.Count;
				}
			}

			public void Push(double key, int value)
			{
				items.Add(new KeyValuePair<double, int>(key, value));
				int i = items.Count - 1;
				while (i > 0)
				{
					int parent = (i - 1) / 2;
					if (items[parent].Key <= items[i].Key)
					{
						break;
					}
					KeyValuePair<double, int> tmp = items[parent];
					items[parent] = items[i];
					items[i] = tmp;
					i = parent;
				}
			}

			public int Pop()
			{
				int top = items[0].Value;
				int last = items.Count - 1;
				items[0] = items[last];
				items.RemoveAt(last);
				int i = 0;
				while (true)
				{
					int l = i * 2 + 1;
					int r = l + 1;
					int m = i;
					if (l < items.Count && items[l].Key < items[m].Key)
					{
						m = l;
					}
					if (r < items.Count && items[r].Key < items[m].Key)
					{
						m = r;
					}
					if (m == i)
					{
						break;
					}
					KeyValuePair<double, int> tmp = items[m];
					items[m] = items[i];
					items[i] = tmp;
					i = m;
				}
				return top;
			}
		}

		private const float Cell = 1.5f;

		private const float CellY = 2f;

		public const int MaxNodes = 6000;

		private const int MaxLinks = 12;

		public readonly List<Node> Nodes = new List<Node>();

		private readonly Dictionary<long, int> grid = new Dictionary<long, int>();

		public bool Dirty;

		private static long Key(float[] p)
		{
			long x = (long)Math.Floor(p[0] / Cell) + 1000000;
			long y = (long)Math.Floor(p[1] / CellY) + 1000000;
			long z = (long)Math.Floor(p[2] / Cell) + 1000000;
			return (x << 42) | (y << 21) | z;
		}

		public static double Dist(float[] a, float[] b)
		{
			double dx = a[0] - b[0];
			double dy = (a[1] - b[1]) * 1.5;
			double dz = a[2] - b[2];
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		public int Add(float[] p)
		{
			long k = Key(p);
			int id;
			if (grid.TryGetValue(k, out id))
			{
				return id;
			}
			if (Nodes.Count >= MaxNodes)
			{
				return -1;
			}
			Node n = new Node();
			n.P = LanMotion.Copy(p);
			Nodes.Add(n);
			id = Nodes.Count - 1;
			grid[k] = id;
			Dirty = true;
			return id;
		}

		public void Link(int a, int b)
		{
			if (a < 0 || b < 0 || a == b || a >= Nodes.Count || b >= Nodes.Count)
			{
				return;
			}
			Node na = Nodes[a];
			Node nb = Nodes[b];
			if (na.Links.Contains(b) || na.Links.Count >= MaxLinks || nb.Links.Count >= MaxLinks)
			{
				return;
			}
			na.Links.Add(b);
			nb.Links.Add(a);
			Dirty = true;
		}

		public int Nearest(float[] p, double max)
		{
			int best = -1;
			double bestD = max;
			for (int i = 0; i < Nodes.Count; i++)
			{
				if (Nodes[i].Links.Count == 0)
				{
					continue;
				}
				double d = Dist(Nodes[i].P, p);
				if (d < bestD)
				{
					bestD = d;
					best = i;
				}
			}
			return best;
		}

		public List<float[]> Path(float[] from, float[] to)
		{
			int s = Nearest(from, 6.0);
			int e = Nearest(to, 10.0);
			if (s < 0 || e < 0)
			{
				return null;
			}
			if (s == e)
			{
				return new List<float[]>();
			}
			Dictionary<int, double> g = new Dictionary<int, double>();
			Dictionary<int, int> came = new Dictionary<int, int>();
			HashSet<int> closed = new HashSet<int>();
			Heap open = new Heap();
			g[s] = 0.0;
			open.Push(Dist(Nodes[s].P, Nodes[e].P), s);
			while (open.Count > 0)
			{
				int cur = open.Pop();
				if (cur == e)
				{
					List<float[]> path = new List<float[]>();
					int at = e;
					while (at != s)
					{
						path.Add(LanMotion.Copy(Nodes[at].P));
						at = came[at];
					}
					path.Reverse();
					return path;
				}
				if (!closed.Add(cur))
				{
					continue;
				}
				foreach (int next in Nodes[cur].Links)
				{
					if (closed.Contains(next))
					{
						continue;
					}
					double cost = g[cur] + Dist(Nodes[cur].P, Nodes[next].P);
					double known;
					if (g.TryGetValue(next, out known) && known <= cost)
					{
						continue;
					}
					g[next] = cost;
					came[next] = cur;
					open.Push(cost + Dist(Nodes[next].P, Nodes[e].P), next);
				}
			}
			return null;
		}

		private List<int> Reachable(int start, int limit)
		{
			List<int> result = new List<int>();
			HashSet<int> seen = new HashSet<int>
			{
				start
			};
			Queue<int> queue = new Queue<int>();
			queue.Enqueue(start);
			while (queue.Count > 0 && result.Count < limit)
			{
				int cur = queue.Dequeue();
				result.Add(cur);
				foreach (int next in Nodes[cur].Links)
				{
					if (seen.Add(next))
					{
						queue.Enqueue(next);
					}
				}
			}
			return result;
		}

		public float[] RandomReachable(float[] from, double min, double max, Random random)
		{
			int s = Nearest(from, 6.0);
			if (s < 0)
			{
				return null;
			}
			List<int> all = Reachable(s, 4000);
			List<int> fit = all.Where(delegate(int i)
			{
				double d = Dist(Nodes[i].P, from);
				return d >= min && d <= max;
			}).ToList();
			List<int> pool = (fit.Count > 0) ? fit : all;
			if (pool.Count <= 1)
			{
				return null;
			}
			return LanMotion.Copy(Nodes[pool[random.Next(pool.Count)]].P);
		}

		public float[] RandomNear(float[] center, double radius, Random random)
		{
			List<int> pool = new List<int>();
			for (int i = 0; i < Nodes.Count; i++)
			{
				if (Nodes[i].Links.Count == 0)
				{
					continue;
				}
				double dx = Nodes[i].P[0] - center[0];
				double dz = Nodes[i].P[2] - center[2];
				if (Math.Sqrt(dx * dx + dz * dz) <= radius && Math.Abs(Nodes[i].P[1] - center[1]) <= 6.0)
				{
					pool.Add(i);
				}
			}
			return (pool.Count > 0) ? LanMotion.Copy(Nodes[pool[random.Next(pool.Count)]].P) : null;
		}

		public object ToJson()
		{
			List<object> n = new List<object>();
			List<object> e = new List<object>();
			for (int i = 0; i < Nodes.Count; i++)
			{
				float[] p = Nodes[i].P;
				n.Add(new List<object>
				{
					Math.Round(p[0], 2),
					Math.Round(p[1], 2),
					Math.Round(p[2], 2)
				});
				foreach (int j in Nodes[i].Links)
				{
					if (j > i)
					{
						e.Add(new List<object>
						{
							(long)i,
							(long)j
						});
					}
				}
			}
			return new Dictionary<string, object>
			{
				{
					"n",
					n
				},
				{
					"e",
					e
				}
			};
		}

		public static LanBotMap FromJson(object o)
		{
			LanBotMap map = new LanBotMap();
			List<int> ids = new List<int>();
			foreach (object p in (LanJson.Get(o, "n") as List<object>) ?? new List<object>())
			{
				float[] v = LanWorldData.Vec3(p);
				ids.Add((v != null) ? map.Add(v) : -1);
			}
			foreach (object e in (LanJson.Get(o, "e") as List<object>) ?? new List<object>())
			{
				List<object> pair = e as List<object>;
				int a;
				int b;
				if (pair != null && pair.Count == 2 && LanJson.TryInt(pair[0], out a) && LanJson.TryInt(pair[1], out b) && a >= 0 && b >= 0 && a < ids.Count && b < ids.Count)
				{
					map.Link(ids[a], ids[b]);
				}
			}
			map.Dirty = false;
			return map;
		}
	}

	public sealed class LanBots
	{
		private sealed class LastSample
		{
			public string Zone;

			public int Node;

			public float[] P;

			public long Ms;
		}

		private const double TickMs = 200.0;

		private const int MaxBots = 99;

		private const double SaveEveryMs = 30000.0;



		private static readonly string[] NameStart = new string[16]
		{
			"Snowy",
			"Icy",
			"Frosty",
			"Puffle",
			"Fishy",
			"Berg",
			"Igloo",
			"Flipper",
			"Waddle",
			"Chilly",
			"Polar",
			"Sled",
			"Aqua",
			"Cocoa",
			"Blizzard",
			"Pebble"
		};

		private static readonly string[] NameEnd = new string[10]
		{
			"Waddler",
			"Flipper",
			"Slider",
			"Diver",
			"Glider",
			"Skipper",
			"Pal",
			"Buddy",
			"Dash",
			"Bean"
		};

		public readonly LanServer Server;

		private readonly string path;

		private readonly Dictionary<string, LanBotMap> maps = new Dictionary<string, LanBotMap>();



		private readonly Dictionary<int, LastSample> lastSamples = new Dictionary<int, LastSample>();

		private readonly Dictionary<int, string> positionRoom = new Dictionary<int, string>();

		public volatile bool ChatEnabled = true;

		public volatile bool EmotesEnabled = true;

		public volatile bool TubesEnabled = true;

		public volatile bool PropsEnabled = true;

		public volatile bool FollowEnabled;

		public volatile bool ActivitiesEnabled = true;

		private readonly List<Dispenser> dispensers = new List<Dispenser>();

		private readonly Dictionary<int, long> airborneUntil = new Dictionary<int, long>();

		private readonly Dictionary<string, string> knownRooms = new Dictionary<string, string>();

		private readonly Dictionary<int, LastSample> lastSpot = new Dictionary<int, LastSample>();

		private readonly Dictionary<string, Portal> portals = new Dictionary<string, Portal>();

		private readonly Dictionary<int, LastSample> departSpot = new Dictionary<int, LastSample>();

		private readonly Dictionary<int, PendingInteract> pendingInteract = new Dictionary<int, PendingInteract>();

		private readonly List<Ride> rides = new List<Ride>();

		private readonly Dictionary<string, LanBotGrid> grids = new Dictionary<string, LanBotGrid>();


		private readonly List<Seat> seats = new List<Seat>();

		private bool usesDirty;

		private sealed class PendingInteract
		{
			public string RoomKey;

			public float[] P;

			public string Obj;

			public long Ms;

			public bool Zipline;

			public bool Sat;
		}

		public sealed class Ride
		{
			public string Zone;

			public string Obj;

			public float[] Start;

			public float[] End;

			public long DurationMs;

			public bool Zipline;
		}

		public sealed class Seat
		{
			public string Zone;

			public string Obj;

			public float[] P;

			public LanBot Taken;
		}

		public sealed class Dispenser
		{
			public string Zone;

			public string Obj;

			public float[] P;

			public string Kind;

			public string Name;
		}

		public sealed class Portal
		{
			public string FromZone;

			public string ToZone;

			public float[] Exit;

			public float[] Entry;
		}

		private long danceCheckAt;

		private long danceWindow;

		private const int MapVersion = 2;

		private readonly List<LanBot> bots = new List<LanBot>();

		private readonly Dictionary<int, int> heading = new Dictionary<int, int>();

		private volatile int target;

		private volatile int count;

		private int[] colours = new int[0];

		private int[] tubes = new int[0];

		private string[] botOutfits = new string[0];

		private int[] emotes = new int[0];

		private int[] sizzles = new int[0];

		private bool running;


		private long nextSave;

		private long nextSpawn;

		private long lastTick;

		public LanBots(LanServer server, string path)
		{
			Server = server;
			this.path = path;
		}

		public int Count
		{
			get
			{
				return count;
			}
		}

		public long Now
		{
			get
			{
				return Server.Loop.NowMs;
			}
		}

		public void Start()
		{
			running = true;
			Load();
			lastTick = Now;
			nextSave = Now + (long)SaveEveryMs;
			Server.Loop.After(TickMs, Tick);
		}

		public void Stop()
		{
			running = false;
			Save();
			foreach (LanBot b in bots.ToList())
			{
				Server.BotLeave(b.Player);
			}
			bots.Clear();
			count = 0;
		}

		public int[] Emotes
		{
			get
			{
				return emotes;
			}
		}

		public int[] Tubes
		{
			get
			{
				return tubes;
			}
		}

		public int[] Sizzles
		{
			get
			{
				return sizzles;
			}
		}

		private const string EmptyOutfit = "{\"parts\":[]}";

		public void Redress(string[] outfitJsons)
		{
			string[] o = (outfitJsons ?? new string[0]).Where(LanServer.IsOutfitJson).ToArray();
			if (o.Length == 0)
			{
				return;
			}
			Server.Loop.Post(delegate
			{
				botOutfits = o;
				foreach (LanBot b in bots)
				{
					if (Server.Random.NextDouble() >= 0.92)
					{
						continue;
					}
					b.Player.OutfitJson = botOutfits[Server.Random.Next(botOutfits.Length)];
					if (b.Player.Registered)
					{
						Server.BroadcastUserVars(b.Player, LanServer.StringVar("outfit", b.Player.OutfitJson));
					}
				}
			});
		}

		public void SetTarget(int wanted, int[] colourIds, int[] tubeIds, string[] outfitJsons, int[] emoteCodes, int[] sizzleIds)
		{
			int clamped = Math.Max(0, Math.Min(MaxBots, wanted));
			int[] c = colourIds ?? new int[0];
			int[] t = tubeIds ?? new int[0];
			string[] o = (outfitJsons ?? new string[0]).Where(LanServer.IsOutfitJson).ToArray();
			int[] e = emoteCodes ?? new int[0];
			int[] z = sizzleIds ?? new int[0];
			Server.Loop.Post(delegate
			{
				target = clamped;
				if (o.Length > 0)
				{
					bool firstOutfits = botOutfits.Length == 0;
					botOutfits = o;
					if (firstOutfits)
					{
						foreach (LanBot b in bots)
						{
							if (b.Player.OutfitJson == EmptyOutfit && Server.Random.NextDouble() < 0.92)
							{
								b.Player.OutfitJson = botOutfits[Server.Random.Next(botOutfits.Length)];
								if (b.Player.Registered)
								{
									Server.BroadcastUserVars(b.Player, LanServer.StringVar("outfit", b.Player.OutfitJson));
								}
							}
						}
					}
				}
				if (e.Length > 0)
				{
					emotes = e;
				}
				if (z.Length > 0)
				{
					sizzles = z;
				}
				if (c.Length > 0)
				{
					colours = c;
				}
				if (t.Length > 0)
				{
					bool first = tubes.Length == 0;
					tubes = t;
					if (first)
					{
						foreach (LanBot b in bots)
						{
							int id = tubes[Server.Random.Next(tubes.Length)];
							b.Player.SelectedTubeId = id;
							if (b.Player.Registered)
							{
								Server.BroadcastUserVars(b.Player, LanServer.IntVar("tube", id));
							}
						}
					}
				}
			});
		}

		public LanBotMap MapOf(string zone)
		{
			LanBotMap map;
			if (!maps.TryGetValue(zone ?? "", out map))
			{
				map = new LanBotMap();
				maps[zone ?? ""] = map;
			}
			return map;
		}

		public static bool IsTubeRace(LanPartyGameDef def)
		{
			return def == null || LanJson.Get(def.GameData, "RaceEndPosition") != null;
		}

		public bool HasTubes
		{
			get
			{
				return tubes.Length > 0;
			}
		}

		public static bool IsIgloo(string roomName)
		{
			return roomName != null && roomName.StartsWith("Igloo", StringComparison.Ordinal);
		}

		private static readonly string[] MascotNames = new string[4] { "Rookie", "AuntArctic", "DJCadence", "Rockhopper" };

		private static readonly int[] MascotMaxLevels = new int[4] { 6, 9, 3, 9 };

		public static Dictionary<string, long> RandomMascotXP(int level, Random random)
		{
			int[] levels = new int[MascotNames.Length];
			for (int n = 0; n < level; n++)
			{
				List<int> open = new List<int>();
				for (int i = 0; i < levels.Length; i++)
				{
					if (levels[i] < MascotMaxLevels[i])
					{
						open.Add(i);
					}
				}
				if (open.Count == 0)
				{
					break;
				}
				levels[open[random.Next(open.Count)]]++;
			}
			Dictionary<string, long> xp = new Dictionary<string, long>();
			for (int i = 0; i < levels.Length; i++)
			{
				long partial = (levels[i] < MascotMaxLevels[i]) ? random.Next(1000000) : 0;
				xp[MascotNames[i]] = levels[i] * 1000000L + partial;
			}
			return xp;
		}

		private static readonly string[] BlockedZones = new string[5] { "Diving", "ClothingDesigner", "ClassicMiniGames", "EndCredits", "MtBlizzardSummit" };

		public static bool IsBotBlocked(string roomName)
		{
			if (roomName == null)
			{
				return false;
			}
			string zone = LanServer.ZoneOfRoom(LanServer.RoomKeyOf(roomName));
			if (string.IsNullOrEmpty(zone))
			{
				return false;
			}
			if (zone.StartsWith("mg_", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			foreach (string blocked in BlockedZones)
			{
				if (string.Equals(zone, blocked, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private void Tick()
		{
			if (!running)
			{
				return;
			}
			long now = Now;
			double dt = Math.Max(0.05, Math.Min(0.5, (now - lastTick) / 1000.0));
			lastTick = now;
			try
			{
				Reconcile(now);
				PlanDanceTrips(now);
				foreach (LanBot b in bots.ToList())
				{
					try
					{
						b.Tick(now, dt);
					}
					catch (Exception)
					{
						b.Reset(now);
					}
				}
				if (now >= nextSave)
				{
					nextSave = now + (long)SaveEveryMs;
					if (usesDirty || maps.Values.Any((LanBotMap m) => m.Dirty))
					{
						Save();
					}
				}
			}
			catch (Exception)
			{
			}
			Server.Loop.After(TickMs, Tick);
		}

		private void PlanDanceTrips(long now)
		{
			if (now < danceCheckAt)
			{
				return;
			}
			danceCheckAt = now + 5000;
			if (!ActivitiesEnabled)
			{
				return;
			}
			LanPartyGameDef def = Server.PartyGames.Definitions.Get(LanBot.DanceTemplate);
			if (def == null || def.EveryXMinutes <= 0.0)
			{
				return;
			}
			string zone = string.IsNullOrEmpty(def.ScheduledZone) ? LanBot.DanceZone : def.ScheduledZone;
			long epoch = LanServer.EpochMs;
			long period = (long)(def.EveryXMinutes * 60000.0);
			long lobbyMs = (long)(LanJson.Number(LanJson.Get(def.LobbyData, "LobbyLengthInSeconds"), 30.0) * 1000.0);
			long opensAt = epoch / period * period;
			if (epoch > opensAt + lobbyMs - 8000)
			{
				opensAt += period;
			}
			if (opensAt - epoch > 25000 || opensAt == danceWindow)
			{
				return;
			}
			danceWindow = opensAt;
			string roomName = null;
			foreach (LanBot b in bots)
			{
				if (b.Player.Registered && LanServer.ZoneOfRoom(b.Player.RoomKey) == zone)
				{
					roomName = b.Player.RoomName;
					break;
				}
			}
			if (roomName == null)
			{
				foreach (LanPlayer h in Humans())
				{
					if (LanServer.ZoneOfRoom(h.RoomKey) == zone)
					{
						roomName = h.RoomName;
						break;
					}
				}
			}
			if (roomName == null)
			{
				roomName = KnownRoom(zone);
			}
			if (roomName == null)
			{
				return;
			}
			object loc = LanJson.Get(def.LobbyData, "LobbyLocation");
			float[] lobbyAt = new float[3]
			{
				(float)LanJson.Number(LanJson.Get(loc, "x"), 0.0),
				(float)LanJson.Number(LanJson.Get(loc, "y"), 0.0),
				(float)LanJson.Number(LanJson.Get(loc, "z"), 0.0)
			};
			long stayUntil = now + (opensAt - epoch) + lobbyMs + 200000;
			foreach (LanBot b in bots)
			{
				if (Server.Random.NextDouble() < 0.6)
				{
					b.PlanDance(roomName, lobbyAt, stayUntil, now);
				}
			}
		}

		public float[] ArrivalNear(string zone, float[] center)
		{
			LanBotMap map = MapOf(zone);
			float[] near = map.RandomNear(center, 7.0, Server.Random);
			if (near != null)
			{
				return near;
			}
			double a = Server.Random.NextDouble() * Math.PI * 2.0;
			double r = 2.0 + Server.Random.NextDouble() * 3.0;
			return new float[3]
			{
				center[0] + (float)(Math.Cos(a) * r),
				center[1],
				center[2] + (float)(Math.Sin(a) * r)
			};
		}

		private void Reconcile(long now)
		{
			while (bots.Count > target)
			{
				LanBot last = bots[bots.Count - 1];
				bots.RemoveAt(bots.Count - 1);
				Unhead(last);
				ReleaseSeat(last);
				Server.BotLeave(last.Player);
			}
			if (bots.Count < target && now >= nextSpawn)
			{
				nextSpawn = now + 700;
				Spawn();
			}
			count = bots.Count;
		}

		private List<LanPlayer> Humans()
		{
			return Server.All().Where((LanPlayer p) => !p.IsBot && InPlace(p) && !IsIgloo(p.RoomName) && !IsBotBlocked(p.RoomName)).ToList();
		}

		public LanPlayer HopTarget(LanPlayer bot, bool otherRoom)
		{
			List<LanPlayer> humans = Humans();
			if (otherRoom)
			{
				humans = humans.Where((LanPlayer h) => h.RoomKey != bot.RoomKey).ToList();
			}
			return (humans.Count > 0) ? humans[Server.Random.Next(humans.Count)] : null;
		}

		public LanPlayer PickLeader()
		{
			List<LanPlayer> humans = Server.All().Where((LanPlayer p) => !p.IsBot && p.Registered).ToList();
			LanPlayer local = humans.FirstOrDefault((LanPlayer h) => h.Connection != null && LanServer.IsLoopback(h.Connection.Address));
			if (local != null)
			{
				return local;
			}
			return (humans.Count > 0) ? humans[Server.Random.Next(humans.Count)] : null;
		}

		public LanPlayer LocalHuman()
		{
			return Server.All().FirstOrDefault((LanPlayer h) => !h.IsBot && h.Registered && h.Connection != null && LanServer.IsLoopback(h.Connection.Address));
		}

		public bool HasHumansElsewhere(LanPlayer bot)
		{
			return Humans().Any((LanPlayer h) => h.RoomKey != bot.RoomKey);
		}

		private LanPlayer SpawnAnchor()
		{
			List<LanPlayer> humans = Humans();
			LanPlayer local = humans.FirstOrDefault((LanPlayer h) => h.Connection != null && LanServer.IsLoopback(h.Connection.Address));
			if (local != null)
			{
				return local;
			}
			return (humans.Count > 0) ? humans[Server.Random.Next(humans.Count)] : null;
		}

		public float[] ArrivalPoint(LanPlayer anchor)
		{
			LanBotMap map = MapOf(LanServer.ZoneOfRoom(anchor.RoomKey));
			float[] near = map.RandomNear(anchor.LastPosition, 4.0, Server.Random);
			if (near != null)
			{
				return near;
			}
			double a = Server.Random.NextDouble() * Math.PI * 2.0;
			double r = 1.5 + Server.Random.NextDouble();
			return new float[3]
			{
				anchor.LastPosition[0] + (float)(Math.Cos(a) * r),
				anchor.LastPosition[1],
				anchor.LastPosition[2] + (float)(Math.Sin(a) * r)
			};
		}

		private string UniqueName()
		{
			HashSet<string> taken = new HashSet<string>(Server.All().Select((LanPlayer p) => p.UserName), StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < 50; i++)
			{
				string name = NameStart[Server.Random.Next(NameStart.Length)] + NameEnd[Server.Random.Next(NameEnd.Length)];
				if (Server.Random.Next(3) > 0)
				{
					name += Server.Random.Next(1, 100).ToString(CultureInfo.InvariantCulture);
				}
				if (!taken.Contains(name))
				{
					return name;
				}
			}
			return "Penguin" + Server.Random.Next(1000, 10000).ToString(CultureInfo.InvariantCulture);
		}

		private string UniqueSessionId()
		{
			HashSet<string> taken = new HashSet<string>(from p in Server.All()
				where p.PlayerSessionId != null
				select p.PlayerSessionId);
			while (true)
			{
				long id = 910000000000L + (long)(Server.Random.NextDouble() * 80000000000.0);
				string s = id.ToString(CultureInfo.InvariantCulture);
				if (!taken.Contains(s))
				{
					return s;
				}
			}
		}

		private void Spawn()
		{
			LanPlayer anchor = SpawnAnchor();
			if (anchor == null)
			{
				return;
			}
			LanPlayer p = new LanPlayer();
			p.ZoneName = anchor.ZoneName;
			p.UserName = UniqueName();
			p.Swid = "bot-" + Guid.NewGuid().ToString("N");
			p.PlayerSessionId = UniqueSessionId();
			p.Colour = (colours.Length > 0) ? colours[Server.Random.Next(colours.Length)] : (anchor.Colour ?? 1);
			p.SelectedTubeId = (tubes.Length > 0) ? tubes[Server.Random.Next(tubes.Length)] : 0;
			if (botOutfits.Length > 0 && Server.Random.NextDouble() < 0.92)
			{
				p.OutfitJson = botOutfits[Server.Random.Next(botOutfits.Length)];
			}
			else
			{
				p.OutfitJson = EmptyOutfit;
			}
			p.LoggedIn = true;
			p.DaysOld = Server.Random.Next(3601);
			p.MascotXP = RandomMascotXP(Server.Random.Next(1, 28), Server.Random);
			float[] at = ArrivalPoint(anchor);
			LanBot bot = new LanBot(this, p);
			bots.Add(bot);
			Server.BotEnter(p, anchor.RoomName, at);
			bot.Leader = anchor;
			bot.Arrived(Now, at);
			ResendLooks(p);
		}

		public int HeadingTo(int holderUserId)
		{
			int n;
			return heading.TryGetValue(holderUserId, out n) ? n : 0;
		}

		public void Head(LanBot bot, int holderUserId)
		{
			Unhead(bot);
			bot.HeadingHolder = holderUserId;
			heading[holderUserId] = HeadingTo(holderUserId) + 1;
		}

		public void Unhead(LanBot bot)
		{
			if (bot.HeadingHolder == 0)
			{
				return;
			}
			int n = HeadingTo(bot.HeadingHolder) - 1;
			if (n <= 0)
			{
				heading.Remove(bot.HeadingHolder);
			}
			else
			{
				heading[bot.HeadingHolder] = n;
			}
			bot.HeadingHolder = 0;
		}

		public bool BotHostingIn(string roomKey)
		{
			return bots.Any((LanBot b) => b.Player.RoomKey == roomKey && b.IsHosting);
		}

		public void OnChat(LanPlayer sender, string message)
		{
			if (sender.IsBot || !running || string.IsNullOrEmpty(message) || sender.LastPosition == null)
			{
				return;
			}
			int replies = 0;
			foreach (LanBot b in bots.OrderBy((LanBot x) => Server.Random.Next()).ToList())
			{
				if (replies >= 2)
				{
					break;
				}
				if (b.Player.RoomKey == sender.RoomKey && b.Player.LastPosition != null && LanMotion.Dist(b.Player.LastPosition, sender.LastPosition) <= 14.0 && b.Hear(message, Now))
				{
					replies++;
				}
			}
		}

		public bool InPlace(LanPlayer p)
		{
			if (p == null || !p.Registered || p.LastPosition == null)
			{
				return false;
			}
			if (p.IsBot)
			{
				return true;
			}
			string room;
			return positionRoom.TryGetValue(p.UserId, out room) && room == p.RoomKey;
		}

		public float[] ExitSpot(LanPlayer human, string roomKey)
		{
			LastSample s;
			if (lastSpot.TryGetValue(human.UserId, out s) && s.Zone == roomKey && Now - s.Ms <= 30000)
			{
				return s.P;
			}
			if (departSpot.TryGetValue(human.UserId, out s) && s.Zone == roomKey && Now - s.Ms <= 30000)
			{
				return s.P;
			}
			return null;
		}

		private static bool IsLocalObject(string obj, out string id)
		{
			id = null;
			object parsed;
			int type;
			if (string.IsNullOrEmpty(obj) || !LanJson.TryParse(obj, out parsed) || !LanJson.TryInt(LanJson.Get(parsed, "type"), out type) || type != 0)
			{
				return false;
			}
			id = LanJson.Get(parsed, "id") as string;
			return !string.IsNullOrEmpty(id);
		}

		private void ObserveUse(LanPlayer player, byte action, float[] position, string obj, long now)
		{
			string id;
			if (action == 2 && IsLocalObject(obj, out id))
			{
				PendingInteract pi = new PendingInteract();
				pi.RoomKey = player.RoomKey;
				pi.P = LanMotion.Copy(position);
				pi.Obj = obj;
				pi.Ms = now;
				pendingInteract[player.UserId] = pi;
				return;
			}
			PendingInteract pending;
			if (!pendingInteract.TryGetValue(player.UserId, out pending))
			{
				return;
			}
			if (pending.RoomKey != player.RoomKey || (!pending.Sat && now - pending.Ms > 180000))
			{
				pendingInteract.Remove(player.UserId);
				return;
			}
			if (action != 0 && action != 13)
			{
				return;
			}
			double moved = LanMotion.Dist(pending.P, position);
			long gap = now - pending.Ms;
			string zone = LanServer.ZoneOfRoom(player.RoomKey);
			if (pending.Sat)
			{
				pendingInteract.Remove(player.UserId);
				if (moved <= 4.0)
				{
					AddSeat(zone, pending.Obj, pending.P);
				}
				return;
			}
			if (gap < 1200)
			{
				if (moved > 2.0)
				{
					pendingInteract.Remove(player.UserId);
				}
				return;
			}
			pendingInteract.Remove(player.UserId);
			if (moved >= 6.0 && gap <= 180000)
			{
				AddRide(zone, pending.Obj, pending.P, position, gap, pending.Zipline);
			}
		}

		public void OnLocomotionState(LanPlayer player)
		{
			if (player.IsBot)
			{
				return;
			}
			PendingInteract pending;
			if (!pendingInteract.TryGetValue(player.UserId, out pending) || pending.RoomKey != player.RoomKey)
			{
				return;
			}
			if (player.LocomotionState == 5)
			{
				pending.Sat = true;
			}
			else if (player.LocomotionState == 6)
			{
				pending.Zipline = true;
			}
		}

		private void AddRide(string zone, string obj, float[] start, float[] end, long duration, bool zipline)
		{
			rides.RemoveAll((Ride r) => r.Zone == zone && r.Obj == obj && LanMotion.Dist(r.Start, start) < 4.0);
			Ride ride = new Ride();
			ride.Zone = zone;
			ride.Obj = obj;
			ride.Start = LanMotion.Copy(start);
			ride.End = LanMotion.Copy(end);
			ride.DurationMs = duration;
			ride.Zipline = zipline;
			rides.Add(ride);
			usesDirty = true;
		}

		private void AddSeat(string zone, string obj, float[] p)
		{
			if (seats.Any((Seat x) => x.Zone == zone && x.Obj == obj && LanMotion.Dist(x.P, p) < 1.5))
			{
				return;
			}
			Seat seat = new Seat();
			seat.Zone = zone;
			seat.Obj = obj;
			seat.P = LanMotion.Copy(p);
			seats.Add(seat);
			usesDirty = true;
		}

		public Ride PickRide(string zone, float[] from, Random r)
		{
			List<Ride> near = rides.Where((Ride x) => x.Zone == zone).ToList();
			return (near.Count > 0) ? near[r.Next(near.Count)] : null;
		}

		public Seat PickSeat(string zone, float[] from, Random r, LanBot bot)
		{
			List<Seat> near = seats.Where((Seat x) => x.Zone == zone && x.Taken == null).ToList();
			if (near.Count == 0)
			{
				return null;
			}
			Seat seat = near[r.Next(near.Count)];
			seat.Taken = bot;
			return seat;
		}

		public void ReleaseSeat(LanBot bot)
		{
			foreach (Seat x in seats)
			{
				if (x.Taken == bot)
				{
					x.Taken = null;
				}
			}
		}

		public void ObserveDispenser(LanPlayer player, string kind, string name)
		{
			if (player.IsBot || !running || player.LastPosition == null || player.RoomKey == null || IsIgloo(player.RoomName) || string.IsNullOrEmpty(kind) || string.IsNullOrEmpty(name))
			{
				return;
			}
			string obj = null;
			float[] p = LanMotion.Copy(player.LastPosition);
			PendingInteract pending;
			if (pendingInteract.TryGetValue(player.UserId, out pending))
			{
				if (pending.RoomKey == player.RoomKey && Now - pending.Ms <= 5000)
				{
					obj = pending.Obj;
					p = LanMotion.Copy(pending.P);
				}
				pendingInteract.Remove(player.UserId);
			}
			string zone = LanServer.ZoneOfRoom(player.RoomKey);
			dispensers.RemoveAll((Dispenser x) => x.Zone == zone && x.Name == name && LanMotion.Dist(x.P, p) < 2.0);
			Dispenser d = new Dispenser();
			d.Zone = zone;
			d.Obj = obj;
			d.P = p;
			d.Kind = kind;
			d.Name = name;
			dispensers.Add(d);
			usesDirty = true;
			UnityEngine.Debug.Log("[LAN Bots] Learned prop spot " + name + " in " + zone + ((obj != null) ? " (interactable)" : ""));
		}

		private static readonly Dispenser[] BuiltInDispensers = new Dispenser[]
		{
			BuiltIn("Beach", 15.972f, 1.510f, 1.802f, "Keytar"),
			BuiltIn("Beach", 17.176f, 1.535f, 3.175f, "Guitar"),
			BuiltIn("Beach", 15.378f, 1.461f, 0.724f, "Drums"),
			BuiltIn("Beach", 17.612f, 0.794f, 15.781f, "FirstAidKit"),
			BuiltIn("Boardwalk", 12.552f, 0.230f, 12.972f, "FishingRod"),
			BuiltIn("Boardwalk", 12.406f, 0.454f, 12.853f, "FishingRod"),
			BuiltIn("Boardwalk", 12.788f, 0.399f, 0.754f, "FishDogMultiTool"),
			BuiltIn("Boardwalk", 26.483f, 1.340f, 13.893f, "Sword"),
			BuiltIn("Boardwalk", 30.199f, 3.570f, 4.685f, "PirateKit"),
			BuiltIn("MtBlizzard", 2.264f, 1.610f, 6.302f, "PomPomsBlue"),
			BuiltIn("MtBlizzard", -13.740f, 1.450f, -16.880f, "PomPomsBlue"),
			BuiltIn("MtBlizzard", -17.831f, 1.366f, -17.767f, "PomPomsRed"),
			BuiltIn("MtBlizzard", -30.869f, 2.354f, 8.149f, "PomPomsRed"),
			BuiltIn("MtBlizzard", -47.453f, 32.982f, -5.659f, "CPSNCamera"),
			BuiltIn("MtBlizzard", 7.246f, 1.127f, -1.719f, "GaryGear"),
			BuiltIn("EventMedievalDungeon1", -201.727f, 7.909f, -22.400f, "Sword"),
			BuiltIn("EventMedievalDungeon1", -204.346f, 4.184f, -5.109f, "Sword"),
			BuiltIn("EventMedievalDungeon1", -44.136f, 3.439f, 8.485f, "FishDogMultiTool"),
			BuiltIn("EventPirateParty", 69.753f, 6.389f, 0.278f, "Drums"),
			BuiltIn("EventPirateParty", 70.910f, 6.438f, 0.699f, "Keytar"),
			BuiltIn("EventPirateParty", 71.190f, 6.655f, 5.136f, "Microphone"),
			BuiltIn("EventPirateParty", 72.720f, 6.463f, 0.941f, "Guitar"),
			BuiltIn("Town", -19.561f, 0.627f, -15.585f, "FishDogMultiTool"),
			BuiltIn("Town", 61.506f, -14.614f, 0.492f, "FishDogMultiTool"),
			BuiltIn("Town", 16.912f, 1.126f, -13.125f, "PomPomsBlue"),
			BuiltIn("Town", 19.174f, 0.805f, -10.666f, "PomPomsRed"),
			BuiltIn("Town", -2.280f, 0.642f, -13.256f, "AcousticGuitar"),
			BuiltIn("Town", -0.861f, 0.641f, -13.659f, "BongoDrums"),
			BuiltIn("Town", -6.265f, 0.641f, -12.166f, "Microphone"),
			BuiltIn("Town", 75.975f, -5.905f, -23.088f, "AcousticGuitar")
		};

		private static Dispenser BuiltIn(string zone, float x, float y, float z, string name)
		{
			Dispenser d = new Dispenser();
			d.Zone = zone;
			d.P = new float[3] { x, y, z };
			d.Kind = "dispns";
			d.Name = name;
			return d;
		}

		private sealed class PropArea
		{
			public string Zone;

			public string Name;

			public float[] Box;
		}

		private static readonly PropArea[] PropAreas = new PropArea[]
		{
			ZoneArea("Beach", "Keytar", 15.500f, 2.922f, 2.699f, 2.952f, -40.337f),
			ZoneArea("Beach", "Guitar", 15.500f, 2.922f, 2.699f, 2.952f, -40.337f),
			ZoneArea("Beach", "Drums", 15.500f, 2.922f, 2.699f, 2.952f, -40.337f),
			ZoneArea("Beach", "FirstAidKit", 15.784f, 15.840f, 2.986f, 1.882f, -27.269f),
			ZoneArea("Boardwalk", "FishingRod", 10.447f, 12.560f, 2.994f, 1.237f, -25.331f),
			ZoneArea("Boardwalk", "FishDogMultiTool", 13.506f, 0.862f, 2.645f, 1.411f, -16.619f),
			ZoneArea("Boardwalk", "Sword", 26.659f, 14.803f, 5.312f, 14.376f, 0.679f),
			ZoneArea("Boardwalk", "PirateKit", 27.568f, 14.030f, 12.504f, 5.155f, -90.606f),
			ZoneArea("MtBlizzard", "PomPomsBlue", 1.637f, 6.354f, 4.074f, 1.351f, 0.282f),
			ZoneArea("MtBlizzard", "PomPomsBlue", -16.123f, -13.970f, 7.111f, 7.499f, -5.839f),
			ZoneArea("MtBlizzard", "PomPomsRed", -15.808f, -13.758f, 6.825f, 6.723f, 86.005f),
			ZoneArea("MtBlizzard", "PomPomsRed", -30.094f, 7.405f, 3.749f, 1.894f, 9.870f),
			ZoneArea("MtBlizzard", "CPSNCamera", -40.965f, -7.863f, 16.247f, 7.870f, 31.923f),
			ZoneArea("MtBlizzard", "GaryGear", 5.148f, -5.106f, 5.178f, 2.383f, -51.801f),
			ZoneArea("EventMedievalDungeon1", "Sword", -201.170f, -8.132f, 17.417f, 17.072f, 0.000f),
			ZoneArea("EventMedievalDungeon1", "FishDogMultiTool", -50.219f, 7.334f, 7.519f, 5.703f, -24.070f),
			ZoneArea("EventPirateParty", "Drums", 71.278f, 2.552f, 4.033f, 3.052f, 0.000f),
			ZoneArea("EventPirateParty", "Keytar", 71.278f, 2.552f, 4.033f, 3.052f, 0.000f),
			ZoneArea("EventPirateParty", "Microphone", 71.278f, 2.552f, 4.033f, 3.052f, 0.000f),
			ZoneArea("EventPirateParty", "Guitar", 71.278f, 2.552f, 4.033f, 3.052f, 0.000f),
			ZoneArea("Town", "AcousticGuitar", -3.458f, -12.618f, 3.969f, 4.479f, -70.024f),
			ZoneArea("Town", "BongoDrums", -3.458f, -12.618f, 3.969f, 4.479f, -70.024f),
			ZoneArea("Town", "Microphone", -3.458f, -12.618f, 3.969f, 4.479f, -70.024f),
			ZoneArea("Town", "FishDogMultiTool", -16.643f, -13.066f, 3.979f, 3.507f, 49.285f),
			ZoneArea("Town", "FishDogMultiTool", 59.872f, 0.600f, 2.123f, 1.155f, 0.000f),
			ZoneArea("Town", "PomPomsBlue", 16.485f, -10.162f, 4.759f, 4.729f, -5.839f),
			ZoneArea("Town", "PomPomsRed", 16.520f, -9.840f, 4.638f, 4.326f, -4.571f)
		};

		private static PropArea ZoneArea(string zone, string name, float x, float z, float hx, float hz, float yaw)
		{
			PropArea a = new PropArea();
			a.Zone = zone;
			a.Name = name;
			a.Box = new float[5] { x, z, hx, hz, yaw };
			return a;
		}

		public static float[] AreaFor(string zone, string name, float[] p)
		{
			float[] best = null;
			double bestDist = 8.0;
			foreach (PropArea a in PropAreas)
			{
				if (a.Zone != zone || a.Name != name)
				{
					continue;
				}
				if (p == null || InArea(a.Box, p))
				{
					return a.Box;
				}
				double dx = p[0] - a.Box[0];
				double dz = p[2] - a.Box[1];
				double d = Math.Sqrt(dx * dx + dz * dz) - Math.Max(a.Box[2], a.Box[3]);
				if (d < bestDist)
				{
					bestDist = d;
					best = a.Box;
				}
			}
			return best;
		}

		public static bool InArea(float[] area, float[] p)
		{
			double a = area[4] * Math.PI / 180.0;
			double dx = p[0] - area[0];
			double dz = p[2] - area[1];
			return Math.Abs(dx * Math.Cos(a) - dz * Math.Sin(a)) <= area[2] && Math.Abs(dx * Math.Sin(a) + dz * Math.Cos(a)) <= area[3];
		}

		public static float[] RandomInArea(float[] area, float y, Random r)
		{
			double a = area[4] * Math.PI / 180.0;
			double lx = (r.NextDouble() * 2.0 - 1.0) * area[2] * 0.75;
			double lz = (r.NextDouble() * 2.0 - 1.0) * area[3] * 0.75;
			return new float[3]
			{
				(float)(area[0] + lx * Math.Cos(a) + lz * Math.Sin(a)),
				y,
				(float)(area[1] - lx * Math.Sin(a) + lz * Math.Cos(a))
			};
		}

		public static double SwimSpeed = 1.5;

		private sealed class SwimVolume
		{
			public string Zone;

			public float[] Box;

			public float Surface;
		}

		private static readonly SwimVolume[] SwimVolumes = new SwimVolume[5]
		{
			Water("Beach", 2.97f, 16.00f, 13.76f, 30.82f, 61.18f, -0.06f),
			Water("Beach", 28.34f, 14.01f, 1.87f, 0.74f, -33.65f, 2.75f),
			Water("Beach", 28.96f, 12.63f, 1.68f, 0.88f, -33.65f, 2.75f),
			Water("Beach", 30.12f, 10.93f, 2.17f, 2.72f, -33.65f, 2.75f),
			Water("Boardwalk", 5.79f, 25.89f, 16.08f, 25.19f, 61.18f, -1.66f)
		};

		private static SwimVolume Water(string zone, float x, float z, float hx, float hz, float yaw, float surface)
		{
			SwimVolume v = new SwimVolume();
			v.Zone = zone;
			v.Box = new float[5] { x, z, hx, hz, yaw };
			v.Surface = surface;
			return v;
		}

		public static float[] RandomWater(string zone, Random r)
		{
			List<SwimVolume> pool = SwimVolumes.Where((SwimVolume v) => v.Zone == zone && v.Box[2] >= 4f && v.Box[3] >= 4f).ToList();
			if (pool.Count == 0)
			{
				return null;
			}
			SwimVolume pick = pool[r.Next(pool.Count)];
			return RandomInArea(pick.Box, pick.Surface, r);
		}

		public static bool InWater(string zone, float[] p)
		{
			if (zone == null || p == null)
			{
				return false;
			}
			foreach (SwimVolume v in SwimVolumes)
			{
				if (v.Zone == zone && p[1] <= v.Surface + 0.5f && InArea(v.Box, p))
				{
					return true;
				}
			}
			return false;
		}

		public Dispenser PickDispenser(string zone, float[] from, double maxDist, Random r)
		{
			List<Dispenser> near = dispensers.Concat(BuiltInDispensers).Where((Dispenser x) => x.Zone == zone && (from == null || LanMotion.Dist(x.P, from) <= maxDist)).ToList();
			return (near.Count > 0) ? near[r.Next(near.Count)] : null;
		}

		public void SetGrid(string zone, LanBotGrid grid)
		{
			if (string.IsNullOrEmpty(zone) || grid == null)
			{
				return;
			}
			Server.Loop.Post(delegate
			{
				grids[zone] = grid;
			});
		}

		public LanBotGrid GridOf(string zone)
		{
			LanBotGrid g;
			return (zone != null && grids.TryGetValue(zone, out g)) ? g : null;
		}

		public Portal PortalFor(string fromZone, string toZone)
		{
			Portal portal;
			return portals.TryGetValue(fromZone + ">" + toZone, out portal) ? portal : null;
		}

		public string KnownRoom(string zone)
		{
			string room;
			return knownRooms.TryGetValue(zone, out room) ? room : null;
		}

		public void Observe(LanPlayer player, byte action, float[] position)
		{
			Observe(player, action, position, null);
		}

		public void Observe(LanPlayer player, byte action, float[] position, string obj)
		{
			if (player.IsBot)
			{
				return;
			}
			string previousRoom;
			bool arrived = !positionRoom.TryGetValue(player.UserId, out previousRoom) || previousRoom != player.RoomKey;
			positionRoom[player.UserId] = player.RoomKey;
			LastSample spot;
			if (arrived && previousRoom != null && lastSpot.TryGetValue(player.UserId, out spot) && spot.Zone == previousRoom && Now - spot.Ms <= 20000)
			{
				Portal portal = new Portal();
				portal.FromZone = LanServer.ZoneOfRoom(previousRoom);
				portal.ToZone = LanServer.ZoneOfRoom(player.RoomKey);
				portal.Exit = LanMotion.Copy(spot.P);
				portal.Entry = LanMotion.Copy(position);
				if (portal.FromZone != portal.ToZone)
				{
					portals[portal.FromZone + ">" + portal.ToZone] = portal;
				}
				departSpot[player.UserId] = spot;
			}
			LastSample here = new LastSample();
			here.Zone = player.RoomKey;
			here.Node = -1;
			here.P = LanMotion.Copy(position);
			here.Ms = Now;
			lastSpot[player.UserId] = here;
			if (!running)
			{
				return;
			}
			string zone = LanServer.ZoneOfRoom(player.RoomKey);
			long now = Now;
			if (arrived)
			{
				lastSamples.Remove(player.UserId);
				airborneUntil[player.UserId] = now + 1500;
				SendBotLooks(player);
			}
			if (!IsIgloo(player.RoomName) && player.RoomName != null)
			{
				knownRooms[zone] = player.RoomName;
			}
			LanPartyGameSession session = Server.PartyGames.SessionOf(player);
			ObserveUse(player, action, position, obj, now);
			if (action != 0 && action != 13 && action != 12)
			{
				airborneUntil[player.UserId] = now + 1600;
				lastSamples.Remove(player.UserId);
				return;
			}
			if (session != null || (player.LocomotionState != 0 && player.LocomotionState != 8) || IsIgloo(player.RoomName))
			{
				lastSamples.Remove(player.UserId);
				return;
			}
			LastSample last;
			bool hasLast = lastSamples.TryGetValue(player.UserId, out last) && last.Zone == zone && now - last.Ms <= 1500;
			if (hasLast)
			{
				double dt = Math.Max(0.05, (now - last.Ms) / 1000.0);
				double dx = position[0] - last.P[0];
				double dz = position[2] - last.P[2];
				double vy = Math.Abs(position[1] - last.P[1]) / dt;
				double vh = Math.Sqrt(dx * dx + dz * dz) / dt;
				if (vy > 3.0 || vh > 8.5)
				{
					airborneUntil[player.UserId] = now + 1200;
					lastSamples.Remove(player.UserId);
					return;
				}
			}
			long until;
			if (airborneUntil.TryGetValue(player.UserId, out until) && now < until)
			{
				LastSample hold = new LastSample();
				hold.Zone = zone;
				hold.Node = -1;
				hold.P = LanMotion.Copy(position);
				hold.Ms = now;
				lastSamples[player.UserId] = hold;
				return;
			}
			if (!hasLast)
			{
				LastSample first = new LastSample();
				first.Zone = zone;
				first.Node = -1;
				first.P = LanMotion.Copy(position);
				first.Ms = now;
				lastSamples[player.UserId] = first;
				return;
			}
			LanBotMap map = MapOf(zone);
			int node = map.Add(position);
			if (node < 0)
			{
				return;
			}
			if (last.Node >= 0 && now - last.Ms <= 800 && LanMotion.Dist(last.P, position) <= 3.0)
			{
				map.Link(last.Node, node);
			}
			LastSample s = new LastSample();
			s.Zone = zone;
			s.Node = node;
			s.P = LanMotion.Copy(position);
			s.Ms = now;
			lastSamples[player.UserId] = s;
		}

		private void SendBotLooks(LanPlayer human)
		{
			string roomKey = human.RoomKey;
			Server.Loop.After(2500.0, delegate
			{
				if (!running || !human.Registered || human.RoomKey != roomKey)
				{
					return;
				}
				foreach (LanBot b in bots)
				{
					if (b.Player.Registered && b.Player.RoomKey == roomKey)
					{
						human.Send(LanServer.UserVarsPacket(b.Player, LanServer.IntVar("tube", b.Player.SelectedTubeId.GetValueOrDefault()), LanServer.IntVar("colour", b.Player.Colour.GetValueOrDefault())));
					}
				}
			});
		}

		public void ResendLooks(LanPlayer bot)
		{
			Server.Loop.After(2500.0, delegate
			{
				if (running && bot.Registered)
				{
					Server.BroadcastUserVars(bot, LanServer.IntVar("tube", bot.SelectedTubeId.GetValueOrDefault()), LanServer.IntVar("colour", bot.Colour.GetValueOrDefault()));
				}
			});
		}

		private void Load()
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				return;
			}
			try
			{
				object root;
				if (!LanJson.TryParse(File.ReadAllText(path, Encoding.UTF8), out root))
				{
					return;
				}
				double version;
				Dictionary<string, object> zones = LanJson.Get(root, "zones") as Dictionary<string, object>;
				if (zones != null && LanJson.TryNumber(LanJson.Get(root, "mapVersion"), out version) && (int)version == MapVersion)
				{
					foreach (KeyValuePair<string, object> kv in zones)
					{
						maps[kv.Key] = LanBotMap.FromJson(kv.Value);
					}
				}
				LoadUses(root);
			}
			catch (Exception)
			{
			}
		}

		private static float[] Vec(object v)
		{
			List<object> l = v as List<object>;
			double x;
			double y;
			double z;
			if (l == null || l.Count != 3 || !LanJson.TryNumber(l[0], out x) || !LanJson.TryNumber(l[1], out y) || !LanJson.TryNumber(l[2], out z))
			{
				return null;
			}
			return new float[3]
			{
				(float)x,
				(float)y,
				(float)z
			};
		}

		private static object VecJson(float[] p)
		{
			return new List<object>
			{
				Math.Round(p[0], 2),
				Math.Round(p[1], 2),
				Math.Round(p[2], 2)
			};
		}

		private void LoadUses(object root)
		{
			foreach (object o in (LanJson.Get(root, "rides") as List<object>) ?? new List<object>())
			{
				Ride ride = new Ride();
				ride.Zone = LanJson.Get(o, "zone") as string;
				ride.Obj = LanJson.Get(o, "obj") as string;
				ride.Start = Vec(LanJson.Get(o, "start"));
				ride.End = Vec(LanJson.Get(o, "end"));
				ride.DurationMs = (long)LanJson.Number(LanJson.Get(o, "ms"), 3000.0);
				ride.Zipline = LanJson.Number(LanJson.Get(o, "zip"), 0.0) != 0.0;
				if (ride.Zone != null && ride.Obj != null && ride.Start != null && ride.End != null)
				{
					rides.Add(ride);
				}
			}
			foreach (object o in (LanJson.Get(root, "seats") as List<object>) ?? new List<object>())
			{
				Seat seat = new Seat();
				seat.Zone = LanJson.Get(o, "zone") as string;
				seat.Obj = LanJson.Get(o, "obj") as string;
				seat.P = Vec(LanJson.Get(o, "p"));
				if (seat.Zone != null && seat.Obj != null && seat.P != null)
				{
					seats.Add(seat);
				}
			}
			foreach (object o in (LanJson.Get(root, "props") as List<object>) ?? new List<object>())
			{
				Dispenser d = new Dispenser();
				d.Zone = LanJson.Get(o, "zone") as string;
				d.Obj = LanJson.Get(o, "obj") as string;
				d.P = Vec(LanJson.Get(o, "p"));
				d.Kind = LanJson.Get(o, "kind") as string;
				d.Name = LanJson.Get(o, "name") as string;
				if (d.Zone != null && d.P != null && d.Kind != null && d.Name != null)
				{
					dispensers.Add(d);
				}
			}
		}

		private void Save()
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			try
			{
				Dictionary<string, object> zones = new Dictionary<string, object>();
				foreach (KeyValuePair<string, LanBotMap> kv in maps)
				{
					if (kv.Value.Nodes.Count > 0)
					{
						zones[kv.Key] = kv.Value.ToJson();
					}
					kv.Value.Dirty = false;
				}
				usesDirty = false;
				List<object> rideList = rides.Select((Ride x) => (object)new Dictionary<string, object>
				{
					{ "zone", x.Zone },
					{ "obj", x.Obj },
					{ "start", VecJson(x.Start) },
					{ "end", VecJson(x.End) },
					{ "ms", x.DurationMs },
					{ "zip", x.Zipline ? 1L : 0L }
				}).ToList();
				List<object> seatList = seats.Select((Seat x) => (object)new Dictionary<string, object>
				{
					{ "zone", x.Zone },
					{ "obj", x.Obj },
					{ "p", VecJson(x.P) }
				}).ToList();
				List<object> propList = dispensers.Select((Dispenser x) => (object)new Dictionary<string, object>
				{
					{ "zone", x.Zone },
					{ "obj", x.Obj },
					{ "p", VecJson(x.P) },
					{ "kind", x.Kind },
					{ "name", x.Name }
				}).ToList();
				string json = LanJson.Serialize(new Dictionary<string, object>
				{
					{
						"rides",
						rideList
					},
					{
						"props",
						propList
					},
					{
						"seats",
						seatList
					},
					{
						"mapVersion",
						(long)MapVersion
					},
					{
						"zones",
						zones
					}
				});
				string tmp = path + ".tmp";
				File.WriteAllText(tmp, json, Encoding.UTF8);
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				File.Move(tmp, path);
			}
			catch (Exception)
			{
			}
		}
	}

	public sealed class LanBot
	{
		private const byte ActMove = 0;

		private const byte ActJump = 1;

		private const byte ActInteract = 2;

		private const byte ActCharge = 3;

		private const byte ActLaunch = 4;

		private const byte ActTorpedo = 5;

		private const byte ActAction1 = 7;

		private const byte ActAction2 = 8;

		private const byte ActAction3 = 9;

		private const byte ActCancelThrow = 10;

		private const byte ActRotate = 12;

		public const int DanceTemplate = 3;

		public const string DanceZone = "Town";

		private const float DanceSlotY = 0.44f;

		private static readonly float[][] DanceSlots = new float[20][]
		{
			new float[4] { 14.52f, -9.83f, -0.786f, 0.619f },
			new float[4] { 14.56f, -11.36f, -0.766f, 0.643f },
			new float[4] { 16.09f, -11.46f, -0.763f, 0.646f },
			new float[4] { 16.11f, -12.75f, -0.786f, 0.618f },
			new float[4] { 13.24f, -10.04f, -0.682f, 0.731f },
			new float[4] { 15.50f, -10.51f, -0.764f, 0.645f },
			new float[4] { 13.76f, -9.05f, -0.763f, 0.646f },
			new float[4] { 13.92f, -10.95f, -0.795f, 0.607f },
			new float[4] { 15.45f, -12.18f, -0.780f, 0.626f },
			new float[4] { 18.29f, -11.90f, -0.742f, 0.670f },
			new float[4] { 15.29f, -8.90f, -0.750f, 0.662f },
			new float[4] { 17.13f, -8.58f, -0.804f, 0.594f },
			new float[4] { 17.28f, -9.93f, -0.851f, 0.525f },
			new float[4] { 18.36f, -10.00f, -0.755f, 0.655f },
			new float[4] { 15.35f, -7.69f, -0.873f, 0.488f },
			new float[4] { 16.31f, -9.60f, -0.787f, 0.617f },
			new float[4] { 14.47f, -8.21f, -0.798f, 0.602f },
			new float[4] { 16.51f, -8.06f, -0.775f, 0.632f },
			new float[4] { 17.90f, -9.07f, -0.767f, 0.642f },
			new float[4] { 17.60f, -10.86f, 0.591f, 0.806f }
		};

		private static double WalkSpeed => LanBotRunParams.JogSpeed;

		private const double Gravity = -9.81;

		private static readonly string[] Greetings = new string[8]
		{
			"Hi!",
			"Hello!",
			"Hey everyone!",
			"Waddle on!",
			"Hi there!",
			"What's up?",
			"Hey!",
			"Howdy!"
		};

		private static readonly string[] IdleChat = new string[16]
		{
			"This place is cool!",
			"Anyone want to play?",
			"Let's go sledding!",
			"I love this island!",
			"Nice outfit!",
			"Who wants to race?",
			"Dance battle soon!",
			"Let's play Find Four!",
			"I'm hungry for pizza",
			"Have you seen Rookie?",
			"Crate Co is so fun",
			"Brr, it's cold!",
			"Let's explore!",
			"Want to be friends?",
			"This music is great",
			"Waddle waddle"
		};

		private static readonly string[] AfterGame = new string[5]
		{
			"Good game!",
			"GG!",
			"That was fun!",
			"Again?",
			"Well played!"
		};

		private static readonly string[] HowAreYou = new string[4]
		{
			"Great, thanks!",
			"I'm good!",
			"Doing awesome!",
			"Pretty good, you?"
		};

		private static readonly string[] Agree = new string[5]
		{
			"Yeah!",
			"Sure!",
			"Let's go!",
			"Ok!",
			"Totally!"
		};

		private static readonly string[] HostTypes = new string[3]
		{
			"FindFourGroup",
			"FishBucketGroup",
			"ScavengerHuntGroup"
		};

		public readonly LanPlayer Player;

		public int HeadingHolder;

		public LanPlayer Leader;

		private long stayUntil;

		private string hopRoom;

		private float[] hopAt3;

		private bool danceTrip;

		private long leaveUntil;

		private bool sprinting;

		private bool sprintSkid;

		private long sprintOffAt;

		private double sprintBase;

		private LanBots.Portal leavePortal;

		private float[] leaveExit;

		private LanBots.Ride ride;

		private long rideEndsAt;

		private LanBots.Seat seat;

		private long standAt;

		private long followAt;

		private bool tubing;

		private long tubeUntil;

		private long tubeRetryAt;

		private double tubeVel;

		private static double TubeSpeed => LanBotRunParams.SlideManualSpeed;

		private bool runStyle;

		private double runMult = 1.0;

		private readonly LanBots bots;

		private readonly List<float[]> route = new List<float[]>();

		private readonly Dictionary<string, long> decided = new Dictionary<string, long>();

		private float[] pos = new float[3];

		private float[] facing = new float[3]
		{
			0f,
			0f,
			1f
		};

		private double speed = WalkSpeed;

		private bool moving;

		private bool sliding;

		private Action arrived;

		private string mode = "idle";

		private long modeSince;

		private long busyUntil;

		private long nextScan;

		private long nextHop;

		private long noHumansSince = -1;

		private int lobbyItem;

		private long lastMemberAt;

		private LanPlayer holder;

		private int holderRetries;

		private string hostType;

		private bool ownerStartSent;

		private long hostGoneAt;

		private string targetsKey;

		private long nextThrow;

		private long launchAt;

		private float[] launchVelocity;

		private long targetsSeen;

		private long trayAt;

		private long nextTorpedoAt;

		private long torpedoUntil;

		private string chatText;

		private int chatSizzle;

		private long chatAt;

		private long nextChatAllowed;

		private long aimAt;

		private int lobbyTemplate;

		private bool slotPlaced;

		private LanPlayer hopTarget;

		private long hopAt;

		private int gameId;





		private int danceRound = -1;

		private readonly List<KeyValuePair<long, int>> danceMoves = new List<KeyValuePair<long, int>>();

		private int fourSeq = -1;

		private long fourAt;

		private int fishStep = -1;

		private long fishAt;

		private bool fishPlaced;

		private List<float[]> huntDrops;

		private int huntIndex;

		private long huntNext;

		private long huntFindStart;

		private int huntTarget = -1;

		private float[] huntHome;

		private readonly HashSet<int> huntSent = new HashSet<int>();

		private LanBots.Dispenser gear;

		private long gearUntil;

		private float[] gearArea;

		private bool gearInside;

		private long heldFreeAt;

		private string snack;

		private long snackAt;

		private long snackRollAt;

		private bool snackUsed;

		private static readonly string[] Snacks = new string[32] { "PizzaSingle", "SpicyPizzaSingle", "PizzaPlanetSingle", "HotChocolateSingle", "SquidStickSingle", "SeaweedSmoothieSingle", "RainbowSmoothieSingle", "FishdogSingle", "StinkyCheeseSingle", "CandyCaneSingle", "FluffyCakeSingle", "IceCreamSingle", "Popscorn", "BirthdayCakeSingle", "20thBirthdayCakeSingle", "ZootopiaPawpsicle", "FindingDoryBubbleWand", "MonstersScareCanister", "FrozenSnowflake", "FireHydrant", "Firework", "SuperFirework", "HolidayFirework", "GlowStickYellow", "GlowStickPink", "GlowStickRainbow", "GlowStickMint", "HalloweenBasketOrange", "HalloweenBasketPurple", "HalloweenBasketGreen", "Science_BeakerSingle", "Science_BeakerGreenSingle" };

		private static readonly HashSet<string> HeldSnacks = new HashSet<string> { "PizzaPlanetSingle" };

		private static readonly string[] Trays = new string[23] { "BirthdayCakeGroup", "HotChocolateGroup", "PizzaGroup", "SquidStickGroup", "StinkyCheeseTray", "FishdogGroup", "GlowstickPack", "PizzaPlanetGroup", "SeaweedSmoothieTray", "SpicyPizzaGroup", "HalloweenBasketTray", "HotChocolateGroupNonMember", "PizzaGroupNonMember", "SquidStickGroupNonMember", "CandyCane", "FluffyCakeTray", "RainbowSmoothieTray", "BirthdayCakeGroupNonMember", "IceCreamTray", "PopscornTray", "20thBirthdayCakeGroup", "Science_BeakerGroup", "Science_BeakerGreenGroup" };

		private static readonly HashSet<string> TraySet = new HashSet<string>(Trays);

		private bool snackHeld;

		private long gearActionAt;

		private long gearActedAt;

		private float[] followGoal;

		private bool gearRetried;

		public LanBot(LanBots bots, LanPlayer player)
		{
			this.bots = bots;
			Player = player;
		}

		private LanServer Server
		{
			get
			{
				return bots.Server;
			}
		}

		private Random R
		{
			get
			{
				return bots.Server.Random;
			}
		}

		public bool IsHosting
		{
			get
			{
				return mode == "host";
			}
		}

		private string Zone
		{
			get
			{
				return LanServer.ZoneOfRoom(Player.RoomKey);
			}
		}

		private LanBotMap Map
		{
			get
			{
				return bots.MapOf(Zone);
			}
		}

		public void Arrived(long now, float[] at)
		{
			pos = LanMotion.Copy(at);
			route.Clear();
			moving = false;
			arrived = null;
			sliding = false;
			SetMode("idle", now);
			busyUntil = now + 1000 + R.Next(3000);
			nextHop = now + 90000 + R.Next(150000);
			if (R.NextDouble() < 0.2)
			{
				Say(Pick(Greetings), now);
			}
		}

		public void Reset(long now)
		{
			PutAway();
			followGoal = null;
			StandUp(now);
			ride = null;
			EndSprint();
			EndTube(now);
			if (launchAt > 0 || aimAt > 0)
			{
				launchAt = 0;
				aimAt = 0;
				Send(ActCancelThrow, null, null, null);
			}
			route.Clear();
			moving = false;
			arrived = null;
			sliding = false;
			bots.Unhead(this);
			SetMode("idle", now);
			busyUntil = now + 2000;
		}

		private void SetMode(string next, long now)
		{
			mode = next;
			modeSince = now;
		}

		private string EmoteString(int count)
		{
			int[] list = bots.Emotes;
			if (list.Length == 0)
			{
				return "";
			}
			string text = "";
			for (int i = 0; i < count; i++)
			{
				text += ((char)list[R.Next(list.Length)]).ToString();
			}
			return text;
		}

		private string StripEmotes(string text)
		{
			int[] list = bots.Emotes;
			StringBuilder sb = new StringBuilder();
			foreach (char c in text)
			{
				if (Array.IndexOf(list, (int)c) < 0)
				{
					sb.Append(c);
				}
			}
			return sb.ToString().Trim();
		}

		private void Say(string text, long now)
		{
			Say(text, 0, now);
		}

		private void Say(string text, int sizzle, long now)
		{
			if (chatAt > 0 || now < nextChatAllowed || string.IsNullOrEmpty(text))
			{
				return;
			}
			if (!bots.EmotesEnabled)
			{
				text = StripEmotes(text);
				sizzle = 0;
			}
			if (!bots.ChatEnabled)
			{
				if (!bots.EmotesEnabled)
				{
					return;
				}
				text = EmoteString(1 + R.Next(2));
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			chatText = text;
			chatSizzle = sizzle;
			chatAt = now + 900 + R.Next(1600);
			nextChatAllowed = now + 6000;
			Server.BotChatActivity(Player);
		}

		private void Chatter(long now)
		{
			double roll = R.NextDouble();
			int[] clips = bots.Sizzles;
			if (roll < 0.45)
			{
				string text = Pick(IdleChat);
				if (bots.Emotes.Length > 0 && R.NextDouble() < 0.35)
				{
					text += " " + EmoteString(1);
				}
				Say(text, now);
			}
			else if (roll < 0.75 && bots.Emotes.Length > 0)
			{
				Say(Pick(IdleChat) + " " + EmoteString(1 + R.Next(2)), now);
			}
			else if (clips.Length > 0)
			{
				Say(Pick(Agree), clips[R.Next(clips.Length)], now);
			}
			else
			{
				Say(Pick(IdleChat), now);
			}
		}

		private void ChatTick(long now)
		{
			if (chatAt > 0 && now >= chatAt)
			{
				chatAt = 0;
				Server.BotChat(Player, chatText, chatSizzle);
			}
		}

		private static readonly HashSet<string> GreetingWords = new HashSet<string>
		{
			"hi",
			"hii",
			"hey",
			"heya",
			"hello",
			"hiya",
			"howdy",
			"yo",
			"sup",
			"greetings"
		};

		private static bool IsGreeting(string message)
		{
			string[] words = message.Split(new char[6] { ' ', '!', '.', ',', '?', '\'' }, StringSplitOptions.RemoveEmptyEntries);
			return words.Length > 0 && GreetingWords.Contains(words[0]);
		}

		public bool Hear(string message, long now)
		{
			string m = message.ToLowerInvariant();
			string reply = null;
			if (m.Contains("how are you") || m.Contains("how r u") || m.Contains("hru"))
			{
				reply = Pick(HowAreYou);
			}
			else if (m.Contains("good game") || m == "gg" || m.StartsWith("gg ", StringComparison.Ordinal))
			{
				reply = Pick(AfterGame);
			}
			else if (IsGreeting(m))
			{
				reply = Pick(Greetings);
			}
			else if (m.Split(new char[4] { ' ', '!', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Any((string w) => w == "bot" || w == "bots"))
			{
				reply = "Beep boop! " + EmoteString(1);
			}
			else if (m.Contains("dance") || m.Contains("race") || m.Contains("play") || m.Contains("join"))
			{
				reply = Pick(Agree);
			}
			if (reply == null || R.NextDouble() > 0.6 || chatAt > 0 || now < nextChatAllowed)
			{
				return false;
			}
			Say(reply.Trim(), now);
			return true;
		}

		public void PlanDance(string roomName, float[] lobbyAt, long until, long now)
		{
			if (!Player.Registered || mode == "hopping" || mode == "lobby" || mode == "game")
			{
				return;
			}
			LanPartyGameSession session = Server.PartyGames.SessionOf(Player);
			if (session != null && !session.Ended)
			{
				return;
			}
			stayUntil = until;
			danceTrip = true;
			followAt = 0;
			if (LanServer.RoomKeyOf(roomName) == Player.RoomKey)
			{
				return;
			}
			Reset(now);
			EndTube(now);
			bots.Unhead(this);
			Server.BotLeave(Player);
			route.Clear();
			moving = false;
			arrived = null;
			chatAt = 0;
			hopTarget = null;
			hopRoom = roomName;
			hopAt3 = bots.ArrivalNear(LanServer.ZoneOfRoom(LanServer.RoomKeyOf(roomName)), lobbyAt);
			hopAt = now + 1000 + R.Next(6000);
			SetMode("hopping", now);
		}

		private bool FollowLeader(long now)
		{
			if (Leader == null || Leader.IsBot || !Leader.Registered)
			{
				Leader = bots.PickLeader();
				followAt = 0;
			}
			if (FollowOn)
			{
				LanPlayer local = bots.LocalHuman();
				if (local != null && Leader != local)
				{
					Leader = local;
					followAt = 0;
				}
			}
			if (now < stayUntil || mode == "lobby")
			{
				followAt = 0;
				return false;
			}
			danceTrip = false;
			if (Leader == null || Leader.RoomKey == null || Leader.RoomKey == Player.RoomKey || LanBots.IsBotBlocked(Leader.RoomName))
			{
				followAt = 0;
				return false;
			}
			float[] exit = LanBots.IsIgloo(Player.RoomName) ? null : bots.ExitSpot(Leader, Player.RoomKey);
			if (exit == null && !LanBots.IsIgloo(Player.RoomName) && bots.InPlace(Leader))
			{
				LanBots.Portal known = bots.PortalFor(LanServer.ZoneOfRoom(Player.RoomKey), LanServer.ZoneOfRoom(Leader.RoomKey));
				if (known != null)
				{
					exit = known.Exit;
				}
			}
			if (exit == null && (!bots.InPlace(Leader) || !Leader.Placed))
			{
				followAt = 0;
				return false;
			}
			if (followAt == 0)
			{
				if (FollowOn)
				{
					followAt = now + ((exit != null) ? (200 + R.Next(600)) : (500 + R.Next(1500)));
				}
				else
				{
					followAt = now + ((exit != null) ? (300 + R.Next(1500)) : (1500 + R.Next(5000)));
				}
				return false;
			}
			if (now < followAt)
			{
				return false;
			}
			followAt = 0;
			if (exit != null && HDist(pos, exit) <= 80.0)
			{
				PutAway();
				followGoal = null;
				StandUp(now);
				EndTube(now);
				if (launchAt > 0 || aimAt > 0)
				{
					launchAt = 0;
					aimAt = 0;
					Send(ActCancelThrow, null, null, null);
				}
				bots.Unhead(this);
				leaveExit = LanMotion.Copy(exit);
				leaveUntil = now + 8000 + (long)(HDist(pos, exit) / (WalkSpeed * 1.4) * 1600.0);
				SetMode("leaving", now);
				GoTo(Near(exit, 0.6), WalkSpeed * 1.4, delegate
				{
					moving = false;
				}, true);
				return true;
			}
			return HopTo(Leader, now);
		}

		private bool FollowOn
		{
			get
			{
				if (!bots.FollowEnabled)
				{
					return false;
				}
				return !HostBusy;
			}
		}

		private bool HostBusy
		{
			get
			{
				LanPlayer host = bots.LocalHuman() ?? Leader;
				return host != null && (Server.Lobbies.IsMember(host) || Server.PartyGames.SessionOf(host) != null);
			}
		}

		private static bool IsCannon(LanBots.Ride ride)
		{
			return ride != null && (!ride.Zipline || (ride.Obj ?? "").IndexOf("cannon", StringComparison.OrdinalIgnoreCase) >= 0);
		}

		private bool LeaderHere()
		{
			LanPlayer l = Leader;
			return l != null && !l.IsBot && l.Registered && l.RoomKey != null && l.RoomKey == Player.RoomKey && l.LastPosition != null && bots.InPlace(l);
		}

		private bool FollowHere(long now)
		{
			if (!FollowOn || tubing || !LeaderHere())
			{
				followGoal = null;
				return false;
			}
			float[] at = Leader.LastPosition;
			double d = HDist(pos, at);
			if (moving && followGoal != null)
			{
				if (HDist(followGoal, at) <= 3.0)
				{
					return true;
				}
			}
			else if (d <= 4.5)
			{
				return false;
			}
			followGoal = Near(at, 2.5);
			if (mode != "prop" && mode != "follow")
			{
				SetMode("follow", now);
			}
			GoTo(followGoal, (d > 12.0) ? (WalkSpeed * 1.4) : WalkSpeed, delegate
			{
				followGoal = null;
				if (mode == "follow")
				{
					long t = bots.Now;
					SetMode("idle", t);
					busyUntil = t + 500 + R.Next(1500);
				}
			}, true);
			return true;
		}

		private bool TryProp(long now)
		{
			if (!bots.PropsEnabled || tubing || gear != null || snack != null || now < heldFreeAt || Player.HeldObject != null || Player.EquippedConsumable != null || R.NextDouble() > 0.3)
			{
				return false;
			}
			bool stayClose = FollowOn && LeaderHere();
			LanBots.Dispenser pick = bots.PickDispenser(LanServer.ZoneOfRoom(Player.RoomKey), stayClose ? Leader.LastPosition : pos, stayClose ? 25.0 : 1000.0, R);
			if (pick == null)
			{
				return false;
			}
			gear = pick;
			gearArea = LanBots.AreaFor(pick.Zone, pick.Name, pick.P);
			gearInside = false;
			gearUntil = 0;
			gearRetried = false;
			SetMode("prop", now);
			GoTo(pick.P, WalkSpeed, delegate
			{
				PickUpGear(bots.Now);
			}, true);
			return true;
		}

		private void PickUpGear(long now)
		{
			if (gear != null && Player.Registered && Player.HeldObject == null && Player.EquippedConsumable == null && HDist(pos, gear.P) > 2.5)
			{
				double gap = HDist(pos, gear.P);
				if (!gearRetried && gap <= 15.0)
				{
					gearRetried = true;
					GoTo(gear.P, WalkSpeed, delegate
					{
						PickUpGear(bots.Now);
					}, false);
					return;
				}
				if (gap <= 6.0)
				{
					pos[0] = gear.P[0];
					pos[2] = gear.P[2];
					Send(ActMove, new float[3], null, null);
				}
				else
				{
				}
			}
			if (gear == null || !Player.Registered || HDist(pos, gear.P) > 2.5 || Player.HeldObject != null || Player.EquippedConsumable != null || snack != null || bots.Now < heldFreeAt)
			{
				gear = null;
				gearUntil = 0;
				if (mode == "prop")
				{
					SetMode("idle", now);
					busyUntil = now + 1500 + R.Next(2000);
				}
				return;
			}
			EndSprint();
			Player.EquippedItemName = gear.Name;
			Server.Consumables.EquipObject(Player, new LanEquippedObject(gear.Kind, gear.Name));
			gearUntil = now + 30000 + R.Next(60000);
			gearActionAt = now + 3000 + R.Next(2500);
			gearActedAt = 0;
		}

		private bool TrySnack(long now)
		{
			return TrySnack(now, 0.08);
		}

		private bool TrySnack(long now, double chance)
		{
			if (!bots.PropsEnabled || gear != null || snack != null || now < heldFreeAt || Player.HeldObject != null || Player.EquippedConsumable != null || Server.Lobbies.IsMember(Player) || Server.PartyGames.SessionOf(Player) != null || R.NextDouble() > chance)
			{
				return false;
			}
			snack = (R.NextDouble() < 0.25) ? Trays[R.Next(Trays.Length)] : Snacks[R.Next(Snacks.Length)];
			snackHeld = HeldSnacks.Contains(snack) || TraySet.Contains(snack);
			snackUsed = false;
			snackAt = now + 2500 + R.Next(2500);
			if (snackHeld)
			{
				Server.Consumables.UseAsBot(Player, snack);
				snackUsed = true;
				snackAt = now + 6000 + R.Next(6000);
				return false;
			}
			Player.EquippedConsumable = snack;
			Player.HeldObject = snack;
			Server.BroadcastUserVars(Player, LanServer.ConVar("consum", snack));
			return false;
		}

		private void SnackTick(long now)
		{
			if (snack == null)
			{
				if (now < snackRollAt)
				{
					return;
				}
				snackRollAt = now + 15000 + R.Next(30000);
				if (mode == "idle" || mode == "wander" || mode == "follow" || mode == "seated")
				{
					TrySnack(now, 0.3);
				}
				return;
			}
			if ((!snackUsed || snackHeld) && Player.EquippedConsumable != snack)
			{
				snack = null;
				snackHeld = false;
				return;
			}
			if (now < snackAt)
			{
				return;
			}
			if (snackHeld)
			{
				if (HoldingTray())
				{
					return;
				}
				PutAway();
				heldFreeAt = now + 5000;
				return;
			}
			if (!snackUsed)
			{
				snackUsed = true;
				Server.Consumables.UseAsBot(Player, snack);
				heldFreeAt = now + 7000;
				snackAt = now + 4000 + R.Next(2000);
				return;
			}
			snack = null;
		}

		private bool HoldingTray()
		{
			return snack != null && TraySet.Contains(snack) && Player.Registered && Player.EquippedConsumable == snack && Player.HeldActionCount.GetValueOrDefault() > 0;
		}

		private void PutAway()
		{
			if (HoldingTray())
			{
				return;
			}
			if (snack != null)
			{
				string s = snack;
				bool held = snackHeld;
				snack = null;
				snackHeld = false;
				if (Player.Registered && Player.EquippedConsumable == s)
				{
					Player.EquippedItemName = null;
					Player.EquippedConsumable = null;
					Player.HeldObject = null;
					if (held)
					{
						Player.HeldActionCount = null;
						Server.BroadcastUserVars(Player, LanServer.NullVar("con"), LanServer.NullVar("con_prop"));
					}
					else
					{
						Server.BroadcastUserVars(Player, LanServer.NullVar("con"));
					}
					heldFreeAt = bots.Now + 3000;
				}
			}
			gear = null;
			gearArea = null;
			gearUntil = 0;
			if (!Player.Registered || Player.HeldObject == null || Player.EquippedConsumable != null || Server.PartyGames.SessionOf(Player) != null)
			{
				return;
			}
			Player.EquippedItemName = null;
			Player.HeldObject = null;
			Server.BroadcastUserVars(Player, LanServer.NullVar("con"));
			heldFreeAt = bots.Now + 3000;
		}

		private void PropTick(long now)
		{
			if (gear != null && !bots.PropsEnabled)
			{
				if (gearUntil != 0)
				{
					PutAway();
				}
				gear = null;
				gearUntil = 0;
				Stop();
				SetMode("idle", now);
				busyUntil = now + 1000;
				return;
			}
			if (gear == null)
			{
				SetMode("idle", now);
				busyUntil = now + 1000;
				return;
			}
			if (gearUntil == 0)
			{
				if (!moving && now - modeSince > 60000)
				{
					gear = null;
					SetMode("idle", now);
					busyUntil = now + 1500;
				}
				return;
			}
			if (Player.HeldObject != gear.Name || (gearArea != null && gearInside && !LanBots.InArea(gearArea, pos)))
			{
				if (Player.HeldObject == gear.Name && now - gearActedAt < 3000)
				{
					return;
				}
				PutAway();
				SetMode("idle", now);
				busyUntil = now + 1500;
				return;
			}
			if (gearArea != null && !gearInside)
			{
				if (LanBots.InArea(gearArea, pos))
				{
					gearInside = true;
				}
				else
				{
					if (!moving)
					{
						GoTo(LanBots.RandomInArea(gearArea, pos[1], R), WalkSpeed, null, true);
					}
					return;
				}
			}
			if (now >= gearUntil)
			{
				if (now - gearActedAt < 3000)
				{
					return;
				}
				Stop();
				PutAway();
				SetMode("idle", now);
				busyUntil = now + 2000 + R.Next(3000);
				return;
			}
			if (FollowHere(now) || moving)
			{
				return;
			}
			if (now >= gearActionAt && gearUntil - now > 4000)
			{
				gearActedAt = now;
				double roll = R.NextDouble();
				Send((roll < 0.7) ? ActAction1 : ((roll < 0.9) ? ActAction2 : ActAction3), null, null, null);
				gearActionAt = now + 2500 + R.Next(5500);
				return;
			}
			if (!FollowOn && R.NextDouble() < 0.03)
			{
				float[] step = (gearArea != null) ? LanBots.RandomInArea(gearArea, pos[1], R) : (Map.RandomNear(pos, 4.0, R) ?? Near(pos, 3.0));
				GoTo(step, WalkSpeed, null, true);
			}
		}

		private void ExitThrough(long now)
		{
			LanPlayer target = Leader;
			if (target != null && target.Registered && !bots.InPlace(target) && now < leaveUntil + 20000)
			{
				return;
			}
			leaveExit = null;
			if (target == null || !bots.InPlace(target) || target.RoomKey == Player.RoomKey || LanBots.IsBotBlocked(target.RoomName))
			{
				SetMode("idle", now);
				busyUntil = now + 1000;
				return;
			}
			LanBots.Portal portal = bots.PortalFor(LanServer.ZoneOfRoom(Player.RoomKey), LanServer.ZoneOfRoom(target.RoomKey));
			float[] entry = (portal != null) ? Near(portal.Entry, 1.2) : bots.ArrivalPoint(target);
			route.Clear();
			moving = false;
			arrived = null;
			chatAt = 0;
			sprinting = false;
			sprintOffAt = 0;
			EndTube(now);
			Server.BotLeave(Player);
			hopTarget = null;
			hopRoom = target.RoomName;
			hopAt3 = entry;
			hopAt = now + 300 + R.Next(1500);
			SetMode("hopping", now);
		}


		private bool TryTube(long now)
		{
			if (tubing || !bots.TubesEnabled || R.NextDouble() > 0.06)
			{
				return false;
			}
			int[] tubeIds = bots.Tubes;
			if (tubeIds.Length > 0 && R.NextDouble() < 0.6)
			{
				int next = tubeIds[R.Next(tubeIds.Length)];
				if (next != Player.SelectedTubeId.GetValueOrDefault())
				{
					Player.SelectedTubeId = next;
					Server.BroadcastUserVars(Player, LanServer.IntVar("tube", next));
				}
			}
			float[] target = Map.RandomReachable(pos, 6.0, 25.0, R);
			if (target == null)
			{
				return false;
			}
			tubing = true;
			sliding = true;
			tubeUntil = now + 60000 + R.Next(60001);
			tubeRetryAt = 0;
			tubeVel = 0.0;
			EnsureTube();
			Server.PartyGames.SetLocomotion(Player, 1);
			SetMode("wander", now);
			float[] first = NextTubeTarget();
			TubeLeg(first ?? target);
			return true;
		}

		private void TubeLeg(float[] target)
		{
			float[] start = LanMotion.Copy(pos);
			LanBotGrid grid = Grid();
			bool path = grid == null || !grid.Clear(pos, target);
			GoTo(target, TubeSpeed, delegate
			{
				long t = bots.Now;
				if (!tubing)
				{
					return;
				}
				if (t >= tubeUntil)
				{
					EndTube(t);
					SetMode("idle", t);
					busyUntil = t + 1500 + R.Next(4000);
					return;
				}
				if (HDist(start, pos) < 1.5)
				{
					tubeRetryAt = t + 1000 + R.Next(2000);
					return;
				}
				float[] next = NextTubeTarget();
				if (next != null)
				{
					TubeLeg(next);
					return;
				}
				tubeRetryAt = t + 1500;
			}, path);
		}

		private float[] NextTubeTarget()
		{
			LanBotGrid grid = Grid();
			if (grid == null)
			{
				return Map.RandomReachable(pos, 6.0, 25.0, R);
			}
			double heading = Math.Atan2(facing[2], facing[0]);
			double[] cones = { Math.PI / 4.0, Math.PI / 2.0, Math.PI * 0.85 };
			foreach (double cone in cones)
			{
				for (int tries = 0; tries < 14; tries++)
				{
					double a = heading + (R.NextDouble() * 2.0 - 1.0) * cone;
					double d = 8.0 + R.NextDouble() * 14.0;
					float[] p = new float[3]
					{
						pos[0] + (float)(Math.Cos(a) * d),
						pos[1],
						pos[2] + (float)(Math.Sin(a) * d)
					};
					if (grid.Walkable(p) && grid.Clear(pos, p))
					{
						return p;
					}
				}
			}
			return null;
		}

		private void EndTube(long now)
		{
			if (!tubing)
			{
				return;
			}
			tubing = false;
			sliding = false;
			if (Player.Registered)
			{
				Server.PartyGames.SetLocomotion(Player, 0);
			}
		}

		private bool TryRide(long now)
		{
			if (tubing || R.NextDouble() > 0.2)
			{
				return false;
			}
			LanBots.Ride pick = bots.PickRide(LanServer.ZoneOfRoom(Player.RoomKey), pos, R);
			if (pick == null || (IsCannon(pick) && HostBusy))
			{
				return false;
			}
			SetMode("wander", now);
			GoTo(pick.Start, WalkSpeed, delegate
			{
				StartRide(pick, bots.Now);
			}, true);
			return true;
		}

		private void StartRide(LanBots.Ride pick, long now)
		{
			if (IsCannon(pick) && HostBusy)
			{
				SetMode("idle", now);
				busyUntil = now + 1500;
				return;
			}
			if (!Player.Registered || HDist(pos, pick.Start) > 6.0)
			{
				SetMode("idle", now);
				return;
			}
			EndSprint();
			ride = pick;
			pos = LanMotion.Copy(pick.Start);
			Send(ActInteract, null, null, pick.Obj);
			if (pick.Zipline)
			{
				Server.PartyGames.SetLocomotion(Player, 6);
			}
			rideEndsAt = now + Math.Max(1500, pick.DurationMs);
			SetMode("riding", now);
		}

		private void RideTick(long now)
		{
			if (ride == null || now < rideEndsAt)
			{
				return;
			}
			LanBots.Ride done = ride;
			ride = null;
			if (done.Zipline && Player.LocomotionState == 6)
			{
				Server.PartyGames.SetLocomotion(Player, 0);
			}
			pos = LanMotion.Copy(done.End);
			Send(ActMove, new float[3], null, null);
			SetMode("idle", now);
			busyUntil = now + 1500 + R.Next(3000);
		}

		private bool TrySeat(long now)
		{
			if (tubing || seat != null || R.NextDouble() > 0.2)
			{
				return false;
			}
			LanBots.Seat pick = bots.PickSeat(LanServer.ZoneOfRoom(Player.RoomKey), pos, R, this);
			if (pick == null)
			{
				return false;
			}
			seat = pick;
			SetMode("wander", now);
			GoTo(pick.P, WalkSpeed, delegate
			{
				SitDown(bots.Now);
			}, true);
			return true;
		}

		private void SitDown(long now)
		{
			if (seat == null || !Player.Registered || HDist(pos, seat.P) > 6.0)
			{
				StandUp(now);
				SetMode("idle", now);
				return;
			}
			EndSprint();
			pos = LanMotion.Copy(seat.P);
			Send(ActInteract, null, null, seat.Obj);
			Server.PartyGames.SetLocomotion(Player, 5);
			standAt = now + 15000 + R.Next(45000);
			SetMode("seated", now);
		}

		private void SeatTick(long now)
		{
			if (now < standAt)
			{
				return;
			}
			StandUp(now);
			float[] away = Map.RandomNear(pos, 5.0, R) ?? Near(pos, 3.0);
			SetMode("wander", now);
			GoTo(away, WalkSpeed, delegate
			{
				long t = bots.Now;
				SetMode("idle", t);
				busyUntil = t + 1000 + R.Next(3000);
			}, true);
		}

		private void StandUp(long now)
		{
			if (seat == null)
			{
				return;
			}
			seat = null;
			bots.ReleaseSeat(this);
			if (Player.Registered && Player.LocomotionState == 5)
			{
				Server.PartyGames.SetLocomotion(Player, 0);
			}
		}

		private void Tidy(long now)
		{
			if (now - modeSince < 4000 || snack != null || Server.Lobbies.IsMember(Player))
			{
				return;
			}
			if (Player.EquippedConsumable != null)
			{
				Server.Consumables.HandleClientConChanged(Player, null);
			}
			else if (Player.HeldObject != null)
			{
				Server.PartyGames.SetProp(Player, null);
			}
			if (Player.LocomotionState != 0)
			{
				Server.PartyGames.SetLocomotion(Player, 0);
			}
			sliding = false;
		}

		private string Pick(string[] list)
		{
			return list[R.Next(list.Length)];
		}

		private bool Decide(string key, long now, long ttl)
		{
			long at;
			if (decided.TryGetValue(key, out at) && now - at < ttl)
			{
				return true;
			}
			if (decided.Count > 400)
			{
				decided.Clear();
			}
			decided[key] = now;
			return false;
		}

		private static double HDist(float[] a, float[] b)
		{
			double dx = a[0] - b[0];
			double dz = a[2] - b[2];
			return Math.Sqrt(dx * dx + dz * dz);
		}

		private static float[] Flat(float[] d)
		{
			double len = Math.Sqrt(d[0] * d[0] + d[2] * d[2]);
			if (len < 0.0001)
			{
				return new float[3];
			}
			return new float[3]
			{
				(float)(d[0] / len),
				0f,
				(float)(d[2] / len)
			};
		}

		private static float[] Unit(float[] d)
		{
			double len = Math.Sqrt(d[0] * d[0] + d[1] * d[1] + d[2] * d[2]);
			if (len < 0.0001)
			{
				return new float[3]
				{
					0f,
					0f,
					1f
				};
			}
			return new float[3]
			{
				(float)(d[0] / len),
				(float)(d[1] / len),
				(float)(d[2] / len)
			};
		}

		private static float[] Sub(float[] a, float[] b)
		{
			return new float[3]
			{
				a[0] - b[0],
				a[1] - b[1],
				a[2] - b[2]
			};
		}

		private LanBotGrid Grid()
		{
			if (!Player.Registered || LanBots.IsIgloo(Player.RoomName))
			{
				return null;
			}
			LanPartyGameSession session = Server.PartyGames.SessionOf(Player);
			if (session != null && !session.Ended)
			{
				return null;
			}
			return bots.GridOf(LanServer.ZoneOfRoom(Player.RoomKey));
		}

		private float[] Near(float[] center, double radius)
		{
			LanBotGrid grid = Grid();
			for (int tries = 0; tries < 8; tries++)
			{
				float[] p = NearRaw(center, radius);
				if (grid == null || grid.Walkable(p))
				{
					return p;
				}
			}
			return LanMotion.Copy(center);
		}

		private float[] NearRaw(float[] center, double radius)
		{
			double a = R.NextDouble() * Math.PI * 2.0;
			double r = R.NextDouble() * radius;
			return new float[3]
			{
				center[0] + (float)(Math.Cos(a) * r),
				center[1],
				center[2] + (float)(Math.Sin(a) * r)
			};
		}

		private void Send(byte action, float[] direction, float[] velocity, string obj)
		{
			Server.BotLocomotion(Player, action, LanMotion.Copy(pos), direction, velocity, obj);
		}

		private void Interact(int type, string id, string tag)
		{
			Send(ActInteract, null, null, "{\"type\":" + type + ",\"id\":" + LanJson.Quote(id) + ",\"tag\":" + LanJson.Quote(tag) + "}");
		}

		private void GoTo(float[] target, double spd, Action then, bool usePath)
		{
			route.Clear();
			bool wasRunning = moving && runStyle;
			runStyle = spd >= WalkSpeed * 0.99 && spd <= WalkSpeed * 1.41;
			if (runStyle && !wasRunning)
			{
				runMult = LanBotRunParams.JogIntoSpeedMult;
			}
			speed = spd;
			arrived = then;
			moving = true;
			if (usePath)
			{
				List<float[]> path = Map.Path(pos, target);
				if (path != null)
				{
					route.AddRange(path);
				}
			}
			route.Add(LanMotion.Copy(target));
			MaybeSprint();
		}

		private void MaybeSprint()
		{
			if (!runStyle || sliding || tubing || speed <= 0.0 || (Player.LocomotionState != 0 && Player.LocomotionState != 8))
			{
				return;
			}
			LanPartyGameSession session = Server.PartyGames.SessionOf(Player);
			if (session != null && !session.Ended)
			{
				return;
			}
			double length = 0.0;
			float[] from = pos;
			foreach (float[] p in route)
			{
				length += HDist(from, p);
				from = p;
			}
			double chance = (mode == "leaving") ? 0.5 : 0.25;
			if (length < 9.0 || R.NextDouble() > chance)
			{
				if (sprinting)
				{
					EndSprint();
				}
				return;
			}
			if (mode == "wander")
			{
				ExtendSprintRoute(length, 25.0 + R.NextDouble() * 25.0);
			}
			sprintBase = speed;
			runMult = LanBotRunParams.SprintIntoSpeedMult;
			sprintSkid = R.NextDouble() < 0.6;
			sprintOffAt = 0;
			if (!sprinting)
			{
				sprinting = true;
				Server.PartyGames.SetLocomotion(Player, 8);
			}
		}

		private void ExtendSprintRoute(double length, double wanted)
		{
			for (int legs = 0; legs < 4 && length < wanted && route.Count > 0; legs++)
			{
				float[] end = route[route.Count - 1];
				float[] next = Map.RandomReachable(end, 8.0, 20.0, R);
				if (next == null)
				{
					return;
				}
				List<float[]> path = Map.Path(end, next);
				float[] from = end;
				if (path != null)
				{
					foreach (float[] p in path)
					{
						length += HDist(from, p);
						from = p;
					}
					route.AddRange(path);
				}
				length += HDist(from, next);
				route.Add(LanMotion.Copy(next));
			}
		}

		private void EndSprint()
		{
			sprintOffAt = 0;
			if (!sprinting)
			{
				return;
			}
			sprinting = false;
			speed = sprintBase;
			if (Player.Registered && Player.LocomotionState == 8)
			{
				Server.PartyGames.SetLocomotion(Player, 0);
			}
		}

		private void Stop()
		{
			route.Clear();
			arrived = null;
		}

		private void Move(double dt)
		{
			if (route.Count == 0)
			{
				if (moving)
				{
					moving = false;
					Send(ActMove, new float[3], sliding ? Unit(facing) : null, null);
					if (sprinting)
					{
						if (sprintSkid)
						{
							sprintOffAt = bots.Now + 900;
						}
						else
						{
							EndSprint();
						}
					}
					Action then = arrived;
					arrived = null;
					if (then != null)
					{
						then();
					}
				}
				return;
			}
			if (sprinting && !sprintSkid && route.Count == 1 && HDist(pos, route[0]) < 2.5)
			{
				EndSprint();
				speed = sprintBase;
			}
			if (tubing && sliding)
			{
				TubeMove(dt);
				return;
			}
			float[] facingBefore = LanMotion.Copy(facing);
			double left = RunSpeed(dt) * dt;
			if (LanBots.InWater(LanServer.ZoneOfRoom(Player.RoomKey), pos))
			{
				if (sprinting)
				{
					EndSprint();
				}
				double swim = LanBots.SwimSpeed;
				long clock = bots.Now;
				if (clock >= nextTorpedoAt && route.Count > 0 && HDist(pos, route[route.Count - 1]) > 4.0)
				{
					nextTorpedoAt = clock + 4000 + R.Next(6000);
					torpedoUntil = clock + 1200;
					Send(ActTorpedo, Flat(facing), null, null);
				}
				if (clock < torpedoUntil)
				{
					swim *= 2.0;
				}
				left = Math.Min(left, swim * dt);
			}
			LanBotGrid grid = Grid();
			if (grid != null && !grid.Walkable(pos))
			{
				grid = null;
			}
			if (grid != null && route.Count > 0 && LanBots.InWater(LanServer.ZoneOfRoom(Player.RoomKey), route[route.Count - 1]))
			{
				grid = null;
			}
			while (left > 0.0 && route.Count > 0)
			{
				float[] t = route[0];
				float[] delta = Sub(t, pos);
				double d = Math.Sqrt(delta[0] * delta[0] + delta[1] * delta[1] + delta[2] * delta[2]);
				if (d > 0.01)
				{
					facing = delta;
				}
				float[] step;
				bool reached = d <= left;
				if (reached)
				{
					step = LanMotion.Copy(t);
				}
				else
				{
					double k = left / d;
					step = new float[3]
					{
						pos[0] + (float)(delta[0] * k),
						pos[1] + (float)(delta[1] * k),
						pos[2] + (float)(delta[2] * k)
					};
				}
				if (grid != null && !grid.Clear(pos, step))
				{
					route.Clear();
					break;
				}
				float gy;
				if (grid != null && !sliding && grid.Ground(step, out gy) && Math.Abs(gy - step[1]) < 4f)
				{
					step[1] = gy;
				}
				pos = step;
				if (reached)
				{
					route.RemoveAt(0);
					left -= d;
				}
				else
				{
					left = 0.0;
				}
			}
			moving = true;
			ApplyTurnMomentum(facingBefore, dt);
			Send(ActMove, Flat(facing), sliding ? Unit(facing) : null, null);
		}

		private void TubeMove(double dt)
		{
			double top = LanBotRunParams.SlideManualSpeed;
			tubeVel = Math.Min(top, tubeVel + top / Math.Max(0.05, LanBotRunParams.SlideBuildMomentumTime) * dt);
			while (route.Count > 0 && HDist(pos, route[0]) < Math.Max(1.2, tubeVel * dt))
			{
				route.RemoveAt(0);
			}
			if (route.Count == 0)
			{
				return;
			}
			float[] want = Flat(Sub(route[0], pos));
			float[] cur = Flat(facing);
			if (cur[0] == 0f && cur[2] == 0f)
			{
				cur = want;
			}
			double k = Math.Min(1.0, dt * LanBotRunParams.SlideRotationSmoothing);
			float[] next = new float[3]
			{
				(float)(cur[0] + (want[0] - cur[0]) * k),
				0f,
				(float)(cur[2] + (want[2] - cur[2]) * k)
			};
			next = Flat(next);
			if (next[0] == 0f && next[2] == 0f)
			{
				next = new float[3] { -cur[2], 0f, cur[0] };
			}
			float[] step = new float[3]
			{
				pos[0] + (float)(next[0] * tubeVel * dt),
				pos[1],
				pos[2] + (float)(next[2] * tubeVel * dt)
			};
			LanBotGrid grid = Grid();
			if (grid != null && grid.Walkable(pos) && !grid.Clear(pos, step))
			{
				route.Clear();
				tubeVel = 0.0;
				return;
			}
			pos = step;
			facing = next;
			moving = true;
			Send(ActMove, next, Unit(next), null);
		}

		private double RunSpeed(double dt)
		{
			if (!runStyle || sliding || (Player.LocomotionState != 0 && Player.LocomotionState != 8))
			{
				return speed;
			}
			double accel = sprinting ? LanBotRunParams.SprintAcceleration : LanBotRunParams.JogAcceleration;
			runMult = Math.Min(1.0, runMult + accel * dt);
			double top = sprinting ? LanBotRunParams.SprintSpeed : Math.Min(speed, LanBotRunParams.JogSpeed);
			return top * runMult;
		}

		private void ApplyTurnMomentum(float[] before, double dt)
		{
			if (!runStyle || sliding || dt <= 0.0)
			{
				return;
			}
			double a = Math.Atan2(before[2], before[0]);
			double b = Math.Atan2(facing[2], facing[0]);
			double turn = Math.Abs(b - a) * 180.0 / Math.PI;
			if (turn > 180.0)
			{
				turn = 360.0 - turn;
			}
			if (turn >= LanBotRunParams.MinFacingAngleToResetMomentum * dt)
			{
				double cap = sprinting ? LanBotRunParams.SprintTurnSpeedMult : LanBotRunParams.JogTurnSpeedMult;
				runMult = Math.Min(runMult, cap);
			}
		}

		public void Tick(long now, double dt)
		{
			if (mode == "hopping")
			{
				HopTick(now);
				return;
			}
			if (!Player.Registered)
			{
				return;
			}
			ChatTick(now);
			SnackTick(now);
			if (tubing && (now > tubeUntil || !bots.TubesEnabled))
			{
				Stop();
				EndTube(now);
				if (mode == "wander")
				{
					SetMode("idle", now);
					busyUntil = now + 1500 + R.Next(4000);
				}
			}
			if (sprintOffAt > 0 && now >= sprintOffAt)
			{
				EndSprint();
			}
			if (mode == "leaving")
			{
				if (now >= leaveUntil || !moving)
				{
					if (moving)
					{
						Stop();
						moving = false;
					}
					ExitThrough(now);
				}
				else
				{
					Move(dt);
				}
				return;
			}
			if (mode == "riding")
			{
				RideTick(now);
				return;
			}
			if (mode == "seated")
			{
				if (Leader != null && Leader.Registered && Leader.RoomKey != null && Leader.RoomKey != Player.RoomKey && now >= stayUntil)
				{
					standAt = 0;
				}
				if (FollowOn && LeaderHere() && HDist(pos, Leader.LastPosition) > 6.0)
				{
					standAt = 0;
				}
				SeatTick(now);
				return;
			}
			LanPartyGameSession session = Server.PartyGames.SessionOf(Player);
			if ((session == null || session.Ended) && FollowLeader(now))
			{
				return;
			}
			if (session != null && !session.Ended)
			{
				if (gameId != session.Id)
				{
					BeginGame(session, now);
				}
				GameTick(session, now);
				Move(dt);
				return;
			}
			if (gameId != 0)
			{
				EndGame(now);
			}
			switch (mode)
			{
			case "lobby":
				LobbyTick(now);
				break;
			case "invite":
			case "guest":
				InviteTick(now);
				break;
			case "host":
				HostTick(now);
				break;
			case "targets":
			case "throwing":
				TargetsTick(now);
				break;
			case "tray":
				TrayTick(now);
				break;
			case "prop":
				PropTick(now);
				break;
			case "follow":
				if (!FollowHere(now) && !moving)
				{
					followGoal = null;
					SetMode("idle", now);
					busyUntil = now + 500;
				}
				break;
			default:
				IdleTick(now);
				break;
			}
			Move(dt);
		}

		private void IdleTick(long now)
		{
			if (tubing && now > tubeUntil)
			{
				Stop();
				EndTube(now);
				SetMode("idle", now);
			}
			if (!moving && !tubing)
			{
				Tidy(now);
			}
			if (mode == "wander" && now >= nextScan)
			{
				nextScan = now + 1500;
				if (!tubing && (TryLobby(now) || TryInvitation(now) || TryTargets(now)))
				{
					return;
				}
			}
			if (tubing)
			{
				if (!moving && now >= tubeRetryAt)
				{
					float[] next = NextTubeTarget() ?? Map.RandomReachable(pos, 4.0, 20.0, R);
					if (next != null)
					{
						TubeLeg(next);
					}
					else
					{
						tubeRetryAt = now + 1500;
					}
				}
				return;
			}
			if (moving || now < busyUntil)
			{
				return;
			}
			Think(now);
		}

		private void Think(long now)
		{
			if (gear != null || (Player.HeldObject != null && Player.EquippedConsumable == null))
			{
				PutAway();
			}
			if (LanBots.IsBotBlocked(Player.RoomName) && Hop(now, false))
			{
				return;
			}
			List<LanPlayer> here = Server.InRoom(Player.RoomKey);
			List<LanPlayer> humans = here.Where((LanPlayer p) => !p.IsBot).ToList();
			if (humans.Count == 0)
			{
				if (noHumansSince < 0)
				{
					noHumansSince = now;
				}
				if (now >= stayUntil && now - noHumansSince > 6000 && (Leader == null || !Leader.Registered) && Hop(now, false))
				{
					return;
				}
			}
			else
			{
				noHumansSince = -1;
			}
			if (FollowOn && LeaderHere())
			{
				if (FollowHere(now) || TryLobby(now) || TryInvitation(now) || TryTargets(now) || TryTray(now, here) || TryTube(now) || TryRide(now) || TrySeat(now) || TryProp(now) || TrySnack(now))
				{
					return;
				}
				busyUntil = now + 1500 + R.Next(3000);
				double idleRoll = R.NextDouble();
				if (idleRoll < 0.08)
				{
					Chatter(now);
				}
				else if (idleRoll < 0.25)
				{
					SetMode("follow", now);
					followGoal = Near(Leader.LastPosition, 3.0);
					GoTo(followGoal, WalkSpeed, delegate
					{
						followGoal = null;
						if (mode == "follow")
						{
							long t = bots.Now;
							SetMode("idle", t);
							busyUntil = t + 1000 + R.Next(2500);
						}
					}, true);
				}
				return;
			}
			if (TryLobby(now) || TryInvitation(now) || TryTargets(now) || TryTray(now, here) || TryTube(now) || TryRide(now) || TrySeat(now) || TryProp(now) || TrySnack(now))
			{
				return;
			}
			if (now >= nextHop && now >= stayUntil)
			{
				nextHop = now + 90000 + R.Next(150000);
				if (R.NextDouble() < 0.35 && bots.HasHumansElsewhere(Player) && Hop(now, true))
				{
					return;
				}
			}
			Wander(now, humans);
		}

		private bool Hop(long now, bool otherRoom)
		{
			LanPlayer target = bots.HopTarget(Player, otherRoom);
			if (target != null && !target.IsBot && otherRoom)
			{
				Leader = target;
			}
			return HopTo(target, now);
		}

		private bool HopTo(LanPlayer target, long now)
		{
			if (target == null || target.RoomKey == Player.RoomKey)
			{
				return false;
			}
			PutAway();
			followGoal = null;
			StandUp(now);
			ride = null;
			EndTube(now);
			sprinting = false;
			sprintOffAt = 0;
			launchAt = 0;
			aimAt = 0;
			bots.Unhead(this);
			Server.BotLeave(Player);
			route.Clear();
			moving = false;
			arrived = null;
			chatAt = 0;
			hopTarget = target;
			hopAt = now + 2000 + R.Next(2000);
			SetMode("hopping", now);
			return true;
		}

		private void HopTick(long now)
		{
			if (now < hopAt)
			{
				return;
			}
			if (hopRoom != null && LanBots.IsBotBlocked(hopRoom))
			{
				hopRoom = null;
				hopAt3 = null;
			}
			if (hopRoom != null)
			{
				string room = hopRoom;
				float[] spot = hopAt3;
				hopRoom = null;
				hopAt3 = null;
				Server.BotEnter(Player, room, spot);
				Arrived(now, spot);
				bots.ResendLooks(Player);
				return;
			}
			LanPlayer target = hopTarget;
			hopTarget = null;
			if (Leader != null && bots.InPlace(Leader) && now >= stayUntil && !LanBots.IsBotBlocked(Leader.RoomName))
			{
				target = Leader;
			}
			if (target == null || !bots.InPlace(target) || (LanBots.IsIgloo(target.RoomName) && target != Leader) || LanBots.IsBotBlocked(target.RoomName))
			{
				target = bots.HopTarget(Player, false);
			}
			if (target == null)
			{
				hopAt = now + 3000;
				return;
			}
			float[] at = bots.ArrivalPoint(target);
			Server.BotEnter(Player, target.RoomName, at);
			Arrived(now, at);
			bots.ResendLooks(Player);
		}

		private void Wander(long now, List<LanPlayer> humans)
		{
			float[] target = Map.RandomReachable(pos, 6.0, 1000.0, R);
			bool usePath = true;
			float[] water = (R.NextDouble() < 0.25) ? LanBots.RandomWater(LanServer.ZoneOfRoom(Player.RoomKey), R) : null;
			if (water != null)
			{
				target = water;
				usePath = false;
			}
			if (target == null)
			{
				int home = Map.Nearest(pos, 1000.0);
				if (home >= 0)
				{
					target = LanMotion.Copy(Map.Nodes[home].P);
					usePath = false;
				}
			}
			if (target == null)
			{
				List<LanPlayer> others = humans.Where((LanPlayer h) => h.LastPosition != null).ToList();
				if (others.Count == 0)
				{
					busyUntil = now + 3000;
					return;
				}
				LanPlayer h2 = others[R.Next(others.Count)];
				if (HDist(h2.LastPosition, pos) < 6.0 && R.NextDouble() < 0.5)
				{
					busyUntil = now + 2000 + R.Next(4000);
					return;
				}
				target = Near(h2.LastPosition, 4.0 + R.NextDouble() * 8.0);
				usePath = false;
			}
			SetMode("wander", now);
			GoTo(target, WalkSpeed, delegate
			{
				long t = bots.Now;
				SetMode("idle", t);
				busyUntil = t + 2000 + R.Next(7000);
				double roll = R.NextDouble();
				if (roll < 0.07)
				{
					Send(ActJump, Flat(facing), null, null);
				}
				else if (roll < 0.2)
				{
					Chatter(t);
				}
			}, usePath);
		}

		private bool TryLobby(long now)
		{
			foreach (LanLobbyView l in Server.Lobbies.Open(Player.RoomKey))
			{
				if (LanBots.IsTubeRace(Server.PartyGames.Definitions.Get(l.TemplateId)))
				{
					continue;
				}
				if (!bots.ActivitiesEnabled && l.TemplateId == DanceTemplate)
				{
					continue;
				}
				bool eager = danceTrip && l.TemplateId == DanceTemplate;
				if (!eager && Decide("L" + l.ItemId, now, 600000))
				{
					continue;
				}
				if (l.Members >= l.MaxPlayers - 1)
				{
					continue;
				}
				double travel = HDist(pos, l.Position) / WalkSpeed * 1600.0 + 2500.0;
				if (l.EndsAtMs - now < travel || (!eager && R.NextDouble() > 0.8))
				{
					continue;
				}
				bots.Unhead(this);
				lobbyItem = l.ItemId;
				lobbyTemplate = l.TemplateId;
				slotPlaced = false;
				lastMemberAt = 0;
				SetMode("lobby", now);
				GoTo(Near(l.Position, 2.0), WalkSpeed * 1.2, JoinLobby, true);
				return true;
			}
			return false;
		}

		private void JoinLobby()
		{
			LanLobbyView l = Server.Lobbies.Open(Player.RoomKey).FirstOrDefault((LanLobbyView x) => x.ItemId == lobbyItem);
			if (l == null)
			{
				Reset(bots.Now);
				return;
			}
			Interact(1, lobbyItem.ToString(CultureInfo.InvariantCulture), "Untagged");
			lastMemberAt = bots.Now;
		}

		private void LobbyTick(long now)
		{
			if (Server.Lobbies.IsMember(Player))
			{
				lastMemberAt = now;
				if (!slotPlaced && !moving)
				{
					slotPlaced = true;
					GoToDanceSlot();
				}
				return;
			}
			if (moving)
			{
				if (!Server.Lobbies.Open(Player.RoomKey).Any((LanLobbyView x) => x.ItemId == lobbyItem))
				{
					Stop();
					Reset(now);
				}
				return;
			}
			if (lastMemberAt == 0 || now - lastMemberAt > 3000)
			{
				Reset(now);
			}
		}

		private void GoToDanceSlot()
		{
			if (lobbyTemplate != DanceTemplate || Zone != DanceZone)
			{
				return;
			}
			LanSlot slot = Server.Lobbies.SlotOf(Player);
			LanPartyGameDef def = Server.PartyGames.Definitions.Get(lobbyTemplate);
			if (slot == null || def == null)
			{
				return;
			}
			int perTeam = def.MaxPlayers / Math.Max(1, def.NumTeams);
			int index = slot.RoleId + ((slot.TeamId == 1) ? perTeam : 0);
			if (index < 0 || index >= DanceSlots.Length)
			{
				return;
			}
			float[] d = DanceSlots[index];
			float[] final = new float[3]
			{
				d[0],
				DanceSlotY,
				d[1]
			};
			float[] face = new float[3]
			{
				d[2],
				0f,
				d[3]
			};
			float[] before = new float[3]
			{
				d[0] - d[2] * 0.5f,
				DanceSlotY,
				d[1] - d[3] * 0.5f
			};
			GoTo(before, WalkSpeed, delegate
			{
				GoTo(final, 1.5, delegate
				{
					facing = face;
					Send(ActRotate, face, null, null);
				}, false);
			}, false);
		}

		private static bool IsInvitation(LanPlayer p)
		{
			LanConsumableDef def = LanConsumables.Get(p.EquippedConsumable);
			return def != null && def.Experience == "partyGameLobby" && p.HeldActionCount.HasValue && p.HeldActionCount.Value > 0;
		}

		private bool TryInvitation(long now)
		{
			if (Player.EquippedConsumable != null || Player.HeldObject != null)
			{
				return false;
			}
			foreach (LanPlayer h in Server.InRoomExcept(Player.RoomKey, Player))
			{
				if (h.LastPosition == null || !IsInvitation(h) || Server.PartyGames.SessionOf(h) != null || Server.PartyGames.SessionOf(Player) != null)
				{
					continue;
				}
				if (h.HeldActionCount.Value - bots.HeadingTo(h.UserId) <= 0)
				{
					continue;
				}
				if (Decide("I" + h.UserId + ":" + h.EquippedConsumable, now, 60000))
				{
					continue;
				}
				if (R.NextDouble() > (h.IsBot ? 0.5 : 0.7))
				{
					continue;
				}
				holder = h;
				holderRetries = 0;
				bots.Head(this, h.UserId);
				SetMode("invite", now);
				GoTo(Near(h.LastPosition, 1.3), WalkSpeed * 1.2, ReachHolder, true);
				return true;
			}
			return false;
		}

		private void ReachHolder()
		{
			long now = bots.Now;
			if (holder == null || !holder.Registered || holder.RoomKey != Player.RoomKey || !IsInvitation(holder))
			{
				Reset(now);
				return;
			}
			if (HDist(holder.LastPosition, pos) > 2.5)
			{
				if (++holderRetries > 3)
				{
					Reset(now);
					return;
				}
				GoTo(Near(holder.LastPosition, 1.2), WalkSpeed * 1.2, ReachHolder, true);
				return;
			}
			facing = Sub(holder.LastPosition, pos);
			Interact(2, holder.PlayerSessionId, "RemotePlayer");
			SetMode("guest", now);
		}

		private void InviteTick(long now)
		{
			if (mode == "invite")
			{
				if (holder == null || !holder.Registered || !IsInvitation(holder) || now - modeSince > 40000)
				{
					Stop();
					Reset(now);
				}
				return;
			}
			if (holder == null || !holder.Registered || holder.RoomKey != Player.RoomKey || now - modeSince > 45000)
			{
				Reset(now);
				return;
			}
			if (!IsInvitation(holder) && now - modeSince > 3000 && Server.PartyGames.SessionOf(holder) == null)
			{
				Reset(now);
			}
		}

		private bool TryHost(long now, List<LanPlayer> here, List<LanPlayer> humans)
		{
			if (humans.Count == 0 || Player.EquippedConsumable != null || Player.HeldObject != null || R.NextDouble() > 0.06)
			{
				return false;
			}
			if (here.Any(IsInvitation) || bots.BotHostingIn(Player.RoomKey))
			{
				return false;
			}
			List<string> types = HostTypes.Where(delegate(string t)
			{
				int? template = Server.PartyGames.Definitions.TemplateForTrigger(t);
				return template.HasValue && Server.PartyGames.CanStart(template.Value);
			}).ToList();
			if (types.Count == 0)
			{
				return false;
			}
			hostType = types[R.Next(types.Count)];
			Server.Consumables.UseAsBot(Player, hostType);
			if (Player.EquippedConsumable != hostType)
			{
				return false;
			}
			ownerStartSent = false;
			hostGoneAt = 0;
			SetMode("host", now);
			if (R.NextDouble() < 0.5)
			{
				Say("Anyone want to play? " + EmoteString(1), now);
			}
			return true;
		}

		private void HostTick(long now)
		{
			if (Player.EquippedConsumable != hostType)
			{
				if (hostGoneAt == 0)
				{
					hostGoneAt = now;
				}
				if (now - hostGoneAt > 3000 || Player.EquippedConsumable != null)
				{
					Reset(now);
				}
				return;
			}
			hostGoneAt = 0;
			int? template = Server.PartyGames.Definitions.TemplateForTrigger(hostType);
			LanPartyGameDef def = template.HasValue ? Server.PartyGames.Definitions.Get(template.Value) : null;
			LanConsumableDef cdef = LanConsumables.Get(hostType);
			if (def != null && def.LobbyType == 1 && cdef != null && cdef.TotalItemQuantity.HasValue && Player.HeldActionCount.HasValue && Player.HeldActionCount.Value < cdef.TotalItemQuantity.Value && now - modeSince > 12000 && !ownerStartSent)
			{
				ownerStartSent = true;
				Send(ActAction1, null, null, null);
				return;
			}
			if (now - modeSince > 50000)
			{
				Server.Consumables.HandleClientConChanged(Player, null);
				Reset(now);
			}
		}

		private bool TryTray(long now, List<LanPlayer> here)
		{
			if (Player.EquippedConsumable != null || Player.HeldObject != null)
			{
				return false;
			}
			foreach (LanPlayer h in here)
			{
				if (h == Player || h.LastPosition == null)
				{
					continue;
				}
				LanConsumableDef tray = LanConsumables.Get(h.EquippedConsumable);
				LanConsumableDef recipient = (tray != null) ? LanConsumables.Get(tray.Recipient) : null;
				if (tray == null || tray.Experience != "playerHeld" || !tray.Shareable || recipient == null || !recipient.ServerAdded || !h.HeldActionCount.HasValue || h.HeldActionCount.Value <= 0)
				{
					continue;
				}
				if (Decide("F" + h.UserId + ":" + h.EquippedConsumable, now, 120000) || R.NextDouble() > 0.35)
				{
					continue;
				}
				holder = h;
				trayAt = 0;
				SetMode("tray", now);
				GoTo(Near(h.LastPosition, 1.2), WalkSpeed, delegate
				{
					if (holder != null && holder.Registered && holder.LastPosition != null && HDist(holder.LastPosition, pos) <= 2.5)
					{
						facing = Sub(holder.LastPosition, pos);
						Interact(2, holder.PlayerSessionId, "RemotePlayer");
						trayAt = bots.Now + 2500 + R.Next(2000);
					}
					else
					{
						Reset(bots.Now);
					}
				}, true);
				return true;
			}
			return false;
		}

		private void TrayTick(long now)
		{
			if (moving)
			{
				if (now - modeSince > 30000)
				{
					Stop();
					Reset(now);
				}
				return;
			}
			if (trayAt == 0)
			{
				if (now - modeSince > 30000)
				{
					Reset(now);
				}
				return;
			}
			if (now < trayAt)
			{
				return;
			}
			if (Player.EquippedConsumable != null && Player.HeldActionCount.HasValue && Player.HeldActionCount.Value > 0)
			{
				Interact(2, Player.PlayerSessionId, "Player");
			}
			Reset(now);
			busyUntil = now + 4500;
		}

		private bool TryTargets(long now)
		{
			LanTargetsView view = bots.ActivitiesEnabled ? Server.Targets.View(Player.RoomKey, false) : null;
			if (view == null || view.Center == null || !TargetsShown(view) || Decide("T" + view.Key, now, 3600000) || R.NextDouble() > 0.85)
			{
				return false;
			}
			bots.Unhead(this);
			targetsKey = view.Key;
			targetsSeen = now;
			SetMode("targets", now);
			float[] stand = Map.RandomNear(view.Center, view.Radius * 0.7, R) ?? Near(view.Center, view.Radius * 0.4);
			GoTo(stand, WalkSpeed * 1.3, delegate
			{
				SetMode("throwing", bots.Now);
				nextThrow = bots.Now + 300 + R.Next(800);
				launchAt = 0;
				aimAt = 0;
			}, true);
			return true;
		}

		private bool TargetsShown(LanTargetsView view)
		{
			if (view.Path == null || view.Path.IndexOf("Halloween", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return true;
			}
			string date = LanServer.ContentDateOf(Player.RoomName);
			return date != null && date.Substring(5, 2) == "10";
		}

		private float[] Aim(float[] target, double radius)
		{
			double spread = (R.NextDouble() < 0.18) ? (radius * 2.5 + 0.6) : (radius * 0.3);
			double a = R.NextDouble() * Math.PI * 2.0;
			double tx = target[0] + Math.Cos(a) * spread * R.NextDouble();
			double ty = target[1] + (R.NextDouble() - 0.5) * spread;
			double tz = target[2] + Math.Sin(a) * spread * R.NextDouble();
			double ox = pos[0];
			double oy = pos[1] + 0.75;
			double oz = pos[2];
			float[] best = null;
			double bestScore = double.MaxValue;
			for (double t = 0.35; t <= 2.0; t += 0.05)
			{
				double vx = (tx - ox) / t;
				double vz = (tz - oz) / t;
				double vy = (ty - oy - 0.5 * Gravity * t * t) / t;
				double s = Math.Sqrt(vx * vx + vy * vy + vz * vz);
				if (s > 19.0)
				{
					continue;
				}
				double score = Math.Abs(s - 13.0);
				if (score < bestScore)
				{
					bestScore = score;
					best = new float[3]
					{
						(float)vx,
						(float)vy,
						(float)vz
					};
				}
			}
			return best;
		}

		private void TargetsTick(long now)
		{
			LanTargetsView running = Server.Targets.View(Player.RoomKey, false);
			if (running == null || running.Key != targetsKey)
			{
				if (launchAt > 0 || aimAt > 0)
				{
					launchAt = 0;
					aimAt = 0;
					Send(ActCancelThrow, null, null, null);
				}
				if (now - targetsSeen > 4000)
				{
					Stop();
					Reset(now);
				}
				return;
			}
			targetsSeen = now;
			if (mode != "throwing")
			{
				return;
			}
			if (aimAt > 0)
			{
				if (now >= aimAt)
				{
					aimAt = 0;
					Send(ActCharge, Flat(facing), null, null);
					launchAt = now + 700 + R.Next(500);
				}
				return;
			}
			if (launchAt > 0)
			{
				if (now >= launchAt)
				{
					launchAt = 0;
					Send(ActLaunch, Flat(facing), launchVelocity, null);
					nextThrow = now + 1200 + R.Next(1200);
				}
				return;
			}
			if (now < nextThrow)
			{
				return;
			}
			LanTargetsView live = Server.Targets.View(Player.RoomKey, true);
			if (live == null || live.Targets.Count == 0)
			{
				nextThrow = now + 500;
				return;
			}
			int pick = R.Next(live.Targets.Count);
			float[] v = Aim(live.Targets[pick], live.Radii[pick]);
			if (v == null)
			{
				nextThrow = now + 1500;
				return;
			}
			facing = new float[3]
			{
				v[0],
				0f,
				v[2]
			};
			launchVelocity = v;
			Send(ActRotate, Flat(facing), null, null);
			aimAt = now + 450 + R.Next(250);
		}

		private void EnsureTube()
		{
			int[] ids = bots.Tubes;
			if (ids.Length > 0 && Array.IndexOf(ids, Player.SelectedTubeId.GetValueOrDefault()) < 0)
			{
				int id = ids[R.Next(ids.Length)];
				Player.SelectedTubeId = id;
				Server.BroadcastUserVars(Player, LanServer.IntVar("tube", id));
			}
		}

		private void BeginGame(LanPartyGameSession session, long now)
		{
			StandUp(now);
			ride = null;
			sprinting = false;
			sprintOffAt = 0;
			EndTube(now);
			bots.Unhead(this);
			Stop();
			gameId = session.Id;
			SetMode("game", now);
			danceRound = -1;
			danceMoves.Clear();
			fourSeq = -1;
			fourAt = 0;
			fishStep = -1;
			fishAt = 0;
			fishPlaced = false;
			huntDrops = null;
			huntIndex = 0;
			huntNext = 0;
			huntFindStart = 0;
			huntTarget = -1;
			huntHome = LanMotion.Copy(pos);
			huntSent.Clear();
		}

		private void EndGame(long now)
		{
			gameId = 0;
			Stop();
			moving = true;
			sliding = false;
			SetMode("idle", now);
			busyUntil = now + 2500 + R.Next(3000);
			if (R.NextDouble() < 0.25)
			{
				Say(Pick(AfterGame), now);
			}
		}

		private int IndexIn(LanPartyGameSession session)
		{
			return session.Players.FindIndex((LanPartyGamePlayer p) => p.Player == Player);
		}

		private void GameTick(LanPartyGameSession session, long now)
		{
			if (session is LanDanceBattle)
			{
				DanceTick((LanDanceBattle)session, now);
			}
			else if (session is LanFindFour)
			{
				FourTick((LanFindFour)session, now);
			}
			else if (session is LanFishBucket)
			{
				FishTick((LanFishBucket)session, now);
			}
			else if (session is LanScavengerHunt)
			{
				HuntTick((LanScavengerHunt)session, now);
			}
		}

		private void DanceTick(LanDanceBattle battle, long now)
		{
			if (battle.Phase == "input" && battle.Round != danceRound)
			{
				danceRound = battle.Round;
				danceMoves.Clear();
				List<int> expected = battle.Expected;
				if (expected.Count > 0)
				{
					double gap = Math.Max(250.0, (battle.TurnSeconds * 1000.0 - 1000.0) / expected.Count);
					long at = now + 500 + R.Next(400);
					foreach (int move in expected)
					{
						int played = move;
						if (R.NextDouble() > 0.87)
						{
							played = 1 + (move + R.Next(2)) % 3;
						}
						danceMoves.Add(new KeyValuePair<long, int>(at, played));
						at += (long)(gap * (0.8 + R.NextDouble() * 0.3));
					}
				}
			}
			while (danceMoves.Count > 0 && now >= danceMoves[0].Key)
			{
				int move2 = danceMoves[0].Value;
				danceMoves.RemoveAt(0);
				if (battle.Phase == "input")
				{
					Server.BotGameMessage(Player, battle.Id, 5, "{\"ScoreDelta\":" + move2.ToString(CultureInfo.InvariantCulture) + "}");
				}
			}
		}

		private void FourTick(LanFindFour game, long now)
		{
			int me = IndexIn(game);
			if (me < 0)
			{
				return;
			}
			if (game.WaitingFor == me && game.TurnSeq != fourSeq)
			{
				fourSeq = game.TurnSeq;
				fourAt = now + 1500 + R.Next(2500);
			}
			if (fourAt > 0 && now >= fourAt)
			{
				fourAt = 0;
				if (game.WaitingFor == me && game.TurnSeq == fourSeq)
				{
					int col = ChooseColumn(game, me);
					Server.BotGameMessage(Player, game.Id, 5, "{\"ScoreDelta\":" + col.ToString(CultureInfo.InvariantCulture) + "}");
				}
			}
		}

		private static bool Line(int[,] g, int w, int h, int c, int r, int who, int k)
		{
			int[,] dirs = new int[4, 2]
			{
				{ 1, 0 },
				{ 0, 1 },
				{ 1, 1 },
				{ 1, -1 }
			};
			for (int d = 0; d < 4; d++)
			{
				int n = 1;
				for (int s = 1; s >= -1; s -= 2)
				{
					int x = c + dirs[d, 0] * s;
					int y = r + dirs[d, 1] * s;
					while (x >= 0 && x < w && y >= 0 && y < h && g[x, y] == who)
					{
						n++;
						x += dirs[d, 0] * s;
						y += dirs[d, 1] * s;
					}
				}
				if (n >= k)
				{
					return true;
				}
			}
			return false;
		}

		private static bool WinsAt(int[,] g, int[] heights, int w, int h, int c, int who, int k)
		{
			int r = heights[c];
			if (r >= h)
			{
				return false;
			}
			g[c, r] = who;
			bool win = Line(g, w, h, c, r, who, k);
			g[c, r] = -1;
			return win;
		}

		private int ChooseColumn(LanFindFour game, int me)
		{
			int w = game.Width;
			int h = game.Height;
			int k = game.ToWin;
			int opp = 1 - me;
			int[,] g = new int[w, h];
			int[] heights = new int[w];
			for (int c = 0; c < w; c++)
			{
				for (int r = 0; r < h; r++)
				{
					g[c, r] = -1;
				}
				List<int> col = game.Board[c];
				heights[c] = Math.Min(h, col.Count);
				for (int r2 = 0; r2 < heights[c]; r2++)
				{
					g[c, r2] = col[r2];
				}
			}
			List<int> legal = Enumerable.Range(0, w).Where((int c) => heights[c] < h).ToList();
			if (legal.Count == 0)
			{
				return game.DefaultColumn;
			}
			foreach (int c2 in legal)
			{
				if (WinsAt(g, heights, w, h, c2, me, k))
				{
					return c2;
				}
			}
			foreach (int c3 in legal)
			{
				if (WinsAt(g, heights, w, h, c3, opp, k) && R.NextDouble() < 0.92)
				{
					return c3;
				}
			}
			if (R.NextDouble() < 0.08)
			{
				return legal[R.Next(legal.Count)];
			}
			double center = (w - 1) / 2.0;
			double best = double.MinValue;
			int pick = legal[0];
			foreach (int c4 in legal)
			{
				double score = (w / 2.0 - Math.Abs(c4 - center)) + R.NextDouble() * 1.5;
				int r3 = heights[c4];
				g[c4, r3] = me;
				heights[c4]++;
				if (WinsAt(g, heights, w, h, c4, opp, k))
				{
					score -= 10.0;
				}
				heights[c4]--;
				for (int dx = -1; dx <= 1; dx++)
				{
					for (int dy = -1; dy <= 1; dy++)
					{
						int x = c4 + dx;
						int y = r3 + dy;
						if ((dx != 0 || dy != 0) && x >= 0 && x < w && y >= 0 && y < h && g[x, y] == me)
						{
							score += 0.6;
						}
					}
				}
				g[c4, r3] = -1;
				if (score > best)
				{
					best = score;
					pick = c4;
				}
			}
			return pick;
		}

		private void FishTick(LanFishBucket game, long now)
		{
			int me = IndexIn(game);
			if (me < 0)
			{
				return;
			}
			float[][] starts = game.StartPositions;
			if (!fishPlaced && starts != null && me < starts.Length && starts[me] != null)
			{
				fishPlaced = true;
				float[] slot = LanMotion.Copy(starts[me]);
				float[] bucket = game.BucketPosition;
				float[] toBucket = (bucket != null) ? Flat(Sub(bucket, slot)) : null;
				if (toBucket != null && (toBucket[0] != 0f || toBucket[2] != 0f))
				{
					float[] prep = new float[3]
					{
						slot[0] - toBucket[0] * 0.6f,
						slot[1],
						slot[2] - toBucket[2] * 0.6f
					};
					GoTo(prep, WalkSpeed, delegate
					{
						GoTo(slot, 1.0, null, false);
					}, false);
				}
				else
				{
					GoTo(slot, WalkSpeed, null, false);
				}
			}
			if (game.Phase == "turn" && game.Current == me && game.Step != fishStep)
			{
				fishStep = game.Step;
				fishAt = now + 1200 + R.Next(1800);
			}
			if (fishAt > 0 && now >= fishAt)
			{
				fishAt = 0;
				if (game.Phase == "turn" && game.Current == me && game.Step == fishStep)
				{
					Send(game.Drew && !KeepFishing(game, me) ? ActAction2 : ActAction1, null, null, null);
				}
			}
		}

		private bool KeepFishing(LanFishBucket game, int me)
		{
			List<int> rest = game.Remaining;
			if (rest.Count == 0)
			{
				return false;
			}
			double squid = rest.Count((int c) => c < 0) / (double)rest.Count;
			double limit = 0.28 + R.NextDouble() * 0.1;
			int[] scores = game.Scores;
			int leader = 0;
			for (int i = 0; i < scores.Length; i++)
			{
				if (i != me)
				{
					leader = Math.Max(leader, scores[i]);
				}
			}
			if (scores[me] < leader)
			{
				limit += 0.1;
			}
			return squid < limit;
		}

		private void HuntTick(LanScavengerHunt game, long now)
		{
			if (game.Hider != null && game.Hider.Player == Player)
			{
				HideTick(game, now);
			}
			else if (game.Finder != null && game.Finder.Player == Player)
			{
				FindTick(game, now);
			}
		}

		private void HideTick(LanScavengerHunt game, long now)
		{
			if (!game.Hiding)
			{
				return;
			}
			if (huntDrops == null)
			{
				huntDrops = new List<float[]>();
				for (int i = 0; i < game.MaxHidden * 6 && huntDrops.Count < game.MaxHidden; i++)
				{
					float[] spot = Map.RandomReachable(huntHome, 4.0, 14.0, R);
					if (spot == null)
					{
						double a = R.NextDouble() * Math.PI * 2.0;
						double r = 3.0 + R.NextDouble() * 4.0;
						spot = new float[3]
						{
							huntHome[0] + (float)(Math.Cos(a) * r),
							huntHome[1],
							huntHome[2] + (float)(Math.Sin(a) * r)
						};
					}
					if (huntDrops.All((float[] d) => LanMotion.Dist(d, spot) >= 2.5))
					{
						huntDrops.Add(spot);
					}
				}
				huntIndex = 0;
				huntNext = now + 800 + R.Next(1500);
			}
			if (moving || now < huntNext || huntIndex >= huntDrops.Count)
			{
				return;
			}
			float[] target = huntDrops[huntIndex];
			if (LanMotion.Dist(target, pos) > 0.6)
			{
				GoTo(target, WalkSpeed, delegate
				{
					huntNext = bots.Now + 300 + R.Next(500);
				}, true);
				return;
			}
			Send(ActAction1, null, null, null);
			huntIndex++;
			huntNext = now + 700 + R.Next(800);
		}

		private void FindTick(LanScavengerHunt game, long now)
		{
			if (!game.Finding)
			{
				return;
			}
			if (huntFindStart == 0)
			{
				huntFindStart = now;
			}
			Dictionary<int, float[]> marbles = game.Marbles;
			foreach (int sent in huntSent.ToList())
			{
				if (!marbles.ContainsKey(sent))
				{
					huntSent.Remove(sent);
				}
			}
			int nearest = -1;
			double nearestD = double.MaxValue;
			foreach (KeyValuePair<int, float[]> kv in marbles)
			{
				if (huntSent.Contains(kv.Key))
				{
					continue;
				}
				double d = LanMotion.Dist(kv.Value, pos);
				if (d < nearestD)
				{
					nearestD = d;
					nearest = kv.Key;
				}
			}
			if (nearest < 0)
			{
				return;
			}
			if (nearestD <= 1.6)
			{
				Stop();
				huntSent.Add(nearest);
				huntTarget = -1;
				Server.BotGameMessage(Player, game.Id, 3, "{\"ItemId\":" + nearest.ToString(CultureInfo.InvariantCulture) + "}");
				return;
			}
			bool late = now - huntFindStart > game.FindSeconds * 1000.0 * 0.45;
			if (nearestD <= 9.0 || late)
			{
				if (huntTarget != nearest || !moving)
				{
					huntTarget = nearest;
					GoTo(marbles[nearest], WalkSpeed * 1.2, null, true);
				}
				return;
			}
			if (!moving)
			{
				huntTarget = -1;
				float[] spot = Map.RandomReachable(huntHome, 3.0, 14.0, R) ?? Near(huntHome, 8.0);
				GoTo(spot, WalkSpeed, null, true);
			}
		}
	}

	internal static class LanBotRunParams
	{
		public static volatile float JogSpeedF = 3.25f;

		public static double JogSpeed => JogSpeedF;

		public static double JogAcceleration = 3.0;

		public static double JogIntoSpeedMult = 0.3;

		public static double JogTurnSpeedMult = 1.0;

		public static double SprintSpeed = 4.5;

		public static double SprintAcceleration = 2.0;

		public static double SprintIntoSpeedMult = 0.5;

		public static double SprintTurnSpeedMult = 0.9;

		public static double MinFacingAngleToResetMomentum = 600.0;

		public static double SlideManualSpeed = 2.0;

		public static double SlideBuildMomentumTime = 1.0;

		public static double SlideRotationSmoothing = 2.0;

		public static bool Loaded;

		public static bool SlideLoaded;

		public static void SetSlide(float manualSpeed, float buildMomentumTime, float rotationSmoothing)
		{
			SlideManualSpeed = manualSpeed;
			SlideBuildMomentumTime = buildMomentumTime;
			SlideRotationSmoothing = rotationSmoothing;
			SlideLoaded = true;
		}

		public static void Set(float jogSpeed, float jogAccel, float jogInto, float jogTurn, float sprintSpeed, float sprintAccel, float sprintInto, float sprintTurn, float resetAngle)
		{
			JogAcceleration = jogAccel;
			JogIntoSpeedMult = jogInto;
			JogTurnSpeedMult = jogTurn;
			SprintSpeed = sprintSpeed;
			SprintAcceleration = sprintAccel;
			SprintIntoSpeedMult = sprintInto;
			SprintTurnSpeedMult = sprintTurn;
			MinFacingAngleToResetMomentum = resetAngle;
			JogSpeedF = jogSpeed;
			Loaded = true;
		}
	}
}
#endif
