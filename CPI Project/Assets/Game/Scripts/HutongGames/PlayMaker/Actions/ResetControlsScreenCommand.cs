using ClubPenguin;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class ResetControlsScreenCommand : FsmStateAction
	{
		public FsmBool ResetRightControls = false;

		public FsmBool ResetLeftControls = false;

		public override void OnEnter()
		{
			if (ResetLeftControls.Value)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ControlsScreenEvents.ReturnToDefaultLeftOption));
			}
			if (ResetRightControls.Value)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(ControlsScreenEvents.ReturnToDefaultRightOption));
			}
			Finish();
		}
	}
}
