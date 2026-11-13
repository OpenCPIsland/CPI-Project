namespace Disney.Mix.SDK.Internal
{
	internal class ReceivedOutgoingFriendInvitationEventArgs : AbstractReceivedOutgoingFriendInvitationEventArgs
	{
		public ReceivedOutgoingFriendInvitationEventArgs(IOutgoingFriendInvitation invitation)
		{
			base.Invitation = invitation;
		}
	}
}
