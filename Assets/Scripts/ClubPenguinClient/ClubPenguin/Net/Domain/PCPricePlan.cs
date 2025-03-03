using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PCPricePlan
	{
		public double ChargeAmount;

		public string Currency;

		public string Description;

		public long Id;

		public string Name;

		public double RenewalChargeAmount;

		public bool Subscription;

		public int SubscriptionBillingCycle;

		public int SubscriptionBillingCycleIterations;

		public string SubscriptionBillingCycleName;

		public int Type;

		public List<AdditionalPricePlanProperties> AdditionalProperties;

		public List<ExternalReference> ExternalReferences;

		public List<int> ChangePricingPlanIds;

		public bool Default;

		public string DisplayName;
	}
}
