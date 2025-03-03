using System;

namespace Disney.PushNotificationUnityPlugin
{
	public abstract class AbstractTokenGeneratedEventArgs : EventArgs
	{
		public abstract string Token
		{
			get;
			protected set;
		}
	}
}
