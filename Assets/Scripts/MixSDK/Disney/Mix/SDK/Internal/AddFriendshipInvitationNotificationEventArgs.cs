using Disney.Mix.SDK.Internal.MixDomain;

namespace Disney.Mix.SDK.Internal
{
	public class AddFriendshipInvitationNotificationEventArgs : AbstractAddFriendshipInvitationNotificationEventArgs
	{
		public override AddFriendshipInvitationNotification Notification
		{
			get;
			protected set;
		}

		public AddFriendshipInvitationNotificationEventArgs(AddFriendshipInvitationNotification notification)
		{
			Notification = notification;
		}
	}
}
