using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class GetLastQuestPathNodeAction : FsmStateAction
	{
		public FsmGameObject QuestPathObject;

		public FsmGameObject OUT_NodeObject;

		public override void OnEnter()
		{
			QuestPath component = QuestPathObject.Value.GetComponent<QuestPath>();
			if (component != null)
			{
				OUT_NodeObject.Value = component.GetLastActiveNode();
			}
			Finish();
		}
	}
}
