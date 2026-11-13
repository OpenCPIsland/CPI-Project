using System;

namespace ClubPenguin.Net.Domain.ScheduledEvent
{
	[Serializable]
	public class DonationResult
	{
		public long cfcTotal;

		public int remainingCoinBalance;

		public RewardJsonReader reward;
	}
}
