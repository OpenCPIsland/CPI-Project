using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.MiniGames.Fishing
{
	[ActionCategory("Fishing")]
	public class UpdateBaitCounterAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(FishingEvents.UpdateFishingBaitUI));
			Finish();
		}
	}
}
