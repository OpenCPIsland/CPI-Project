using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.MiniGames.Fishing
{
	[ActionCategory("Fishing")]
	public class SetFishingStateAction : FsmStateAction
	{
		public FishingEvents.FishingState FishingState;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FishingEvents.SetFishingState(FishingState));
			Finish();
		}
	}
}
