public class DRewardPopupScreen
{
	public enum RewardScreenPopupType
	{
		splash,
		items,
		count,
		coins,
		xp,
		coinsxp,
		splash_levelup,
		splash_replay,
		quests,
		custom
	}

	public RewardScreenPopupType ScreenType;

	public bool IsRewardsAllNonMember;

	public int PreferredSortOrder;
}
