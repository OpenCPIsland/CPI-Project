using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net
{
	public interface IMinigameService : INetworkService
	{
		void CastFishingRod(ICastFishingRodErrorHandler errorHandler);

		void CatchFish(SignedResponse<FishingResult> fish, string winningRewardName);
	}
}
