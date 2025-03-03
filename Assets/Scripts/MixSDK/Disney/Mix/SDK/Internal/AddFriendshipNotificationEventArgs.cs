using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class AddFriendshipNotificationEventArgs : AbstractAddFriendshipNotificationEventArgs
	{
		public override AddFriendshipNotification Notification
		{
			get;
			protected set;
		}

		public AddFriendshipNotificationEventArgs(AddFriendshipNotification notification)
		{
			Notification = notification;
		}
	}
}
