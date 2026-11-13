using HutongGames.PlayMaker;

namespace ClubPenguin
{
	[ActionCategory("Misc")]
	public class IsInCaptchaShard : FsmStateAction
	{
		public FsmEvent OnInCaptchaShard;

		public FsmEvent OnNotInCaptchaShard;

		public override void OnEnter()
		{
			base.Fsm.Event(OnNotInCaptchaShard);
		}
	}
}
