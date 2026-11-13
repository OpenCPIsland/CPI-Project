namespace Disney.Mix.SDK.Internal
{
	internal class ReceivedIncomingFriendInvitationEventArgs : AbstractReceivedIncomingFriendInvitationEventArgs
	{
		public ReceivedIncomingFriendInvitationEventArgs(IIncomingFriendInvitation invitation)
		{
			base.Invitation = invitation;
		}
	}
}
