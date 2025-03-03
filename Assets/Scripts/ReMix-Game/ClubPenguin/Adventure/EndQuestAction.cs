using ClubPenguin.Analytics;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class EndQuestAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<ICPSwrveService>().Action("game.quest", "complete", base.Fsm.Name);
			Service.Get<QuestService>().EndQuest(base.Owner, base.Fsm.Name);
			Finish();
		}
	}
}
