using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class SetButtonInteractableAction : FsmStateAction
	{
		public string ButtonPanelName = "";

		public string ButtonName = "";

		public bool IsInteractable = true;

		public bool HideOnDisable = false;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ButtonEvents.SetButtonInteractable(ButtonPanelName, ButtonName, IsInteractable, HideOnDisable));
			Finish();
		}
	}
}
