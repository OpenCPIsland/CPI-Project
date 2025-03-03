using Disney.Kelowna.Common;

namespace ClubPenguin.Rewards
{
	public class RewardPopupConstants
	{
		public const string TWEEN_DESTINATION_CUSTOMIZER = "MainNavButton_Profile";

		public const string TWEEN_DESTINATION_SIZZLECLIPS = "MainNavButton_Profile";

		public const string ANIMATOR_TRIGGER_CHESTOPEN = "ChestOpen";

		public const string ANIMATOR_TRIGGER_CHESTDISMIESS = "ChestDismiss";

		public const string ANIMATOR_TRIGGER_LAUNCHITEMS = "LaunchItems";

		public const string ANIMATOR_TRIGGER_CHESTLANDS = "ChestLands";

		public const string ANIMATOR_TRIGGER_LEVELUP = "LevelUp";

		public const string ANIMATOR_TRIGGER_QUEST = "Quest";

		public static PrefabContentKey RewardPopupContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardPopup");

		public static PrefabContentKey RewardPopupItemContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardPopupItem");

		public static PrefabContentKey RewardPopupQuestsContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardQuestsItem_*");

		public static PrefabContentKey ScreenSplashContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_Splash");

		public static PrefabContentKey ScreenLevelUpContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_Splash_LevelUp");

		public static PrefabContentKey ScreenReplayContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_Replay_Splash");

		public static PrefabContentKey ScreenItemContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_Item");

		public static PrefabContentKey ScreenCoinsXPContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_CoinXP");

		public static PrefabContentKey ScreenCountContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_Count");

		public static PrefabContentKey ScreenQuestsContentKey = new PrefabContentKey("Rewards/RewardPopupScreens/RewardPopupScreen_Quests");

		public static Texture2DContentKey DefaultIconContentKey = new Texture2DContentKey("Sprites/Achievements/PlayerProfile_DecalSlotlPH_1");
	}
}
