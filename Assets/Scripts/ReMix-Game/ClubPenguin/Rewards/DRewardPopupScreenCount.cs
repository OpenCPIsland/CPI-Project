namespace ClubPenguin.Rewards
{
	public class DRewardPopupScreenCount : DRewardPopupScreen
	{
		public RewardCategory CountCategory;

		public int Count;

		public DRewardPopupScreenCount()
		{
			ScreenType = RewardScreenPopupType.count;
		}
	}
}
