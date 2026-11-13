using System;
using System.Collections;

namespace Disney.Mix.SDK.Internal
{
	public class Session : IInternalSession, ISession, IDisposable
	{
		private readonly AbstractLogger logger;

		private readonly IInternalLocalUser localUser;

		private readonly INotificationPoller notificationPoller;

		private readonly ICoroutineManager coroutineManager;

		private readonly IDatabase database;

		private readonly IUserDatabase userDatabase;

		private readonly IGuestControllerClient guestControllerClient;

		private readonly IMixWebCallFactory mixWebCallFactory;

		private readonly IEnumerator updateEnumerator;

		private readonly IEpochTime epochTime;

		private readonly DatabaseCorruptionHandler databaseCorruptionHandler;

		private readonly ISessionStatus sessionStatus;

		private readonly IKeychain keychain;

		private readonly IGetStateResponseParser getStateResponseParser;

		private readonly string clientVersion;

		private readonly INotificationQueue notificationQueue;

		public ILocalUser LocalUser
		{
			get
			{
				EnsureNotDisposed();
				return localUser;
			}
		}

		public IInternalLocalUser InternalLocalUser
		{
			get
			{
				EnsureNotDisposed();
				return localUser;
			}
		}

		public TimeSpan ServerTimeOffset
		{
			get
			{
				return epochTime.Offset;
			}
		}

		public string GuestControllerAccessToken
		{
			get;
			private set;
		}

		public bool IsDisposed
		{
			get;
			private set;
		}

		public event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost = delegate
		{
		};

		public event EventHandler<AbstractSessionTerminatedEventArgs> OnTerminated = delegate
		{
		};

		public event EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> OnGuestControllerAccessTokenChanged = delegate
		{
		};

		public event EventHandler<AbstractSessionPausedEventArgs> OnPaused = delegate
		{
		};

		public Session(AbstractLogger logger, IInternalLocalUser localUser, string guestControllerAccessToken, bool pushNotificationsEnabled, INotificationPoller notificationPoller, ICoroutineManager coroutineManager, IDatabase database, IUserDatabase userDatabase, IGuestControllerClient guestControllerClient, IMixWebCallFactory mixWebCallFactory, IEpochTime epochTime, DatabaseCorruptionHandler databaseCorruptionHandler, ISessionStatus sessionStatus, IKeychain keychain, IGetStateResponseParser getStateResponseParser, string clientVersion, INotificationQueue notificationQueue)
		{
			this.logger = logger;
			this.localUser = localUser;
			this.notificationPoller = notificationPoller;
			this.coroutineManager = coroutineManager;
			this.database = database;
			this.userDatabase = userDatabase;
			this.guestControllerClient = guestControllerClient;
			this.mixWebCallFactory = mixWebCallFactory;
			this.epochTime = epochTime;
			this.databaseCorruptionHandler = databaseCorruptionHandler;
			this.sessionStatus = sessionStatus;
			this.keychain = keychain;
			this.getStateResponseParser = getStateResponseParser;
			this.clientVersion = clientVersion;
			this.notificationQueue = notificationQueue;
			GuestControllerAccessToken = guestControllerAccessToken;
			guestControllerClient.OnAccessTokenChanged += HandleGuestControllerAccessTokenChanged;
			guestControllerClient.OnAuthenticationLost += HandleAuthenticationLost;
			mixWebCallFactory.OnAuthenticationLost += HandleAuthenticationLost;
			localUser.OnPushNotificationsToggled += HandlePushNotificationsToggled;
			localUser.OnPushNotificationReceived += HandlePushNotificationReceived;
			localUser.OnDisplayNameUpdated += HandleDisplayNameUpdated;
			databaseCorruptionHandler.OnCorruptionDetected += HandleCorruptionDetected;
			updateEnumerator = Update();
			coroutineManager.Start(updateEnumerator);
			notificationPoller.OnNotificationsPolled += HandleNotificationsPolled;
			notificationPoller.UsePollIntervals = !pushNotificationsEnabled;
			this.sessionStatus.IsPaused = true;
			notificationPoller.OnSynchronizationError += HandleNotificationPollerSynchronizationError;
		}

		private IEnumerator Update()
		{
			while (!IsDisposed)
			{
				if (!sessionStatus.IsPaused)
				{
					try
					{
						notificationPoller.Update();
					}
					catch (Exception arg)
					{
						logger.Critical("Exception during Update(): " + arg);
					}
				}
				yield return null;
			}
		}

		public void Pause(Action<IPauseSessionResult> callback)
		{
			EnsureNotDisposed();
			sessionStatus.IsPaused = true;
			sessionStatus.IsPaused = true;
			notificationPoller.Pause();
			PresenceSetter.SetAway(logger, mixWebCallFactory, delegate
			{
				callback(new PauseSessionResult(true));
			}, delegate
			{
				callback(new PauseSessionResult(false));
			});
		}

		public void Resume(Action<IResumeSessionResult> callback)
		{
			EnsureNotDisposed();
			try
			{
				SessionResumer.Resume(logger, getStateResponseParser, epochTime, clientVersion, notificationQueue, mixWebCallFactory, localUser, database, userDatabase, notificationPoller, delegate(IResumeSessionResult result)
				{
					if (result.Success)
					{
						sessionStatus.IsPaused = false;
						notificationPoller.Resume();
					}
					callback(result);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Error while resuming: " + arg);
				callback(new ResumeSessionResult(false));
			}
		}

		public void LogOut(Action<ISessionLogOutResult> callback)
		{
			EnsureNotDisposed();
			database.LogOutSession(localUser.Swid);
			Dispose();
			MixSessionEnder.End(logger, database, keychain, mixWebCallFactory, localUser.Swid, delegate
			{
				callback(new SessionLogOutResult(true));
			}, delegate
			{
				callback(new SessionLogOutResult(false));
			});
		}

		public void Expire(Action<IExpireSessionResult> callback)
		{
			EnsureNotDisposed();
			MixSessionExpirer.Expire(logger, mixWebCallFactory, delegate(bool success)
			{
				callback(new ExpireSessionResult(success));
			});
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				guestControllerClient.OnAccessTokenChanged -= HandleGuestControllerAccessTokenChanged;
				localUser.OnPushNotificationsToggled -= HandlePushNotificationsToggled;
				notificationPoller.OnNotificationsPolled -= HandleNotificationsPolled;
				localUser.OnPushNotificationReceived -= HandlePushNotificationReceived;
				databaseCorruptionHandler.OnCorruptionDetected -= HandleCorruptionDetected;
				notificationPoller.Dispose();
				userDatabase.Dispose();
				mixWebCallFactory.Dispose();
				coroutineManager.Stop(updateEnumerator);
				IsDisposed = true;
			}
		}

		private void HandlePushNotificationsToggled(bool enabled)
		{
			notificationPoller.UsePollIntervals = !enabled;
		}

		private void HandlePushNotificationReceived(bool notificationsAvailable)
		{
			if (notificationsAvailable)
			{
				notificationPoller.RequestPoll();
			}
		}

		private void HandleCorruptionDetected(object sender, CorruptionDetectedEventArgs e)
		{
			Dispose();
			this.OnTerminated(this, new LocalStorageCorruptedEventArgs(e.Recovered));
		}

		private void HandleNotificationPollerSynchronizationError(object sender, AbstractNotificationPollerSynchronizationErrorEventArgs e)
		{
			sessionStatus.IsPaused = true;
			Resume(delegate(IResumeSessionResult resumeResult)
			{
				if (!resumeResult.Success)
				{
					sessionStatus.IsPaused = true;
					notificationPoller.Pause();
					this.OnPaused(this, new SessionPausedEventArgs());
				}
			});
		}

		private void HandleNotificationsPolled(object sender, AbstractNotificationsPolledEventArgs e)
		{
			database.UpdateSessionDocument(localUser.Swid, delegate(SessionDocument doc)
			{
				doc.LastNotificationTime = e.LastNotificationTimestamp;
			});
		}

		private void HandleGuestControllerAccessTokenChanged(object sender, AbstractGuestControllerAccessTokenChangedEventArgs e)
		{
			GuestControllerAccessToken = e.GuestControllerAccessToken;
			this.OnGuestControllerAccessTokenChanged(this, e);
		}

		private void HandleAuthenticationLost(object sender, AbstractAuthenticationLostEventArgs e)
		{
			if (e is AbstractAuthenticationRevokedEventArgs && localUser.RegistrationProfile.AccountStatus == AccountStatus.AwaitingParentalConsent)
			{
				this.OnAuthenticationLost(this, new AuthenticationRequiresParentalConsentArgs());
			}
			else
			{
				this.OnAuthenticationLost(this, e);
			}
			database.LogOutSession(localUser.Swid);
			Dispose();
		}

		private void HandleDisplayNameUpdated(string displayName)
		{
			database.UpdateSessionDocument(localUser.Swid, delegate(SessionDocument doc)
			{
				doc.DisplayNameText = displayName;
				doc.ProposedDisplayNameStatus = "ACCEPTED";
			});
		}

		private void EnsureNotDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Session", "Session is disposed");
			}
		}
	}
}
