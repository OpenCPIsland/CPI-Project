using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Consumable
{
	[Serializable]
	public class ConsumableReward : AbstractListReward<int>
	{
		public List<int> Consumable
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
				return "partySupplies";
			}
		}

		public ConsumableReward()
		{
		}

		public ConsumableReward(int value)
			: base(value)
		{
		}
	}
}
