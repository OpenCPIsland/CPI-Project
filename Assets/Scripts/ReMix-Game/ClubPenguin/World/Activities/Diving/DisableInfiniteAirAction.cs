using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.World.Activities.Diving
{
	public class DisableInfiniteAirAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(DivingEvents.DisableLocalInfiniteAir));
			Finish();
		}
	}
}
