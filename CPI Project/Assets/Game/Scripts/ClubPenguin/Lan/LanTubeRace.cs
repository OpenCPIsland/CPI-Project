#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanTubeRace : LanPartyGameSession
	{
		private sealed class Result
		{
			public long ElapsedMs;

			public double Score;

			public bool Valid;
		}

		private sealed class Racer
		{
			public readonly List<float[]> Path = new List<float[]>();

			public readonly List<long> Times = new List<long>();

			public bool Overflow;

			public readonly List<int> Claims = new List<int>();

			public Result Result;
		}

		private const int SetLayout = 10;

		private const int CollectModifier = 11;

		private const int PlayerFinished = 12;

		private const int PlayerResult = 13;

		private const int LocomotionDefault = 0;

		private const int LocomotionRacing = 7;

		private const double EndSlackSeconds = 5;

		private const int MaxPathPoints = 2000;

		private const double CleanupDelaySeconds = 2;

		private const double ModifierReach = 3;

		private const int LastPlacement = 4;

		private List<LanTubeModifier> layout = new List<LanTubeModifier>();

		private long startMs;

		private bool endScheduled;

		private readonly Dictionary<LanPartyGamePlayer, Racer> racers = new Dictionary<LanPartyGamePlayer, Racer>();

		public LanTubeRace(LanPartyGames host, int id, LanPartyGameDef def, List<LanPlayer> players, List<LanSlot> slots)
			: base(host, id, def, players, slots)
		{
		}

		private double D(string key, double fallback)
		{
			return Def.Data(key, fallback);
		}

		public override void OnStart()
		{
			object l = LanJson.Get(Def.LobbyData, "LobbyLocation");
			float[] lobby = null;
			double lx;
			double ly;
			double lz;
			if (l != null && LanJson.TryNumber(LanJson.Get(l, "x"), out lx) && LanJson.TryNumber(LanJson.Get(l, "y"), out ly) && LanJson.TryNumber(LanJson.Get(l, "z"), out lz))
			{
				lobby = new float[3]
				{
					(float)lx,
					(float)ly,
					(float)lz
				};
			}
			foreach (LanPartyGamePlayer p in Players)
			{
				Racer r = new Racer();
				if (lobby != null)
				{
					r.Path.Add(lobby);
					r.Times.Add(0L);
				}
				float[] pos = p.Player.LastPosition;
				if (pos != null && pos.Length == 3)
				{
					r.Path.Add(LanMotion.Copy(pos));
					r.Times.Add(0L);
				}
				racers[p] = r;
				Host.SetLocomotion(p.Player, LocomotionRacing);
			}
			List<KeyValuePair<int, List<LanTubeModifier>>> layouts = Host.Definitions.TubeLayouts(Def.Id);
			int layoutId = 0;
			if (layouts.Count > 0)
			{
				KeyValuePair<int, List<LanTubeModifier>> chosen = layouts[Host.Server.Random.Next(layouts.Count)];
				layoutId = chosen.Key;
				layout = chosen.Value;
			}
			SendMessage(SetLayout, "{\"ModifierLayoutId\":" + layoutId + "}");
			startMs = Loop.NowMs;
			Timer(D("MaximumCompletionTimeInSeconds", 120.0) + EndSlackSeconds, TimeUp);
		}

		private long ClockMs()
		{
			return Loop.NowMs - startMs;
		}

		public override void OnPlayerMoved(LanPartyGamePlayer player, float[] position)
		{
			Racer racer;
			if (!racers.TryGetValue(player, out racer) || racer.Result != null)
			{
				return;
			}
			if (racer.Path.Count >= MaxPathPoints)
			{
				racer.Overflow = true;
				return;
			}
			racer.Path.Add(LanMotion.Copy(position));
			racer.Times.Add(ClockMs());
		}

		private static double SegmentDistance(float[] a, float[] b, float[] p)
		{
			double abx = b[0] - a[0];
			double aby = b[1] - a[1];
			double abz = b[2] - a[2];
			double len2 = abx * abx + aby * aby + abz * abz;
			double t = (len2 == 0.0) ? 0.0 : Math.Max(0.0, Math.Min(1.0, ((p[0] - a[0]) * abx + (p[1] - a[1]) * aby + (p[2] - a[2]) * abz) / len2));
			double cx = a[0] + t * abx - p[0];
			double cy = a[1] + t * aby - p[1];
			double cz = a[2] + t * abz - p[2];
			return Math.Sqrt(cx * cx + cy * cy + cz * cz);
		}

		private static bool PathReaches(List<float[]> path, float[] point, double reach)
		{
			if (path.Count == 1)
			{
				return LanMotion.Dist(path[0], point) <= reach;
			}
			for (int i = 1; i < path.Count; i++)
			{
				if (SegmentDistance(path[i - 1], path[i], point) <= reach)
				{
					return true;
				}
			}
			return false;
		}

		private double ModifierTotal(Racer racer)
		{
			double total = 0.0;
			foreach (int id in racer.Claims.Distinct())
			{
				LanTubeModifier m = layout.FirstOrDefault((LanTubeModifier x) => x.Id == id);
				if (m != null && PathReaches(racer.Path, m.Pos, ModifierReach))
				{
					total += m.Delta;
				}
			}
			return total;
		}

		private bool JudgeFinish(long elapsedMs, Racer racer)
		{
			if ((double)elapsedMs < D("MinimumCompletionTimeInSeconds", 0.0) * 1000.0 || (double)elapsedMs > D("MaximumCompletionTimeInSeconds", 120.0) * 1000.0)
			{
				return false;
			}
			if (racer.Overflow)
			{
				return false;
			}
			List<float[]> path = racer.Path;
			List<long> times = racer.Times;
			double maxJump = 0.0;
			double maxSpeed = 0.0;
			List<double> along = new List<double>
			{
				0.0
			};
			for (int j = 1; j < path.Count; j++)
			{
				double d = LanMotion.Dist(path[j - 1], path[j]);
				maxJump = Math.Max(maxJump, d);
				along.Add(along[j - 1] + d);
				for (int i = j - 1; i >= 0; i--)
				{
					long span = times[j] - times[i];
					maxSpeed = Math.Max(maxSpeed, (along[j] - along[i]) * 1000.0 / (double)Math.Max(span, LanMotion.SpeedSpanMs));
					if (span > LanMotion.SpeedSpanMs)
					{
						break;
					}
				}
			}
			if (maxJump > LanMotion.MaxJump || maxSpeed > LanMotion.MaxSpeed || path.Count == 0)
			{
				return false;
			}
			object e = LanJson.Get(Def.GameData, "RaceEndPosition");
			float[] end = new float[3]
			{
				(float)LanJson.Number(LanJson.Get(e, "x"), 0.0),
				(float)LanJson.Number(LanJson.Get(e, "y"), 0.0),
				(float)LanJson.Number(LanJson.Get(e, "z"), 0.0)
			};
			return LanMotion.Dist(path[path.Count - 1], end) <= D("MaxValidationRadius", 10.0);
		}

		public override void OnMessage(LanPartyGamePlayer player, int type, object message)
		{
			Racer racer;
			if (!racers.TryGetValue(player, out racer) || racer.Result != null)
			{
				return;
			}
			if (type == CollectModifier)
			{
				long id;
				if (LanJson.TryLong(LanJson.Get(message, "ModifierId"), out id) && id >= int.MinValue && id <= int.MaxValue && layout.Any((LanTubeModifier m) => m.Id == (int)id) && !racer.Claims.Contains((int)id))
				{
					racer.Claims.Add((int)id);
				}
			}
			else if (type == PlayerFinished)
			{
				long elapsed = ClockMs();
				bool valid = JudgeFinish(elapsed, racer);
				double modifier = valid ? ModifierTotal(racer) : 0.0;
				double score = valid ? (D("StartingPoints", 0.0) - Math.Floor((double)elapsed * D("PointsDeductedPerSecond", 0.0) / 1000.0) + modifier) : D("DefaultPoints", 0.0);
				Result result = new Result();
				result.ElapsedMs = elapsed;
				result.Valid = valid;
				result.Score = score;
				Publish(player, result, modifier);
				EndIfAllFinished();
			}
		}

		private void Publish(LanPartyGamePlayer player, Result result, double modifier)
		{
			racers[player].Result = result;
			SendMessage(PlayerResult, "{\"PlayerId\":" + Sid(player) + ",\"CompletionTimeInMilliseconds\":" + result.ElapsedMs.ToString(CultureInfo.InvariantCulture) + ",\"ScoreModifier\":" + LanJson.Serialize(modifier) + ",\"OverallScore\":" + LanJson.Serialize(result.Score) + "}");
		}

		private List<LanPartyGamePlayer> Here()
		{
			return Players.Where((LanPartyGamePlayer p) => !p.Left).ToList();
		}

		private Result ResultOf(LanPartyGamePlayer p)
		{
			Racer r;
			return racers.TryGetValue(p, out r) ? r.Result : null;
		}

		private void EndIfAllFinished()
		{
			if (Here().All((LanPartyGamePlayer p) => ResultOf(p) != null))
			{
				ScheduleEnd();
			}
		}

		private void TimeUp()
		{
			long cap = (long)(D("MaximumCompletionTimeInSeconds", 120.0) * 1000.0);
			foreach (LanPartyGamePlayer p in Here())
			{
				if (ResultOf(p) == null)
				{
					Result r = new Result();
					r.ElapsedMs = cap;
					r.Score = D("DefaultPoints", 0.0);
					r.Valid = false;
					Publish(p, r, 0.0);
				}
			}
			ScheduleEnd();
		}

		private void ScheduleEnd()
		{
			if (endScheduled)
			{
				return;
			}
			endScheduled = true;
			Timer(D("ShowRewardsDelayInSeconds", 3.0), Finish);
		}

		private void Finish()
		{
			List<KeyValuePair<LanPartyGamePlayer, Result>> rows = (from p in Here()
				where ResultOf(p) != null
				select new KeyValuePair<LanPartyGamePlayer, Result>(p, ResultOf(p))).ToList();
			HashSet<LanPlayer> unpaid = new HashSet<LanPlayer>(from p in Here()
				where ResultOf(p) == null || !ResultOf(p).Valid
				select p.Player);
			int single = (int)D("SinglePlayerPlacement", 0.0);
			List<KeyValuePair<LanPartyGamePlayer, Result>> valid = (from r in rows
				where r.Value.Valid
				orderby r.Value.Score descending, r.Value.ElapsedMs
				select r).ToList();
			List<KeyValuePair<LanPartyGamePlayer, Result>> invalid = (from r in rows
				where !r.Value.Valid
				orderby r.Value.ElapsedMs
				select r).ToList();
			List<KeyValuePair<LanPartyGamePlayer, Result>> ordered = valid.Concat(invalid).ToList();
			List<KeyValuePair<LanPlayer, int>> placements = new List<KeyValuePair<LanPlayer, int>>();
			if (ordered.Count > 0 && (rows.Count == 1 || valid.Count == 1))
			{
				placements.Add(Place(ordered[0].Key.Player, single));
				for (int i = 1; i < ordered.Count; i++)
				{
					placements.Add(Place(ordered[i].Key.Player, Math.Min(single + i, LastPlacement)));
				}
			}
			else
			{
				for (int j = 0; j < ordered.Count; j++)
				{
					placements.Add(Place(ordered[j].Key.Player, Math.Min(j, LastPlacement)));
				}
			}
			EndWith(placements, unpaid);
		}

		public override void OnPlayerLeft(LanPartyGamePlayer player)
		{
			if (Here().Count == 0)
			{
				EndWith(new List<KeyValuePair<LanPlayer, int>>());
			}
			else
			{
				EndIfAllFinished();
			}
		}

		public override void OnPlayerQuit(LanPartyGamePlayer player)
		{
		}

		public override void OnEnded()
		{
			List<LanPartyGamePlayer> here = Here();
			string roomKey = (here.Count > 0) ? here[0].Player.RoomKey : "";
			Host.Server.Loop.After(CleanupDelaySeconds * 1000.0, delegate
			{
				HashSet<LanPlayer> room = new HashSet<LanPlayer>(Host.Server.InRoom(roomKey));
				foreach (LanPartyGamePlayer p in here)
				{
					if (room.Contains(p.Player))
					{
						Host.SetLocomotion(p.Player, LocomotionDefault);
					}
				}
			});
		}
	}
}
#endif
