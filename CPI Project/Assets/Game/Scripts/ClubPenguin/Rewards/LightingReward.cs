using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class LightingReward : AbstractListReward<int>
	{
		public List<int> Lighting
		{
			get
			{
				return data;
			}
		}

		public override string RewardType
		{
			get
			{
				return "lighting";
			}
		}

		public LightingReward()
		{
		}

		public LightingReward(int value)
			: base(value)
		{
		}
	}
}
