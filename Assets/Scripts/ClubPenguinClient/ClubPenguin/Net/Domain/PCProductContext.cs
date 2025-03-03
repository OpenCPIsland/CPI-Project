using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PCProductContext
	{
		public List<PCOrderablePricePlan> OrderablePricingPlans;

		public long ProductId;
	}
}
