using System.Collections;

namespace Disney.PushNotificationUnityPlugin.Internal
{
	public class NotificationReceivedEventArgs : AbstractNotificationReceivedEventArgs
	{
		public override IDictionary UserData
		{
			get;
			protected set;
		}

		public NotificationReceivedEventArgs(IDictionary userData)
		{
			UserData = userData;
		}
	}
}
