using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.PartyGames;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleMmoItemObserver : AbstractMmoItemObserver<DanceBattleMmoItem>
	{
		private const int MAX_TIME_SHOW_DANCE_MOVES_IN_SECONDS = 5;

		public Action GameStartedAction;

		public Action GameEndedAction;

		public Action<DanceBattleScoreData> ScoresUpdatedAction;

		public Action<DanceBattleTurnData> TurnDataUpdatedAction;

		public Action<DanceBattleTurnOutcomeMoveData> TurnOutcomeMoveDataUpdatedAction;

		private DanceBattleScoreData danceBattleScoreData;

		private DanceBattleTurnData danceBattleTurnData;

		private DanceBattleTurnOutcomeMoveData danceBattleTurnOutcomeMoveData;

		protected override void awake()
		{
			danceBattleScoreData = new DanceBattleScoreData();
			danceBattleTurnData = new DanceBattleTurnData();
			danceBattleTurnOutcomeMoveData = new DanceBattleTurnOutcomeMoveData();
		}

		protected override void onItemAdded(DanceBattleMmoItem item)
		{
			if (GameStartedAction != null)
			{
				GameStartedAction();
			}
		}

		protected override void onItemUpdated(DanceBattleMmoItem item)
		{
			handleScoreDataUpdated(item);
			handleTurnDataUpdated(item);
			handleTurnOutcomeDanceMoveDataUpdated(item);
		}

		protected override void onItemRemoved(DanceBattleMmoItem item)
		{
			if (GameEndedAction != null)
			{
				GameEndedAction();
			}
		}

		private void handleScoreDataUpdated(DanceBattleMmoItem item)
		{
			if (string.IsNullOrEmpty(item.getScores()))
			{
				return;
			}
			DanceBattleScoreData scoreDataFromMmoItem = getScoreDataFromMmoItem(item);
			if (!danceBattleScoreData.Equals(scoreDataFromMmoItem))
			{
				danceBattleScoreData = scoreDataFromMmoItem;
				if (ScoresUpdatedAction != null)
				{
					ScoresUpdatedAction(danceBattleScoreData);
				}
			}
		}

		private void handleTurnDataUpdated(DanceBattleMmoItem item)
		{
			if (string.IsNullOrEmpty(item.getTurnData()))
			{
				return;
			}
			DanceBattleTurnData turnDataFromMmoItem = getTurnDataFromMmoItem(item);
			if (!danceBattleTurnData.Equals(turnDataFromMmoItem))
			{
				danceBattleTurnData = turnDataFromMmoItem;
				if (TurnDataUpdatedAction != null)
				{
					TurnDataUpdatedAction(danceBattleTurnData);
				}
			}
		}

		private void handleTurnOutcomeDanceMoveDataUpdated(DanceBattleMmoItem item)
		{
			if (string.IsNullOrEmpty(item.getTurnOutcomeDanceMoveData()))
			{
				return;
			}
			DanceBattleTurnOutcomeMoveData turnOutcomeMoveDataFromMmoItem = getTurnOutcomeMoveDataFromMmoItem(item);
			long num = Service.Get<INetworkServicesManager>().GameTimeMilliseconds / 1000;
			if (!danceBattleTurnOutcomeMoveData.Equals(turnOutcomeMoveDataFromMmoItem) && Math.Abs(turnOutcomeMoveDataFromMmoItem.StartTimeInSeconds - num) < 5)
			{
				danceBattleTurnOutcomeMoveData = turnOutcomeMoveDataFromMmoItem;
				if (TurnOutcomeMoveDataUpdatedAction != null)
				{
					TurnOutcomeMoveDataUpdatedAction(danceBattleTurnOutcomeMoveData);
				}
			}
		}

		private DanceBattleTurnOutcomeMoveData getTurnOutcomeMoveDataFromMmoItem(DanceBattleMmoItem item)
		{
			return Service.Get<JsonService>().Deserialize<DanceBattleTurnOutcomeMoveData>(item.getTurnOutcomeDanceMoveData());
		}

		private DanceBattleScoreData getScoreDataFromMmoItem(DanceBattleMmoItem item)
		{
			return Service.Get<JsonService>().Deserialize<DanceBattleScoreData>(item.getScores());
		}

		private DanceBattleTurnData getTurnDataFromMmoItem(DanceBattleMmoItem item)
		{
			return Service.Get<JsonService>().Deserialize<DanceBattleTurnData>(item.getTurnData());
		}
	}
}
