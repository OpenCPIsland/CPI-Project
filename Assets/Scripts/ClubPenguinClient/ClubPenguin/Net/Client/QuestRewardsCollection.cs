using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public struct QuestRewardsCollection
	{
		public Reward StartReward;

		public Reward CompleteReward;

		public Dictionary<string, Reward> ObjectiveRewards;
	}
}
