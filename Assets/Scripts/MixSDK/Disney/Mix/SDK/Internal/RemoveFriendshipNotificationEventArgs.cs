using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class RemoveFriendshipNotificationEventArgs : AbstractRemoveFriendshipNotificationEventArgs
	{
		public override RemoveFriendshipNotification Notification
		{
			get;
			protected set;
		}

		public RemoveFriendshipNotificationEventArgs(RemoveFriendshipNotification notification)
		{
			Notification = notification;
		}
	}
}
