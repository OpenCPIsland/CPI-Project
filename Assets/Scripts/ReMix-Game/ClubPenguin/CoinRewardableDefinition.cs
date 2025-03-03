using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class CoinRewardableDefinition : IRewardableDefinition
	{
		public int Amount;

		IRewardable IRewardableDefinition.Reward
		{
			get
			{
				return new CoinReward(Amount);
			}
		}

		public static int Coins(RewardDefinition reward)
		{
			if (reward == null)
			{
				return 0;
			}
			List<CoinRewardableDefinition> definitions = reward.GetDefinitions<CoinRewardableDefinition>();
			int num = 0;
			if (definitions.Count > 1)
			{
			}
			for (int i = 0; i < definitions.Count; i++)
			{
				num += definitions[i].Amount;
			}
			return num;
		}
	}
}
