using ClubPenguin.Net.Domain;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Durable
{
	[Serializable]
	public class DurableReward : AbstractListReward<int>
	{
		public List<int> Durables
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
				return "durables";
			}
		}

		public DurableReward()
		{
		}

		public DurableReward(int value)
			: base(value)
		{
		}
	}
}
