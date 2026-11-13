using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckQuestState : FsmStateAction
	{
		public string QuestName;

		public Quest.QuestState StateToCheck;

		public FsmEvent EqualEvent;

		public FsmEvent NotEqualEvent;

		public override void OnEnter()
		{
			Quest quest = Service.Get<QuestService>().GetQuest(QuestName);
			if (quest != null)
			{
				if (StateToCheck == quest.State)
				{
					base.Fsm.Event(EqualEvent);
				}
				else
				{
					base.Fsm.Event(NotEqualEvent);
				}
			}
			Finish();
		}
	}
}
