namespace Disney.Mix.SDK.Internal
{
	public class OutgoingFriendInvitation : AbstractFriendInvitation, IInternalOutgoingFriendInvitation, IOutgoingFriendInvitation
	{
		private readonly IInternalLocalUser inviter;

		private readonly IInternalUnidentifiedUser invitee;

		public ILocalUser Inviter
		{
			get
			{
				return inviter;
			}
		}

		public IUnidentifiedUser Invitee
		{
			get
			{
				return invitee;
			}
		}

		public IInternalLocalUser InternalInviter
		{
			get
			{
				return inviter;
			}
		}

		public IInternalUnidentifiedUser InternalInvitee
		{
			get
			{
				return invitee;
			}
		}

		public OutgoingFriendInvitation(IInternalLocalUser inviter, IInternalUnidentifiedUser invitee, bool requestTrust)
			: base(requestTrust)
		{
			this.inviter = inviter;
			this.invitee = invitee;
		}
	}
}
