using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ShowHideQuestNotifierAction : FsmStateAction
	{
		public bool Show;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.ShowHideQuestNotifier(Show));
			Finish();
		}
	}
}
