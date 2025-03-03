using HutongGames.PlayMaker;

namespace ClubPenguin
{
	[ActionCategory("Misc")]
	public class IsInCaptchaShard : FsmStateAction
	{
		private readonly int CAPTCHA_SHARD = 1;

		public FsmEvent OnInCaptchaShard;

		public FsmEvent OnNotInCaptchaShard;

		public override void OnEnter()
		{
			base.Fsm.Event(OnNotInCaptchaShard);
		}
	}
}
