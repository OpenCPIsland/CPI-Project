using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractRemoveFriendshipTrustNotificationEventArgs : EventArgs
	{
		public abstract RemoveFriendshipTrustNotification Notification
		{
			get;
			protected set;
		}
	}
}
