using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[Tooltip("Validates that the player actioned on the object of the given mascot")]
	[ActionCategory("Quest (Server)")]
	public class ActionedOnMascotAction : FsmStateAction, ServerVerifiableAction
	{
		[RequiredField]
		public MascotDefinition Mascot;

		public string GetVerifiableType()
		{
			return "ActionMascot";
		}

		public object GetVerifiableParameters()
		{
			return Mascot.name;
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
