using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ClearFishingGameQuestPrizeAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<QuestService>().CurrentFishingPrize = "";
			Finish();
		}
	}
}
