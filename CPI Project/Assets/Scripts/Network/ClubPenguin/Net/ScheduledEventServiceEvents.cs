using ClubPenguin.Net.Domain.ScheduledEvent;

namespace ClubPenguin.Net
{
	public static class ScheduledEventServiceEvents
	{
		public struct CFCDonationPosted
		{
			public DonationResult DonationResult;

			public int DonationAmount;

			public CFCDonationPosted(DonationResult donationResult)
				: this(donationResult, 0)
			{
			}

			public CFCDonationPosted(DonationResult donationResult, int donationAmount)
			{
				DonationResult = donationResult;
				DonationAmount = donationAmount;
			}
		}

		public struct CFCDonationsLoaded
		{
			public CFCDonations CFCDonations;

			public CFCDonationsLoaded(CFCDonations cfcDonations)
			{
				CFCDonations = cfcDonations;
			}
		}
	}
}
