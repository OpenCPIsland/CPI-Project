using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class RewardJsonWritter : Dictionary<string, object>
	{
		public RewardJsonWritter()
		{
		}

		protected RewardJsonWritter(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static RewardJsonWritter FromReward(Reward reward)
		{
			RewardJsonWritter rewardJsonWritter = new RewardJsonWritter();
			foreach (IRewardable item in reward)
			{
				rewardJsonWritter.Add(item.RewardType, item.Reward);
			}
			return rewardJsonWritter;
		}
	}
}
