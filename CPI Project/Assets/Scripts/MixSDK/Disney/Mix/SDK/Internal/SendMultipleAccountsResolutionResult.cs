namespace Disney.Mix.SDK.Internal
{
	internal class SendMultipleAccountsResolutionResult : ISendMultipleAccountsResolutionResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendMultipleAccountsResolutionResult(bool success)
		{
			Success = success;
		}
	}
}
