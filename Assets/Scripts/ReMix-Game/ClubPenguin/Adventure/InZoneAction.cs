using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[Tooltip("A very weak server validation step, that the user is in the expected room to complete the action")]
	[ActionCategory("Quest (Server)")]
	public class InZoneAction : FsmStateAction, ServerVerifiableAction
	{
		[RequiredField]
		public ZoneDefinition Zone;

		public string GetVerifiableType()
		{
			return "EnterRoom";
		}

		public object GetVerifiableParameters()
		{
			return Zone.ZoneName;
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
