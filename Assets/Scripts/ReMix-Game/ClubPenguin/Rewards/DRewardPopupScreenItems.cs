namespace ClubPenguin.Rewards
{
	public class DRewardPopupScreenItems : DRewardPopupScreen
	{
		public RewardCategory ItemCategory;

		public DReward[] Rewards;

		public DRewardPopup.RewardPopupType RewardPopupType;

		public DRewardPopupScreenItems()
		{
			ScreenType = RewardScreenPopupType.items;
		}
	}
}
