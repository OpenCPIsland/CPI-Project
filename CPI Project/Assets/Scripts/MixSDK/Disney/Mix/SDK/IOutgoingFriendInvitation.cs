using System;

namespace Disney.Mix.SDK
{
	public interface IOutgoingFriendInvitation
	{
		ILocalUser Inviter
		{
			get;
		}

		IUnidentifiedUser Invitee
		{
			get;
		}

		bool RequestTrust
		{
			get;
		}

		bool Sent
		{
			get;
		}

		string Id
		{
			get;
		}

		event EventHandler<AbstractFriendInvitationAcceptedEventArgs> OnAccepted;

		event EventHandler<AbstractFriendInvitationRejectedEventArgs> OnRejected;
	}
}
