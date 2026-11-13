using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	public class StructureReward : AbstractListReward<int>
	{
		public List<int> Structures
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
				return "structurePurchaseRights";
			}
		}

		public StructureReward()
		{
		}

		public StructureReward(int value)
			: base(value)
		{
		}
	}
}
