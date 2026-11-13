using System;

namespace Disney.PushNotificationUnityPlugin
{
	public interface IPushNotificationClient
	{
		event EventHandler<AbstractNotificationReceivedEventArgs> OnNotificationReceived;

		event EventHandler<AbstractTokenGeneratedEventArgs> OnTokenGenerated;

		void Register();

		void CheckForToken();

		void Unregister();

		void Update();

		void OnPause();

		void OnResume();

		bool IsNotificationEnabled();
	}
}
