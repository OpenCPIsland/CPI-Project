namespace Disney.Mix.SDK.Internal.MixDomain
{
	public class AddFriendshipNotification : BaseNotification
	{
		public User Friend;

		public long? FriendshipInvitationId;

		public bool? IsTrusted;
	}
}
