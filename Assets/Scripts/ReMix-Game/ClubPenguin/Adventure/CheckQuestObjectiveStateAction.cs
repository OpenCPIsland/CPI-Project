using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckQuestObjectiveStateAction : FsmStateAction
	{
		public string QuestName;

		public string ObjectiveName;

		public bool IgnoreQuestCompletionCheck;

		public FsmEvent CompletedEvent;

		public FsmEvent NotCompletedEvent;

		public override void OnEnter()
		{
			Quest quest = Service.Get<QuestService>().GetQuest(QuestName);
			if (quest != null)
			{
				if ((!IgnoreQuestCompletionCheck && quest.State == Quest.QuestState.Completed) || quest.IsObjectiveComplete(ObjectiveName))
				{
					base.Fsm.Event(CompletedEvent);
				}
				else
				{
					base.Fsm.Event(NotCompletedEvent);
				}
			}
			else
			{
				base.Fsm.Event(NotCompletedEvent);
			}
			Finish();
		}
	}
}
