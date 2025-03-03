using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner
{
	[Serializable]
	public class DecalReward : AbstractListReward<int>
	{
		public List<int> Decals
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
				return "decals";
			}
		}

		public DecalReward()
		{
		}

		public DecalReward(int value)
			: base(value)
		{
		}
	}
}
