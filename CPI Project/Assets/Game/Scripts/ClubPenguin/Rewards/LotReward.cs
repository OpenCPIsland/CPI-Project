using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Rewards
{
	[Serializable]
	public class LotReward : AbstractListReward<string>
	{
		public List<string> Lots
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
				return "lots";
			}
		}

		public LotReward()
		{
		}

		public LotReward(string value)
			: base(value)
		{
		}
	}
}
