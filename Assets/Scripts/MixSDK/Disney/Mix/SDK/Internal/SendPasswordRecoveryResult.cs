namespace Disney.Mix.SDK.Internal
{
	internal class SendPasswordRecoveryResult : ISendPasswordRecoveryResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendPasswordRecoveryResult(bool success)
		{
			Success = success;
		}
	}
}
