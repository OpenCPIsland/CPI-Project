namespace ClubPenguin.Net.Domain
{
	public class QASetDailySpinDataParams
	{
		public int CurrentChestId;

		public int NumPunchesOnCurrentChest;

		public int NumChestsReceivedOfCurrentChestId;

		public long TimeOfLastSpinInMilliseconds;

		public bool ResetRewards;

		public QASetDailySpinDataParams(int currentChestId, int numPunchesOnCurrentChest, int numChestsReceivedOfCurrentChestId, long timeOfLastSpinInMilliseconds, bool resetRewards)
		{
			CurrentChestId = currentChestId;
			NumPunchesOnCurrentChest = numPunchesOnCurrentChest;
			NumChestsReceivedOfCurrentChestId = numChestsReceivedOfCurrentChestId;
			TimeOfLastSpinInMilliseconds = timeOfLastSpinInMilliseconds;
			ResetRewards = resetRewards;
		}
	}
}
