namespace Disney.Mix.SDK
{
	public interface ISendFriendInvitationResult
	{
		bool Success
		{
			get;
		}

		IOutgoingFriendInvitation Invitation
		{
			get;
		}
	}
}
