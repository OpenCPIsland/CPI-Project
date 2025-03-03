using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.Net.Client;
using ClubPenguin.Net.Offline;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.IO;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class SessionManager
	{
		public bool AutoPause = false;

		private string accessToken;

		private string localPlayerSwid;

		private ISession mixSession;

		private EventDispatcher eventDispatcher;

		public bool IsLoggingOut = false;

		public ILocalUser LocalUser
		{
			get;
			protected set;
		}

		public virtual bool HasSession
		{
			get
			{
				return mixSession != null;
			}
		}

		public SessionManager()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<CPKeyValueDatabaseErrorEvents.CorruptionErrorEvent>(onDeviceDBCorruption);
		}

		public virtual void AddMixSession(ISession mixSession, bool canPrepopulateDisplayName = false)
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
			LocalUser = mixSession.LocalUser;
			localPlayerSwid = LocalUser.Id;
			setLanguage();
			addListeners();
			GameSettings gameSettings = Service.Get<GameSettings>();
			if (!gameSettings.OfflineMode)
			{
				string username = LocalUser.RegistrationProfile.Username;
				if (string.IsNullOrEmpty(username))
				{
					LocalUser.RefreshProfile(delegate
					{
						continueLoginWithOfflineDataCopy();
					});
				}
				else
				{
					continueLoginWithOfflineDataCopy();
				}
			}
			else
			{
				eventDispatcher.DispatchEvent(new SessionEvents.SessionStartedEvent(accessToken, localPlayerSwid));
			}
			if (LocalUser.RegistrationProfile.DisplayNameProposedStatus == DisplayNameProposedStatus.None && !string.IsNullOrEmpty(LocalUser.RegistrationProfile.Username) && canPrepopulateDisplayName)
			{
				LocalUser.ValidateDisplayName(LocalUser.RegistrationProfile.Username, delegate(IValidateDisplayNameResult result)
				{
					if (result.Success)
					{
						AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
						accountFlowData.PreValidatedDisplayNames.Add(LocalUser.RegistrationProfile.Username);
					}
				});
			}
		}

		private void continueLoginWithOfflineDataCopy()
		{
			OfflineDatabase offlineDatabase = Service.Get<OfflineDatabase>();
			string username = LocalUser.RegistrationProfile.Username;
			offlineDatabase.AccessToken = RegistrationProfile.Id(username);
			RegistrationProfile value = offlineDatabase.Read<RegistrationProfile>();
			value.userName = username;
			value.displayName = LocalUser.RegistrationProfile.DisplayName;
			value.firstName = LocalUser.RegistrationProfile.FirstName;
			value.parentEmail = LocalUser.RegistrationProfile.ParentEmail;
			offlineDatabase.Write(value);
			eventDispatcher.DispatchEvent(new SessionEvents.SessionStartedEvent(accessToken, localPlayerSwid));
		}

		private void setLanguage()
		{
			LocalUser.SetLanguagePreference(LocalizationLanguage.GetISOLanguageString(Service.Get<Localizer>().Language), delegate(ISetLangaugePreferenceResult result)
			{
				if (!result.Success)
				{
					Log.LogError(result, "Unable to set Crisp language preference to: " + LocalizationLanguage.GetISOLanguageString(Service.Get<Localizer>().Language));
				}
			});
		}

		public virtual void ReturnToRestorePurchases()
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

		public virtual void Logout(AsynchOnFinishedManifold asynchOnFinishedManifold = null)
		{
			if (mixSession != null)
			{
				Service.Get<ICPSwrveService>().StartTimer("mixsdktimer", "log_out");
				IsLoggingOut = true;
				mixSession.OnAuthenticationLost -= onAuthenticationLost;
				if (asynchOnFinishedManifold != null)
				{
					asynchOnFinishedManifold.AsynchStart();
				}
				mixSession.LogOut(delegate(ISessionLogOutResult result)
				{
					if (result.Success)
					{
						Service.Get<ICPSwrveService>().EndTimer("mixsdktimer", null, "success");
					}
					else
					{
						Service.Get<ICPSwrveService>().EndTimer("mixsdktimer", null, "fail");
						if (!mixSession.IsDisposed)
						{
							mixSession.Dispose();
						}
					}
					clearSession();
					IsLoggingOut = false;
					eventDispatcher.DispatchEvent(default(SessionEvents.SessionEndedEvent));
					eventDispatcher.DispatchEvent(default(SessionEvents.SessionLogoutEvent));
					TechAnalytics.LogNetworkLatency(Service.Get<INetworkServicesManager>().GameServerLatency, Service.Get<INetworkServicesManager>().WebServiceLatency);
					Service.Get<ICPSwrveService>().Action("game.logout", "success");
					if (asynchOnFinishedManifold != null)
					{
						asynchOnFinishedManifold.AsynchFinished();
					}
				});
			}
		}

		public virtual void DisposeSession()
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
			mixSession.OnGuestControllerAccessTokenChanged += onAccessTokenChanged;
			mixSession.OnAuthenticationLost += onAuthenticationLost;
			mixSession.OnTerminated += onSessionTerminated;
			Localizer localizer = Service.Get<Localizer>();
			localizer.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Combine(localizer.TokensUpdated, new Localizer.TokensUpdatedDelegate(setLanguage));
		}

		private void removeListeners()
		{
			if (mixSession != null)
			{
				mixSession.OnGuestControllerAccessTokenChanged -= onAccessTokenChanged;
				mixSession.OnAuthenticationLost -= onAuthenticationLost;
				mixSession.OnTerminated -= onSessionTerminated;
			}
			Localizer localizer = Service.Get<Localizer>();
			localizer.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Remove(localizer.TokensUpdated, new Localizer.TokensUpdatedDelegate(setLanguage));
		}

		public virtual void PauseSession(bool immediately = false)
		{
			if (HasSession)
			{
				eventDispatcher.DispatchEvent(default(SessionEvents.SessionPausingEvent));
				if (immediately)
				{
					mixSession.Pause(delegate
					{
					});
					eventDispatcher.DispatchEvent(default(SessionEvents.SessionPausedEvent));
				}
				else
				{
					mixSession.Pause(delegate(IPauseSessionResult result)
					{
						if (result.Success)
						{
							eventDispatcher.DispatchEvent(default(SessionEvents.SessionPausedEvent));
						}
					});
				}
			}
		}

		public virtual void ResumeSession()
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
				mixSession.Resume(delegate(IResumeSessionResult result)
				{
					if (!result.Success)
					{
						Service.Get<EventDispatcher>().DispatchEvent(default(SessionErrorEvents.NoSessionOnResumeError));
					}
					else
					{
						eventDispatcher.DispatchEvent(default(SessionEvents.SessionResumedEvent));
					}
					Service.Get<LoadingController>().RemoveLoadingSystem(this);
				});
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(SessionErrorEvents.NoSessionOnResumeError));
			}
		}

		private void onAccessTokenChanged(object sender, AbstractGuestControllerAccessTokenChangedEventArgs args)
		{
			accessToken = args.GuestControllerAccessToken;
			eventDispatcher.DispatchEvent(new SessionEvents.AccessTokenUpdatedEvent(accessToken));
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
			LocalUser = null;
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
