using ClubPenguin.Net.Domain;

namespace ClubPenguin.Commerce
{
	public static class CommerceServiceEvents
	{
		public struct BillingEnabled
		{
			public readonly bool Success;

			public BillingEnabled(bool success)
			{
				Success = success;
			}
		}

		public struct PurchaseVerified
		{
			public readonly bool Success;

			public readonly MembershipRightsRefresh Data;

			public PurchaseVerified(bool success, MembershipRightsRefresh data)
			{
				Success = success;
				Data = data;
			}
		}

		public struct ProductsLoaded
		{
			public readonly bool Success;

			public ProductsLoaded(bool success)
			{
				Success = success;
			}
		}

		public struct RestoreVerified
		{
			public readonly bool Success;

			public readonly MembershipRightsRefresh Data;

			public RestoreVerified(bool success, MembershipRightsRefresh data)
			{
				Success = success;
				Data = data;
			}
		}

		public struct UnexpectedPurchase
		{
			public readonly PurchaseInfo PI;

			public readonly SkuInfo SI;

			public UnexpectedPurchase(PurchaseInfo pi, SkuInfo si)
			{
				PI = pi;
				SI = si;
			}
		}
	}
}
