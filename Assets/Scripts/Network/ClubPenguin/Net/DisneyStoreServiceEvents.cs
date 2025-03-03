namespace ClubPenguin.Net
{
	public static class DisneyStoreServiceEvents
	{
		public enum DisneyStorePurchaseResult
		{
			Success,
			Error
		}

		public struct DisneyStorePurchaseComplete
		{
			public readonly DisneyStorePurchaseResult Result;

			public DisneyStorePurchaseComplete(DisneyStorePurchaseResult result)
			{
				Result = result;
			}
		}
	}
}
