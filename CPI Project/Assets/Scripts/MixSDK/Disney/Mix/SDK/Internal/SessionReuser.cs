using DeviceDB;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class SessionReuser : ISessionReuser
	{
		private readonly AbstractLogger logger;

		private readonly IDatabase database;

		private readonly IMixSessionStarter mixSessionStarter;

		private readonly ISessionFactory sessionFactory;

		public SessionReuser(AbstractLogger logger, IDatabase database, IMixSessionStarter mixSessionStarter, ISessionFactory sessionFactory)
		{
			this.logger = logger;
			this.database = database;
			this.mixSessionStarter = mixSessionStarter;
			this.sessionFactory = sessionFactory;
		}

		public void Reuse(string swid, string accessToken, string refreshToken, string displayName, string proposedDisplayName, string proposedDisplayNameStatus, string firstName, string etag, string ageBand, string accountStatus, string countryCode, Action<IReuseExistingGuestControllerLoginResult> callback)
		{
			try
			{
				database.ClearServerTimeOffsetMillis();
				database.StoreSession(swid, accessToken, refreshToken, displayName, firstName, etag, ageBand, proposedDisplayName, proposedDisplayNameStatus, accountStatus, false, countryCode);
				mixSessionStarter.Start(swid, accessToken, delegate
				{
					HandleMixSessionStartSuccess(swid, callback);
				}, delegate
				{
					callback(new ReuseExistingGuestControllerLoginResult(false, null));
				});
			}
			catch (CorruptionException arg)
			{
				logger.Fatal("Corruption detected during session reuse: " + arg);
				callback(new ReuseExistingGuestControllerLoginCorruptionDetectedResult());
			}
			catch (Exception arg2)
			{
				logger.Critical("Unhandled exception: " + arg2);
				callback(new ReuseExistingGuestControllerLoginResult(false, null));
			}
		}

		private void HandleMixSessionStartSuccess(string swid, Action<IReuseExistingGuestControllerLoginResult> callback)
		{
			try
			{
				IInternalSession session = sessionFactory.Create(swid);
				session.Resume(delegate(IResumeSessionResult r)
				{
					HandleOfflineSessionResumed(r, session, callback);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Error creating session: " + arg);
				callback(new ReuseExistingGuestControllerLoginResult(false, null));
			}
		}

		private static void HandleOfflineSessionResumed(IResumeSessionResult result, ISession session, Action<IReuseExistingGuestControllerLoginResult> callback)
		{
			callback(result.Success ? new ReuseExistingGuestControllerLoginResult(true, session) : new ReuseExistingGuestControllerLoginResult(false, null));
		}
	}
}
