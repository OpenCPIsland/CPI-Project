#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanDanceBattle : LanPartyGameSession
	{
		private sealed class SequenceSet
		{
			public double SequenceDisplayTimeInSeconds;

			public double TurnTimeInSeconds;

			public int MinRound;

			public int MaxRound;

			public List<int> DanceMoveIds = new List<int>();

			public int SequenceLength;

			public double TurnOutcomeSequenceDelayInSeconds;
		}

		private const string DanceProp = "DanceBattleGameServerAdded";

		private const int LocomotionPartyGame = 2;

		private const int LocomotionDefault = 0;

		private const int ShowTurnOutput = 5;

		private const double ItemDelaySeconds = 0.1;

		private const double CleanupDelaySeconds = 2;

		private const double TieEpsilon = 0.001;

		private static readonly int[] Moves = new int[3]
		{
			1,
			2,
			3
		};

		private string phase = "intro";

		private LanRoomItem item;

		private int round;

		private SequenceSet set;

		private List<int> expected = new List<int>();

		private readonly Dictionary<LanPartyGamePlayer, List<int>> moves = new Dictionary<LanPartyGamePlayer, List<int>>();

		private readonly double[] totals = new double[2];

		private readonly HashSet<LanPartyGamePlayer> quit = new HashSet<LanPartyGamePlayer>();

		private readonly Dictionary<string, long> stamps = new Dictionary<string, long>();

		private readonly List<SequenceSet> sets = new List<SequenceSet>();

		public override string GameProp
		{
			get
			{
				return DanceProp;
			}
		}

		public LanDanceBattle(LanPartyGames host, int id, LanPartyGameDef def, List<LanPlayer> players, List<LanSlot> slots)
			: base(host, id, def, players, slots)
		{
			foreach (object s in (LanJson.Get(def.GameData, "DanceSequenceSets") as List<object>) ?? new List<object>())
			{
				SequenceSet ss = new SequenceSet();
				ss.SequenceDisplayTimeInSeconds = LanJson.Number(LanJson.Get(s, "SequenceDisplayTimeInSeconds"), 3.0);
				ss.TurnTimeInSeconds = LanJson.Number(LanJson.Get(s, "TurnTimeInSeconds"), 5.0);
				ss.MinRound = (int)LanJson.Number(LanJson.Get(s, "MinRound"), 0.0);
				ss.MaxRound = (int)LanJson.Number(LanJson.Get(s, "MaxRound"), 0.0);
				ss.SequenceLength = (int)LanJson.Number(LanJson.Get(s, "SequenceLength"), 4.0);
				ss.TurnOutcomeSequenceDelayInSeconds = LanJson.Number(LanJson.Get(s, "TurnOutcomeSequenceDelayInSeconds"), 3.0);
				foreach (object m in (LanJson.Get(s, "DanceMoveIds") as List<object>) ?? new List<object>())
				{
					int v;
					if (LanJson.TryInt(m, out v))
					{
						ss.DanceMoveIds.Add(v);
					}
				}
				sets.Add(ss);
			}
		}

		private long Stamp(string name)
		{
			long prev;
			stamps.TryGetValue(name, out prev);
			long t = Math.Max(LanServer.EpochMs / 1000, prev + 1);
			stamps[name] = t;
			return t;
		}

		private void SetVar(string name, string json)
		{
			if (item != null)
			{
				Host.Server.RoomItems.SetVar(item, name, LanRoomItems.Str(json));
			}
		}

		public override void OnStart()
		{
			foreach (LanPartyGamePlayer p in Players)
			{
				Host.SetProp(p.Player, DanceProp);
				Host.SetLocomotion(p.Player, LocomotionPartyGame);
			}
			Timer(ItemDelaySeconds, delegate
			{
				object loc = LanJson.Get(Def.GameData, "GameLocation");
				float[] pos = new float[3]
				{
					(float)LanJson.Number(LanJson.Get(loc, "x"), 0.0),
					(float)LanJson.Number(LanJson.Get(loc, "y"), 0.0),
					(float)LanJson.Number(LanJson.Get(loc, "z"), 0.0)
				};
				item = Host.Server.RoomItems.Add(Players[0].Player.RoomKey, pos, LanRoomItems.V("category", LanRoomItems.Int(7)), LanRoomItems.V("creator", LanRoomItems.Int(0)), LanRoomItems.V("int_a", LanRoomItems.Int(Def.Id)), LanRoomItems.V("statetimestamp", LanRoomItems.Str((LanServer.EpochMs / 1000).ToString(CultureInfo.InvariantCulture))), LanRoomItems.V("score", LanRoomItems.Str("{}")), LanRoomItems.V("turn", LanRoomItems.Str("{}")), LanRoomItems.V("dancemoves", LanRoomItems.Str("{}")));
				Timer(Seconds("IntroTimeInSeconds", 5.0), StartRound);
			});
		}

		private static string IntList(IEnumerable<int> values)
		{
			return "[" + string.Join(",", values.Select((int v) => v.ToString(CultureInfo.InvariantCulture)).ToArray()) + "]";
		}

		private static string D(double d)
		{
			return LanJson.Serialize(d);
		}

		private void StartRound()
		{
			phase = "display";
			set = sets.FirstOrDefault((SequenceSet s) => round >= s.MinRound && round <= s.MaxRound);
			if (set == null)
			{
				throw new InvalidOperationException("no sequence set");
			}
			expected = new List<int>();
			for (int i = 0; i < set.SequenceLength; i++)
			{
				expected.Add(Moves[Host.Server.Random.Next(Moves.Length)]);
			}
			moves.Clear();
			SetVar("turn", "{\"Moves\":" + IntList(expected) + ",\"StartTime\":" + Stamp("turn") + ",\"RoundNum\":" + round + "}");
			Timer(set.SequenceDisplayTimeInSeconds, delegate
			{
				phase = "input";
				Timer(set.TurnTimeInSeconds + Seconds("TurnOutcomeStartDelayInSeconds", 1.0), PerformMoves);
			});
		}

		private List<int> OutcomeFor(List<int> played, int failureMoveId, int successMoveId)
		{
			List<int> result = new List<int>();
			bool perfect = true;
			for (int i = 0; i < expected.Count; i++)
			{
				int e = expected[i];
				if (i < played.Count && played[i] == e)
				{
					result.Add((e - 1 < set.DanceMoveIds.Count) ? set.DanceMoveIds[e - 1] : e);
				}
				else
				{
					result.Add(failureMoveId);
					perfect = false;
				}
			}
			if (perfect && successMoveId > 0)
			{
				result.Add(successMoveId);
			}
			return result;
		}

		private List<int> MovesOf(LanPartyGamePlayer p)
		{
			List<int> m;
			return moves.TryGetValue(p, out m) ? m : new List<int>();
		}

		private void PerformMoves()
		{
			phase = "perform";
			int failure = (int)Seconds("FailureMoveId", 0.0);
			int success = (int)Seconds("SuccessMoveId", 0.0);
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (LanPartyGamePlayer p in Players.Where((LanPartyGamePlayer x) => !x.Left))
			{
				if (!first)
				{
					sb.Append(',');
				}
				first = false;
				sb.Append("{\"PlayerSessionId\":").Append(Sid(p)).Append(",\"DanceMoveIds\":")
					.Append(IntList(OutcomeFor(MovesOf(p), failure, success)))
					.Append('}');
			}
			SetVar("dancemoves", "{\"PlayerMoveDatas\":[" + sb + "],\"StartTimeInSeconds\":" + Stamp("dancemoves") + "}");
			Timer(set.TurnOutcomeSequenceDelayInSeconds, ScoreRound);
		}

		private List<LanPartyGamePlayer> ActiveTeam(int teamId)
		{
			return Players.Where((LanPartyGamePlayer p) => p.TeamId == teamId && !p.Left && !quit.Contains(p)).ToList();
		}

		private double ScorePlayer(List<int> played)
		{
			if (expected.Count == 0)
			{
				return 0.0;
			}
			int correct = 0;
			for (int i = 0; i < Math.Min(played.Count, expected.Count); i++)
			{
				if (played[i] == expected[i])
				{
					correct++;
				}
			}
			return (double)correct / (double)expected.Count;
		}

		private void ScoreRound()
		{
			phase = "results";
			for (int team = 0; team < 2; team++)
			{
				List<double> scores = ActiveTeam(team).Select((LanPartyGamePlayer p) => ScorePlayer(MovesOf(p))).ToList();
				totals[team] += (scores.Count == 0) ? 0.0 : scores.Average();
			}
			SetVar("score", "{\"Team1Score\":" + D(totals[0]) + ",\"Team2Score\":" + D(totals[1]) + ",\"StartTimeInSeconds\":" + Stamp("score") + "}");
			round++;
			if ((double)round < Seconds("NumberOfRounds", 4.0))
			{
				Timer(Seconds("TurnStartDelayInSeconds", 2.0), StartRound);
				return;
			}
			phase = "outro";
			Timer(Seconds("OutroTimeInSeconds", 3.0), Finish);
		}

		private static void Decide(double t0, double t1, out int p0, out int p1, out bool paid)
		{
			paid = Math.Max(t0, t1) >= TieEpsilon;
			if (Math.Abs(t0 - t1) < TieEpsilon)
			{
				p0 = 0;
				p1 = 0;
			}
			else if (t0 > t1)
			{
				p0 = 0;
				p1 = 1;
			}
			else
			{
				p0 = 1;
				p1 = 0;
			}
		}

		private void Finish()
		{
			int p0;
			int p1;
			bool paid;
			Decide(totals[0], totals[1], out p0, out p1, out paid);
			List<KeyValuePair<LanPlayer, int>> placements = new List<KeyValuePair<LanPlayer, int>>();
			HashSet<LanPlayer> unpaid = new HashSet<LanPlayer>();
			foreach (LanPartyGamePlayer p in Players)
			{
				if (p.Left)
				{
					continue;
				}
				placements.Add(Place(p.Player, (p.TeamId == 0) ? p0 : p1));
				if (!paid || quit.Contains(p))
				{
					unpaid.Add(p.Player);
				}
			}
			EndWith(placements, unpaid);
		}

		public override void OnMessage(LanPartyGamePlayer player, int type, object message)
		{
			if (type != ShowTurnOutput || phase != "input" || quit.Contains(player))
			{
				return;
			}
			int move;
			if (!LanJson.TryInt(LanJson.Get(message, "ScoreDelta"), out move) || !Moves.Contains(move))
			{
				return;
			}
			List<int> mine = MovesOf(player);
			if (mine.Count >= expected.Count)
			{
				return;
			}
			List<int> next = new List<int>(mine);
			next.Add(move);
			moves[player] = next;
		}

		private void CheckTeams()
		{
			if (Ended)
			{
				return;
			}
			List<int> empty = new List<int>();
			for (int t = 0; t < 2; t++)
			{
				if (ActiveTeam(t).Count == 0)
				{
					empty.Add(t);
				}
			}
			if (empty.Count == 0)
			{
				return;
			}
			List<KeyValuePair<LanPlayer, int>> placements = new List<KeyValuePair<LanPlayer, int>>();
			HashSet<LanPlayer> unpaid = new HashSet<LanPlayer>();
			if (empty.Count == 1)
			{
				int winner = (empty[0] == 0) ? 1 : 0;
				int a;
				int b;
				bool paid;
				Decide(totals[winner], 0.0, out a, out b, out paid);
				foreach (LanPartyGamePlayer p in Players)
				{
					if (p.Left)
					{
						continue;
					}
					placements.Add(Place(p.Player, (p.TeamId == empty[0]) ? 1 : 0));
					if (quit.Contains(p) || !paid)
					{
						unpaid.Add(p.Player);
					}
				}
			}
			EndWith(placements, unpaid);
		}

		public override void OnPlayerLeft(LanPartyGamePlayer player)
		{
			CheckTeams();
		}

		public override void OnPlayerQuit(LanPartyGamePlayer player)
		{
			quit.Add(player);
			CheckTeams();
		}

		public override void OnEnded()
		{
			phase = "ended";
			LanRoomItem captured = item;
			List<LanPartyGamePlayer> dancers = Players.Where((LanPartyGamePlayer p) => !p.Left).ToList();
			string roomKey = Players[0].Player.RoomKey;
			Host.Server.Loop.After(CleanupDelaySeconds * 1000.0, delegate
			{
				if (captured != null)
				{
					Host.Server.RoomItems.Remove(captured);
				}
				HashSet<LanPlayer> here = new HashSet<LanPlayer>(Host.Server.InRoom(roomKey));
				foreach (LanPartyGamePlayer p in dancers)
				{
					if (here.Contains(p.Player))
					{
						if (p.Player.HeldObject == DanceProp)
						{
							Host.SetProp(p.Player, null);
						}
						Host.SetLocomotion(p.Player, LocomotionDefault);
					}
				}
			});
		}
	}
}
#endif
