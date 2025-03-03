using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.TutorialUI
{
	[ActionCategory("Tutorial")]
	public class HideTutorialTooltipAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(TutorialUIEvents.HideTooltip));
			Finish();
		}
	}
}
