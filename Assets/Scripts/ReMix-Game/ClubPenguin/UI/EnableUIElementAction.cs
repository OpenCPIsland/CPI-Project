using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.UI
{
	[ActionCategory("UI")]
	public class EnableUIElementAction : FsmStateAction
	{
		public string UIElementID;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement(UIElementID));
			Finish();
		}
	}
}
