using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.UI
{
	[ActionCategory("UI")]
	public class EnableAllUIElementsAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(UIDisablerEvents.EnableAllUIElements));
			Finish();
		}
	}
}
