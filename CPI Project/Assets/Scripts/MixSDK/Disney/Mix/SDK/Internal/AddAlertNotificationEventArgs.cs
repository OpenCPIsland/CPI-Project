using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class AddAlertNotificationEventArgs : AbstractAddAlertNotificationEventArgs
	{
		public override AddAlertNotification Notification
		{
			get;
			protected set;
		}

		public AddAlertNotificationEventArgs(AddAlertNotification notification)
		{
			Notification = notification;
		}
	}
}
