using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Consumable
{
	[Serializable]
	public class ConsumableInstanceReward : AbstractDictionaryReward<string, int>
	{
		public Dictionary<string, int> Consumables
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
				return "consumables";
			}
		}

		public ConsumableInstanceReward()
		{
		}

		public ConsumableInstanceReward(string key, int value)
			: base(key, value)
		{
		}

		protected override int combineValues(int val1, int val2)
		{
			return val1 + val2;
		}
	}
}
