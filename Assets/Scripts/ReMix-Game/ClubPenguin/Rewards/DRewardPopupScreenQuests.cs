using ClubPenguin.Adventure;

namespace ClubPenguin.Rewards
{
	public class DRewardPopupScreenQuests : DRewardPopupScreen
	{
		public QuestDefinition[] quests;

		public DRewardPopupScreenQuests()
		{
			ScreenType = RewardScreenPopupType.quests;
		}
	}
}
