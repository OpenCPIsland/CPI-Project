using Disney.Kelowna.Common.DataModel;
using System.Runtime.InteropServices;

namespace ClubPenguin.Game.PartyGames
{
	public static class DanceBattleEvents
	{
		public struct SequenceDisplayStart
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public readonly DanceBattleTurnData TurnData;

			public SequenceDisplayStart(DanceBattleVisualsController danceBattleVisualsController, DanceBattleTurnData turnData)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
				TurnData = turnData;
			}
		}

		public struct TurnStart
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public readonly DanceBattleTurnData TurnData;

			public TurnStart(DanceBattleVisualsController danceBattleVisualsController, DanceBattleTurnData turnData)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
				TurnData = turnData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct TurnTimerComplete
		{
		}

		public struct TurnEnd
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public readonly DanceBattleTurnData TurnData;

			public readonly DanceBattleScoreData ScoreData;

			public TurnEnd(DanceBattleVisualsController danceBattleVisualsController, DanceBattleTurnData turnData, DanceBattleScoreData scoreData)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
				TurnData = turnData;
				ScoreData = scoreData;
			}
		}

		public struct SequenceDisplayEnd
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public SequenceDisplayEnd(DanceBattleVisualsController danceBattleVisualsController)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
			}
		}

		public struct DanceBattleStart
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public DanceBattleStart(DanceBattleVisualsController danceBattleVisualsController)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
			}
		}

		public struct ResultsDisplayStart
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public ResultsDisplayStart(DanceBattleVisualsController danceBattleVisualsController)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
			}
		}

		public struct ResultsDisplayEnd
		{
			public readonly DanceBattleVisualsController DanceBattleVisualsController;

			public ResultsDisplayEnd(DanceBattleVisualsController danceBattleVisualsController)
			{
				DanceBattleVisualsController = danceBattleVisualsController;
			}
		}

		public struct TurnInputSent
		{
			public readonly int Input;

			public readonly int InputCount;

			public readonly long PlayerId;

			public TurnInputSent(int input, int inputCount, long playerId)
			{
				Input = input;
				InputCount = inputCount;
				PlayerId = playerId;
			}
		}

		public struct DanceMoveAnimationComplete
		{
			public readonly DataEntityHandle Handle;

			public DanceMoveAnimationComplete(DataEntityHandle handle)
			{
				Handle = handle;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LocalPlayerDanceSequenceComplete
		{
		}

		public struct FinalResultsShown
		{
			public readonly bool IsTie;

			public readonly int WinningTeamId;

			public FinalResultsShown(bool isTie, int winningTeamId)
			{
				IsTie = isTie;
				WinningTeamId = winningTeamId;
			}
		}
	}
}
