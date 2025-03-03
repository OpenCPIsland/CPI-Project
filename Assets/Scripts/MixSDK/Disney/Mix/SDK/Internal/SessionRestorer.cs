using DeviceDB;
using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class SessionRestorer : ISessionRestorer
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClientFactory guestControllerClientFactory;

		private readonly IDatabase database;

		private readonly ISessionFactory sessionFactory;

		public SessionRestorer(AbstractLogger logger, IGuestControllerClientFactory guestControllerClientFactory, IDatabase database, ISessionFactory sessionFactory)
		{
			this.logger = logger;
			this.guestControllerClientFactory = guestControllerClientFactory;
			this.database = database;
			this.sessionFactory = sessionFactory;
		}

		public void RestoreLastSession(Action<IRestoreLastSessionResult> callback)
		{
			try
			{
				database.ClearServerTimeOffsetMillis();
				SessionDocument lastSessionDoc = database.GetLastLoggedInSessionDocument();
				if (lastSessionDoc == null)
				{
					callback(new RestoreLastSessionNotFoundResult());
				}
				else
				{
					IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create(lastSessionDoc.Swid);
					guestControllerClient.Refresh(delegate(GuestControllerResult<RefreshResponse> r)
					{
						if (r.Response == null)
						{
							logger.Error("Error refreshing auth token");
							callback(new RestoreLastSessionResult(false, null));
						}
						else
						{
							HandleRefreshSuccess(callback, r, lastSessionDoc);
						}
					});
				}
			}
			catch (CorruptionException arg)
			{
				logger.Fatal("Corruption detected during session restoration: " + arg);
				callback(new RestoreLastSessionCorruptionDetectedResult());
			}
			catch (Exception arg2)
			{
				logger.Critical("Unhandled exception: " + arg2);
				callback(new RestoreLastSessionResult(false, null));
			}
		}

		private void HandleRefreshSuccess(Action<IRestoreLastSessionResult> callback, GuestControllerResult<RefreshResponse> result, SessionDocument lastSessionDoc)
		{
			try
			{
				GuestApiErrorCollection error2 = result.Response.error;
				RefreshData data = result.Response.data;
				IRestoreLastSessionResult error = GuestControllerErrorParser.GetRestoreLastSessionResult(error2);
				if (data == null && error != null)
				{
					if (error is IRestoreLastSessionFailedInvalidOrExpiredTokenResult && lastSessionDoc.AccountStatus == "AWAIT_PARENT_CONSENT")
					{
						callback(new RestoreLastSessionFailedParentalConsentResult());
					}
					else
					{
						callback(error);
					}
				}
				else if (data == null)
				{
					if (error2 != null)
					{
						logger.Critical("Received unhandled error exception:\n" + JsonParser.ToJson(error2) + "\nResponse headers:\n" + string.Join("\n", result.ResponseHeaders.Select((KeyValuePair<string, string> h) => h.Key + ": " + h.Value).ToArray()));
					}
					callback(new RestoreLastSessionResult(false, null));
				}
				else if (!ValidateRefreshData(data))
				{
					logger.Critical("Error parsing the refresh data: " + JsonParser.ToJson(data));
					callback(new RestoreLastSessionResult(false, null));
				}
				else
				{
					Token token = data.token;
					lastSessionDoc.GuestControllerAccessToken = token.access_token;
					lastSessionDoc.GuestControllerEtag = data.etag;
					database.UpdateGuestControllerToken(token, data.etag);
					try
					{
						IInternalSession session = sessionFactory.Create(lastSessionDoc.Swid);
						session.Resume(delegate(IResumeSessionResult r)
						{
							HandleOfflineSessionResumed(r, session, error, callback);
						});
					}
					catch (Exception arg)
					{
						logger.Critical("Error creating session: " + arg);
						callback(new RestoreLastSessionResult(false, null));
					}
				}
			}
			catch (CorruptionException arg2)
			{
				logger.Fatal("Corruption detected during session restoration: " + arg2);
				callback(new RestoreLastSessionCorruptionDetectedResult());
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new RestoreLastSessionResult(false, null));
			}
		}

		private static void HandleOfflineSessionResumed(IResumeSessionResult result, ISession session, IRestoreLastSessionResult error, Action<IRestoreLastSessionResult> callback)
		{
			if (!result.Success)
			{
				callback(new RestoreLastSessionResult(false, null));
			}
			else if (error is IRestoreLastSessionSuccessMissingInfoResult)
			{
				callback(new RestoreLastSessionSuccessMissingInfoResult(true, session));
			}
			else if (error is IRestoreLastSessionSuccessRequiresLegalMarketingUpdateResult)
			{
				callback(new RestoreLastSessionSuccessRequiresLegalMarketingUpdateResult(true, session));
			}
			else
			{
				callback(new RestoreLastSessionResult(true, session));
			}
		}

		private static bool ValidateRefreshData(RefreshData data)
		{
			return data.token != null && data.token.access_token != null;
		}
	}
}
