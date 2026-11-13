using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class ChangeNavigationTypeAction : FsmStateAction
	{
		public bool Use3DNavigation = true;

		public bool UseZNavigationFor2D = false;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SetNavigationStyle(Use3DNavigation, UseZNavigationFor2D));
			Finish();
		}
	}
}
