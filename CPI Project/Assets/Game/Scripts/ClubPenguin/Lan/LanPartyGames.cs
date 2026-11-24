#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanPartyGameDef
	{
		public int Id;

		public int GameType;

		public int LobbyType;

		public int MinPlayers;

		public int MaxPlayers;

		public int NumTeams;

		public Dictionary<string, object> GameData = new Dictionary<string, object>();

		public Dictionary<string, object> LobbyData = new Dictionary<string, object>();

		public readonly Dictionary<int, LanReward> Rewards = new Dictionary<int, LanReward>();

		public bool HasLauncher;

		public int LauncherMethod;

		public string TriggerProp;

		public string LobbyProp;

		public string ScheduledZone;

		public double EveryXMinutes;

		public string PlayerProp;

		public double Data(string key, double fallback)
		{
			return LanJson.Number(LanJson.Get(GameData, key), fallback);
		}
	}

	public sealed class LanTubeModifier
	{
		public int Id;

		public double Delta;

		public float[] Pos;
	}

	public sealed class LanPartyGameDefinitions
	{
		private readonly Dictionary<int, LanPartyGameDef> games = new Dictionary<int, LanPartyGameDef>();

		private readonly Dictionary<string, int> byTrigger = new Dictionary<string, int>();

		private readonly Dictionary<int, List<KeyValuePair<int, List<LanTubeModifier>>>> tubeLayouts = new Dictionary<int, List<KeyValuePair<int, List<LanTubeModifier>>>>();

		public LanPartyGameDefinitions(string partyGamesJson, string tubeRacesJson)
		{
			object root;
			if (!string.IsNullOrEmpty(partyGamesJson) && LanJson.TryParse(partyGamesJson, out root) && root is Dictionary<string, object>)
			{
				foreach (KeyValuePair<string, object> kv in (Dictionary<string, object>)root)
				{
					try
					{
						object g = kv.Value;
						LanPartyGameDef def = new LanPartyGameDef();
						def.Id = (int)LanJson.Number(LanJson.Get(g, "id"), -1.0);
						def.GameType = (int)LanJson.Number(LanJson.Get(g, "gameType"), -1.0);
						def.LobbyType = (int)LanJson.Number(LanJson.Get(g, "lobbyType"), 0.0);
						def.MinPlayers = (int)LanJson.Number(LanJson.Get(g, "minPlayers"), 1.0);
						def.MaxPlayers = (int)LanJson.Number(LanJson.Get(g, "maxPlayers"), 1.0);
						def.NumTeams = (int)LanJson.Number(LanJson.Get(g, "numTeams"), 1.0);
						def.GameData = (LanJson.Get(g, "gameData") as Dictionary<string, object>) ?? new Dictionary<string, object>();
						def.LobbyData = (LanJson.Get(g, "lobbyData") as Dictionary<string, object>) ?? new Dictionary<string, object>();
						foreach (object r in (LanJson.Get(g, "rewards") as List<object>) ?? new List<object>())
						{
							int placement;
							if (LanJson.TryInt(LanJson.Get(r, "placement"), out placement) && !def.Rewards.ContainsKey(placement))
							{
								def.Rewards[placement] = LanReward.From(LanJson.Get(r, "reward"));
							}
						}
						object launcher = LanJson.Get(g, "launcher");
						if (launcher != null)
						{
							def.HasLauncher = true;
							def.LauncherMethod = (int)LanJson.Number(LanJson.Get(launcher, "method"), 0.0);
							def.TriggerProp = LanJson.Get(launcher, "triggerProp") as string;
							def.LobbyProp = LanJson.Get(launcher, "lobbyProp") as string;
							def.ScheduledZone = LanJson.Get(launcher, "scheduledZone") as string;
							def.EveryXMinutes = LanJson.Number(LanJson.Get(launcher, "everyXMinutes"), 0.0);
						}
						def.PlayerProp = LanJson.Get(g, "playerProp") as string;
						games[def.Id] = def;
						if (!string.IsNullOrEmpty(def.TriggerProp))
						{
							byTrigger[def.TriggerProp] = def.Id;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			object tubes;
			if (string.IsNullOrEmpty(tubeRacesJson) || !LanJson.TryParse(tubeRacesJson, out tubes) || !(tubes is Dictionary<string, object>))
			{
				return;
			}
			foreach (KeyValuePair<string, object> kv in (Dictionary<string, object>)tubes)
			{
				int template;
				if (!int.TryParse(kv.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out template))
				{
					continue;
				}
				List<KeyValuePair<int, List<LanTubeModifier>>> layouts = new List<KeyValuePair<int, List<LanTubeModifier>>>();
				Dictionary<string, object> raw = LanJson.Get(kv.Value, "layouts") as Dictionary<string, object>;
				if (raw != null)
				{
					foreach (KeyValuePair<string, object> layout in raw)
					{
						int layoutId;
						if (!int.TryParse(layout.Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out layoutId))
						{
							continue;
						}
						List<LanTubeModifier> mods = new List<LanTubeModifier>();
						foreach (object m in (layout.Value as List<object>) ?? new List<object>())
						{
							LanTubeModifier mod = new LanTubeModifier();
							int id;
							if (!LanJson.TryInt(LanJson.Get(m, "id"), out id))
							{
								continue;
							}
							mod.Id = id;
							mod.Delta = LanJson.Number(LanJson.Get(m, "delta"), 0.0);
							mod.Pos = LanWorldData.Vec3(LanJson.Get(m, "pos"));
							if (mod.Pos != null)
							{
								mods.Add(mod);
							}
						}
						layouts.Add(new KeyValuePair<int, List<LanTubeModifier>>(layoutId, mods));
					}
				}
				tubeLayouts[template] = layouts;
			}
		}

		public LanPartyGameDef Get(int templateId)
		{
			LanPartyGameDef d;
			return games.TryGetValue(templateId, out d) ? d : null;
		}

		public int? TemplateForTrigger(string propName)
		{
			int id;
			return (propName != null && byTrigger.TryGetValue(propName, out id)) ? new int?(id) : null;
		}

		public LanReward RewardFor(LanPartyGameDef def, int placement)
		{
			LanReward r;
			return def.Rewards.TryGetValue(placement, out r) ? r : null;
		}

		public List<LanPartyGameDef> ScheduledGames()
		{
			return games.Values.Where((LanPartyGameDef g) => g.HasLauncher && g.LauncherMethod == 1 && g.EveryXMinutes > 0.0 && !string.IsNullOrEmpty(g.ScheduledZone)).ToList();
		}

		public List<KeyValuePair<int, List<LanTubeModifier>>> TubeLayouts(int templateId)
		{
			List<KeyValuePair<int, List<LanTubeModifier>>> l;
			return tubeLayouts.TryGetValue(templateId, out l) ? l : new List<KeyValuePair<int, List<LanTubeModifier>>>();
		}
	}

	public sealed class LanPartyGamePlayer
	{
		public LanPlayer Player;

		public bool Left;

		public bool Released;

		public int TeamId;

		public int RoleId;
	}

	public sealed class LanSlot
	{
		public int TeamId;

		public int RoleId;

		public LanSlot(int teamId, int roleId)
		{
			TeamId = teamId;
			RoleId = roleId;
		}
	}

	public abstract class LanPartyGameSession
	{
		public readonly int Id;

		public readonly LanPartyGameDef Def;

		public readonly List<LanPartyGamePlayer> Players = new List<LanPartyGamePlayer>();

		public bool Ended;

		protected readonly LanPartyGames Host;

		private readonly List<LanTimer> timers = new List<LanTimer>();

		public int TemplateId
		{
			get
			{
				return Def.Id;
			}
		}

		protected LanPartyGameSession(LanPartyGames host, int id, LanPartyGameDef def, List<LanPlayer> players, List<LanSlot> slots)
		{
			Host = host;
			Id = id;
			Def = def;
			for (int i = 0; i < players.Count; i++)
			{
				LanPartyGamePlayer p = new LanPartyGamePlayer();
				p.Player = players[i];
				p.TeamId = (slots != null && i < slots.Count) ? slots[i].TeamId : 0;
				p.RoleId = (slots != null && i < slots.Count) ? slots[i].RoleId : 0;
				Players.Add(p);
			}
		}

		protected LanLoop Loop
		{
			get
			{
				return Host.Server.Loop;
			}
		}

		protected void Timer(double seconds, Action fn)
		{
			LanTimer t = null;
			t = Host.Server.Loop.After(seconds * 1000.0, delegate
			{
				timers.Remove(t);
				if (!Ended)
				{
					Host.Guarded(this, fn);
				}
			});
			timers.Add(t);
		}

		public void ClearTimers()
		{
			foreach (LanTimer t in timers)
			{
				t.Cancel();
			}
			timers.Clear();
		}

		protected void SendMessage(int type, string body)
		{
			Host.SendGameMessage(this, type, body);
		}

		protected void SendMessageTo(LanPartyGamePlayer player, int type, string body)
		{
			Host.SendGameMessageTo(this, player.Player, type, body);
		}

		protected void Release(LanPartyGamePlayer player)
		{
			Host.ReleasePlayer(this, player);
		}

		protected void EndWith(List<KeyValuePair<LanPlayer, int>> placements, HashSet<LanPlayer> unpaid = null)
		{
			Host.EndSession(this, placements, unpaid);
		}

		protected double Seconds(string key, double fallback)
		{
			return Def.Data(key, fallback);
		}

		protected static string Sid(LanPartyGamePlayer p)
		{
			return p.Player.PlayerSessionId;
		}

		public virtual string GameProp
		{
			get
			{
				return Def.PlayerProp;
			}
		}

		public virtual string GamePropFor(LanPartyGamePlayer player)
		{
			return GameProp;
		}

		public abstract void OnStart();

		public virtual void OnEnded()
		{
		}

		public abstract void OnMessage(LanPartyGamePlayer player, int type, object message);

		public virtual void OnPlayerMoved(LanPartyGamePlayer player, float[] position)
		{
		}

		public virtual void OnAction(LanPartyGamePlayer player, int action, float[] position)
		{
		}

		public abstract void OnPlayerLeft(LanPartyGamePlayer player);

		public abstract void OnPlayerQuit(LanPartyGamePlayer player);

		protected static List<KeyValuePair<LanPlayer, int>> Placements(params KeyValuePair<LanPlayer, int>[] items)
		{
			return new List<KeyValuePair<LanPlayer, int>>(items);
		}

		protected static KeyValuePair<LanPlayer, int> Place(LanPlayer p, int placement)
		{
			return new KeyValuePair<LanPlayer, int>(p, placement);
		}
	}

	public sealed class LanPartyGames
	{
		private const int RewardSourceMiniGame = 5;

		private const int MaxMessageBytes = 4096;

		public readonly LanServer Server;

		public readonly LanPartyGameDefinitions Definitions;

		private readonly Dictionary<int, LanPartyGameSession> sessions = new Dictionary<int, LanPartyGameSession>();

		private readonly Dictionary<int, LanPartyGameSession> sessionByUser = new Dictionary<int, LanPartyGameSession>();

		private int nextSessionId = 1;

		public LanPartyGames(LanServer server, LanPartyGameDefinitions definitions)
		{
			Server = server;
			Definitions = definitions;
		}

		private static bool HasFactory(int gameType)
		{
			return gameType >= 0 && gameType <= 5;
		}

		private LanPartyGameSession Create(int id, LanPartyGameDef def, List<LanPlayer> players, List<LanSlot> slots)
		{
			switch (def.GameType)
			{
			case 0:
				return new LanScavengerHunt(this, id, def, players, slots);
			case 1:
				return new LanFishBucket(this, id, def, players);
			case 2:
				return new LanFindFour(this, id, def, players);
			case 3:
				return new LanDanceBattle(this, id, def, players, slots);
			case 4:
			case 5:
				return new LanTubeRace(this, id, def, players, slots);
			default:
				return null;
			}
		}

		private static bool Startable(LanPartyGameDef def)
		{
			return def != null && (!string.IsNullOrEmpty(def.PlayerProp) || !string.IsNullOrEmpty(def.TriggerProp) || (def.HasLauncher && def.LauncherMethod == 1)) && HasFactory(def.GameType);
		}

		public bool CanStart(int templateId)
		{
			return Startable(Definitions.Get(templateId));
		}

		public bool IsInGame(LanPlayer p)
		{
			return sessionByUser.ContainsKey(p.UserId);
		}

		public bool CanStartWith(int templateId, List<LanPlayer> players)
		{
			LanPartyGameDef def = Definitions.Get(templateId);
			if (!Startable(def))
			{
				return false;
			}
			if (players.Count < def.MinPlayers || players.Count > def.MaxPlayers)
			{
				return false;
			}
			if (players.Select((LanPlayer p) => p.UserId).Distinct().Count() != players.Count)
			{
				return false;
			}
			HashSet<LanPlayer> connected = new HashSet<LanPlayer>(Server.All());
			return players.All((LanPlayer p) => connected.Contains(p) && p.PlayerSessionId != null && !sessionByUser.ContainsKey(p.UserId));
		}

		public LanPartyGameSession SessionOf(LanPlayer p)
		{
			LanPartyGameSession s;
			return sessionByUser.TryGetValue(p.UserId, out s) ? s : null;
		}

		public void Guarded(LanPartyGameSession game, Action fn)
		{
			try
			{
				fn();
			}
			catch (Exception)
			{
				EndSession(game, new List<KeyValuePair<LanPlayer, int>>(), null);
			}
		}

		public void SetProp(LanPlayer p, string name)
		{
			p.HeldObject = name;
			SfsArray v = (name != null) ? LanServer.ConVar("pgame", name) : LanServer.NullVar("con");
			Server.BroadcastUserVars(p, v);
		}

		public void SetLocomotion(LanPlayer p, int state)
		{
			p.LocomotionState = state;
			Server.BroadcastUserVars(p, LanServer.IntVar("loc", state));
		}

		private static List<LanPlayer> Present(LanPartyGameSession game)
		{
			return (from p in game.Players
				where !p.Left && !p.Released
				select p.Player).ToList();
		}

		public LanPartyGameSession Start(int templateId, List<LanPlayer> players, List<LanSlot> slots)
		{
			LanPartyGameDef def = Definitions.Get(templateId);
			if (def == null || (slots != null && slots.Count != players.Count) || !CanStartWith(templateId, players))
			{
				return null;
			}
			LanPartyGameSession game = Create(nextSessionId++, def, players, slots);
			if (game == null)
			{
				return null;
			}
			sessions[game.Id] = game;
			foreach (LanPlayer p in players)
			{
				sessionByUser[p.UserId] = game;
			}
			if (!string.IsNullOrEmpty(def.PlayerProp))
			{
				foreach (LanPlayer p in players)
				{
					SetProp(p, def.PlayerProp);
				}
			}
			StringBuilder sb = new StringBuilder("{\"Players\":[");
			for (int i = 0; i < players.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(',');
				}
				int team = (slots != null) ? slots[i].TeamId : 0;
				int role = (slots != null) ? slots[i].RoleId : 0;
				sb.Append("{\"TeamId\":").Append(team).Append(",\"UserSessionId\":").Append(players[i].PlayerSessionId).Append(",\"RoleId\":")
					.Append(role)
					.Append('}');
			}
			sb.Append("]}");
			byte[] start = LanServer.Ext("pgame.start.v2", new SfsObject().PutInt("id", game.Id).PutInt("template", templateId).PutString("players", sb.ToString()));
			foreach (LanPlayer p in players)
			{
				p.Send(start);
			}
			Guarded(game, game.OnStart);
			return game;
		}

		private static byte[] MessagePacket(LanPartyGameSession game, int type, string message)
		{
			return LanServer.Ext("pgame.message", new SfsObject().PutInt("id", game.Id).PutInt("type", type).PutString("message", message));
		}

		public void SendGameMessage(LanPartyGameSession game, int type, string message)
		{
			if (game.Ended)
			{
				return;
			}
			byte[] packet = MessagePacket(game, type, message);
			foreach (LanPlayer p in Present(game))
			{
				p.Send(packet);
			}
		}

		public void SendGameMessageTo(LanPartyGameSession game, LanPlayer player, int type, string message)
		{
			if (!game.Ended && Present(game).Contains(player))
			{
				player.Send(MessagePacket(game, type, message));
			}
		}

		public void ReleasePlayer(LanPartyGameSession game, LanPartyGamePlayer player)
		{
			if (game.Ended || player.Left || player.Released)
			{
				return;
			}
			player.Released = true;
			player.Player.Send(LanServer.Ext("pgame.end", new SfsObject().PutInt("id", game.Id).PutString("results", "[]")));
			LanPartyGameSession current;
			if (sessionByUser.TryGetValue(player.Player.UserId, out current) && current == game)
			{
				sessionByUser.Remove(player.Player.UserId);
			}
			string prop = game.GamePropFor(player);
			if (prop != null && player.Player.HeldObject == prop)
			{
				SetProp(player.Player, null);
			}
		}

		public void EndSession(LanPartyGameSession game, List<KeyValuePair<LanPlayer, int>> placements, HashSet<LanPlayer> unpaid)
		{
			if (game.Ended)
			{
				return;
			}
			game.Ended = true;
			game.ClearTimers();
			List<LanPlayer> here = Present(game);
			StringBuilder sb = new StringBuilder("[");
			for (int i = 0; i < placements.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(',');
				}
				sb.Append("{\"PlayerSessionId\":").Append(placements[i].Key.PlayerSessionId).Append(",\"Placement\":")
					.Append(placements[i].Value)
					.Append('}');
			}
			sb.Append(']');
			byte[] end = LanServer.Ext("pgame.end", new SfsObject().PutInt("id", game.Id).PutString("results", sb.ToString()));
			foreach (LanPlayer p in here)
			{
				p.Send(end);
			}
			foreach (LanPartyGamePlayer p in game.Players)
			{
				string prop = game.GamePropFor(p);
				if (prop != null && !p.Left && !p.Released && p.Player.HeldObject == prop)
				{
					SetProp(p.Player, null);
				}
			}
			foreach (KeyValuePair<LanPlayer, int> kv in placements)
			{
				LanReward reward = Definitions.RewardFor(game.Def, kv.Value);
				if (reward != null && kv.Key.PlayerSessionId != null && (unpaid == null || !unpaid.Contains(kv.Key)) && here.Contains(kv.Key))
				{
					Server.SendRewardAdd(kv.Key, "reward.add_delayed", RewardSourceMiniGame, game.Id.ToString(CultureInfo.InvariantCulture), reward.ToJson());
				}
			}
			try
			{
				game.OnEnded();
			}
			catch (Exception)
			{
			}
			sessions.Remove(game.Id);
			foreach (LanPartyGamePlayer p in game.Players)
			{
				LanPartyGameSession current;
				if (sessionByUser.TryGetValue(p.Player.UserId, out current) && current == game)
				{
					sessionByUser.Remove(p.Player.UserId);
				}
			}
		}

		public void HandleMessage(LanPlayer player, SfsObject p)
		{
			long id;
			long type;
			string text = p.GetString("message");
			if (!p.TryGetInteger("id", out id) || !p.TryGetInteger("type", out type) || text == null)
			{
				return;
			}
			LanPartyGameSession game;
			if (id < int.MinValue || id > int.MaxValue || !sessions.TryGetValue((int)id, out game))
			{
				return;
			}
			LanPartyGamePlayer gp = game.Players.FirstOrDefault((LanPartyGamePlayer x) => x.Player == player && !x.Left);
			if (gp == null || Encoding.UTF8.GetByteCount(text) > MaxMessageBytes)
			{
				return;
			}
			object message;
			if (!LanJson.TryParse(text, out message))
			{
				return;
			}
			Guarded(game, delegate
			{
				game.OnMessage(gp, (int)type, message);
			});
		}

		private LanPartyGamePlayer Find(LanPlayer p, out LanPartyGameSession game)
		{
			game = SessionOf(p);
			if (game == null || game.Ended)
			{
				return null;
			}
			return game.Players.FirstOrDefault((LanPartyGamePlayer x) => x.Player == p && !x.Left);
		}

		public void OnPlayerQuit(LanPlayer p)
		{
			LanPartyGameSession game;
			LanPartyGamePlayer gp = Find(p, out game);
			if (gp != null)
			{
				Guarded(game, delegate
				{
					game.OnPlayerQuit(gp);
				});
			}
		}

		public void OnPlayerMoved(LanPlayer p, float[] position)
		{
			LanPartyGameSession game;
			LanPartyGamePlayer gp = Find(p, out game);
			if (gp != null)
			{
				Guarded(game, delegate
				{
					game.OnPlayerMoved(gp, position);
				});
			}
		}

		public void OnAction(LanPlayer p, int action, float[] position)
		{
			LanPartyGameSession game;
			LanPartyGamePlayer gp = Find(p, out game);
			if (gp != null)
			{
				Guarded(game, delegate
				{
					game.OnAction(gp, action, position);
				});
			}
		}

		public void OnPlayerLeft(LanPlayer p)
		{
			LanPartyGameSession game;
			if (!sessionByUser.TryGetValue(p.UserId, out game))
			{
				return;
			}
			sessionByUser.Remove(p.UserId);
			LanPartyGamePlayer gp = game.Players.FirstOrDefault((LanPartyGamePlayer x) => x.Player == p);
			if (gp == null || gp.Left || game.Ended)
			{
				return;
			}
			gp.Left = true;
			Guarded(game, delegate
			{
				game.OnPlayerLeft(gp);
			});
		}
	}

	public sealed class LanLobbyView
	{
		public int ItemId;

		public int TemplateId;

		public float[] Position;

		public int Members;

		public int MaxPlayers;

		public long EndsAtMs;
	}

	public sealed class LanLobbies
	{
		private sealed class Member
		{
			public LanPlayer Player;

			public int TeamId;

			public int RoleId;
		}

		private sealed class Lobby
		{
			public string RoomKey;

			public LanPartyGameDef Def;

			public LanRoomItem Item;

			public readonly List<Member> Members = new List<Member>();

			public LanTimer Timer;

			public long EndsAtMs;
		}

		private const int LocomotionInteract = 2;

		private const int LocomotionChargeThrow = 3;

		private const int LocomotionLaunchThrow = 4;

		private const int LocomotionCancelThrow = 10;

		private const long MaxChargeMs = 10000;

		private const int ItemCategoryPartyGameLobby = 6;

		private const double StartDelaySeconds = 1;

		private readonly LanServer server;

		private readonly Dictionary<string, Lobby> lobbies = new Dictionary<string, Lobby>();

		private readonly Dictionary<string, List<LanTimer>> schedules = new Dictionary<string, List<LanTimer>>();

		public LanLobbies(LanServer server)
		{
			this.server = server;
		}

		private static string Key(string roomKey, int templateId)
		{
			return roomKey + "\0" + templateId.ToString(CultureInfo.InvariantCulture);
		}

		private List<Lobby> In(string roomKey)
		{
			return lobbies.Values.Where((Lobby l) => l.RoomKey == roomKey).ToList();
		}

		public List<LanLobbyView> Open(string roomKey)
		{
			return In(roomKey).Select(delegate(Lobby l)
			{
				LanLobbyView v = new LanLobbyView();
				v.ItemId = l.Item.ItemId;
				v.TemplateId = l.Def.Id;
				v.Position = l.Item.Position;
				v.Members = l.Members.Count;
				v.MaxPlayers = l.Def.MaxPlayers;
				v.EndsAtMs = l.EndsAtMs;
				return v;
			}).ToList();
		}

		public LanSlot SlotOf(LanPlayer player)
		{
			foreach (Lobby l in lobbies.Values)
			{
				Member m = l.Members.FirstOrDefault((Member x) => x.Player == player);
				if (m != null)
				{
					return new LanSlot(m.TeamId, m.RoleId);
				}
			}
			return null;
		}

		public bool IsMember(LanPlayer player)
		{
			return lobbies.Values.Any((Lobby l) => l.Members.Any((Member m) => m.Player == player));
		}

		private static string RosterJson(List<Member> members)
		{
			StringBuilder sb = new StringBuilder("{\"Players\":[");
			for (int i = 0; i < members.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(',');
				}
				sb.Append("{\"TeamId\":").Append(members[i].TeamId).Append(",\"RoleId\":").Append(members[i].RoleId)
					.Append(",\"UserSessionId\":")
					.Append(members[i].Player.PlayerSessionId)
					.Append('}');
			}
			sb.Append("]}");
			return sb.ToString();
		}

		private static double LobbyLength(LanPartyGameDef def)
		{
			return LanJson.Number(LanJson.Get(def.LobbyData, "LobbyLengthInSeconds"), 30.0);
		}

		private static int LobbyLocomotion(LanPartyGameDef def)
		{
			return (int)LanJson.Number(LanJson.Get(def.LobbyData, "LobbyLocomotionState"), 0.0);
		}

		private static float[] LobbyLocation(LanPartyGameDef def)
		{
			object loc = LanJson.Get(def.LobbyData, "LobbyLocation");
			return new float[3]
			{
				(float)LanJson.Number(LanJson.Get(loc, "x"), 0.0),
				(float)LanJson.Number(LanJson.Get(loc, "y"), 0.0),
				(float)LanJson.Number(LanJson.Get(loc, "z"), 0.0)
			};
		}

		private bool RoomHasGame(string roomKey, int templateId)
		{
			return server.InRoom(roomKey).Any(delegate(LanPlayer p)
			{
				LanPartyGameSession s = server.PartyGames.SessionOf(p);
				return s != null && s.TemplateId == templateId;
			});
		}

		public bool OpenLobby(string roomKey, int templateId, double ttlSeconds)
		{
			LanPartyGameDef def = server.PartyGames.Definitions.Get(templateId);
			if (def == null || !def.HasLauncher || def.LauncherMethod != 1 || !server.PartyGames.CanStart(templateId))
			{
				return false;
			}
			string key = Key(roomKey, templateId);
			if (lobbies.ContainsKey(key) || RoomHasGame(roomKey, templateId))
			{
				return false;
			}
			Lobby lobby = new Lobby();
			lobby.RoomKey = roomKey;
			lobby.Def = def;
			lobby.Item = server.RoomItems.Add(roomKey, LobbyLocation(def), LanRoomItems.V("category", LanRoomItems.Int(ItemCategoryPartyGameLobby)), LanRoomItems.V("creator", LanRoomItems.Int(0)), LanRoomItems.V("int_a", LanRoomItems.Int(templateId)), LanRoomItems.V("statetimestamp", LanRoomItems.Str((LanServer.EpochMs / 1000).ToString(CultureInfo.InvariantCulture))), LanRoomItems.V("ttl", LanRoomItems.Int(Math.Max(1, (int)Math.Round(ttlSeconds)))), LanRoomItems.V("score", LanRoomItems.Str(RosterJson(lobby.Members))));
			lobby.EndsAtMs = server.Loop.NowMs + (long)(ttlSeconds * 1000.0);
			lobby.Timer = server.Loop.After(ttlSeconds * 1000.0, delegate
			{
				Finish(lobby);
			});
			lobbies[key] = lobby;
			return true;
		}

		private void Finish(Lobby lobby)
		{
			string key = Key(lobby.RoomKey, lobby.Def.Id);
			Lobby current;
			if (!lobbies.TryGetValue(key, out current) || current != lobby)
			{
				return;
			}
			lobbies.Remove(key);
			lobby.Timer.Cancel();
			server.RoomItems.Remove(lobby.Item);
			HashSet<LanPlayer> here = new HashSet<LanPlayer>(server.InRoom(lobby.RoomKey));
			List<Member> members = lobby.Members.Where((Member m) => here.Contains(m.Player)).ToList();
			if (!string.IsNullOrEmpty(lobby.Def.LobbyProp))
			{
				foreach (Member m in members)
				{
					server.PartyGames.SetProp(m.Player, null);
				}
			}
			int teams = members.Select((Member m) => m.TeamId).Distinct().Count();
			if (members.Count < lobby.Def.MinPlayers || teams < lobby.Def.NumTeams)
			{
				if (LobbyLocomotion(lobby.Def) != 0)
				{
					foreach (Member m in members)
					{
						server.PartyGames.SetLocomotion(m.Player, 0);
					}
				}
				return;
			}
			server.Loop.After(StartDelaySeconds * 1000.0, delegate
			{
				HashSet<LanPlayer> stillHere = new HashSet<LanPlayer>(server.InRoom(lobby.RoomKey));
				List<Member> ready = members.Where((Member m) => stillHere.Contains(m.Player)).OrderBy((Member m) => m.Player.IsBot ? 1 : 0).ToList();
				server.PartyGames.Start(lobby.Def.Id, ready.Select((Member m) => m.Player).ToList(), ready.Select((Member m) => new LanSlot(m.TeamId, m.RoleId)).ToList());
			});
		}

		private static LanSlot AssignSlot(List<Member> taken, int perTeam, int numTeams)
		{
			if (numTeams <= 0)
			{
				return null;
			}
			int[] sizes = new int[numTeams];
			foreach (Member m in taken)
			{
				if (m.TeamId >= 0 && m.TeamId < numTeams)
				{
					sizes[m.TeamId]++;
				}
			}
			int teamId = Array.IndexOf(sizes, sizes.Min());
			if (sizes[teamId] >= perTeam)
			{
				return null;
			}
			HashSet<int> used = new HashSet<int>(from m in taken
				where m.TeamId == teamId
				select m.RoleId);
			int roleId = 0;
			while (used.Contains(roleId))
			{
				roleId++;
			}
			return new LanSlot(teamId, roleId);
		}

		public void HandleInteraction(LanPlayer player, int action, string objectJson)
		{
			if (action != LocomotionInteract || objectJson == null)
			{
				return;
			}
			object obj;
			if (!LanJson.TryParse(objectJson, out obj))
			{
				return;
			}
			object type = LanJson.Get(obj, "type");
			long t;
			bool isServer = (LanJson.TryLong(type, out t) && t == 1) || (type as string) == "SERVER";
			string id = LanJson.Get(obj, "id") as string;
			int itemId;
			if (!isServer || id == null || !int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out itemId))
			{
				return;
			}
			Lobby lobby = In(player.RoomKey).FirstOrDefault((Lobby l) => l.Item.ItemId == itemId);
			if (lobby == null)
			{
				return;
			}
			if (player.ThrowChargingSince.HasValue && server.Loop.NowMs - player.ThrowChargingSince.Value < MaxChargeMs)
			{
				player.PendingLobbyItem = itemId;
				return;
			}
			Join(player, lobby);
		}

		public void OnThrowAction(LanPlayer player, int action)
		{
			if (action == LocomotionChargeThrow)
			{
				player.ThrowChargingSince = server.Loop.NowMs;
				return;
			}
			if (action != LocomotionLaunchThrow && action != LocomotionCancelThrow)
			{
				return;
			}
			player.ThrowChargingSince = null;
			int? pending = player.PendingLobbyItem;
			player.PendingLobbyItem = null;
			if (!pending.HasValue)
			{
				return;
			}
			Lobby lobby = In(player.RoomKey).FirstOrDefault((Lobby l) => l.Item.ItemId == pending.Value);
			if (lobby != null)
			{
				Join(player, lobby);
			}
		}

		private void Join(LanPlayer player, Lobby lobby)
		{
			player.PendingLobbyItem = null;
			if (!server.InRoom(player.RoomKey).Contains(player) || player.PlayerSessionId == null || server.PartyGames.IsInGame(player))
			{
				return;
			}
			if (In(player.RoomKey).Any((Lobby l) => l.Members.Any((Member m) => m.Player == player)))
			{
				return;
			}
			int numTeams = Math.Max(1, lobby.Def.NumTeams);
			LanSlot slot = AssignSlot(lobby.Members, lobby.Def.MaxPlayers / numTeams, numTeams);
			if (slot == null)
			{
				return;
			}
			Member member = new Member();
			member.Player = player;
			member.TeamId = slot.TeamId;
			member.RoleId = slot.RoleId;
			lobby.Members.Add(member);
			if (!string.IsNullOrEmpty(lobby.Def.LobbyProp))
			{
				server.PartyGames.SetProp(player, lobby.Def.LobbyProp);
			}
			int state = LobbyLocomotion(lobby.Def);
			if (state != 0)
			{
				server.PartyGames.SetLocomotion(player, state);
			}
			server.RoomItems.SetVar(lobby.Item, "score", LanRoomItems.Str(RosterJson(lobby.Members)));
		}

		public void OnPlayerLeft(LanPlayer player)
		{
			foreach (Lobby lobby in lobbies.Values.ToList())
			{
				DropMember(lobby, player);
			}
		}

		public void OnLobbyPropPutAway(LanPlayer player, string nextName)
		{
			if (player.HeldObject == null || player.HeldObject == nextName)
			{
				return;
			}
			foreach (Lobby lobby in In(player.RoomKey))
			{
				if (lobby.Def.LobbyProp == player.HeldObject && DropMember(lobby, player) && LobbyLocomotion(lobby.Def) != 0 && server.InRoom(player.RoomKey).Contains(player))
				{
					server.PartyGames.SetLocomotion(player, 0);
				}
			}
		}

		private bool DropMember(Lobby lobby, LanPlayer player)
		{
			int i = lobby.Members.FindIndex((Member m) => m.Player == player);
			if (i < 0)
			{
				return false;
			}
			lobby.Members.RemoveAt(i);
			server.RoomItems.SetVar(lobby.Item, "score", LanRoomItems.Str(RosterJson(lobby.Members)));
			return true;
		}

		private void ScheduleNext(string roomKey, LanPartyGameDef def, List<LanTimer> timers)
		{
			double lobbySeconds = LobbyLength(def);
			long now = LanServer.EpochMs;
			long period = (long)(def.EveryXMinutes * 60000.0);
			long opensAt = now / period * period;
			if (now >= opensAt + (long)(lobbySeconds * 1000.0))
			{
				opensAt += period;
			}
			long endsAt = opensAt + (long)(lobbySeconds * 1000.0);
			LanTimer open = null;
			open = server.Loop.After(Math.Max(0L, opensAt - now), delegate
			{
				timers.Remove(open);
				List<LanTimer> current;
				if (!schedules.TryGetValue(roomKey, out current) || current != timers)
				{
					return;
				}
				OpenLobby(roomKey, def.Id, Math.Max(1.0, (endsAt - LanServer.EpochMs) / 1000.0));
				LanTimer next = null;
				next = server.Loop.After(Math.Max(0L, endsAt - LanServer.EpochMs) + 100, delegate
				{
					timers.Remove(next);
					List<LanTimer> again;
					if (schedules.TryGetValue(roomKey, out again) && again == timers)
					{
						ScheduleNext(roomKey, def, timers);
					}
				});
				timers.Add(next);
			});
			timers.Add(open);
		}

		public void EnsureRoom(string roomKey)
		{
			if (schedules.ContainsKey(roomKey))
			{
				return;
			}
			string zone = LanServer.ZoneOfRoom(roomKey);
			List<LanPartyGameDef> games = server.PartyGames.Definitions.ScheduledGames().Where((LanPartyGameDef g) => server.PartyGames.CanStart(g.Id) && g.ScheduledZone == zone).ToList();
			if (games.Count == 0)
			{
				return;
			}
			List<LanTimer> timers = new List<LanTimer>();
			schedules[roomKey] = timers;
			foreach (LanPartyGameDef def in games)
			{
				ScheduleNext(roomKey, def, timers);
			}
		}

		public void ReleaseRoomIfEmpty(string roomKey)
		{
			if (server.RoomOccupancy(roomKey) > 0)
			{
				return;
			}
			List<LanTimer> timers;
			if (schedules.TryGetValue(roomKey, out timers))
			{
				foreach (LanTimer t in timers)
				{
					t.Cancel();
				}
				schedules.Remove(roomKey);
			}
			foreach (Lobby lobby in In(roomKey))
			{
				lobbies.Remove(Key(roomKey, lobby.Def.Id));
				lobby.Timer.Cancel();
				server.RoomItems.Remove(lobby.Item);
			}
		}
	}
}
#endif
