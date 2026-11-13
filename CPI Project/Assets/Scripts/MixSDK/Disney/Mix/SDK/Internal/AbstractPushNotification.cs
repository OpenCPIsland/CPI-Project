namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractPushNotification : IInternalPushNotification, IPushNotification
	{
		public bool NotificationsAvailable
		{
			get;
			private set;
		}

		protected AbstractPushNotification(bool notificationsAvailable)
		{
			NotificationsAvailable = notificationsAvailable;
		}
	}
}
