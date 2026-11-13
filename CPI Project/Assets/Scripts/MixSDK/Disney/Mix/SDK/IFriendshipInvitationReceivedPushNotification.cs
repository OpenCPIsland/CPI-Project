namespace Disney.Mix.SDK
{
	public interface IFriendshipInvitationReceivedPushNotification : IPushNotification
	{
		string FriendshipInvitationId
		{
			get;
		}
	}
}
