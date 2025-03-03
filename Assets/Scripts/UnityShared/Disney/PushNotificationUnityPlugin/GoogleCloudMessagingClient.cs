using System;

namespace Disney.PushNotificationUnityPlugin
{
	public class GoogleCloudMessagingClient : IPushNotificationClient
	{
		public event EventHandler<AbstractNotificationReceivedEventArgs> OnNotificationReceived = delegate
		{
		};

		public event EventHandler<AbstractTokenGeneratedEventArgs> OnTokenGenerated = delegate
		{
		};

		public GoogleCloudMessagingClient(string senderId)
		{
			throw new PlatformNotSupportedException("Can't use GoogleCloudMessagingClient except on Android");
		}

		public void Register()
		{
		}

		public void CheckForToken()
		{
		}

		public void Unregister()
		{
		}

		public void Update()
		{
		}

		public void OnPause()
		{
		}

		public void OnResume()
		{
		}

		public bool IsNotificationEnabled()
		{
			return true;
		}
	}
}
