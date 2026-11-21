#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanFindFour : LanPartyGameSession
	{
		private const int SetGameState = 1;

		private const int ShowTurnOutput = 5;

		private const int PlayerTurnStart = 6;

		private const int StateIntro = 0;

		private const int StateGame = 1;

		private const int StateFinished = 2;

		private const double BackstopGraceSeconds = 2;

		private readonly int width;

		private readonly int height;

		private readonly int toWin;

		private readonly int defaultColumn;

		private readonly List<int>[] board;

		private int turn;

		private int waitingFor = -1;

		private int turnSeq;

		private List<KeyValuePair<LanPlayer, int>> decided;

		public LanFindFour(LanPartyGames host, int id, LanPartyGameDef def, List<LanPlayer> players)
			: base(host, id, def, players, null)
		{
			width = (int)def.Data("GameBoardWidth", 7.0);
			height = (int)def.Data("GameBoardHeight", 6.0);
			toWin = (int)def.Data("SequenceCountToWin", 4.0);
			defaultColumn = (int)def.Data("DefaultColumn", 3.0);
			board = new List<int>[width];
			for (int i = 0; i < width; i++)
			{
				board[i] = new List<int>();
			}
		}

		public int WaitingFor
		{
			get
			{
				return waitingFor;
			}
		}

		public int TurnSeq
		{
			get
			{
				return turnSeq;
			}
		}

		public int Width
		{
			get
			{
				return width;
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
		}

		public int ToWin
		{
			get
			{
				return toWin;
			}
		}

		public int DefaultColumn
		{
			get
			{
				return defaultColumn;
			}
		}

		public List<int>[] Board
		{
			get
			{
				return board;
			}
		}

		public static int NormaliseColumn(List<int>[] board, object wanted, int defaultColumn, int height)
		{
			int w = board.Length;
			double d;
			int col = LanJson.TryNumber(wanted, out d) ? ((int)Math.Floor(d)) : defaultColumn;
			col = Math.Min(w - 1, Math.Max(0, col));
			for (int delta = 0; delta < w; delta++)
			{
				int[] candidates = new int[2]
				{
					col - delta,
					col + delta
				};
				foreach (int c in candidates)
				{
					if (c >= 0 && c < w && board[c].Count < height)
					{
						return c;
					}
				}
			}
			return -1;
		}

		public override void OnStart()
		{
			SendMessage(SetGameState, "{\"GameStateId\":" + StateIntro + "}");
			Timer(Seconds("IntroTimeInSeconds", 5.0), delegate
			{
				SendMessage(SetGameState, "{\"GameStateId\":" + StateGame + "}");
				StartTurn();
			});
		}

		private void StartTurn()
		{
			waitingFor = turn;
			int mover = turn;
			int seq = ++turnSeq;
			SendMessage(PlayerTurnStart, "{\"PlayerId\":" + Sid(Players[mover]) + "}");
			Timer(Seconds("TurnTimeInSeconds", 20.0) + BackstopGraceSeconds, delegate
			{
				if (turnSeq == seq && waitingFor == mover)
				{
					Drop(mover, defaultColumn);
				}
			});
		}

		public override void OnMessage(LanPartyGamePlayer player, int type, object message)
		{
			if (type != ShowTurnOutput)
			{
				return;
			}
			int index = Players.IndexOf(player);
			if (index != waitingFor)
			{
				return;
			}
			Drop(index, LanJson.Get(message, "ScoreDelta"));
		}

		private void Drop(int index, object wanted)
		{
			int col = NormaliseColumn(board, wanted, defaultColumn, height);
			if (col < 0)
			{
				return;
			}
			waitingFor = -1;
			board[col].Add(index);
			SendMessage(ShowTurnOutput, "{\"PlayerId\":" + Sid(Players[index]) + ",\"ScoreDelta\":" + col + "}");
			Timer(Seconds("ChipPlaceTimeInSeconds", 2.0), delegate
			{
				LanPlayer other = Players[1 - index].Player;
				LanPlayer me = Players[index].Player;
				if (IsWin(col))
				{
					Finish(Placements(Place(me, 0), Place(other, 1)));
				}
				else if (board.All((List<int> c) => c.Count >= height))
				{
					Finish(Placements(Place(Players[0].Player, 1), Place(Players[1].Player, 1)));
				}
				else
				{
					turn = 1 - index;
					StartTurn();
				}
			});
		}

		private int? At(int x, int y)
		{
			if (x >= 0 && x < width && y >= 0 && y < board[x].Count)
			{
				return board[x][y];
			}
			return null;
		}

		private bool IsWin(int col)
		{
			int row = board[col].Count - 1;
			int who = board[col][row];
			int[,] dirs = new int[4, 2]
			{
				{ 1, 0 },
				{ 0, 1 },
				{ 1, 1 },
				{ 1, -1 }
			};
			for (int d = 0; d < 4; d++)
			{
				int dx = dirs[d, 0];
				int dy = dirs[d, 1];
				int count = 1;
				for (int s = 1; s >= -1; s -= 2)
				{
					int x = col + dx * s;
					int y = row + dy * s;
					while (At(x, y) == who)
					{
						count++;
						x += dx * s;
						y += dy * s;
					}
				}
				if (count >= toWin)
				{
					return true;
				}
			}
			return false;
		}

		private void Finish(List<KeyValuePair<LanPlayer, int>> placements)
		{
			decided = placements;
			SendMessage(SetGameState, "{\"GameStateId\":" + StateFinished + "}");
			Timer(Seconds("OutroTimeInSeconds", 4.0), delegate
			{
				EndWith(placements);
			});
		}

		public override void OnPlayerQuit(LanPartyGamePlayer player)
		{
			if (decided != null)
			{
				EndWith(decided);
				return;
			}
			LanPartyGamePlayer stayer = Players.FirstOrDefault((LanPartyGamePlayer p) => p != player && !p.Left);
			List<KeyValuePair<LanPlayer, int>> placements = new List<KeyValuePair<LanPlayer, int>>();
			if (stayer != null)
			{
				placements.Add(Place(stayer.Player, 0));
			}
			placements.Add(Place(player.Player, 1));
			EndWith(placements, new HashSet<LanPlayer>
			{
				player.Player
			});
		}

		public override void OnPlayerLeft(LanPartyGamePlayer player)
		{
			if (decided != null)
			{
				EndWith(decided.Where((KeyValuePair<LanPlayer, int> kv) => kv.Key != player.Player).ToList());
				return;
			}
			LanPartyGamePlayer stayer = Players.FirstOrDefault((LanPartyGamePlayer p) => p != player && !p.Left);
			EndWith((stayer != null) ? Placements(Place(stayer.Player, 0)) : new List<KeyValuePair<LanPlayer, int>>());
		}
	}
}
#endif
