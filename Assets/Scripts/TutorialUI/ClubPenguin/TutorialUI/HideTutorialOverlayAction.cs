using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.TutorialUI
{
	[ActionCategory("Tutorial")]
	public class HideTutorialOverlayAction : FsmStateAction
	{
		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(TutorialUIEvents.HideHighlightOverlay));
			Finish();
		}
	}
}
