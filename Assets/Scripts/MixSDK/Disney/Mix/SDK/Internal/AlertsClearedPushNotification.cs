namespace Disney.Mix.SDK.Internal
{
	public class AlertsClearedPushNotification : AbstractPushNotification, IAlertsClearedPushNotification, IPushNotification
	{
		public AlertsClearedPushNotification(bool notificationsAvailable)
			: base(notificationsAvailable)
		{
		}
	}
}
