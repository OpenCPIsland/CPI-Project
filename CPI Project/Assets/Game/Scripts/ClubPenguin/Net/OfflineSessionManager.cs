using ClubPenguin.Mix;
using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class OfflineSessionManager : SessionManager
	{
		private string accessToken;

		private string localPlayerSwid;

		private ISession mixSession;

		private EventDispatcher eventDispatcher;

		public override bool HasSession
		{
			get
			{
				return mixSession != null;
			}
		}

		public OfflineSessionManager()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<CPKeyValueDatabaseErrorEvents.CorruptionErrorEvent>(onDeviceDBCorruption);
		}

		public override void AddMixSession(ISession mixSession, bool canPrepopulateDisplayName = false)
		{
			if (mixSession == null)
			{
				throw new ArgumentException("Mix Session cannot be null!");
			}
			if (this.mixSession != null)
			{
				clearSession();
			}
			this.mixSession = mixSession;
			accessToken = mixSession.GuestControllerAccessToken;
			base.LocalUser = mixSession.LocalUser;
			localPlayerSwid = base.LocalUser.Id;
			addListeners();
			eventDispatcher.DispatchEvent(new SessionEvents.SessionStartedEvent(accessToken, localPlayerSwid));
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.PreValidatedDisplayNames.Add(base.LocalUser.RegistrationProfile.Username);
		}

		public override void ReturnToRestorePurchases()
		{
			if (HasSession)
			{
				eventDispatcher.DispatchEvent(default(SessionEvents.ReturnToRestorePurchases));
			}
			else
			{
				Log.LogError(this, "Game cannot be started without a session");
			}
		}

		public override void Logout(AsynchOnFinishedManifold asynchOnFinishedManifold = null)
		{
			if (mixSession != null)
			{
				IsLoggingOut = true;
				mixSession.OnAuthenticationLost -= onAuthenticationLost;
				if (asynchOnFinishedManifold != null)
				{
					asynchOnFinishedManifold.AsynchStart();
				}
				CoroutineRunner.StartPersistent(finishLogout(asynchOnFinishedManifold), this, "Finish Logout");
			}
		}

		private IEnumerator finishLogout(AsynchOnFinishedManifold asynchOnFinishedManifold)
		{
			yield return null;
			if (!mixSession.IsDisposed)
			{
				mixSession.Dispose();
			}
			clearSession();
			IsLoggingOut = false;
			eventDispatcher.DispatchEvent(default(SessionEvents.SessionEndedEvent));
			eventDispatcher.DispatchEvent(default(SessionEvents.SessionLogoutEvent));
			if (asynchOnFinishedManifold != null)
			{
				asynchOnFinishedManifold.AsynchFinished();
			}
		}

		public override void DisposeSession()
		{
			if (mixSession != null)
			{
				removeListeners();
				mixSession.Dispose();
			}
			clearSession();
			IsLoggingOut = false;
			eventDispatcher.DispatchEvent(default(SessionEvents.SessionEndedEvent));
		}

		private void addListeners()
		{
			mixSession.OnAuthenticationLost += onAuthenticationLost;
			mixSession.OnTerminated += onSessionTerminated;
		}

		private void removeListeners()
		{
			if (mixSession != null)
			{
				mixSession.OnAuthenticationLost -= onAuthenticationLost;
				mixSession.OnTerminated -= onSessionTerminated;
			}
		}

		public override void PauseSession(bool immediately = false)
		{
			if (HasSession)
			{
				eventDispatcher.DispatchEvent(default(SessionEvents.SessionPausingEvent));
				if (immediately)
				{
					eventDispatcher.DispatchEvent(default(SessionEvents.SessionPausedEvent));
				}
				else
				{
					CoroutineRunner.StartPersistent(delayPause(), this, "delay pause");
				}
			}
		}

		private IEnumerator delayPause()
		{
			yield return null;
			eventDispatcher.DispatchEvent(default(SessionEvents.SessionPausedEvent));
		}

		public override void ResumeSession()
		{
			if (HasSession)
			{
				if (Service.Get<ConnectionManager>().ConnectionState == ConnectionManager.NetworkConnectionState.NoConnection)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(SessionErrorEvents.NoNetworkOnResumeError));
					return;
				}
				LoadingController loadingController = Service.Get<LoadingController>();
				if (!loadingController.HasLoadingSystem(this))
				{
					loadingController.AddLoadingSystem(this);
				}
				CoroutineRunner.StartPersistent(delayResume(), this, "delay resume");
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(SessionErrorEvents.NoSessionOnResumeError));
			}
		}

		private IEnumerator delayResume()
		{
			yield return null;
			eventDispatcher.DispatchEvent(default(SessionEvents.SessionResumedEvent));
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
		}

		private void onAuthenticationLost(object sender, AbstractAuthenticationLostEventArgs args)
		{
			eventDispatcher.DispatchEvent(default(SessionEvents.AuthenticationLostEvent));
			if (args is AbstractAccountBannedEventArgs)
			{
				AbstractAccountBannedEventArgs abstractAccountBannedEventArgs = args as AbstractAccountBannedEventArgs;
				eventDispatcher.DispatchEvent(new SessionErrorEvents.AccountBannedEvent(AlertType.Unknown, abstractAccountBannedEventArgs.ExpirationDate));
			}
			else if (args is AbstractAuthenticationRequiresParentalConsentArgs)
			{
				eventDispatcher.DispatchEvent(default(SessionErrorEvents.AuthenticationRequiresParentalConsentEvent));
			}
			else if (args is AbstractAuthenticationRevokedEventArgs)
			{
				eventDispatcher.DispatchEvent(default(SessionErrorEvents.AuthenticationRevokedEvent));
			}
			else if (args is AbstractAuthenticationUnavailableEventArgs)
			{
				eventDispatcher.DispatchEvent(default(SessionErrorEvents.AuthenticationUnavailableEvent));
			}
			clearSession();
		}

		private void onSessionTerminated(object sender, AbstractSessionTerminatedEventArgs args)
		{
			if (args is AbstractSynchronizationErrorEventArgs)
			{
				eventDispatcher.DispatchEvent(default(SessionErrorEvents.SessionTerminated));
			}
			else if (args is AbstractLocalStorageCorruptedEventArgs)
			{
				resolveDeviceDBCorruption((args as AbstractLocalStorageCorruptedEventArgs).Recovered);
			}
			clearSession();
		}

		private void clearSession()
		{
			removeListeners();
			mixSession = null;
			base.LocalUser = null;
			accessToken = null;
			localPlayerSwid = null;
			Service.Get<MixLoginCreateService>().ResetUpdateAgeBand();
		}

		private bool onDeviceDBCorruption(CPKeyValueDatabaseErrorEvents.CorruptionErrorEvent evt)
		{
			resolveDeviceDBCorruption(evt.Recovered);
			return false;
		}

		private void resolveDeviceDBCorruption(bool recovered)
		{
			Log.LogError(this, "DeviceDB Corruption and Recovery");
			if (HasSession)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new SessionErrorEvents.SessionDataCorrupted(recovered));
				Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
				return;
			}
			resetDeviceDB();
			if (Service.IsSet<INetworkServicesManager>())
			{
				Service.Get<INetworkServicesManager>().Reset();
			}
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			resetDeviceDB();
			if (Service.IsSet<INetworkServicesManager>())
			{
				Service.Get<INetworkServicesManager>().Reset();
			}
			return false;
		}

		private void resetDeviceDB()
		{
			if (HasSession)
			{
				return;
			}
			try
			{
				Service.Get<KeyChainManager>().RemoveString("SessionUnlockKey");
			}
			catch
			{
			}
			string[] array = new string[2]
			{
				"/MixSDK/",
				"/KeyValueDatabase"
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (Directory.Exists(Application.persistentDataPath + array[i]))
				{
					Directory.Delete(Application.persistentDataPath + array[i], true);
				}
			}
		}
	}
}
