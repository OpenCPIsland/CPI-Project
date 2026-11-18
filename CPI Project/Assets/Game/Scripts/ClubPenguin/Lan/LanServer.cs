#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanServerData
	{
		public string PartyGames;

		public string TubeRaces;

		public string WorldObjects;
	}

	public sealed partial class LanServer
	{
		public const int RoomId = 1;

		public const int RoomCap = 100;

		public static readonly int[] Ports = new int[3]
		{
			8088,
			8080,
			9933
		};

		private const int LoginTimeoutMs = 10000;

		private const int IdleTimeoutMs = 120000;

		private const int AnnounceGraceMs = 3000;

		private const int MaxNameBytes = 255;

		private const int MaxOutfitBytes = 32768;

		public readonly LanLoop Loop = new LanLoop();

		public readonly System.Random Random = new System.Random();

		private readonly List<TcpListener> listeners = new List<TcpListener>();

		private readonly List<LanPlayer> players = new List<LanPlayer>();

		private readonly Dictionary<string, List<LanPlayer>> byRoom = new Dictionary<string, List<LanPlayer>>();

		private readonly HashSet<LanConnection> connections = new HashSet<LanConnection>();

		private int nextItemId = 1;

		private volatile bool running;

		public LanConsumables Consumables
		{
			get;
			private set;
		}

		public LanWorldObjects WorldObjects
		{
			get;
			private set;
		}

		public LanRoomItems RoomItems
		{
			get;
			private set;
		}

		public LanIslandTargets Targets
		{
			get;
			private set;
		}

		public LanPartyGames PartyGames
		{
			get;
			private set;
		}

		public LanLobbies Lobbies
		{
			get;
			private set;
		}

		public LanTransientData Transient
		{
			get;
			private set;
		}

		public bool IsRunning
		{
			get
			{
				return running;
			}
		}

		public int PlayerCount
		{
			get
			{
				return players.Count;
			}
		}

		public LanServer(LanServerData data)
		{
			LanWorldData world = new LanWorldData(data.WorldObjects);
			RoomItems = new LanRoomItems(this);
			WorldObjects = new LanWorldObjects(this, world);
			Consumables = new LanConsumables(this);
			Targets = new LanIslandTargets(this, world);
			PartyGames = new LanPartyGames(this, new LanPartyGameDefinitions(data.PartyGames, data.TubeRaces));
			Lobbies = new LanLobbies(this);
			Transient = new LanTransientData(this);
		}

		public bool Start()
		{
			running = true;
			foreach (int port in Ports)
			{
				try
				{
					TcpListener listener = new TcpListener(IPAddress.Any, port);
					listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
					listener.Start();
					listeners.Add(listener);
					Thread t = new Thread(AcceptLoop);
					t.IsBackground = true;
					t.Name = "CPI LAN Accept " + port;
					t.Start(listener);
				}
				catch (Exception)
				{
					if (port == Ports[0])
					{
						Stop();
						return false;
					}
				}
			}
			Loop.Start();
			Loop.After(10000, IdleSweep);
			return true;
		}

		public void Stop()
		{
			running = false;
			foreach (TcpListener l in listeners)
			{
				try
				{
					l.Stop();
				}
				catch (Exception)
				{
				}
			}
			listeners.Clear();
			Loop.Post(delegate
			{
				foreach (LanConnection c in connections.ToList())
				{
					c.Close();
				}
				Loop.Stop();
			});
		}

		private void AcceptLoop(object state)
		{
			TcpListener listener = (TcpListener)state;
			while (running)
			{
				Socket socket;
				try
				{
					socket = listener.AcceptSocket();
				}
				catch (Exception)
				{
					return;
				}
				Loop.Post(delegate
				{
					if (!running)
					{
						try
						{
							socket.Close();
						}
						catch (Exception)
						{
						}
						return;
					}
					LanConnection conn = new LanConnection(this, socket);
					connections.Add(conn);
					conn.Start();
					LanConnection captured = conn;
					Loop.After(LoginTimeoutMs, delegate
					{
						if (!captured.Player.LoggedIn)
						{
							captured.Close();
						}
					});
				});
			}
		}

		private void IdleSweep()
		{
			long now = Loop.NowMs;
			foreach (LanConnection c in connections.ToList())
			{
				if (c.Player.LoggedIn && now - c.LastInboundMs > IdleTimeoutMs)
				{
					c.Close();
				}
			}
			if (running)
			{
				Loop.After(10000, IdleSweep);
			}
		}

		public static long EpochMs
		{
			get
			{
				return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			}
		}

		public int AllocateItemId()
		{
			return nextItemId++;
		}

		public static byte[] Ext(string cmd, SfsObject p)
		{
			return SfsCodec.EncodePacket(1, 13, new SfsObject().PutString("c", cmd).PutObject("p", p));
		}

		public static byte[] Sys(int action, SfsObject p)
		{
			return SfsCodec.EncodePacket(0, action, p);
		}

		public static bool FitsBytes(string s, int max)
		{
			return s != null && Encoding.UTF8.GetByteCount(s) <= max;
		}

		public static string RoomKeyOf(string roomName)
		{
			string[] parts = roomName.Split(':');
			string zone = (parts.Length > 2) ? parts[2] : roomName;
			string instance = (parts.Length > 3) ? parts[3] : "";
			return zone + ":" + instance;
		}

		public static string ZoneOfRoom(string roomKey)
		{
			int i = roomKey.IndexOf(':');
			return (i >= 0) ? roomKey.Substring(0, i) : roomKey;
		}

		public static string ContentDateOf(string roomName)
		{
			string[] parts = roomName.Split(':');
			if (parts.Length < 5)
			{
				return null;
			}
			string[] fields = parts[4].Split(';');
			if (fields.Length < 3)
			{
				return null;
			}
			string date = fields[2];
			DateTime parsed;
			return (date.Length == 10 && DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed)) ? date : null;
		}

		public List<LanPlayer> All()
		{
			return new List<LanPlayer>(players);
		}

		public List<LanPlayer> InRoom(string roomKey)
		{
			List<LanPlayer> list;
			return byRoom.TryGetValue(roomKey, out list) ? new List<LanPlayer>(list) : new List<LanPlayer>();
		}

		public List<LanPlayer> InRoomExcept(string roomKey, LanPlayer except)
		{
			List<LanPlayer> list = InRoom(roomKey);
			list.Remove(except);
			return list;
		}

		public int RoomOccupancy(string roomKey)
		{
			List<LanPlayer> list;
			return byRoom.TryGetValue(roomKey, out list) ? list.Count : 0;
		}

		public void SendToRoom(string roomKey, byte[] packet)
		{
			List<LanPlayer> list;
			if (!byRoom.TryGetValue(roomKey, out list))
			{
				return;
			}
			foreach (LanPlayer p in list.ToArray())
			{
				p.Send(packet);
			}
		}

		public void SendToRoomExcept(string roomKey, LanPlayer except, byte[] packet)
		{
			List<LanPlayer> list;
			if (!byRoom.TryGetValue(roomKey, out list))
			{
				return;
			}
			foreach (LanPlayer p in list.ToArray())
			{
				if (p != except)
				{
					p.Send(packet);
				}
			}
		}

		public LanPlayer BySwid(string swid)
		{
			return players.FirstOrDefault((LanPlayer p) => p.Swid == swid);
		}

		private void Register(LanPlayer player)
		{
			players.Add(player);
			List<LanPlayer> list;
			if (!byRoom.TryGetValue(player.RoomKey, out list))
			{
				list = new List<LanPlayer>();
				byRoom[player.RoomKey] = list;
			}
			list.Add(player);
			player.Registered = true;
		}

		private void Unregister(LanPlayer player)
		{
			if (!player.Registered)
			{
				return;
			}
			player.Registered = false;
			players.Remove(player);
			List<LanPlayer> list;
			if (byRoom.TryGetValue(player.RoomKey, out list))
			{
				list.Remove(player);
				if (list.Count == 0)
				{
					byRoom.Remove(player.RoomKey);
				}
			}
		}

		internal void OnConnectionClosed(LanConnection conn)
		{
			connections.Remove(conn);
			LanPlayer player = conn.Player;
			if (!player.LoggedIn)
			{
				return;
			}
			bool wasRegistered = player.Registered;
			Unregister(player);
			Lobbies.OnPlayerLeft(player);
			PartyGames.OnPlayerLeft(player);
			Consumables.ForgetInvitation(player);
			if (wasRegistered)
			{
				BroadcastUserLeftRoom(player);
			}
			WorldObjects.ReleaseRoomIfEmpty(player.RoomKey);
			Lobbies.ReleaseRoomIfEmpty(player.RoomKey);
			Targets.ForgetThrower(player);
			Targets.ReleaseRoomIfEmpty(player.RoomKey);
		}

		internal void Dispatch(LanConnection conn, int controllerId, int actionId, SfsObject p)
		{
			LanPlayer player = conn.Player;
			if (controllerId == 0 && actionId == 0)
			{
				HandleHandshake(player);
			}
			else if (controllerId == 0 && actionId == 1)
			{
				if (player.LoggedIn)
				{
					return;
				}
				if (!HandleLogin(player, p))
				{
					return;
				}
				player.LoggedIn = true;
				CloseOlderConnections(player);
				Register(player);
			}
			else if (controllerId == 0 && actionId == 29)
			{
				player.Send(Sys(29, new SfsObject()));
			}
			else if (!player.LoggedIn || !player.Registered)
			{
			}
			else if (controllerId == 0 && actionId == 12)
			{
				HandleSetUserVariables(player, p);
			}
			else if (controllerId == 1 && actionId == 13)
			{
				string cmd = p.GetString("c");
				if (cmd != null)
				{
					HandleExtension(player, cmd, p.GetObject("p"));
				}
			}
		}

		private void CloseOlderConnections(LanPlayer player)
		{
			if (player.Swid == null)
			{
				return;
			}
			foreach (LanPlayer other in players.ToArray())
			{
				if (other != player && other.Swid == player.Swid)
				{
					other.Connection.CloseAfter(100);
				}
			}
		}

		private void HandleHandshake(LanPlayer player)
		{
			SfsObject p = new SfsObject().PutString("tk", Guid.NewGuid().ToString()).PutInt("ct", 999999999).PutInt("ms", 1000000)
				.PutBool("ase", false);
			player.Send(Sys(0, p));
		}

		private static byte[] LoginResponse(LanPlayer player)
		{
			SfsObject p = new SfsObject().PutInt("id", player.UserId).PutString("un", player.UserName).PutShort("pi", 0)
				.PutShort("rs", 0)
				.PutString("zn", player.ZoneName)
				.PutArray("rl", new SfsArray())
				.PutObject("p", new SfsObject());
			return Sys(1, p);
		}

		private bool HandleLogin(LanPlayer player, SfsObject p)
		{
			string zoneName = p.GetString("zn") ?? "ClubPenguin";
			string loginName = p.GetString("un");
			string joinRoomData = p.GetObject("p").GetString("joinRoomData");
			object parsed = null;
			if (joinRoomData != null)
			{
				LanJson.TryParse(joinRoomData, out parsed);
			}
			object data = LanJson.Get(parsed, "data");
			string swid = LanJson.Get(parsed, "swid") as string;
			string room = LanJson.Get(data, "room") as string;
			string dataName = LanJson.Get(data, "userName") as string;
			long sessionId;
			if (LanJson.TryLong(LanJson.Get(data, "sessionId"), out sessionId))
			{
				player.PlayerSessionId = sessionId.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
			player.ZoneName = zoneName;
			string shown = string.IsNullOrEmpty(dataName) ? loginName : dataName;
			player.UserName = (!string.IsNullOrEmpty(shown) && FitsBytes(shown, MaxNameBytes)) ? shown : "guest";
			if (!string.IsNullOrEmpty(swid) && FitsBytes(swid, MaxNameBytes))
			{
				player.Swid = swid;
			}
			int colour;
			if (LanJson.TryInt(LanJson.Path(data, "playerRoomData", "profile", "colour"), out colour))
			{
				player.Colour = colour;
			}
			int tube;
			if (LanJson.TryInt(LanJson.Get(data, "selectedTubeId"), out tube))
			{
				player.SelectedTubeId = tube;
			}
			if (joinRoomData != null)
			{
				string outfit = LanJson.ExtractRaw(joinRoomData, "outfit");
				if (IsOutfitJson(outfit))
				{
					player.OutfitJson = outfit;
				}
			}
			Dictionary<string, object> earned = LanJson.Get(data, "earnedRewards") as Dictionary<string, object>;
			if (earned != null)
			{
				foreach (KeyValuePair<string, object> kv in earned)
				{
					long at;
					if (LanJson.TryLong(kv.Value, out at))
					{
						player.RoomCollected[kv.Key] = at;
					}
				}
			}
			if (string.IsNullOrEmpty(room) || !FitsBytes(room, MaxNameBytes) || room.Split(':').Length < 5)
			{
				player.Connection.Close();
				return false;
			}
			string roomName = room;
			player.RoomName = roomName;
			player.RoomKey = RoomKeyOf(roomName);
			Transient.Restore(player);
			player.Send(LoginResponse(player));
			SendJoin(player);
			return true;
		}

		private SfsArray BuildRoomArray(LanPlayer player, int userCount)
		{
			return new SfsArray().AddInt(RoomId).AddString(player.RoomName).AddString("default").AddBool(false)
				.AddBool(false)
				.AddBool(false)
				.AddShort((short)userCount)
				.AddShort((short)RoomCap)
				.AddArray(new SfsArray())
				.AddBool(false)
				.AddShort(0)
				.AddShort(100)
				.AddFloats(new float[3]
				{
					900f,
					900f,
					900f
				})
				.AddFloats(new float[3]
				{
					-900f,
					-900f,
					-900f
				})
				.AddFloats(new float[3]
				{
					900f,
					900f,
					900f
				});
		}

		private void SendJoin(LanPlayer player)
		{
			List<LanPlayer> others = InRoomExcept(player.RoomKey, player);
			SfsArray ul = new SfsArray();
			foreach (LanPlayer o in others)
			{
				ul.AddArray(BuildProximityUserEntry(o, null));
			}
			player.Send(Sys(4, new SfsObject().PutArray("r", BuildRoomArray(player, 1)).PutArray("ul", ul)));
			player.Send(Sys(12, new SfsObject().PutInt("u", player.UserId).PutArray("vl", BuildUserVariables(player))));
			WorldObjects.SendSnapshot(player);
			Consumables.SendSnapshot(player);
			Targets.EnsureRoom(player.RoomKey, player.RoomName);
			RoomItems.SendSnapshot(player);
			Lobbies.EnsureRoom(player.RoomKey);
			SendExistingOccupants(player);
			AnnounceWhenPlaced(player);
		}

		public static SfsArray Var(string name, byte kind, SfsValue value)
		{
			return new SfsArray().AddString(name).AddByte(kind).Add(value).AddBool(false);
		}

		public static SfsArray StringVar(string name, string value)
		{
			return Var(name, SfsVar.String, new SfsValue(SfsType.UtfString, value));
		}

		public static SfsArray IntVar(string name, int value)
		{
			return Var(name, SfsVar.Int, new SfsValue(SfsType.Int, value));
		}

		public static SfsArray NullVar(string name)
		{
			return Var(name, SfsVar.Null, SfsValue.NullValue);
		}

		public static SfsArray ConVar(string type, string name)
		{
			return Var("con", SfsVar.Object, new SfsValue(SfsType.SfsObject, new SfsObject().PutString("type", type).PutString("name", name)));
		}

		public static SfsArray ConPropVar(int actionCount)
		{
			return StringVar("con_prop", "{\"actionCount\":" + actionCount + "}");
		}

		public SfsArray BuildUserVariables(LanPlayer player)
		{
			SfsArray vl = new SfsArray().AddArray(StringVar("sess", player.PlayerSessionId ?? "0"));
			if (player.OutfitJson != null)
			{
				vl.AddArray(StringVar("outfit", player.OutfitJson));
			}
			vl.AddArray(IntVar("colour", player.Colour.GetValueOrDefault()));
			vl.AddArray(IntVar("tube", player.SelectedTubeId.GetValueOrDefault()));
			vl.AddArray(IntVar("loc", 0));
			vl.AddArray(IntVar("awayfromkeyboard", 0));
			vl.AddArray(IntVar("tempheadstatus", 0));
			if (player.EquippedConsumable != null)
			{
				vl.AddArray(ConVar("consum", player.EquippedConsumable));
				if (player.HeldActionCount.HasValue)
				{
					vl.AddArray(ConPropVar(player.HeldActionCount.Value));
				}
			}
			return vl;
		}

		public void BroadcastUserVars(LanPlayer player, params SfsArray[] vars)
		{
			SfsArray vl = new SfsArray();
			foreach (SfsArray v in vars)
			{
				vl.AddArray(v);
			}
			SendToRoom(player.RoomKey, Sys(12, new SfsObject().PutInt("u", player.UserId).PutArray("vl", vl)));
		}

		public static byte[] UserVarsPacket(LanPlayer player, params SfsArray[] vars)
		{
			SfsArray vl = new SfsArray();
			foreach (SfsArray v in vars)
			{
				vl.AddArray(v);
			}
			return Sys(12, new SfsObject().PutInt("u", player.UserId).PutArray("vl", vl));
		}

		public SfsArray BuildProximityUserEntry(LanPlayer player, float[] position)
		{
			SfsArray entry = new SfsArray().AddInt(player.UserId).AddString(player.UserName).AddShort(0).AddShort(0)
				.AddArray(BuildUserVariables(player));
			if (position != null)
			{
				entry.AddFloats(position);
			}
			return entry;
		}

		private void SendExistingOccupants(LanPlayer joiner)
		{
			List<LanPlayer> others = InRoomExcept(joiner.RoomKey, joiner).Where((LanPlayer o) => o.Announced).ToList();
			if (others.Count == 0)
			{
				return;
			}
			SfsArray p = new SfsArray();
			foreach (LanPlayer o in others)
			{
				p.AddArray(BuildProximityUserEntry(o, o.LastPosition ?? new float[3]));
			}
			joiner.Send(Sys(30, new SfsObject().PutInt("r", RoomId).PutArray("p", p)));
		}

		public void BroadcastNewJoiner(LanPlayer joined)
		{
			if (joined.Announced)
			{
				return;
			}
			joined.Announced = true;
			List<LanPlayer> others = InRoomExcept(joined.RoomKey, joined);
			if (others.Count == 0)
			{
				return;
			}
			SfsArray p = new SfsArray().AddArray(BuildProximityUserEntry(joined, joined.LastPosition ?? new float[3]));
			byte[] packet = Sys(30, new SfsObject().PutInt("r", RoomId).PutArray("p", p));
			foreach (LanPlayer o in others)
			{
				o.Send(packet);
			}
		}

		private void AnnounceWhenPlaced(LanPlayer joined)
		{
			if (joined.LastPosition != null)
			{
				BroadcastNewJoiner(joined);
				return;
			}
			Loop.After(AnnounceGraceMs, delegate
			{
				if (joined.Registered)
				{
					BroadcastNewJoiner(joined);
				}
			});
		}

		private void BroadcastUserLeftRoom(LanPlayer left)
		{
			List<LanPlayer> remaining = InRoomExcept(left.RoomKey, left);
			if (remaining.Count == 0)
			{
				return;
			}
			byte[] packet = Sys(30, new SfsObject().PutInt("r", RoomId).PutInts("m", new int[1]
			{
				left.UserId
			}));
			foreach (LanPlayer o in remaining)
			{
				o.Send(packet);
			}
		}

		public static bool IsOutfitJson(string text)
		{
			if (text == null || !text.StartsWith("{", StringComparison.Ordinal) || !FitsBytes(text, MaxOutfitBytes))
			{
				return false;
			}
			object parsed;
			return LanJson.TryParse(text, out parsed) && parsed is Dictionary<string, object>;
		}

		private static readonly Dictionary<string, byte> ClientVariables = new Dictionary<string, byte>
		{
			{
				"outfit",
				SfsVar.String
			},
			{
				"colour",
				SfsVar.String
			},
			{
				"tube",
				SfsVar.String
			},
			{
				"con",
				SfsVar.Object
			},
			{
				"bubble",
				SfsVar.Object
			},
			{
				"loc",
				SfsVar.Int
			},
			{
				"awayfromkeyboard",
				SfsVar.Int
			},
			{
				"tempheadstatus",
				SfsVar.Int
			}
		};

		private static bool IsClientVariable(SfsValue v)
		{
			SfsArray a = v.AsArray;
			if (a == null || a.Count < 3)
			{
				return false;
			}
			string name = a[0].AsString;
			byte expected;
			if (name == null || !ClientVariables.TryGetValue(name, out expected))
			{
				return false;
			}
			if (a[1].Type != SfsType.Byte)
			{
				return false;
			}
			byte kind = (byte)a[1].Value;
			if (kind != SfsVar.Null && kind != expected)
			{
				return false;
			}
			SfsValue value = a[2];
			SfsType wanted = (kind == SfsVar.Null) ? SfsType.Null : ((kind == SfsVar.Int) ? SfsType.Int : ((kind == SfsVar.String) ? SfsType.UtfString : SfsType.SfsObject));
			if (value.Type != wanted)
			{
				return false;
			}
			if (value.Type == SfsType.UtfString && !FitsBytes((string)value.Value, MaxOutfitBytes))
			{
				return false;
			}
			if (value.Type == SfsType.SfsObject)
			{
				SfsObject o = value.AsObject;
				if (o.Count > 4)
				{
					return false;
				}
				foreach (KeyValuePair<string, SfsValue> e in o.Entries)
				{
					if (e.Value.Type == SfsType.UtfString)
					{
						if (!FitsBytes((string)e.Value.Value, 255))
						{
							return false;
						}
					}
					else if (e.Value.Type == SfsType.SfsObject || e.Value.Type == SfsType.SfsArray || e.Value.Type == SfsType.FloatArray || e.Value.Type == SfsType.IntArray || e.Value.Type == SfsType.UtfStringArray)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static string VariableName(SfsValue v)
		{
			SfsArray a = v.AsArray;
			return (a != null && a.Count > 0) ? a[0].AsString : null;
		}

		private SfsValue UnwrapSignedVariable(LanPlayer player, SfsValue v)
		{
			SfsArray a = v.AsArray;
			string name = a[0].AsString;
			if (a[2].Type != SfsType.UtfString)
			{
				return v;
			}
			string json = (string)a[2].Value;
			object parsed;
			switch (name)
			{
			case "colour":
			{
				int colour;
				if (!LanJson.TryParse(json, out parsed) || !LanJson.TryInt(LanJson.Path(parsed, "data", "colour"), out colour))
				{
					return null;
				}
				player.Colour = colour;
				return new SfsValue(SfsType.SfsArray, IntVar("colour", colour));
			}
			case "tube":
			{
				int tubeId;
				if (!LanJson.TryParse(json, out parsed) || !LanJson.TryInt(LanJson.Path(parsed, "data", "tubeId"), out tubeId))
				{
					return null;
				}
				player.SelectedTubeId = tubeId;
				return new SfsValue(SfsType.SfsArray, IntVar("tube", tubeId));
			}
			case "outfit":
			{
				if (!LanJson.TryParse(json, out parsed))
				{
					return null;
				}
				string outfit = LanJson.ExtractRaw(json, "data");
				if (!IsOutfitJson(outfit))
				{
					return null;
				}
				player.OutfitJson = outfit;
				return new SfsValue(SfsType.SfsArray, StringVar("outfit", outfit));
			}
			default:
				return v;
			}
		}

		private void HandleSetUserVariables(LanPlayer player, SfsObject p)
		{
			SfsValue raw = p.Get("vl");
			if (raw == null || raw.AsArray == null)
			{
				return;
			}
			List<SfsValue> accepted = new List<SfsValue>();
			foreach (SfsValue v in raw.AsArray.Items)
			{
				if (!IsClientVariable(v))
				{
					continue;
				}
				SfsValue unwrapped = UnwrapSignedVariable(player, v);
				if (unwrapped != null)
				{
					accepted.Add(unwrapped);
				}
			}
			if (accepted.Count == 0)
			{
				return;
			}
			List<SfsValue> echoed = player.HeldActionCount.HasValue ? accepted.Where((SfsValue v) => VariableName(v) != "con").ToList() : accepted;
			if (echoed.Count > 0)
			{
				SfsArray vl = new SfsArray();
				foreach (SfsValue v in echoed)
				{
					vl.Add(v);
				}
				SendToRoom(player.RoomKey, Sys(12, new SfsObject().PutInt("u", player.UserId).PutArray("vl", vl)));
			}
			foreach (SfsValue v in accepted)
			{
				SfsArray a = v.AsArray;
				string name = a[0].AsString;
				if (name == "con")
				{
					LanEquippedObject next = null;
					SfsObject o = a[2].AsObject;
					if (o != null)
					{
						string type = o.GetString("type");
						string itemName = o.GetString("name");
						if (type != null && itemName != null)
						{
							next = new LanEquippedObject(type, itemName);
						}
					}
					Consumables.HandleClientConChanged(player, next);
				}
				else if (name == "loc")
				{
					player.LocomotionState = (a[2].Type == SfsType.Int) ? ((int)a[2].Value) : 0;
				}
			}
		}
	}

	public sealed class LanEquippedObject
	{
		public readonly string Type;

		public readonly string Name;

		public LanEquippedObject(string type, string name)
		{
			Type = type;
			Name = name;
		}
	}
}
#endif
