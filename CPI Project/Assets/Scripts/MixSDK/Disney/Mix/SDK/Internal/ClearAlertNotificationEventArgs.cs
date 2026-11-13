using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class ClearAlertNotificationEventArgs : AbstractClearAlertNotificationEventArgs
	{
		public override ClearAlertNotification Notification
		{
			get;
			protected set;
		}

		public ClearAlertNotificationEventArgs(ClearAlertNotification notification)
		{
			Notification = notification;
		}
	}
}
