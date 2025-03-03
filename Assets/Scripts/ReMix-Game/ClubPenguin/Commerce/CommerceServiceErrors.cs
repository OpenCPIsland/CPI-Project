namespace ClubPenguin.Commerce
{
	public static class CommerceServiceErrors
	{
		public struct BillingEnabledError
		{
			public readonly CommerceError Error;

			public BillingEnabledError(CommerceError error)
			{
				Error = error;
			}
		}

		public struct PurchaseVerifiedError
		{
			public readonly CommerceError Error;

			public PurchaseVerifiedError(CommerceError error)
			{
				Error = error;
			}
		}

		public struct ProductsLoadedError
		{
			public readonly CommerceError Error;

			public ProductsLoadedError(CommerceError error)
			{
				Error = error;
			}
		}

		public struct RestoreVerifiedError
		{
			public readonly CommerceError Error;

			public RestoreVerifiedError(CommerceError error)
			{
				Error = error;
			}
		}
	}
}
