namespace Disney.Mix.SDK
{
	public interface IAcceptFriendInvitationResult
	{
		bool Success
		{
			get;
		}

		IFriend Friend
		{
			get;
		}
	}
}
