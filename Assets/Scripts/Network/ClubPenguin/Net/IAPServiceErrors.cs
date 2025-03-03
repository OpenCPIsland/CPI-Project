using LitJson;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class IAPServiceErrors
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ProductsLoadedError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ProductsDetailsLoadedError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PurchaseError
		{
		}

		public struct PCPurchaseError
		{
			public readonly JsonData PurchaseError;

			public PCPurchaseError(JsonData purchaseError)
			{
				PurchaseError = purchaseError;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionExpired
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionStartError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CheckRestoreError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SkuLookupError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MembershipGrantError
		{
		}
	}
}
