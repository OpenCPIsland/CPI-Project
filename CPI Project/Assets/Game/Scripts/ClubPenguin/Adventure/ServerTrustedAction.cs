using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[Tooltip("WARNING: Only use this for objective steps that don't require networked user interaction")]
	[ActionCategory("Quest (Server)")]
	public class ServerTrustedAction : FsmStateAction, ServerVerifiableAction
	{
		public object GetVerifiableParameters()
		{
			return null;
		}

		public string GetVerifiableType()
		{
			return "SkipValidation";
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
