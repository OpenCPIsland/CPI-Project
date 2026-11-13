using ClubPenguin.Core;
using HutongGames.PlayMaker;

namespace ClubPenguin
{
	[ActionCategory("Misc")]
	public class CheckPlatformAction : FsmStateAction
	{
		public FsmEvent OnStandalone;

		public FsmEvent OnMobile;

		public override void OnEnter()
		{
			PlatformType platformType = PlatformUtils.GetPlatformType();
			PlatformType platformType2 = platformType;
			if (platformType2 == PlatformType.Standalone)
			{
				base.Fsm.Event(OnStandalone);
			}
			else
			{
				base.Fsm.Event(OnMobile);
			}
			Finish();
		}
	}
}
