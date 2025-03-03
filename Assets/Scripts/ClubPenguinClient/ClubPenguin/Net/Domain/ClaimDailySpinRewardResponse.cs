namespace ClubPenguin.Net.Domain
{
	public class ClaimDailySpinRewardResponse : CPResponse
	{
		public int spinOutcomeId;

		public RewardJsonReader reward;

		public RewardJsonReader chestReward;
	}
}
