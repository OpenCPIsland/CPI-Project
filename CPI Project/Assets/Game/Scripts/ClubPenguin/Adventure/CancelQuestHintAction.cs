using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CancelQuestHintAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(QuestEvents.CancelQuestHint));
			Finish();
		}
	}
}
