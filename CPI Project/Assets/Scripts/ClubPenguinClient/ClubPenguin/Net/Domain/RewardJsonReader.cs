using LitJson;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class RewardJsonReader : Dictionary<string, JsonData>
	{
		public RewardJsonReader()
		{
		}

		protected RewardJsonReader(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public Reward ToReward()
		{
			Reward reward = new Reward();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JsonData> current = enumerator.Current;
					if (RewardableLoader.RewardableTypeMap.ContainsKey(current.Key) && current.Value != null)
					{
						IRewardable rewardable = RewardableLoader.GenerateRewardable(current.Key);
						rewardable.FromJson(current.Value);
						reward.Add(rewardable);
					}
				}
			}
			return reward;
		}
	}
}
