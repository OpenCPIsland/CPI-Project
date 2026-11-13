namespace ClubPenguin.Net.Domain
{
	public class FishingCatchResponse : CPResponse
	{
		public SignedResponse<RewardedUserCollectionJsonHelper> rewards;
	}
}
