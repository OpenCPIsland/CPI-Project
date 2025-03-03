using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class RemoveFriendshipInvitationNotificationEventArgs : AbstractRemoveFriendshipInvitationNotificationEventArgs
	{
		public override RemoveFriendshipInvitationNotification Notification
		{
			get;
			protected set;
		}

		public RemoveFriendshipInvitationNotificationEventArgs(RemoveFriendshipInvitationNotification notification)
		{
			Notification = notification;
		}
	}
}
