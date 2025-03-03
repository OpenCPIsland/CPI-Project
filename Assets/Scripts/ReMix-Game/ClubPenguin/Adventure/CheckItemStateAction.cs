using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckItemStateAction : FsmStateAction
	{
		public string QuestItemName;

		public FsmEvent NotCollectedEvent;

		public FsmEvent CollectedEvent;

		public override void OnEnter()
		{
			switch (Service.Get<QuestService>().GetQuestItemState(base.Owner, base.Fsm.Name, QuestItemName))
			{
			case QuestItem.QuestItemState.Collected:
				base.Fsm.Event(CollectedEvent);
				break;
			case QuestItem.QuestItemState.NotCollected:
				base.Fsm.Event(NotCollectedEvent);
				break;
			}
			Finish();
		}
	}
}
