using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class CheckQuestObjectiveAction : FsmStateAction
	{
		public string[] ObjectiveNames;

		public FsmEvent EqualEvent;

		public FsmEvent NotEqualEvent;

		public override void OnEnter()
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				bool flag = false;
				for (int i = 0; i < ObjectiveNames.Length; i++)
				{
					if (ObjectiveNames[i] == activeQuest.CurrentObjectiveName)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					base.Fsm.Event(EqualEvent);
				}
				else
				{
					base.Fsm.Event(NotEqualEvent);
				}
			}
			else
			{
				Finish();
			}
		}
	}
}
