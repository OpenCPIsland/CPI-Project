using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractReceivedIncomingFriendInvitationEventArgs : EventArgs
	{
		public IIncomingFriendInvitation Invitation
		{
			get;
			protected set;
		}
	}
}
