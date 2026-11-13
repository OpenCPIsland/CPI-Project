using System;

namespace Disney.Mix.SDK
{
	public abstract class AbstractFriendInvitationAcceptedEventArgs : EventArgs
	{
		public bool TrustAccepted
		{
			get;
			protected set;
		}

		public IFriend Friend
		{
			get;
			protected set;
		}
	}
}
