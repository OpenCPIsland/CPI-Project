namespace ClubPenguin.Net.Client
{
	public class ScheduledEventApi
	{
		private ClubPenguinClient clubPenguinClient;

		public ScheduledEventApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<PostCFCDonationOperation> PostCFCDonation(int coins)
		{
			PostCFCDonationOperation operation = new PostCFCDonationOperation(coins);
			return new APICall<PostCFCDonationOperation>(clubPenguinClient, operation);
		}

		public APICall<GetCFCDonationsOperation> GetCFCDonations()
		{
			GetCFCDonationsOperation operation = new GetCFCDonationsOperation();
			return new APICall<GetCFCDonationsOperation>(clubPenguinClient, operation);
		}
	}
}
