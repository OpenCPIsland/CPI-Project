using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Collectibles
{
	[Serializable]
	public class CollectibleReward : AbstractDictionaryReward<string, int>
	{
		public Dictionary<string, int> Collectibles
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
				return "collectibleCurrency";
			}
		}

		public CollectibleReward()
		{
		}

		public CollectibleReward(string key, int value)
			: base(key, value)
		{
		}

		protected override int combineValues(int val1, int val2)
		{
			return val1 + val2;
		}
	}
}
