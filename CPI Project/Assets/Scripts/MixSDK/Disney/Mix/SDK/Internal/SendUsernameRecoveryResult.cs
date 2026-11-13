namespace Disney.Mix.SDK.Internal
{
	internal class SendUsernameRecoveryResult : ISendUsernameRecoveryResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendUsernameRecoveryResult(bool success)
		{
			Success = success;
		}
	}
}
