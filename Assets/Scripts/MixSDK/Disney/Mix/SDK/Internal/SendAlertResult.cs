namespace Disney.Mix.SDK.Internal
{
	public class SendAlertResult : ISendAlertResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendAlertResult(bool success)
		{
			Success = success;
		}
	}
}
