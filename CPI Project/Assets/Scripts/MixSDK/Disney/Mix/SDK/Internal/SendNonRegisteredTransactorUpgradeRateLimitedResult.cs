namespace Disney.Mix.SDK.Internal
{
	internal class SendNonRegisteredTransactorUpgradeRateLimitedResult : ISendNonRegisteredTransactorUpgradeRateLimitedResult, ISendNonRegisteredTransactorUpgradeResult
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
