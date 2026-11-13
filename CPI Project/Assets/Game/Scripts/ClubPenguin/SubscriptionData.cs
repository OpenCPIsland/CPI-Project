using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class SubscriptionData : BaseData
	{
		public string SubscriptionVendor
		{
			get;
			set;
		}

		public string SubscriptionProductId
		{
			get;
			set;
		}

		public bool SubscriptionPaymentPending
		{
			get;
			set;
		}

		public bool SubscriptionRecurring
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SubscriptionDataMonoBehaviour);
			}
		}

		public override string ToString()
		{
			return string.Format("SubscriptionData: \n \t SubscriptionVendor: {0},  \n \t SubscriptionProductId: {1},  \n \t SubscriptionPaymentPending: {2},  \n \t SubscriptionRecurring: {3}", SubscriptionVendor, SubscriptionProductId, SubscriptionPaymentPending, SubscriptionRecurring);
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
