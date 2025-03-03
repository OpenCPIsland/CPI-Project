using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface IRewardService : INetworkService
	{
		void ExchangeAllForCoins();

		void CalculateExchangeAllForCoins(IBaseNetworkErrorHandler errorHandler);

		void ClaimPreregistrationRewards();

		void ClaimReward(int rewardId);

		void ClaimServerAddedRewards();

		void ClaimQuickNotificationReward();

		void ClaimDailySpinReward();

		void QA_SetReward(Reward reward);
	}
}
