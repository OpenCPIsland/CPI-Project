namespace Disney.Mix.SDK.Internal
{
	internal class SendUsernameRecoveryRateLimitedResult : ISendUsernameRecoveryRateLimitedResult, ISendUsernameRecoveryResult
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
