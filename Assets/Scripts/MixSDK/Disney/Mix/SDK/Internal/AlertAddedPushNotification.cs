namespace Disney.Mix.SDK.Internal
{
	public class AlertAddedPushNotification : AbstractPushNotification, IAlertAddedPushNotification, IPushNotification
	{
		public AlertAddedPushNotification(bool notificationsAvailable)
			: base(notificationsAvailable)
		{
		}
	}
}
