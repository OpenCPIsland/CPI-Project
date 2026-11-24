#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanFishBucket : LanPartyGameSession
	{
		private sealed class Vec
		{
			public double X;

			public double Y;

			public double Z;

			public Vec(double x, double y, double z)
			{
				X = x;
				Y = y;
				Z = z;
			}

			public string Json()
			{
				return "{\"x\":" + X.ToString("R", CultureInfo.InvariantCulture) + ",\"y\":" + Y.ToString("R", CultureInfo.InvariantCulture) + ",\"z\":" + Z.ToString("R", CultureInfo.InvariantCulture) + "}";
			}
		}

		public const int ActionStart = 7;

		private const int SetGameState = 1;

		private const int SetGameStartPositions = 4;

		private const int ShowTurnOutput = 5;

		private const int PlayerTurnStart = 6;

		private const int PlayerLeftGame = 7;

		private const int StateIntro = 1;

		private const int StateGame = 2;

		private const int StateFinished = 3;

		private const int FishAction = 7;

		private const int PassAction = 8;

		private const double PositionsBackstopSeconds = 10;

		private const double TurnGraceSeconds = 2;

		private const double OutroSeconds = 1;

		private const int MaxGamePositions = 4;

		private const double SynthRadius = 3;

		private static readonly double[][] SpawnSlots = new double[4][]
		{
			new double[2] { 0.4, -0.79 },
			new double[2] { -0.4, -0.79 },
			new double[2] { 1.0, -0.297 },
			new double[2] { -1.0, -0.269 }
		};

		private const double MaxPositionDistance = 30;

		private List<int> deck = new List<int>();

		private int deckPos;

		private string phase = "positions";

		private int cur;

		private bool drew;

		private int step;

		private readonly int[] scores;

		private readonly HashSet<LanPartyGamePlayer> quit = new HashSet<LanPartyGamePlayer>();

		private List<KeyValuePair<LanPlayer, int>> decided;

		private HashSet<LanPlayer> decidedUnpaid = new HashSet<LanPlayer>();

		public float[][] StartPositions;

		public float[] BucketPosition;

		public LanFishBucket(LanPartyGames host, int id, LanPartyGameDef def, List<LanPlayer> players)
			: base(host, id, def, players, null)
		{
			scores = new int[players.Count];
		}

		public string Phase
		{
			get
			{
				return phase;
			}
		}

		public int Current
		{
			get
			{
				return cur;
			}
		}

		public bool Drew
		{
			get
			{
				return drew;
			}
		}

		public int Step
		{
			get
			{
				return step;
			}
		}

		public List<int> Remaining
		{
			get
			{
				return deck.Skip(deckPos).ToList();
			}
		}

		public int[] Scores
		{
			get
			{
				return (int[])scores.Clone();
			}
		}

		private List<int> BuildDeck(int playerCount)
		{
			List<object> decks = (LanJson.Get(Def.GameData, "CardDecks") as List<object>) ?? new List<object>();
			List<object> matching = decks.Where((object d) => LanJson.Number(LanJson.Get(d, "MinPlayerCount"), 0.0) <= (double)playerCount && (double)playerCount <= LanJson.Number(LanJson.Get(d, "MaxPlayerCount"), 0.0)).ToList();
			if (matching.Count == 0)
			{
				throw new InvalidOperationException("no deck");
			}
			object chosen = matching[Host.Server.Random.Next(matching.Count)];
			List<int> cards = new List<int>();
			foreach (object c in (LanJson.Get(chosen, "Cards") as List<object>) ?? new List<object>())
			{
				int delta = (int)LanJson.Number(LanJson.Get(c, "FishDelta"), 0.0);
				int quantity = (int)LanJson.Number(LanJson.Get(c, "Quantity"), 0.0);
				for (int i = 0; i < quantity; i++)
				{
					cards.Add(delta);
				}
			}
			for (int j = cards.Count - 1; j > 0; j--)
			{
				int k = Host.Server.Random.Next(j + 1);
				int tmp = cards[j];
				cards[j] = cards[k];
				cards[k] = tmp;
			}
			return cards;
		}

		private bool Active(LanPartyGamePlayer p)
		{
			return !p.Left && !quit.Contains(p);
		}

		public override void OnStart()
		{
			deck = BuildDeck(Players.Count);
			if (Players.Count > 0 && Players[0].Player.IsBot)
			{
				Timer(1.0, delegate
				{
					if (phase == "positions")
					{
						SynthesizePositions();
					}
				});
			}
			Timer(PositionsBackstopSeconds, delegate
			{
				if (phase == "positions")
				{
					SynthesizePositions();
				}
			});
		}

		private void SynthesizePositions()
		{
			float[] anchor = Players.Where((LanPartyGamePlayer p) => !p.Left).Select((LanPartyGamePlayer p) => p.Player.LastPosition).FirstOrDefault((float[] at) => at != null && at.Length == 3);
			if (anchor == null)
			{
				EndWith(new List<KeyValuePair<LanPlayer, int>>());
				return;
			}
			double x = anchor[0];
			double y = anchor[1];
			double z = anchor[2];
			double fx = 0.0;
			double fz = 0.0;
			foreach (LanPartyGamePlayer p in Players)
			{
				float[] at = p.Player.LastPosition;
				if (!p.Player.IsBot && !p.Left && at != null && at.Length == 3)
				{
					fx += at[0] - x;
					fz += at[2] - z;
				}
			}
			double len = Math.Sqrt(fx * fx + fz * fz);
			if (len < 0.01)
			{
				fx = 0.0;
				fz = -1.0;
				len = 1.0;
			}
			fx /= len;
			fz /= len;
			double rx = fz;
			double rz = -fx;
			Func<int, Vec> ring = delegate(int i)
			{
				double[] slot = SpawnSlots[Math.Min(i, SpawnSlots.Length - 1)];
				return new Vec(x + rx * slot[0] + fx * slot[1], y, z + rz * slot[0] + fz * slot[1]);
			};
			List<Vec> others = new List<Vec>();
			for (int k = 1; k < Players.Count; k++)
			{
				others.Add(ring(k));
			}
			BeginIntro(ring(0), others, new List<Vec>
			{
				new Vec(x, y, z)
			});
		}

		private static Vec ToVec(object v)
		{
			double x;
			double y;
			double z;
			if (LanJson.TryNumber(LanJson.Get(v, "x"), out x) && LanJson.TryNumber(LanJson.Get(v, "y"), out y) && LanJson.TryNumber(LanJson.Get(v, "z"), out z))
			{
				return new Vec(x, y, z);
			}
			return null;
		}

		private static List<Vec> ToVecs(object list, int min, int max)
		{
			List<object> l = list as List<object>;
			if (l == null || l.Count < min || l.Count > max)
			{
				return null;
			}
			List<Vec> result = l.Select(ToVec).ToList();
			return result.All((Vec v) => v != null) ? result : null;
		}

		public override void OnMessage(LanPartyGamePlayer player, int type, object message)
		{
			if (type != SetGameStartPositions || phase != "positions" || player != Players[0])
			{
				return;
			}
			Vec owner = ToVec(LanJson.Get(message, "OwnerPosition"));
			List<Vec> others = ToVecs(LanJson.Get(message, "PlayerPositions"), Players.Count - 1, Players.Count - 1);
			List<Vec> game = ToVecs(LanJson.Get(message, "GamePositions"), 1, MaxGamePositions);
			if (owner == null || others == null || game == null)
			{
				return;
			}
			float[] at = player.Player.LastPosition;
			if (at != null && at.Length == 3)
			{
				foreach (Vec p in new Vec[1]
				{
					owner
				}.Concat(others).Concat(game))
				{
					double dx = p.X - at[0];
					double dy = p.Y - at[1];
					double dz = p.Z - at[2];
					if (Math.Sqrt(dx * dx + dy * dy + dz * dz) > MaxPositionDistance)
					{
						return;
					}
				}
			}
			BeginIntro(owner, others, game);
		}

		private static string VecList(List<Vec> list)
		{
			StringBuilder sb = new StringBuilder("[");
			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(',');
				}
				sb.Append(list[i].Json());
			}
			return sb.Append(']').ToString();
		}

		private void BeginIntro(Vec owner, List<Vec> others, List<Vec> game)
		{
			float[][] starts = new float[1 + others.Count][];
			starts[0] = new float[3] { (float)owner.X, (float)owner.Y, (float)owner.Z };
			for (int k = 0; k < others.Count; k++)
			{
				starts[k + 1] = new float[3] { (float)others[k].X, (float)others[k].Y, (float)others[k].Z };
			}
			BucketPosition = new float[3] { (float)game[0].X, (float)game[0].Y, (float)game[0].Z };
			StartPositions = starts;
			phase = "intro";
			SendMessage(SetGameStartPositions, "{\"OwnerPosition\":" + owner.Json() + ",\"PlayerPositions\":" + VecList(others) + ",\"GamePositions\":" + VecList(game) + ",\"IntData\":[" + deck.Count + "]}");
			SendMessage(SetGameState, "{\"GameStateId\":" + StateIntro + "}");
			Timer(Seconds("IntroTimeInSeconds", 5.0), delegate
			{
				if (Players.Count(Active) <= 1)
				{
					EndLone();
					return;
				}
				foreach (LanPartyGamePlayer p in Players)
				{
					if (quit.Contains(p) && !p.Left)
					{
						Release(p);
					}
				}
				SendMessage(SetGameState, "{\"GameStateId\":" + StateGame + "}");
				StartTurn();
			});
		}

		private void StartTurn()
		{
			for (int i = 0; i < Players.Count && !Active(Players[cur]); i++)
			{
				cur = (cur + 1) % Players.Count;
			}
			drew = false;
			phase = "turn";
			SendMessage(PlayerTurnStart, "{\"PlayerId\":" + Sid(Players[cur]) + "}");
			ArmTimeout();
		}

		private void ArmTimeout()
		{
			int mine = ++step;
			Timer(Seconds("TurnTimeInSeconds", 10.0) + TurnGraceSeconds, delegate
			{
				if (step != mine || phase != "turn")
				{
					return;
				}
				if (drew)
				{
					NextTurn();
				}
				else
				{
					Draw();
				}
			});
		}

		private void NextTurn()
		{
			step++;
			cur = (cur + 1) % Players.Count;
			StartTurn();
		}

		public override void OnAction(LanPartyGamePlayer player, int action, float[] position)
		{
			if (phase != "turn" || player != Players[cur])
			{
				return;
			}
			if (action == FishAction)
			{
				Draw();
			}
			else if (action == PassAction && drew)
			{
				NextTurn();
			}
		}

		private void Draw()
		{
			phase = "delay";
			step++;
			int card = deck[deckPos++];
			scores[cur] = Math.Max(scores[cur] + card, 0);
			SendMessage(ShowTurnOutput, "{\"PlayerId\":" + Sid(Players[cur]) + ",\"ScoreDelta\":" + card + "}");
			bool fish = card > 0;
			double delay = fish ? Seconds("NextTurnDelayFishInSeconds", 3.0) : Seconds("NextTurnDelaySquidInSeconds", 5.0);
			Timer(delay, delegate
			{
				if (deckPos >= deck.Count)
				{
					Finish();
				}
				else if (!fish || !Active(Players[cur]))
				{
					NextTurn();
				}
				else
				{
					phase = "turn";
					drew = true;
					ArmTimeout();
				}
			});
		}

		private static List<int> PlacementsFromScores(List<int> s)
		{
			List<int> distinct = s.Distinct().OrderByDescending((int v) => v).ToList();
			return s.Select((int v) => distinct.IndexOf(v)).ToList();
		}

		private void Standings(out List<KeyValuePair<LanPlayer, int>> placements, out HashSet<LanPlayer> unpaid)
		{
			placements = new List<KeyValuePair<LanPlayer, int>>();
			unpaid = new HashSet<LanPlayer>();
			List<int> stay = new List<int>();
			for (int i = 0; i < Players.Count; i++)
			{
				if (Active(Players[i]))
				{
					stay.Add(i);
				}
			}
			List<int> ranks = PlacementsFromScores(stay.Select((int i) => scores[i]).ToList());
			for (int k = 0; k < stay.Count; k++)
			{
				placements.Add(Place(Players[stay[k]].Player, ranks[k]));
			}
			int worst = ((ranks.Count > 0) ? ranks.Max() : (-1)) + 1;
			foreach (LanPartyGamePlayer p in Players)
			{
				if (!p.Left && quit.Contains(p))
				{
					placements.Add(Place(p.Player, worst));
					unpaid.Add(p.Player);
				}
			}
		}

		private void Finish()
		{
			phase = "over";
			List<KeyValuePair<LanPlayer, int>> placements;
			HashSet<LanPlayer> unpaid;
			Standings(out placements, out unpaid);
			decided = placements;
			decidedUnpaid = unpaid;
			SendMessage(SetGameState, "{\"GameStateId\":" + StateFinished + "}");
			Timer(OutroSeconds, delegate
			{
				EndWith(placements, unpaid);
			});
		}

		private bool HasLeft(LanPlayer p)
		{
			LanPartyGamePlayer gp = Players.FirstOrDefault((LanPartyGamePlayer x) => x.Player == p);
			return gp != null && gp.Left;
		}

		private void Leave(LanPartyGamePlayer player)
		{
			if (decided != null)
			{
				EndWith(decided.Where((KeyValuePair<LanPlayer, int> kv) => !HasLeft(kv.Key)).ToList(), new HashSet<LanPlayer>(decidedUnpaid.Where((LanPlayer p) => !HasLeft(p))));
				return;
			}
			SendMessage(PlayerLeftGame, "{\"PlayerId\":" + Sid(player) + "}");
			int stay = Players.Count(Active);
			if (stay <= 1 && phase != "positions" && phase != "intro")
			{
				EndLone();
				return;
			}
			if (phase == "turn" && Players[cur] == player)
			{
				NextTurn();
			}
			if (phase == "turn" || phase == "delay")
			{
				Release(player);
			}
		}

		private void EndLone()
		{
			List<KeyValuePair<LanPlayer, int>> placements;
			HashSet<LanPlayer> unpaid;
			Standings(out placements, out unpaid);
			foreach (LanPartyGamePlayer p in Players.Where(Active))
			{
				int index = placements.FindIndex((KeyValuePair<LanPlayer, int> kv) => kv.Key == p.Player);
				if (index >= 0)
				{
					placements[index] = Place(p.Player, 0);
				}
				else
				{
					placements.Add(Place(p.Player, 0));
				}
			}
			EndWith(placements, unpaid);
		}

		public override void OnPlayerQuit(LanPartyGamePlayer player)
		{
			quit.Add(player);
			Leave(player);
		}

		public override void OnPlayerLeft(LanPartyGamePlayer player)
		{
			Leave(player);
		}
	}
}
#endif
