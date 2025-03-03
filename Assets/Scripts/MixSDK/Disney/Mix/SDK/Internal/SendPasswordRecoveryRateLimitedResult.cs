namespace Disney.Mix.SDK.Internal
{
	internal class SendPasswordRecoveryRateLimitedResult : ISendPasswordRecoveryRateLimitedResult, ISendPasswordRecoveryResult
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
