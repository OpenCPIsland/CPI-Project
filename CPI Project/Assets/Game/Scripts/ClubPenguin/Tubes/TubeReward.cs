using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Tubes
{
	[Serializable]
	public class TubeReward : AbstractListReward<int>
	{
		public List<int> Tubes
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
				return "tubes";
			}
		}

		public TubeReward()
		{
		}

		public TubeReward(int value)
			: base(value)
		{
		}
	}
}
