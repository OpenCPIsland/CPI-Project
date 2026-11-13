using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Client
{
	public class MinigameApi
	{
		private ClubPenguinClient clubPenguinClient;

		public MinigameApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<FishingCastOperation> FishingCast(SignedResponse<InWorldState> state)
		{
			FishingCastOperation operation = new FishingCastOperation(state);
			return new APICall<FishingCastOperation>(clubPenguinClient, operation);
		}

		public APICall<FishingCatchOperation> FishingCatch(SignedResponse<FishingResult> fish, string winningRewardName)
		{
			FishingCatchOperation operation = new FishingCatchOperation(request: new FishingCatchRequest(fish, winningRewardName), clubPenguinClient: clubPenguinClient);
			return new APICall<FishingCatchOperation>(clubPenguinClient, operation);
		}
	}
}
