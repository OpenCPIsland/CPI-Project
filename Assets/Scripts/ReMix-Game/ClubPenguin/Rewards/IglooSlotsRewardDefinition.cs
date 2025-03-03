using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class IglooSlotsRewardDefinition : IRewardableDefinition
	{
		public int Count;

		IRewardable IRewardableDefinition.Reward
		{
			get
			{
				return new IglooSlotsReward(Count);
			}
		}

		public static int IglooSlots(RewardDefinition reward)
		{
			if (reward == null)
			{
				return 0;
			}
			List<IglooSlotsRewardDefinition> definitions = reward.GetDefinitions<IglooSlotsRewardDefinition>();
			int num = 0;
			if (definitions.Count > 1)
			{
			}
			for (int i = 0; i < definitions.Count; i++)
			{
				num += definitions[i].Count;
			}
			return num;
		}
	}
}
