using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class MixWebCallFactory : IMixWebCallFactory, IDisposable
	{
		private const long DefaultLatencyWwwCallTimeout = 10000L;

		private const long DefaultMaxWwwCallTimeout = 30000L;

		private readonly AbstractLogger logger;

		private readonly string host;

		private readonly IWwwCallFactory wwwCallFactory;

		private readonly string swid;

		private IWebCallEncryptor webCallEncryptor;

		private string guestControllerAccessToken;

		private readonly string mixClientToken;

		private bool isSessionRefreshing;

		private readonly IMixWebCallQueue webCallQueue;

		private readonly ISessionRefresher sessionRefresher;

		private readonly IEpochTime epochTime;

		private readonly IDatabase database;

		private readonly IList<IDisposable> webCalls;

		public string GuestControllerAccessToken
		{
			get
			{
				return guestControllerAccessToken;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Guest Controller access token can't be null or empty");
				}
				guestControllerAccessToken = value;
			}
		}

		public event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost = delegate
		{
		};

		public IWebCall<GetUsersByDisplayNameRequest, GetUsersResponse> UsersByDisplayNamePost(GetUsersByDisplayNameRequest request)
		{
			return CreateWebCall<GetUsersByDisplayNameRequest, GetUsersResponse>(HttpMethod.POST, new Uri(host + "/users/byDisplayName"), request);
		}

		public IWebCall<GetUsersByUserIdRequest, GetUsersResponse> UsersByUserIdPost(GetUsersByUserIdRequest request)
		{
			return CreateWebCall<GetUsersByUserIdRequest, GetUsersResponse>(HttpMethod.POST, new Uri(host + "/users/byUserId"), request);
		}

		public MixWebCallFactory(AbstractLogger logger, string host, IWwwCallFactory wwwCallFactory, IWebCallEncryptor webCallEncryptor, string swid, string guestControllerAccessToken, string mixClientToken, IMixWebCallQueue webCallQueue, ISessionRefresher sessionRefresher, IEpochTime epochTime, IDatabase database)
		{
			this.logger = logger;
			this.host = host;
			this.wwwCallFactory = wwwCallFactory;
			this.webCallEncryptor = webCallEncryptor;
			this.swid = swid;
			this.guestControllerAccessToken = guestControllerAccessToken;
			this.mixClientToken = mixClientToken;
			this.webCallQueue = webCallQueue;
			this.sessionRefresher = sessionRefresher;
			this.epochTime = epochTime;
			this.database = database;
			webCalls = new List<IDisposable>();
		}

		public void Dispose()
		{
			foreach (IDisposable webCall in webCalls)
			{
				webCall.Dispose();
			}
			webCalls.Clear();
		}

		private IWebCall<TRequest, TResponse> CreateWebCall<TRequest, TResponse>(HttpMethod method, Uri uri, TRequest request) where TRequest : BaseUserRequest where TResponse : BaseResponse, new()
		{
			return CreateWebCall<TRequest, TResponse>(method, uri, request, 10000L, 30000L);
		}

		private IWebCall<TRequest, TResponse> CreateWebCall<TRequest, TResponse>(HttpMethod method, Uri uri, TRequest request, long latencyWwwCallTimeout, long maxWwwCallTimeout) where TRequest : BaseUserRequest where TResponse : BaseResponse, new()
		{
			request.UserId = swid;
			request.Timestamp = (database.GetServerTimeOffsetMillis().HasValue ? new long?(epochTime.Milliseconds) : null);
			string body = JsonParser.ToJson(request);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Content-Type", webCallEncryptor.ContentType);
			dictionary.Add("X-Mix-OneIdToken", guestControllerAccessToken);
			dictionary.Add("X-Mix-ClientToken", mixClientToken);
			dictionary.Add("Cache-Control", "no-cache,no-store,must-revalidate");
			Dictionary<string, string> dictionary2 = dictionary;
			if (webCallEncryptor.SessionId != null)
			{
				dictionary2.Add("X-Mix-UserSessionId", webCallEncryptor.SessionId);
			}
			MixWebCall<TRequest, TResponse> webCall = new MixWebCall<TRequest, TResponse>(logger, uri, body, method, dictionary2, wwwCallFactory, webCallEncryptor, latencyWwwCallTimeout, maxWwwCallTimeout, database, swid);
			webCalls.Add(webCall);
			webCallQueue.AddWebCall(webCall);
			if (isSessionRefreshing)
			{
				webCall.RefreshStatus = WebCallRefreshStatus.WaitingForRefreshCallback;
			}
			webCall.OnResponse += delegate
			{
				webCalls.Remove(webCall);
				webCallQueue.RemoveWebCall(webCall);
			};
			webCall.OnError += delegate
			{
				webCalls.Remove(webCall);
				webCallQueue.RemoveWebCall(webCall);
			};
			webCall.OnUnauthorized += delegate(object sender, WebCallUnauthorizedEventArgs e)
			{
				HandleOnUnauthorized(webCall, e);
			};
			return webCall;
		}

		private void HandleOnUnauthorized<TRequest, TResponse>(IWebCall<TRequest, TResponse> webCall, WebCallUnauthorizedEventArgs e) where TRequest : BaseUserRequest where TResponse : BaseResponse, new()
		{
			if (webCallEncryptor.SessionId == null)
			{
				webCall.DispatchError("Unauthorized access to start a new session!");
			}
			else if (!isSessionRefreshing)
			{
				if (webCall.RefreshStatus == WebCallRefreshStatus.RefreshedWhileWaitingForCallback)
				{
					webCall.RefreshStatus = WebCallRefreshStatus.NotRefreshing;
					webCall.Execute();
					return;
				}
				webCall.RefreshStatus = WebCallRefreshStatus.WaitingForRefreshCallback;
				isSessionRefreshing = true;
				switch (e.Status)
				{
				case "UNAUTHORIZED_ONEID_TOKEN":
					sessionRefresher.RefreshGuestControllerToken(swid, delegate
					{
						isSessionRefreshing = false;
					}, delegate
					{
						isSessionRefreshing = false;
					});
					break;
				case "UNAUTHORIZED_MIX_SESSION":
					sessionRefresher.RefreshSession(guestControllerAccessToken, swid, delegate(IWebCallEncryptor encryptor)
					{
						isSessionRefreshing = false;
						webCallEncryptor = encryptor;
					}, delegate
					{
						isSessionRefreshing = false;
						this.OnAuthenticationLost(this, new AuthenticationUnavailableEventArgs());
					});
					break;
				case "UNAUTHORIZED_BANNED":
					webCall.DispatchError("Account is banned!");
					isSessionRefreshing = false;
					this.OnAuthenticationLost(this, new AccountBannedEventArgs(logger, e.ResponseText));
					break;
				default:
					sessionRefresher.RefreshAll(swid, delegate(IWebCallEncryptor encryptor)
					{
						isSessionRefreshing = false;
						webCallEncryptor = encryptor;
					}, delegate
					{
						isSessionRefreshing = false;
						this.OnAuthenticationLost(this, new AuthenticationUnavailableEventArgs());
					});
					break;
				}
			}
			else
			{
				webCall.RefreshStatus = WebCallRefreshStatus.WaitingForRefreshCallback;
			}
		}

		public IWebCall<ClearAlertsRequest, ClearAlertsResponse> AlertsClearPut(ClearAlertsRequest request)
		{
			return CreateWebCall<ClearAlertsRequest, ClearAlertsResponse>(HttpMethod.PUT, new Uri(host + "/alert/clear"), request);
		}

		public IWebCall<SetDisplayNameRequest, SetDisplayNameResponse> DisplaynamePut(SetDisplayNameRequest request)
		{
			return CreateWebCall<SetDisplayNameRequest, SetDisplayNameResponse>(HttpMethod.PUT, new Uri(host + "/displayname"), request);
		}

		public IWebCall<ValidateDisplayNamesRequest, ValidateDisplayNamesResponse> DisplaynameValidatePost(ValidateDisplayNamesRequest request)
		{
			return CreateWebCall<ValidateDisplayNamesRequest, ValidateDisplayNamesResponse>(HttpMethod.POST, new Uri(host + "/displayname/validate"), request);
		}

		public IWebCall<ValidateDisplayNameRequest, ValidateDisplayNameResponse> DisplaynameValidateV2Post(ValidateDisplayNameRequest request)
		{
			return CreateWebCall<ValidateDisplayNameRequest, ValidateDisplayNameResponse>(HttpMethod.POST, new Uri(host + "/displayname/validate/v2"), request);
		}

		public IWebCall<AddFriendshipRequest, AddFriendshipResponse> FriendshipPut(AddFriendshipRequest request)
		{
			return CreateWebCall<AddFriendshipRequest, AddFriendshipResponse>(HttpMethod.PUT, new Uri(host + "/friendship"), request);
		}

		public IWebCall<RemoveFriendshipRequest, RemoveFriendshipResponse> FriendshipDeletePost(RemoveFriendshipRequest request)
		{
			return CreateWebCall<RemoveFriendshipRequest, RemoveFriendshipResponse>(HttpMethod.POST, new Uri(host + "/friendship/delete"), request);
		}

		public IWebCall<BaseUserRequest, GetFriendshipRecommendationResponse> FriendshipRecommendPost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, GetFriendshipRecommendationResponse>(HttpMethod.POST, new Uri(host + "/friendship/recommend"), request);
		}

		public IWebCall<AddFriendshipInvitationRequest, AddFriendshipInvitationResponse> FriendshipInvitationPut(AddFriendshipInvitationRequest request)
		{
			return CreateWebCall<AddFriendshipInvitationRequest, AddFriendshipInvitationResponse>(HttpMethod.PUT, new Uri(host + "/friendship/invitation"), request);
		}

		public IWebCall<RemoveFriendshipInvitationRequest, RemoveFriendshipInvitationResponse> FriendshipInvitationDeletePost(RemoveFriendshipInvitationRequest request)
		{
			return CreateWebCall<RemoveFriendshipInvitationRequest, RemoveFriendshipInvitationResponse>(HttpMethod.POST, new Uri(host + "/friendship/invitation/delete"), request);
		}

		public IWebCall<BaseUserRequest, GetGeolocationResponse> GeolocationPost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, GetGeolocationResponse>(HttpMethod.POST, new Uri(host + "/geolocation"), request);
		}

		public IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportModerationTempBanPut(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, BaseResponse>(HttpMethod.PUT, new Uri(host + "/integrationTestSupport/moderation/tempBan"), request);
		}

		public IWebCall<TriggerAlertRequest, BaseResponse> IntegrationTestSupportUserAlertPost(TriggerAlertRequest request)
		{
			return CreateWebCall<TriggerAlertRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/integrationTestSupport/user/alert"), request);
		}

		public IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportUserAnonymizePost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/integrationTestSupport/user/anonymize"), request);
		}

		public IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportUserNotificationCounterDeletePost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/integrationTestSupport/user/notificationCounter/delete"), request);
		}

		public IWebCall<BaseUserRequest, BaseResponse> IntegrationTestSupportUserSessionExpirePost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/integrationTestSupport/user/session/expire"), request);
		}

		public IWebCall<SetLanguageRequest, BaseResponse> LanguagePreferencePost(SetLanguageRequest request)
		{
			return CreateWebCall<SetLanguageRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/languagePreference"), request);
		}

		public IWebCall<ReportPlayerRequest, BaseResponse> ModerationReportPlayerPut(ReportPlayerRequest request)
		{
			return CreateWebCall<ReportPlayerRequest, BaseResponse>(HttpMethod.PUT, new Uri(host + "/moderation/reportPlayer"), request);
		}

		public IWebCall<ModerateTextRequest, ModerateTextResponse> ModerationTextPut(ModerateTextRequest request)
		{
			return CreateWebCall<ModerateTextRequest, ModerateTextResponse>(HttpMethod.PUT, new Uri(host + "/moderation/text"), request);
		}

		public IWebCall<GetNotificationsRequest, GetNotificationsResponse> NotificationsPost(GetNotificationsRequest request)
		{
			return CreateWebCall<GetNotificationsRequest, GetNotificationsResponse>(HttpMethod.POST, new Uri(host + "/notifications"), request);
		}

		public IWebCall<GetNotificationsSinceSequenceRequest, GetNotificationsResponse> NotificationsSinceSequencePost(GetNotificationsSinceSequenceRequest request)
		{
			return CreateWebCall<GetNotificationsSinceSequenceRequest, GetNotificationsResponse>(HttpMethod.POST, new Uri(host + "/notifications/sinceSequence"), request);
		}

		public IWebCall<PilCheckRequest, PilCheckResponse> PilCheckPost(PilCheckRequest request)
		{
			return CreateWebCall<PilCheckRequest, PilCheckResponse>(HttpMethod.POST, new Uri(host + "/pil/check"), request);
		}

		public IWebCall<SetPresenceRequest, BaseResponse> PresencePut(SetPresenceRequest request)
		{
			return CreateWebCall<SetPresenceRequest, BaseResponse>(HttpMethod.PUT, new Uri(host + "/presence"), request);
		}

		public IWebCall<BaseUserRequest, PostPresenceResponse> PresencePost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, PostPresenceResponse>(HttpMethod.POST, new Uri(host + "/presence"), request);
		}

		public IWebCall<TogglePushNotificationRequest, BaseResponse> PushNotificationsSettingPost(TogglePushNotificationRequest request)
		{
			return CreateWebCall<TogglePushNotificationRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/pushNotificationsSetting"), request);
		}

		public IWebCall<BaseUserRequest, BaseResponse> PushNotificationsSettingDeletePost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/pushNotificationsSetting/delete"), request);
		}

		public IWebCall<GetRegistrationTextRequest, GetRegistrationTextResponse> RegistrationTextPost(GetRegistrationTextRequest request)
		{
			return CreateWebCall<GetRegistrationTextRequest, GetRegistrationTextResponse>(HttpMethod.POST, new Uri(host + "/registration/text"), request);
		}

		public IWebCall<DisplayNameSearchRequest, DisplayNameSearchResponse> SearchDisplaynamePost(DisplayNameSearchRequest request)
		{
			return CreateWebCall<DisplayNameSearchRequest, DisplayNameSearchResponse>(HttpMethod.POST, new Uri(host + "/search/displayname"), request);
		}

		public IWebCall<StartUserSessionRequest, StartUserSessionResponse> SessionUserPut(StartUserSessionRequest request)
		{
			return CreateWebCall<StartUserSessionRequest, StartUserSessionResponse>(HttpMethod.PUT, new Uri(host + "/session/user"), request);
		}

		public IWebCall<BaseUserRequest, BaseResponse> SessionUserDeletePost(BaseUserRequest request)
		{
			return CreateWebCall<BaseUserRequest, BaseResponse>(HttpMethod.POST, new Uri(host + "/session/user/delete"), request);
		}

		public IWebCall<GetStateRequest, GetStateResponse> StatePost(GetStateRequest request)
		{
			return CreateWebCall<GetStateRequest, GetStateResponse>(HttpMethod.POST, new Uri(host + "/state"), request, 30000L, 40000L);
		}

		public IWebCall<RemoveFriendshipTrustRequest, RemoveFriendshipTrustResponse> FriendshipTrustDeletePost(RemoveFriendshipTrustRequest request)
		{
			return CreateWebCall<RemoveFriendshipTrustRequest, RemoveFriendshipTrustResponse>(HttpMethod.POST, new Uri(host + "/friendship/trust/delete"), request);
		}
	}
}
