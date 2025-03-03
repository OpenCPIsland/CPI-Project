using ClubPenguin.DisneyStore;
using ClubPenguin.Rewards;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	public class DisneyStoreItemData
	{
		private readonly DisneyStoreItemDefinition definition;

		private List<DReward> rewards;

		public DisneyStoreItemDefinition Definition
		{
			get
			{
				return definition;
			}
		}

		public DisneyStoreItemData(DisneyStoreItemDefinition definition)
		{
			this.definition = definition;
		}

		public List<DReward> GetRewards()
		{
			if (rewards == null)
			{
				rewards = RewardUtils.GetDRewardFromReward(definition.Reward.ToReward());
			}
			return rewards;
		}
	}
}
