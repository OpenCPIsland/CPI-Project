using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractRemoveFriendshipNotificationEventArgs : EventArgs
	{
		public abstract RemoveFriendshipNotification Notification
		{
			get;
			protected set;
		}
	}
}
