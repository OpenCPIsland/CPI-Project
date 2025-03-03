using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaitForQuestStateChangeAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<QuestEvents.QuestSyncCompleted>(onQuestSyncCompleted);
			Service.Get<EventDispatcher>().AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		private void removeListeners()
		{
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.QuestSyncCompleted>(onQuestSyncCompleted);
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
		}

		private bool onQuestSyncCompleted(QuestEvents.QuestSyncCompleted evt)
		{
			removeListeners();
			Finish();
			return false;
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			removeListeners();
			Finish();
			return false;
		}
	}
}
