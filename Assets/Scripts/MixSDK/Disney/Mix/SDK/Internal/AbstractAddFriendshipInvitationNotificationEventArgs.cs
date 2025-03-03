using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractAddFriendshipInvitationNotificationEventArgs : EventArgs
	{
		public abstract AddFriendshipInvitationNotification Notification
		{
			get;
			protected set;
		}
	}
}
