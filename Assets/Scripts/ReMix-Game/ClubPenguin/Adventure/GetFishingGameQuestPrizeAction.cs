using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class GetFishingGameQuestPrizeAction : FsmStateAction
	{
		public FsmString questPrize;

		public override void OnEnter()
		{
			questPrize.Value = Service.Get<QuestService>().CurrentFishingPrize;
			if (questPrize.Value == null)
			{
				questPrize.Value = "";
			}
			Finish();
		}
	}
}
