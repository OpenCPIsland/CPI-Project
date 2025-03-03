namespace Disney.Mix.SDK.Internal
{
	public class SendMassPushNotificationResult : ISendMassPushNotificationResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public SendMassPushNotificationResult(bool success)
		{
			Success = success;
		}
	}
}
