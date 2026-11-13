namespace Disney.Mix.SDK.Internal
{
	public class NotificationsPolledEventArgs : AbstractNotificationsPolledEventArgs
	{
		public override long LastNotificationTimestamp
		{
			get;
			protected set;
		}

		public NotificationsPolledEventArgs(long lastNotificationTimestamp)
		{
			LastNotificationTimestamp = lastNotificationTimestamp;
		}
	}
}
