using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct RewardedUserCollectionJsonHelper
	{
		public RewardSource source;

		public string sourceId;

		public Dictionary<string, RewardJsonReader> rewards;

		public RewardedUserCollection toRewardedUserCollection()
		{
			RewardedUserCollection rewardedUserCollection = new RewardedUserCollection();
			rewardedUserCollection.source = source;
			rewardedUserCollection.sourceId = sourceId;
			rewardedUserCollection.rewards = new Dictionary<long, Reward>();
			foreach (KeyValuePair<string, RewardJsonReader> reward in rewards)
			{
				rewardedUserCollection.rewards.Add(long.Parse(reward.Key), reward.Value.ToReward());
			}
			return rewardedUserCollection;
		}
	}
}
