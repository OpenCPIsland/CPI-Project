namespace Disney.Mix.SDK.Internal
{
	internal class SendMultipleAccountsResolutionRateLimitedResult : ISendMultipleAccountsResolutionRateLimitedResult, ISendMultipleAccountsResolutionResult
	{
		public bool Success
		{
			get
			{
				return false;
			}
		}
	}
}
