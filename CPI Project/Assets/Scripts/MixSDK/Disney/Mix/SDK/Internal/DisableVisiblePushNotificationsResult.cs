namespace Disney.Mix.SDK.Internal
{
	public class DisableVisiblePushNotificationsResult : IDisableVisiblePushNotificationsResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public DisableVisiblePushNotificationsResult(bool success)
		{
			Success = success;
		}
	}
}
