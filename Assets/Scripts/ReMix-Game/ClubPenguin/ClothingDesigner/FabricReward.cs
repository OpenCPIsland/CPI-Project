using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class FabricReward : AbstractListReward<int>
	{
		public List<int> Fabrics
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
				return "fabrics";
			}
		}

		public FabricReward()
		{
		}

		public FabricReward(int value)
			: base(value)
		{
		}
	}
}
