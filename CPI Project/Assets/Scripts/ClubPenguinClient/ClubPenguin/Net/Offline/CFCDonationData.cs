using System;

namespace ClubPenguin.Net.Offline
{
	public struct CFCDonationData : IOfflineData
	{
		public long cfcTotal;

		public long personalContribution;

		public int donationCount;

		public long lastDonationTime;

		public int coinBalance;

		public void Init()
		{
			cfcTotal = 0;
			personalContribution = 0;
			donationCount = 0;
			lastDonationTime = 0;
			coinBalance = 0;
		}
	}
}
