using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	internal class MinigameService : BaseNetworkService, IMinigameService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void CastFishingRod(ICastFishingRodErrorHandler errorHandler)
		{
			new CastFishingRodSequence(clubPenguinClient, errorHandler).CastFishingRod();
		}

		public void CatchFish(SignedResponse<FishingResult> fish, string winningRewardName)
		{
			APICall<FishingCatchOperation> aPICall = clubPenguinClient.MinigameApi.FishingCatch(fish, winningRewardName);
			aPICall.OnComplete += onFishingRewardGranted;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onFishingRewardGranted(FishingCatchOperation fishingCatchOperation, HttpResponse response)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.RewardsEarned(fishingCatchOperation.Response.rewards.Data.toRewardedUserCollection()));
			clubPenguinClient.GameServer.SendRewardNotification(fishingCatchOperation.Response.rewards);
			handleCPResponse(fishingCatchOperation.Response);
		}
	}
}
