using ClubPenguin.Mix;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using SwrveUnity;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Analytics
{
	public class CPSwrvePushNotificationListener : ISwrvePushNotificationListener
	{
		private string serviceType;

		private string token;

		private EventDispatcher eventDispatcher;

		private GameSettings gameSettings;

		public bool IsInPushNotificationMode
		{
			get;
			private set;
		}

		public CPSwrvePushNotificationListener()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			gameSettings = Service.Get<GameSettings>();
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				serviceType = "APNS";
				break;
			case RuntimePlatform.Android:
				serviceType = "GCM";
				break;
			}
			addListeners();
		}

		public void OnTokenRegistered(string token)
		{
			this.token = token;
			if (Service.IsSet<SessionManager>() && Service.Get<SessionManager>().HasSession)
			{
				if (!string.IsNullOrEmpty(token))
				{
					enableMixPushNotifications();
				}
				else
				{
					disableMixPushNotifications();
				}
			}
		}

		private void addListeners()
		{
			if (gameSettings.EnablePushNotifications.Value)
			{
				eventDispatcher.AddListener<SessionEvents.SessionStartedEvent>(onSessionStarted);
				eventDispatcher.AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
				eventDispatcher.AddListener<SessionEvents.SessionResumedEvent>(onSessionResumed);
			}
		}

		private bool onSessionStarted(SessionEvents.SessionStartedEvent evt)
		{
			if (gameSettings.EnablePushNotifications.Value && !string.IsNullOrEmpty(token))
			{
				enableMixPushNotifications();
			}
			return false;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			if (gameSettings.EnablePushNotifications.Value && IsInPushNotificationMode)
			{
				disableMixPushNotifications();
			}
			return false;
		}

		private bool onSessionResumed(SessionEvents.SessionResumedEvent evt)
		{
			if (gameSettings.EnablePushNotifications.Value && !string.IsNullOrEmpty(token))
			{
				enableMixPushNotifications();
			}
			return false;
		}

		private void onNotificationRecieved(IDictionary userData)
		{
			try
			{
				Service.Get<ICPSwrveService>().ActionSingular("PushNotificationRecieved", "game.push_notification", "received");
				if (userData.Contains("payload"))
				{
					Service.Get<SessionManager>().LocalUser.ReceivePushNotification(userData);
				}
			}
			catch (Exception)
			{
				Service.Get<ICPSwrveService>().ActionSingular("PushNotificationRecieved", "game.push_notification", "exception_on_recieve");
			}
		}

		private void enableMixPushNotifications()
		{
			if (Service.Get<SessionManager>().LocalUser != null && !string.IsNullOrEmpty(token))
			{
				string iOSProvisioningId = Service.Get<MixLoginCreateService>().IOSProvisioningId;
				Service.Get<SessionManager>().LocalUser.EnableInvisiblePushNotifications(token, serviceType, iOSProvisioningId, delegate(IEnableInvisiblePushNotificationsResult result)
				{
					if (result.Success)
					{
						IsInPushNotificationMode = true;
						Service.Get<ICPSwrveService>().Action("game.push_notification", "enable_success");
					}
					else
					{
						IsInPushNotificationMode = false;
						Service.Get<ICPSwrveService>().Action("game.push_notification", "enable_failed");
					}
				});
			}
		}

		private void disableMixPushNotifications()
		{
			if (Service.Get<SessionManager>().LocalUser != null && IsInPushNotificationMode)
			{
				Service.Get<SessionManager>().LocalUser.DisableAllPushNotifications(delegate(IDisableAllPushNotificationsResult result)
				{
					if (result.Success)
					{
						IsInPushNotificationMode = false;
						Service.Get<ICPSwrveService>().Action("game.push_notification", "disable_success");
					}
					else
					{
						Service.Get<ICPSwrveService>().Action("game.push_notification", "disable_failed");
					}
				});
			}
		}
	}
}
