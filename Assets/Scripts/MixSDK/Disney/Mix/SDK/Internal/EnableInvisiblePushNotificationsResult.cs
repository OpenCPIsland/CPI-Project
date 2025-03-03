namespace Disney.Mix.SDK.Internal
{
	public class EnableInvisiblePushNotificationsResult : IEnableInvisiblePushNotificationsResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public EnableInvisiblePushNotificationsResult(bool success)
		{
			Success = success;
		}
	}
}
