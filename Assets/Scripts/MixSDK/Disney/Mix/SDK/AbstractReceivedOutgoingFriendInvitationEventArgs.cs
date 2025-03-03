using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractReceivedOutgoingFriendInvitationEventArgs : EventArgs
	{
		public IOutgoingFriendInvitation Invitation
		{
			get;
			protected set;
		}
	}
}
