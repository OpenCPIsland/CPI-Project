using System.Runtime.InteropServices;

namespace ClubPenguin.Rewards
{
	public static class RewardEvents
	{
		public struct AddXP
		{
			public readonly string MascotName;

			public readonly long PreviousLevel;

			public readonly long CurrentLevel;

			public readonly int XPAdded;

			public bool ShowReward;

			public AddXP(string mascotName, long previousLevel, long currentLevel, int xpAdded, bool showReward)
			{
				MascotName = mascotName;
				PreviousLevel = previousLevel;
				CurrentLevel = currentLevel;
				XPAdded = xpAdded;
				ShowReward = showReward;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowSuppressedAddXP
		{
		}

		public struct RewardPopupComplete
		{
			public readonly DRewardPopup RewardPopupData;

			public RewardPopupComplete(DRewardPopup rewardPopupData)
			{
				RewardPopupData = rewardPopupData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SuppressLevelUpPopup
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct UnsuppressLevelUpPopup
		{
		}
	}
}
