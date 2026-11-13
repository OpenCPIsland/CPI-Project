using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleTurnOutcomePlayer : MonoBehaviour
	{
		public DanceBattleMmoItemObserver MmoItemObserver;

		private DanceBattleTurnOutcomeMoveData currentTurnOutcomeMoveData;

		private List<DanceBattleTurnOutcomePenguinPlayer> currentPenguinPlayers;

		private void Start()
		{
			DanceBattleMmoItemObserver mmoItemObserver = MmoItemObserver;
			mmoItemObserver.TurnOutcomeMoveDataUpdatedAction = (Action<DanceBattleTurnOutcomeMoveData>)Delegate.Combine(mmoItemObserver.TurnOutcomeMoveDataUpdatedAction, new Action<DanceBattleTurnOutcomeMoveData>(onTurnDataOutcomeMoveDataUpdated));
			DanceBattleMmoItemObserver mmoItemObserver2 = MmoItemObserver;
			mmoItemObserver2.GameEndedAction = (Action)Delegate.Combine(mmoItemObserver2.GameEndedAction, new Action(onGameEnded));
			currentPenguinPlayers = new List<DanceBattleTurnOutcomePenguinPlayer>();
		}

		private void onTurnDataOutcomeMoveDataUpdated(DanceBattleTurnOutcomeMoveData turnOutcomeMoveData)
		{
			currentTurnOutcomeMoveData = turnOutcomeMoveData;
			startDancing();
		}

		private void onGameEnded()
		{
			cleanUpPenguinPlayers();
		}

		private void startDancing()
		{
			cleanUpPenguinPlayers();
			for (int i = 0; i < currentTurnOutcomeMoveData.PlayerMoveDatas.Count; i++)
			{
				currentPenguinPlayers.Add(new DanceBattleTurnOutcomePenguinPlayer(currentTurnOutcomeMoveData.PlayerMoveDatas[i]));
			}
		}

		private void OnDestroy()
		{
			if (MmoItemObserver != null)
			{
				DanceBattleMmoItemObserver mmoItemObserver = MmoItemObserver;
				mmoItemObserver.TurnOutcomeMoveDataUpdatedAction = (Action<DanceBattleTurnOutcomeMoveData>)Delegate.Remove(mmoItemObserver.TurnOutcomeMoveDataUpdatedAction, new Action<DanceBattleTurnOutcomeMoveData>(onTurnDataOutcomeMoveDataUpdated));
				DanceBattleMmoItemObserver mmoItemObserver2 = MmoItemObserver;
				mmoItemObserver2.GameEndedAction = (Action)Delegate.Remove(mmoItemObserver2.GameEndedAction, new Action(onGameEnded));
			}
			onGameEnded();
		}

		private void cleanUpPenguinPlayers()
		{
			for (int i = 0; i < currentPenguinPlayers.Count; i++)
			{
				currentPenguinPlayers[i].Destroy();
			}
			currentPenguinPlayers.Clear();
		}
	}
}
