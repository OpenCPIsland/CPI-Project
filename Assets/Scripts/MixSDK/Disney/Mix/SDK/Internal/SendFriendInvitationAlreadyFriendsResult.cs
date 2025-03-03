namespace Disney.Mix.SDK.Internal
{
	public class SendFriendInvitationAlreadyFriendsResult : SendFriendInvitationResult, ISendFriendInvitationAlreadyFriendsResult
	{
		public SendFriendInvitationAlreadyFriendsResult(bool success, IOutgoingFriendInvitation invitation)
			: base(success, invitation)
		{
		}
	}
}
