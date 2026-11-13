using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class RemoveFriendshipTrustNotificationEventArgs : AbstractRemoveFriendshipTrustNotificationEventArgs
	{
		public override RemoveFriendshipTrustNotification Notification
		{
			get;
			protected set;
		}

		public RemoveFriendshipTrustNotificationEventArgs(RemoveFriendshipTrustNotification notification)
		{
			Notification = notification;
		}
	}
}
