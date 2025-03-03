using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class StartQuestPathAction : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject QuestPathObject;

		public override void OnEnter()
		{
			if (QuestPathObject.Value != null)
			{
				QuestPath component = QuestPathObject.Value.GetComponent<QuestPath>();
				if (component != null)
				{
					component.StartPath();
				}
			}
			Finish();
		}
	}
}
