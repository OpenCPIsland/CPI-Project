namespace ClubPenguin.Net
{
	public interface IScheduledEventService : INetworkService
	{
		void PostCFCDonation(int coins);

		void GetCFCDonations();
	}
}
