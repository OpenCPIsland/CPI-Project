using DeviceDB;
using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class SessionRegister : ISessionRegister
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClientFactory guestControllerClientFactory;

		private readonly IDatabase database;

		private readonly IMixSessionStarter mixSessionStarter;

		private readonly ISessionFactory sessionFactory;

		public SessionRegister(AbstractLogger logger, IGuestControllerClientFactory guestControllerClientFactory, IDatabase database, IMixSessionStarter mixSessionStarter, ISessionFactory sessionFactory)
		{
			this.logger = logger;
			this.guestControllerClientFactory = guestControllerClientFactory;
			this.database = database;
			this.mixSessionStarter = mixSessionStarter;
			this.sessionFactory = sessionFactory;
		}

		public void Register(bool isTestProfile, string username, string password, string firstName, string lastName, string email, string parentEmail, string languagePreference, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback)
		{
			try
			{
				database.ClearServerTimeOffsetMillis();
				List<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing = marketingAgreements.Select((KeyValuePair<IMarketingItem, bool> p) => new Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem
				{
					code = p.Key.Id,
					subscribed = p.Value
				}).ToList();
				object obj;
				if (!dateOfBirth.HasValue)
				{
					obj = new BaseRegisterProfile();
				}
				else
				{
					RegisterProfile registerProfile = new RegisterProfile();
					registerProfile.dateOfBirth = dateOfBirth.Value.ToString("yyyy-MM-dd");
					obj = registerProfile;
				}
				BaseRegisterProfile baseRegisterProfile = (BaseRegisterProfile)obj;
				baseRegisterProfile.testProfileFlag = (isTestProfile ? "Y" : "N");
				baseRegisterProfile.username = username;
				baseRegisterProfile.firstName = firstName;
				baseRegisterProfile.lastName = lastName;
				baseRegisterProfile.email = email;
				baseRegisterProfile.parentEmail = parentEmail;
				baseRegisterProfile.languagePreference = languagePreference;
				baseRegisterProfile.region = assertedCountry;
				BaseRegisterProfile profile = baseRegisterProfile;
				List<string> legalAssertions = acceptedLegalDocuments.Select((ILegalDocument doc) => doc.Id).ToList();
				RegisterRequest registerRequest = new RegisterRequest();
				registerRequest.password = password;
				registerRequest.profile = profile;
				registerRequest.marketing = marketing;
				registerRequest.legalAssertions = legalAssertions;
				RegisterRequest request = registerRequest;
				IGuestControllerClient guestControllerClient = guestControllerClientFactory.Create("NoSwid");
				guestControllerClient.Register(request, delegate(GuestControllerResult<LogInResponse> r)
				{
					HandleRegisterSuccess(r, marketing, callback);
				});
			}
			catch (CorruptionException arg)
			{
				logger.Fatal("Corruption detected during registration: " + arg);
				callback(new RegisterCorruptionDetectedResult());
			}
			catch (Exception arg2)
			{
				logger.Critical("Unhandled exception: " + arg2);
				callback(new RegisterResult(false, null, null));
			}
		}

		private void HandleRegisterSuccess(GuestControllerResult<LogInResponse> result, IEnumerable<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing, Action<IRegisterResult> callback)
		{
			IList<IInvalidProfileItemError> profileItemErrors = null;
			try
			{
				if (!result.Success)
				{
					callback(new RegisterResult(false, null, null));
				}
				else
				{
					IRegisterResult registerResult = GuestControllerErrorParser.GetRegisterResult(result.Response.error);
					if (registerResult != null)
					{
						callback(registerResult);
					}
					else
					{
						profileItemErrors = GuestControllerErrorParser.GetRegisterProfileItemErrors(result.Response.error);
						if (result.Response.data == null)
						{
							if (result.Response.error != null && profileItemErrors == null)
							{
								logger.Critical("Received unhandled error exception" + JsonParser.ToJson(result.Response.error));
							}
							callback(new RegisterResult(false, null, profileItemErrors));
						}
						else
						{
							LogInData loginData = result.Response.data;
							if (loginData.displayName == null || loginData.etag == null || loginData.profile == null || loginData.token == null || loginData.token.access_token == null)
							{
								logger.Critical("Invalid login data:" + JsonParser.ToJson(loginData));
								callback(new RegisterResult(false, null, profileItemErrors));
							}
							else
							{
								loginData.profile.status = GuestControllerErrorParser.GetAccountStatusCode(result.Response.error, loginData.profile.status);
								string countryCode = GuestControllerUtils.GetCountryCode(loginData.profile);
								database.StoreSession(loginData.token.swid, loginData.token.access_token, loginData.token.refresh_token, loginData.displayName.displayName, loginData.profile.firstName, loginData.etag, loginData.profile.ageBand, loginData.displayName.proposedDisplayName, loginData.displayName.proposedStatus, loginData.profile.status, true, countryCode);
								mixSessionStarter.Start(loginData.token.swid, loginData.token.access_token, delegate
								{
									HandleMixSessionStartSuccess(profileItemErrors, loginData.token.swid, loginData.profile, loginData.displayName, marketing, callback);
								}, delegate
								{
									callback(new RegisterResult(false, null, profileItemErrors));
								});
							}
						}
					}
				}
			}
			catch (CorruptionException arg)
			{
				logger.Fatal("Corruption detected during registration: " + arg);
				callback(new RegisterCorruptionDetectedResult());
			}
			catch (Exception arg2)
			{
				logger.Critical("Unhandled exception: " + arg2);
				callback(new RegisterResult(false, null, profileItemErrors));
			}
		}

		private void HandleMixSessionStartSuccess(IEnumerable<IInvalidProfileItemError> errorList, string swid, Profile profile, Disney.Mix.SDK.Internal.GuestControllerDomain.DisplayName displayName, IEnumerable<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing, Action<IRegisterResult> callback)
		{
			try
			{
				IInternalSession session = sessionFactory.Create(swid);
				IInternalLocalUser internalLocalUser = session.InternalLocalUser;
				internalLocalUser.InternalRegistrationProfile.Update(profile, displayName, marketing);
				session.Resume(delegate(IResumeSessionResult r)
				{
					HandleOfflineSessionResumed(r, session, errorList, callback);
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Error creating session: " + arg);
				callback(new RegisterResult(false, null, null));
			}
		}

		private static void HandleOfflineSessionResumed(IResumeSessionResult result, ISession session, IEnumerable<IInvalidProfileItemError> errorList, Action<IRegisterResult> callback)
		{
			callback((!result.Success) ? new RegisterResult(false, null, null) : new RegisterResult(true, session, errorList));
		}
	}
}
