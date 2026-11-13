using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct DailySpinData : IOfflineData
	{
		public List<int> EarnedRepeatableRewardIds;

		public List<int> EarnedNonRepeatableRewardIds;

		public int NumSpinsSinceReceivedChest;

		public int NumSpinsSinceReceivedExtraSpin;

		public long TimeOfLastSpinInMilliseconds;

		public int CurrentChestId;

		public int NumPunchesOnCurrentChest;

		public int NumChestsReceivedOfCurrentChestId;

		public void Init()
		{
			EarnedRepeatableRewardIds = new List<int>();
			EarnedNonRepeatableRewardIds = new List<int>();
		}
	}
}
