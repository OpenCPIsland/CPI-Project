using System;
using System.Collections;

namespace Disney.PushNotificationUnityPlugin
{
	public abstract class AbstractNotificationReceivedEventArgs : EventArgs
	{
		public abstract IDictionary UserData
		{
			get;
			protected set;
		}
	}
}
