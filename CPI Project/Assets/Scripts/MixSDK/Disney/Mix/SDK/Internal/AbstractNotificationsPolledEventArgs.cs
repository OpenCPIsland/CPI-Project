using System;

namespace Disney.Mix.SDK.Internal
{
	public abstract class AbstractNotificationsPolledEventArgs : EventArgs
	{
		public abstract long LastNotificationTimestamp
		{
			get;
			protected set;
		}
	}
}
