namespace Disney.Mix.SDK.Internal
{
	public class FriendshipAddedPushNotification : AbstractPushNotification, IFriendshipAddedPushNotification, IPushNotification
	{
		public string FriendId
		{
			get;
			private set;
		}

		public FriendshipAddedPushNotification(bool notificationsAvailable, string friendId)
			: base(notificationsAvailable)
		{
			FriendId = friendId;
		}
	}
}
