namespace Disney.Mix.SDK.Internal
{
	public interface IInternalOutgoingFriendInvitation : IOutgoingFriendInvitation
	{
		new bool RequestTrust
		{
			get;
			set;
		}

		IInternalLocalUser InternalInviter
		{
			get;
		}

		IInternalUnidentifiedUser InternalInvitee
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
