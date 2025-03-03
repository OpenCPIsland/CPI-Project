namespace Disney.Mix.SDK.Internal
{
	public class IncomingFriendInvitation : AbstractFriendInvitation, IInternalIncomingFriendInvitation, IIncomingFriendInvitation
	{
		private readonly IInternalUnidentifiedUser inviter;

		private readonly IInternalLocalUser invitee;

		public IUnidentifiedUser Inviter
		{
			get
			{
				return inviter;
			}
		}

		public ILocalUser Invitee
		{
			get
			{
				return invitee;
			}
		}

		public IInternalUnidentifiedUser InternalInviter
		{
			get
			{
				return inviter;
			}
		}

		public IInternalLocalUser InternalInvitee
		{
			get
			{
				return invitee;
			}
		}

		public IncomingFriendInvitation(IInternalUnidentifiedUser inviter, IInternalLocalUser invitee, bool requestTrust)
			: base(requestTrust)
		{
			this.inviter = inviter;
			this.invitee = invitee;
		}
	}
}
