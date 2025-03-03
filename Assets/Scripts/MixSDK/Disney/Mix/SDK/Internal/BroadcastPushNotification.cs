namespace Disney.Mix.SDK.Internal
{
	public class BroadcastPushNotification : AbstractPushNotification, IBroadcastPushNotification, IPushNotification
	{
		public BroadcastPushNotification(bool notificationsAvailable)
			: base(notificationsAvailable)
		{
		}
	}
}
