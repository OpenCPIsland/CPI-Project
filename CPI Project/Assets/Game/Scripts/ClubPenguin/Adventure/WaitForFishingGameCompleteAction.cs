using ClubPenguin.MiniGames.Fishing;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Fishing")]
	public class WaitForFishingGameCompleteAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<FishingEvents.FishingGameComplete>(onFishingGameComplete);
		}

		private bool onFishingGameComplete(FishingEvents.FishingGameComplete evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<FishingEvents.FishingGameComplete>(onFishingGameComplete);
			Finish();
			return false;
		}

		public override void OnExit()
		{
			Service.Get<EventDispatcher>().RemoveListener<FishingEvents.FishingGameComplete>(onFishingGameComplete);
		}
	}
}
