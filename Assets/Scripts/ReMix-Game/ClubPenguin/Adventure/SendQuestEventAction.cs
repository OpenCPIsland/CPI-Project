using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SendQuestEventAction : FsmStateAction
	{
		public FsmString QuestEvent;

		public override void OnEnter()
		{
			Service.Get<QuestService>().SendEvent(QuestEvent.Value);
			Finish();
		}
	}
}
