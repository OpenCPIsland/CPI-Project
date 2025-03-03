using ClubPenguin.Net.Domain;
using LitJson;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class IAPServiceEvents
	{
		public struct CheckRestoreReturned
		{
			public readonly PurchaseResponse Purchase;

			public CheckRestoreReturned(PurchaseResponse purchase)
			{
				Purchase = purchase;
			}
		}

		public struct PurchaseReturned
		{
			public readonly PurchaseResponse Purchase;

			public PurchaseReturned(PurchaseResponse purchase)
			{
				Purchase = purchase;
			}
		}

		public struct SKULookupCompleted
		{
			public readonly bool LookupComplete;

			public SKULookupCompleted(bool lookupComplete)
			{
				LookupComplete = lookupComplete;
			}
		}

		public struct SessionStarted
		{
			public readonly string SessionID;

			public readonly JsonData SessionSummary;

			public SessionStarted(string sessionID, JsonData sessionSummary)
			{
				SessionID = sessionID;
				SessionSummary = sessionSummary;
			}
		}

		public struct PCProductDetailsReturned
		{
			public readonly PCGetProductDetailsResponse ProductDetails;

			public PCProductDetailsReturned(PCGetProductDetailsResponse productDetails)
			{
				ProductDetails = productDetails;
			}
		}

		public struct PCPurchaseSuccess
		{
			public readonly JsonData PurchaseDetails;

			public PCPurchaseSuccess(JsonData purchaseDetails)
			{
				PurchaseDetails = purchaseDetails;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PCPurchaseCancelled
		{
		}
	}
}
