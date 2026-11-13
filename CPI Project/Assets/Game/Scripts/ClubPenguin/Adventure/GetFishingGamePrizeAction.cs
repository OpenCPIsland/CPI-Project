using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Deprecated")]
	public class GetFishingGamePrizeAction : FsmStateAction
	{
		public FsmString PrizeName;

		public override void OnEnter()
		{
		}

		private bool onFishingResultReceived(MinigameServiceEvents.FishingResultRecieved evt)
		{
			PrizeName.Value = "";
			Finish();
			return false;
		}

		public override void OnExit()
		{
			Service.Get<EventDispatcher>().RemoveListener<MinigameServiceEvents.FishingResultRecieved>(onFishingResultReceived);
		}
	}
}
