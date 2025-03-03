using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[Tooltip("Validates that the user said the given phrase")]
	[ActionCategory("Quest (Server)")]
	public class ChatValidationAction : FsmStateAction, ServerVerifiableAction
	{
		[RequiredField]
		public string Expression;

		public string GetVerifiableType()
		{
			return "ChatMessage";
		}

		public object GetVerifiableParameters()
		{
			return Expression.Trim();
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
