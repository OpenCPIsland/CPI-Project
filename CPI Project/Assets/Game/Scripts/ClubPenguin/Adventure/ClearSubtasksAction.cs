using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ClearSubtasksAction : FsmStateAction
	{
		public bool PlayAnimation = true;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.DestroySubtaskText(PlayAnimation));
			Finish();
		}
	}
}
