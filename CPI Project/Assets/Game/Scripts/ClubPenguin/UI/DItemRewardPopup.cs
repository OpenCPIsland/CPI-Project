using ClubPenguin.Adventure;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class DItemRewardPopup
	{
		public string HeaderText;

		public string ItemName;

		public int CoinReward;

		public int XpReward;

		public MascotDefinition MascotDefinition;

		public string MascotAbbreviatedName;

		public RewardCategory RewardCategory;

		public DReward RewardData;

		public bool IsQuestItem;

		public SpriteContentKey RewardIcon;
	}
}
