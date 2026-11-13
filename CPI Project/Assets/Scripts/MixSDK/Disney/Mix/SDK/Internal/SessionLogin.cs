using DeviceDB;
using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class SessionLogin : ISessionLogin
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClientFactory guestControllerClientFactory;

		private readonly IMixSessionStarter mixSessionStarter;

		private readonly IDatabase database;

		private readonly ILegalMarketingErrorsBuilder legalMarketingErrorsBuilder;

		private readonly ISessionFactory sessionFactory;

		public SessionLogin(AbstractLogger logger, IGuestControllerClientFactory guestControllerClientFactory, IMixSessionStarter mixSessionStarter, IDatabase database, ILegalMarketingErrorsBuilder legalMarketingErrorsBuilder, ISessionFactory sessionFactory)
		{
			this.logger = logger;
			this.guestControllerClientFactory = guestControllerClientFactory;
			this.mixSessionStarter = mixSessionStarter;
			this.database = database;
			this.legalMarketingErrorsBuilder = legalMarketingErrorsBuilder;
			this.sessionFactory = sessionFactory;
		}

		public void Login(string username, string password, Action<ILoginResult> callback)
		{
			try
			{
				database.ClearServerTimeOffsetMillis();
				IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create("NoSwid");
				guestControllerClient.LogIn(new LogInRequest
				{
					loginValue = username,
					password = password
				}, delegate(GuestControllerResult<LogInResponse> r)
				{
					if (!r.Success)
					{
						logger.Error("Login failed");
						callback(new LoginFailedAuthenticationResult());
					}
					else
					{
						HandleLoginSuccess(r, callback);
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new LoginResult(false, null));
			}
		}

		private void HandleLoginSuccess(GuestControllerResult<LogInResponse> result, Action<ILoginResult> callback)
		{
			try
			{
				LogInResponse response = result.Response;
				LogInData data = response.data;
				GuestApiErrorCollection gcErrorCollection = response.error;
				ILoginResult loginResult = GuestControllerErrorParser.GetLoginResult(gcErrorCollection);
				bool flag = false;
				string hallPassToken = string.Empty;
				string swid = string.Empty;
				if (data != null || loginResult == null)
				{
					goto IL_015d;
				}
				if (loginResult is ILoginFailedParentalConsentResult)
				{
					foreach (GuestApiError error in gcErrorCollection.errors)
					{
						TemporaryToken data2 = error.data;
						if (data2 != null)
						{
							flag = true;
							hallPassToken = data2.accessToken;
							swid = data2.swid;
							break;
						}
					}
					if (flag)
					{
						goto IL_015d;
					}
					callback(loginResult);
				}
				else
				{
					callback(loginResult);
				}
				goto end_IL_0018;
				IL_015d:
				if (data == null && !flag)
				{
					if (gcErrorCollection != null)
					{
						logger.Critical("Received unhandled error exception: " + JsonParser.ToJson(gcErrorCollection));
					}
					callback(new LoginResult(false, null));
				}
				else if (flag)
				{
					database.StoreSession(swid, hallPassToken, null, null, null, null, null, null, null, null, false, null);
					IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create(swid);
					ProfileGetter.GetProfile(logger, guestControllerClient, delegate(ProfileData profileData)
					{
						if (profileData == null)
						{
							database.DeleteSession(swid);
							callback(new LoginFailedParentalConsentResult());
						}
						else
						{
							StoreSession(swid, hallPassToken, null, profileData.etag, profileData.displayName, profileData.profile);
							HandleRefreshProfileSuccess(callback, loginResult, gcErrorCollection, profileData.profile, profileData.displayName, profileData.marketing, swid, hallPassToken);
						}
					});
				}
				else if (!ValidateLogInData(data))
				{
					logger.Critical("Error parsing the login data:" + JsonParser.ToJson(data));
					callback(new LoginResult(false, null));
				}
				else
				{
					Token token = data.token;
					StoreSession(token.swid, token.access_token, token.refresh_token, data.etag, data.displayName, data.profile);
					HandleRefreshProfileSuccess(callback, loginResult, gcErrorCollection, data.profile, data.displayName, data.marketing, token.swid, token.access_token);
				}
				end_IL_0018:;
			}
			catch (CorruptionException arg)
			{
				logger.Fatal("Corruption detected during login: " + arg);
				callback(new LoginCorruptionDetectedResult());
			}
			catch (Exception arg2)
			{
				logger.Critical("Unhandled exception: " + arg2);
				callback(new LoginResult(false, null));
			}
		}

		private void StoreSession(string swid, string accessToken, string refreshToken, string etag, Disney.Mix.SDK.Internal.GuestControllerDomain.DisplayName displayName, Profile profile)
		{
			string countryCode = GuestControllerUtils.GetCountryCode(profile);
			database.StoreSession(swid, accessToken, refreshToken, displayName.displayName, profile.firstName, etag, profile.ageBand, displayName.proposedDisplayName, displayName.proposedStatus, profile.status, true, countryCode);
		}

		private void HandleRefreshProfileSuccess(Action<ILoginResult> callback, ILoginResult loginResult, GuestApiErrorCollection gcErrorCollection, Profile profile, Disney.Mix.SDK.Internal.GuestControllerDomain.DisplayName displayName, IEnumerable<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing, string swid, string accessToken)
		{
			mixSessionStarter.Start(swid, accessToken, delegate
			{
				HandleMixSessionStartSuccess(callback, loginResult, swid, gcErrorCollection, profile, displayName, marketing);
			}, delegate
			{
				callback(new LoginResult(false, null));
			});
		}

		private void HandleMixSessionStartSuccess(Action<ILoginResult> callback, ILoginResult loginResult, string swid, GuestApiErrorCollection gcErrorCollection, Profile profile, Disney.Mix.SDK.Internal.GuestControllerDomain.DisplayName displayName, IEnumerable<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing)
		{
			try
			{
				IInternalSession session = sessionFactory.Create(swid);
				IInternalLocalUser internalLocalUser = session.InternalLocalUser;
				internalLocalUser.InternalRegistrationProfile.Update(profile, displayName, marketing);
				session.Resume(delegate(IResumeSessionResult r)
				{
					HandleOfflineSessionResumed(r, session, loginResult, gcErrorCollection, callback);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Error creating session: " + arg);
				callback(new LoginResult(false, null));
			}
		}

		private void HandleOfflineSessionResumed(IResumeSessionResult result, ISession session, ILoginResult loginResult, GuestApiErrorCollection gcErrorCollection, Action<ILoginResult> callback)
		{
			if (!result.Success)
			{
				callback(new LoginResult(false, null));
			}
			else if (loginResult is ILoginMissingInfoResult)
			{
				callback(new LoginMissingInfoResult(true, session));
			}
			else if (loginResult is ILoginSecurityUpdateResult)
			{
				callback(new LoginSecurityUpdateResult(true, session));
			}
			else if (loginResult is ILoginRequiresLegalMarketingUpdateResult)
			{
				legalMarketingErrorsBuilder.BuildErrors(session, gcErrorCollection, delegate(BuildLegalMarketingErrorsResult response)
				{
					callback(new LoginRequiresLegalMarketingUpdateResult(true, session, response.LegalDocuments, response.Marketing));
				}, delegate
				{
					callback(new LoginResult(false, null));
				});
			}
			else
			{
				callback(new LoginResult(true, session));
			}
		}

		private static bool ValidateLogInData(LogInData data)
		{
			return data.displayName != null && data.etag != null && data.profile != null && data.token != null && data.token.access_token != null;
		}
	}
}
