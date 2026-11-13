namespace Disney.Mix.SDK.Internal
{
	internal class SendNonRegisteredTransactorUpgradeResult : ISendNonRegisteredTransactorUpgradeResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendNonRegisteredTransactorUpgradeResult(bool success)
		{
			Success = success;
		}
	}
}
