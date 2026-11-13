namespace Disney.Mix.SDK.Internal
{
	public class EnableAllPushNotificationsResult : IEnableAllPushNotificationsResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public EnableAllPushNotificationsResult(bool success)
		{
			Success = success;
		}
	}
}
