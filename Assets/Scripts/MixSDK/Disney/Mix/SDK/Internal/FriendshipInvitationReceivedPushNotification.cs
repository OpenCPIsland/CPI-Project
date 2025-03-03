namespace Disney.Mix.SDK.Internal
{
	public class FriendshipInvitationReceivedPushNotification : AbstractPushNotification, IFriendshipInvitationReceivedPushNotification, IPushNotification
	{
		public string FriendshipInvitationId
		{
			get;
			private set;
		}

		public FriendshipInvitationReceivedPushNotification(bool notificationsAvailable, string friendshipInvitationId)
			: base(notificationsAvailable)
		{
			FriendshipInvitationId = friendshipInvitationId;
		}
	}
}
