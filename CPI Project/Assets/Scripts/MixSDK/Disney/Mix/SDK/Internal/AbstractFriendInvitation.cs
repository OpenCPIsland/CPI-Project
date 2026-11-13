using System;

namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractFriendInvitation
	{
		public bool RequestTrust
		{
			get;
			set;
		}

		public long InvitationId
		{
			get;
			private set;
		}

		public string Id
		{
			get;
			private set;
		}

		public bool Sent
		{
			get;
			private set;
		}

		public event EventHandler<AbstractFriendInvitationAcceptedEventArgs> OnAccepted = delegate
		{
		};

		public event EventHandler<AbstractFriendInvitationRejectedEventArgs> OnRejected = delegate
		{
		};

		protected AbstractFriendInvitation(bool requestTrust)
		{
			RequestTrust = requestTrust;
		}

		public void SendComplete(long id)
		{
			Sent = true;
			InvitationId = id;
			Id = IdHasher.HashId(id.ToString());
		}

		public void Accepted(bool trustAccepted, IInternalFriend friend)
		{
			FriendInvitationAcceptedEventArgs e = new FriendInvitationAcceptedEventArgs(trustAccepted, friend);
			this.OnAccepted(this, e);
		}

		public void Rejected()
		{
			FriendInvitationRejectedEventArgs e = new FriendInvitationRejectedEventArgs();
			this.OnRejected(this, e);
		}
	}
}
