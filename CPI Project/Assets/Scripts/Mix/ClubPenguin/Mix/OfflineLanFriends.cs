using Disney.Mix.SDK;
using Disney.Mix.SDK.Internal;
using System;

namespace ClubPenguin.Mix
{
	internal class OfflineLanFriend : IFriend
	{
		public IDisplayName DisplayName
		{
			get;
			private set;
		}

		public string FirstName
		{
			get
			{
				return "";
			}
		}

		public string Id
		{
			get;
			private set;
		}

		public string HashedId
		{
			get
			{
				return Id;
			}
		}

		public bool IsTrusted
		{
			get
			{
				return false;
			}
		}

		public AccountStatus Status
		{
			get
			{
				return AccountStatus.Active;
			}
		}

		public OfflineLanFriend(string swid, string name)
		{
			Id = swid;
			DisplayName = new DisplayName(name);
		}
	}

	internal class OfflineLanFriendAcceptedEventArgs : AbstractFriendInvitationAcceptedEventArgs
	{
		public OfflineLanFriendAcceptedEventArgs(IFriend friend)
		{
			base.TrustAccepted = false;
			base.Friend = friend;
		}
	}

	internal class OfflineLanFriendRejectedEventArgs : AbstractFriendInvitationRejectedEventArgs
	{
	}

	internal class OfflineLanReceivedIncomingEventArgs : AbstractReceivedIncomingFriendInvitationEventArgs
	{
		public OfflineLanReceivedIncomingEventArgs(IIncomingFriendInvitation invitation)
		{
			base.Invitation = invitation;
		}
	}

	internal class OfflineLanReceivedOutgoingEventArgs : AbstractReceivedOutgoingFriendInvitationEventArgs
	{
		public OfflineLanReceivedOutgoingEventArgs(IOutgoingFriendInvitation invitation)
		{
			base.Invitation = invitation;
		}
	}

	internal class OfflineLanUnfriendedEventArgs : AbstractUnfriendedEventArgs
	{
		public override IFriend ExFriend
		{
			get;
			protected set;
		}

		public OfflineLanUnfriendedEventArgs(IFriend exFriend)
		{
			ExFriend = exFriend;
		}
	}

	internal class OfflineLanIncomingInvitation : IIncomingFriendInvitation
	{
		public readonly long InvitationId;

		public readonly string Swid;

		public IUnidentifiedUser Inviter
		{
			get;
			private set;
		}

		public ILocalUser Invitee
		{
			get;
			private set;
		}

		public bool RequestTrust
		{
			get
			{
				return false;
			}
		}

		public bool Sent
		{
			get
			{
				return true;
			}
		}

		public string Id
		{
			get
			{
				return InvitationId.ToString();
			}
		}

		public event EventHandler<AbstractFriendInvitationAcceptedEventArgs> OnAccepted = delegate
		{
		};

		public event EventHandler<AbstractFriendInvitationRejectedEventArgs> OnRejected = delegate
		{
		};

		public OfflineLanIncomingInvitation(long id, string swid, string name, ILocalUser invitee)
		{
			InvitationId = id;
			Swid = swid;
			Inviter = new UnidentifiedUser(new DisplayName(name), swid, null);
			Invitee = invitee;
		}

		public void Accepted(IFriend friend)
		{
			this.OnAccepted(this, new OfflineLanFriendAcceptedEventArgs(friend));
		}

		public void Rejected()
		{
			this.OnRejected(this, new OfflineLanFriendRejectedEventArgs());
		}
	}

	internal class OfflineLanOutgoingInvitation : IOutgoingFriendInvitation
	{
		public readonly long InvitationId;

		public readonly string Swid;

		public ILocalUser Inviter
		{
			get;
			private set;
		}

		public IUnidentifiedUser Invitee
		{
			get;
			private set;
		}

		public bool RequestTrust
		{
			get
			{
				return false;
			}
		}

		public bool Sent
		{
			get
			{
				return true;
			}
		}

		public string Id
		{
			get
			{
				return InvitationId.ToString();
			}
		}

		public event EventHandler<AbstractFriendInvitationAcceptedEventArgs> OnAccepted = delegate
		{
		};

		public event EventHandler<AbstractFriendInvitationRejectedEventArgs> OnRejected = delegate
		{
		};

		public OfflineLanOutgoingInvitation(long id, string swid, string name, ILocalUser inviter)
		{
			InvitationId = id;
			Swid = swid;
			Invitee = new UnidentifiedUser(new DisplayName(name), swid, null);
			Inviter = inviter;
		}

		public void Accepted(IFriend friend)
		{
			this.OnAccepted(this, new OfflineLanFriendAcceptedEventArgs(friend));
		}

		public void Rejected()
		{
			this.OnRejected(this, new OfflineLanFriendRejectedEventArgs());
		}
	}
}
