namespace Disney.Mix.SDK.Internal
{
	public interface IInternalIncomingFriendInvitation : IIncomingFriendInvitation
	{
		new bool RequestTrust
		{
			get;
			set;
		}

		IInternalUnidentifiedUser InternalInviter
		{
			get;
		}

		IInternalLocalUser InternalInvitee
		{
			get;
		}

		long InvitationId
		{
			get;
		}

		void SendComplete(long id);

		void Accepted(bool trustAccepted, IInternalFriend friend);

		void Rejected();
	}
}
