using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class StartDanceBattleQuestGameAction : FsmStateAction
	{
		public override void OnEnter()
		{
			DanceBattleQuestGameController danceBattleQuestGameController = Object.FindObjectOfType<DanceBattleQuestGameController>();
			if (danceBattleQuestGameController != null)
			{
				danceBattleQuestGameController.StartGame();
			}
			Finish();
		}
	}
}
