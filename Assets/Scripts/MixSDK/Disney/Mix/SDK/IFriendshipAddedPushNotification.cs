namespace Disney.Mix.SDK
{
	public interface IFriendshipAddedPushNotification : IPushNotification
	{
		string FriendId
		{
			get;
		}
	}
}
