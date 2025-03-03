using System;

namespace Disney.PushNotificationUnityPlugin
{
	public class ApplePushNotificationServiceClient : IPushNotificationClient
	{
		public event EventHandler<AbstractNotificationReceivedEventArgs> OnNotificationReceived = delegate
		{
		};

		public event EventHandler<AbstractTokenGeneratedEventArgs> OnTokenGenerated = delegate
		{
		};

		public ApplePushNotificationServiceClient()
		{
			throw new PlatformNotSupportedException("APNs not supported on non-iOS platforms");
		}

		public void Register()
		{
		}

		public void Unregister()
		{
		}

		public void CheckForToken()
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
