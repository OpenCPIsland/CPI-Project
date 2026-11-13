namespace Disney.Mix.SDK.Internal
{
	public class SendVerificationEmailResult : ISendVerificationEmailResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendVerificationEmailResult(bool success)
		{
			Success = success;
		}
	}
}
