#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanTargetsView
	{
		public string Key;

		public string Path;

		public float[] Center;

		public double Radius;

		public readonly List<float[]> Targets = new List<float[]>();

		public readonly List<double> Radii = new List<double>();
	}

	public sealed class LanIslandTargets
	{
		private sealed class LiveTarget
		{
			public LanTargetData Data;

			public LanRoomItem Item;

			public int Capacity;

			public int Hits;
		}

		private sealed class Game
		{
			public LanSupersetData Superset;

			public int PhaseIndex;

			public int Serial;

			public LanRoomItem Group;

			public List<LiveTarget> Targets = new List<LiveTarget>();

			public long StartsMs;

			public bool RoundOver;

			public long CreatedMs;

			public readonly Dictionary<int, int> HitsByUser = new Dictionary<int, int>();
		}

		private sealed class Playground
		{
			public string RoomKey;

			public LanPlaygroundData Data;

			public Func<int> Pick;

			public LanRoomItem Stats;

			public int Current;

			public int Best;

			public string Day;

			public Game Game;

			public readonly Dictionary<int, long> LastThrowMs = new Dictionary<int, long>();

			public readonly List<LanTimer> Timers = new List<LanTimer>();
		}

		private const int CategoryTarget = 3;

		private const int CategoryGroup = 4;

		private const int CategoryStats = 5;

		private const int ActionLaunchThrow = 4;

		private const int RewardSourceMiniGame = 5;

		private const string RewardSourceId = "Targets";

		private const double WonRoundLingerSeconds = 2;

		private const double Gravity = -9.81;

		private const double HandOffsetY = 0.75;

		private const double SnowballRadius = 0.1;

		private const double HitSlack = 0.35;

		private const double MaxFlightSeconds = 3;

		private const long MinThrowGapMs = 400;

		private const double MaxThrowSpeed = 20;

		private const double MaxThrowDrift = 3;

		private const double StepSeconds = 1.0 / 240.0;

		private readonly LanServer server;

		private readonly LanWorldData data;

		private readonly Dictionary<string, List<Playground>> rooms = new Dictionary<string, List<Playground>>();

		private readonly Dictionary<string, long> paidAt = new Dictionary<string, long>();

		public LanIslandTargets(LanServer server, LanWorldData data)
		{
			this.server = server;
			this.data = data;
		}

		private LanTimer After(Playground pg, double ms, Action fn)
		{
			LanTimer t = null;
			t = server.Loop.After(ms, delegate
			{
				pg.Timers.Remove(t);
				fn();
			});
			pg.Timers.Add(t);
			return t;
		}

		private static string UtcDay()
		{
			return DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		}

		private void SetStats(Playground pg, int current, int best)
		{
			if (pg.Current != current)
			{
				server.RoomItems.SetVar(pg.Stats, "int_a", LanRoomItems.Int(current));
			}
			if (pg.Best != best)
			{
				server.RoomItems.SetVar(pg.Stats, "actioncount", LanRoomItems.Int(best));
			}
			pg.Current = current;
			pg.Best = best;
		}

		private void RollDay(Playground pg)
		{
			string today = UtcDay();
			if (pg.Day != today)
			{
				pg.Day = today;
				SetStats(pg, 0, 0);
			}
		}

		public void EnsureRoom(string roomKey, string roomName)
		{
			if (rooms.ContainsKey(roomKey))
			{
				return;
			}
			List<Playground> list = new List<Playground>();
			rooms[roomKey] = list;
			foreach (LanPlaygroundData d in data.Playgrounds(LanServer.ZoneOfRoom(roomKey), LanServer.ContentDateOf(roomName)))
			{
				Playground pg = new Playground();
				pg.RoomKey = roomKey;
				pg.Data = d;
				pg.Stats = server.RoomItems.Add(roomKey, d.Position, LanRoomItems.V("category", LanRoomItems.Int(CategoryStats)), LanRoomItems.V("creator", LanRoomItems.Int(0)), LanRoomItems.V("path", LanRoomItems.Str(d.Path)), LanRoomItems.V("int_a", LanRoomItems.Int(0)), LanRoomItems.V("actioncount", LanRoomItems.Int(0)));
				pg.Pick = LanPicker.Create(d.Behaviour, d.Supersets.Count, server.Random);
				pg.Day = UtcDay();
				list.Add(pg);
				ScheduleNext(pg);
			}
		}

		private static long NextGameStartMs(long nowMs, double everyMinutes)
		{
			DateTime d = DateTimeOffset.FromUnixTimeMilliseconds(nowMs).UtcDateTime;
			DateTime hourStart = new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0, DateTimeKind.Utc);
			int every = Math.Max(1, (int)everyMinutes);
			int mm = every * (1 + d.Minute / every);
			DateTime next = (mm < 60) ? hourStart.AddMinutes(mm) : hourStart.AddHours(1.0);
			return new DateTimeOffset(next).ToUnixTimeMilliseconds();
		}

		private void ScheduleNext(Playground pg)
		{
			long now = LanServer.EpochMs;
			long start = NextGameStartMs(now, pg.Data.EveryXMinutes);
			After(pg, start - now, delegate
			{
				if (pg.Game == null)
				{
					StartGame(pg);
				}
				ScheduleNext(pg);
			});
		}

		public void ReleaseRoomIfEmpty(string roomKey)
		{
			List<Playground> list;
			if (!rooms.TryGetValue(roomKey, out list) || server.RoomOccupancy(roomKey) > 0)
			{
				return;
			}
			foreach (Playground pg in list)
			{
				foreach (LanTimer t in pg.Timers.ToList())
				{
					t.Cancel();
				}
				pg.Timers.Clear();
				if (pg.Game != null)
				{
					RemoveRound(pg.Game);
				}
				pg.Game = null;
				server.RoomItems.Remove(pg.Stats);
			}
			rooms.Remove(roomKey);
		}

		private void StartGame(Playground pg)
		{
			RollDay(pg);
			Game game = new Game();
			game.Superset = pg.Data.Supersets[pg.Pick()];
			game.CreatedMs = LanServer.EpochMs;
			pg.Game = game;
			StartPhase(pg, game);
		}

		private List<LanPlayer> PlayersInZone(Playground pg)
		{
			return server.InRoom(pg.RoomKey).Where((LanPlayer p) => !p.IsBot && p.LastPosition != null && LanMotion.Dist(p.LastPosition, pg.Data.RewardsCenter) <= pg.Data.RewardsRadius).ToList();
		}

		private void StartPhase(Playground pg, Game game)
		{
			LanPhaseData phase = game.Superset.Phases[game.PhaseIndex];
			int present = PlayersInZone(pg).Count;
			game.Serial++;
			game.RoundOver = false;
			game.StartsMs = LanServer.EpochMs + (long)(pg.Data.RoundDelaySeconds * 1000.0);
			long expiresMs = game.StartsMs + (long)(phase.SessionSeconds * 1000.0);
			game.Group = server.RoomItems.Add(pg.RoomKey, phase.Position, LanRoomItems.V("category", LanRoomItems.Int(CategoryGroup)), LanRoomItems.V("creator", LanRoomItems.Int(0)), LanRoomItems.V("path", LanRoomItems.Str(phase.Path)), LanRoomItems.V("timestamp", LanRoomItems.Str(game.StartsMs.ToString(CultureInfo.InvariantCulture))), LanRoomItems.V("statetimestamp", LanRoomItems.Str(expiresMs.ToString(CultureInfo.InvariantCulture))));
			game.Targets = phase.Targets.Select(delegate(LanTargetData t)
			{
				int capacity = (int)Math.Max(t.MinHits, t.PopulationMultiplier * (double)present);
				LiveTarget lt = new LiveTarget();
				lt.Data = t;
				lt.Capacity = capacity;
				lt.Item = server.RoomItems.Add(pg.RoomKey, t.Position, LanRoomItems.V("category", LanRoomItems.Int(CategoryTarget)), LanRoomItems.V("creator", LanRoomItems.Int(0)), LanRoomItems.V("path", LanRoomItems.Str(t.Path)), LanRoomItems.V("int_a", LanRoomItems.Int(capacity)), LanRoomItems.V("actioncount", LanRoomItems.Int(0)));
				return lt;
			}).ToList();
			int serial = game.Serial;
			After(pg, expiresMs - LanServer.EpochMs, delegate
			{
				if (pg.Game == game && game.Serial == serial && !game.RoundOver)
				{
					TimeOut(pg, game);
				}
			});
		}

		private void RemoveRound(Game game)
		{
			if (game.Group != null)
			{
				server.RoomItems.Remove(game.Group);
			}
			foreach (LiveTarget t in game.Targets)
			{
				server.RoomItems.Remove(t.Item);
			}
			game.Group = null;
			game.Targets = new List<LiveTarget>();
		}

		private void TimeOut(Playground pg, Game game)
		{
			game.RoundOver = true;
			RemoveRound(game);
			pg.Game = null;
			SetStats(pg, 0, pg.Best);
		}

		private void ApplyHit(Playground pg, Game game, LiveTarget target, int userId)
		{
			target.Hits++;
			server.RoomItems.SetVar(target.Item, "actioncount", LanRoomItems.Int(target.Hits));
			int prev;
			game.HitsByUser.TryGetValue(userId, out prev);
			game.HitsByUser[userId] = prev + 1;
			if (game.Targets.Any((LiveTarget t) => t.Hits < t.Capacity))
			{
				return;
			}
			game.RoundOver = true;
			if (game.Group != null)
			{
				server.RoomItems.Remove(game.Group);
			}
			game.Group = null;
			bool last = game.PhaseIndex == game.Superset.Phases.Count - 1;
			if (last)
			{
				Win(pg, game);
			}
			int serial = game.Serial;
			After(pg, WonRoundLingerSeconds * 1000.0, delegate
			{
				if (game.Serial != serial)
				{
					return;
				}
				RemoveRound(game);
				if (pg.Game != game)
				{
					return;
				}
				if (last)
				{
					pg.Game = null;
					return;
				}
				game.PhaseIndex++;
				StartPhase(pg, game);
			});
		}

		private static LanReward Split(LanReward total, LanReward minPer, int n)
		{
			LanReward r = new LanReward();
			r.Coins = (n <= 0) ? 0 : Math.Max(minPer.Coins, total.Coins / n);
			foreach (string name in total.MascotXP.Keys.Union(minPer.MascotXP.Keys))
			{
				long t;
				long m;
				total.MascotXP.TryGetValue(name, out t);
				minPer.MascotXP.TryGetValue(name, out m);
				r.MascotXP[name] = (n <= 0) ? 0 : Math.Max(m, t / n);
			}
			return r;
		}

		private void Win(Playground pg, Game game)
		{
			RollDay(pg);
			SetStats(pg, pg.Current + 1, Math.Max(pg.Best, pg.Current + 1));
			long now = server.Loop.NowMs;
			long windowMs = (long)(pg.Data.EveryXMinutes * 60000.0);
			foreach (string k in paidAt.Where((KeyValuePair<string, long> kv) => now - kv.Value >= windowMs).Select((KeyValuePair<string, long> kv) => kv.Key).ToList())
			{
				paidAt.Remove(k);
			}
			HashSet<string> accounts = new HashSet<string>();
			List<LanPlayer> eligible = PlayersInZone(pg).Where(delegate(LanPlayer p)
			{
				int hits;
				if (p.PlayerSessionId == null || !game.HitsByUser.TryGetValue(p.UserId, out hits) || hits == 0)
				{
					return false;
				}
				if (accounts.Contains(p.AccountKey) || paidAt.ContainsKey(p.AccountKey))
				{
					return false;
				}
				accounts.Add(p.AccountKey);
				return true;
			}).ToList();
			LanReward reward = Split(game.Superset.TotalReward, game.Superset.MinPerPlayerReward, eligible.Count);
			string json = reward.ToJson();
			foreach (LanPlayer p in eligible)
			{
				paidAt[p.AccountKey] = now;
				server.SendRewardAdd(p, "reward.add_delayed", RewardSourceMiniGame, RewardSourceId, json);
			}
		}

		public LanTargetsView View(string roomKey, bool throwable)
		{
			List<Playground> list;
			if (!rooms.TryGetValue(roomKey, out list))
			{
				return null;
			}
			long epoch = LanServer.EpochMs;
			Playground pg = list.FirstOrDefault((Playground x) => x.Game != null && (!throwable || (x.Game.Group != null && !x.Game.RoundOver && epoch >= x.Game.StartsMs)));
			if (pg == null)
			{
				return null;
			}
			LanTargetsView v = new LanTargetsView();
			v.Key = pg.Data.Path + ":" + pg.Game.CreatedMs.ToString(CultureInfo.InvariantCulture);
			v.Path = pg.Data.Path;
			v.Center = pg.Data.RewardsCenter;
			v.Radius = pg.Data.RewardsRadius;
			foreach (LiveTarget t in pg.Game.Targets)
			{
				if (t.Hits < t.Capacity)
				{
					v.Targets.Add(t.Data.HitCenter);
					v.Radii.Add(t.Data.HitRadius);
				}
			}
			return v;
		}

		public void ForgetThrower(LanPlayer p)
		{
			foreach (List<Playground> list in rooms.Values)
			{
				foreach (Playground pg in list)
				{
					pg.LastThrowMs.Remove(p.UserId);
				}
			}
		}

		private static bool Finite(float[] v)
		{
			return v != null && v.Length == 3 && v.All((float f) => !float.IsNaN(f) && !float.IsInfinity(f));
		}

		public void OnThrow(LanPlayer p, int action, float[] previous, float[] pos, float[] v)
		{
			if (action != ActionLaunchThrow || v == null)
			{
				return;
			}
			List<Playground> list;
			if (!rooms.TryGetValue(p.RoomKey, out list))
			{
				return;
			}
			Playground pg = list.FirstOrDefault((Playground x) => x.Game != null && x.Game.Group != null && !x.Game.RoundOver);
			if (pg == null)
			{
				return;
			}
			Game game = pg.Game;
			long epoch = LanServer.EpochMs;
			long now = server.Loop.NowMs;
			if (epoch < game.StartsMs || !Finite(pos) || !Finite(v))
			{
				return;
			}
			long last;
			if (pg.LastThrowMs.TryGetValue(p.UserId, out last) && now - last < MinThrowGapMs)
			{
				return;
			}
			if (Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]) > MaxThrowSpeed)
			{
				return;
			}
			if (!Finite(previous) || LanMotion.Dist(pos, previous) > MaxThrowDrift)
			{
				return;
			}
			pg.LastThrowMs[p.UserId] = now;
			List<LiveTarget> live = game.Targets.Where((LiveTarget t) => t.Hits < t.Capacity).ToList();
			double ox = pos[0];
			double oy = pos[1] + HandOffsetY;
			double oz = pos[2];
			double[] best = live.Select((LiveTarget t) => double.PositiveInfinity).ToArray();
			double[] bestT = new double[live.Count];
			double?[] contact = new double?[live.Count];
			int hitIndex = -1;
			for (int step = 0; (double)step * StepSeconds <= MaxFlightSeconds; step++)
			{
				double t = (double)step * StepSeconds;
				double x = ox + v[0] * t;
				double y = oy + v[1] * t + 0.5 * Gravity * t * t;
				double z = oz + v[2] * t;
				for (int i = 0; i < live.Count; i++)
				{
					float[] c = live[i].Data.HitCenter;
					double dx = x - c[0];
					double dy = y - c[1];
					double dz = z - c[2];
					double dist = Math.Sqrt(dx * dx + dy * dy + dz * dz) - live[i].Data.HitRadius - SnowballRadius;
					if (dist < best[i])
					{
						best[i] = dist;
						bestT[i] = t;
					}
					if (!contact[i].HasValue && dist <= 0.0)
					{
						contact[i] = t;
					}
					if (hitIndex < 0 && dist <= HitSlack)
					{
						hitIndex = i;
					}
				}
			}
			if (hitIndex < 0)
			{
				return;
			}
			LiveTarget target = live[hitIndex];
			double hitT = contact[hitIndex] ?? bestT[hitIndex];
			int serial = game.Serial;
			int userId = p.UserId;
			After(pg, hitT * 1000.0, delegate
			{
				if (pg.Game == game && game.Serial == serial && !game.RoundOver && target.Hits < target.Capacity)
				{
					ApplyHit(pg, game, target, userId);
				}
			});
		}
	}
}
#endif
