using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ResetQuestNotifierAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ResetQuestNotifier));
			Finish();
		}
	}
}
