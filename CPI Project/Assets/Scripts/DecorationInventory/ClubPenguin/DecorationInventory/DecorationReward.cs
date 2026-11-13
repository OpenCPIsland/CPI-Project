using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class DecorationReward : AbstractListReward<int>
	{
		public List<int> Decorations
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
				return "decorationPurchaseRights";
			}
		}

		public DecorationReward()
		{
		}

		public DecorationReward(int value)
			: base(value)
		{
		}
	}
}
