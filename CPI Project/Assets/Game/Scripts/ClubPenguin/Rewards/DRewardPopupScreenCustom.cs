using Disney.Kelowna.Common;

namespace ClubPenguin.Rewards
{
	public class DRewardPopupScreenCustom : DRewardPopupScreen
	{
		public PrefabContentKey ScreenKey;

		public DRewardPopupScreenCustom(PrefabContentKey screenKey)
		{
			ScreenKey = screenKey;
			ScreenType = RewardScreenPopupType.custom;
		}
	}
}
