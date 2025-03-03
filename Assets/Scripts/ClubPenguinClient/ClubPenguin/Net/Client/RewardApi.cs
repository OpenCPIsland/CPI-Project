using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class RewardApi
	{
		private ClubPenguinClient clubPenguinClient;

		public RewardApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<AddRewardOperation> AddReward(SignedResponse<RewardedUserCollectionJsonHelper> rewards)
		{
			AddRewardOperation operation = new AddRewardOperation(clubPenguinClient, rewards);
			return new APICall<AddRewardOperation>(clubPenguinClient, operation);
		}

		public APICall<AddRoomRewardsOperation> AddRoomRewards(SignedResponse<InRoomRewards> inRoomRewards)
		{
			AddRoomRewardsOperation operation = new AddRoomRewardsOperation(inRoomRewards);
			return new APICall<AddRoomRewardsOperation>(clubPenguinClient, operation);
		}

		public APICall<ExchangeAllForCoinsOperation> ExchangeAllForCoins()
		{
			ExchangeAllForCoinsOperation operation = new ExchangeAllForCoinsOperation();
			return new APICall<ExchangeAllForCoinsOperation>(clubPenguinClient, operation);
		}

		public APICall<CalculateExchangeForCoinsOperation> CalculateExchangeAllForCoins()
		{
			CalculateExchangeForCoinsOperation operation = new CalculateExchangeForCoinsOperation();
			return new APICall<CalculateExchangeForCoinsOperation>(clubPenguinClient, operation);
		}

		public APICall<ClaimPreregistrationRewardOperation> ClaimPreregistationRewards()
		{
			ClaimPreregistrationRewardOperation operation = new ClaimPreregistrationRewardOperation();
			return new APICall<ClaimPreregistrationRewardOperation>(clubPenguinClient, operation);
		}

		public APICall<ClaimRewardOperation> ClaimReward(int rewardId)
		{
			ClaimRewardOperation operation = new ClaimRewardOperation(rewardId);
			return new APICall<ClaimRewardOperation>(clubPenguinClient, operation);
		}

		public APICall<ClaimServerAddedRewardsOperation> ClaimServerAddedRewards()
		{
			ClaimServerAddedRewardsOperation operation = new ClaimServerAddedRewardsOperation();
			return new APICall<ClaimServerAddedRewardsOperation>(clubPenguinClient, operation);
		}

		public APICall<ClaimQuickNotificationRewardOperation> ClaimQuickNotificationReward()
		{
			ClaimQuickNotificationRewardOperation operation = new ClaimQuickNotificationRewardOperation();
			return new APICall<ClaimQuickNotificationRewardOperation>(clubPenguinClient, operation);
		}

		public APICall<ClaimDailySpinRewardOperation> ClaimDailySpinReward()
		{
			ClaimDailySpinRewardOperation operation = new ClaimDailySpinRewardOperation();
			return new APICall<ClaimDailySpinRewardOperation>(clubPenguinClient, operation);
		}

		public APICall<QASetRewardOperation> QA_SetReward(Reward reward)
		{
			QASetRewardOperation operation = new QASetRewardOperation(reward);
			return new APICall<QASetRewardOperation>(clubPenguinClient, operation);
		}
	}
}
