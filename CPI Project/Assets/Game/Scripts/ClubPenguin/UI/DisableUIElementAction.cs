using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.UI
{
	[ActionCategory("UI")]
	public class DisableUIElementAction : FsmStateAction
	{
		public string UIElementID;

		public bool HideElement;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement(UIElementID, HideElement));
			Finish();
		}
	}
}
