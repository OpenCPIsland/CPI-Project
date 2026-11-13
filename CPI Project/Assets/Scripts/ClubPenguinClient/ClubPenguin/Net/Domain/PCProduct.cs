using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PCProduct
	{
		public List<PCPricePlan> PricingPlans;
	}
}
