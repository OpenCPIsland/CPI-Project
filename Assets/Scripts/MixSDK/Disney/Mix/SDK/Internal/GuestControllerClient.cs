using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Disney.Mix.SDK.Internal
{
	public class GuestControllerClient : IGuestControllerClient, IDisposable
	{
		private class QueueItem
		{
			public Uri Uri;

			public HttpMethod HttpMethod;

			public string RequestBodyJson;

			public byte[] RequestBody;

			public GuestControllerAuthenticationType AuthType;

			public Func<string, GuestControllerWebCallResponse> ResponseParser;

			public Action<GuestControllerResult<GuestControllerWebCallResponse>> Callback;

			public readonly StringBuilder TimeoutLogs = new StringBuilder();

			public int NumAttempts;

			public bool RefreshedApiKey;

			public bool RefreshedAccessToken;
		}

		private const long LatencyWwwCallTimeout = 60000L;

		private const long MaxWwwCallTimeout = 70000L;

		private const int MaxTimeoutAttempts = 3;

		private const int MaxServerErrorAttempts = 1;

		private static readonly Encoding stringEncoding = Encoding.UTF8;

		private readonly IWwwCallFactory wwwCallFactory;

		private readonly string spoofedIpAddress;

		private readonly IDatabase database;

		private readonly string swid;

		private readonly string host;

		private readonly string clientId;

		private readonly AbstractLogger logger;

		private readonly List<QueueItem> queue;

		private readonly List<IWwwCall> wwwCalls;

		private bool isExecuting;

		public event EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> OnAccessTokenChanged = delegate
		{
		};

		public event EventHandler<AbstractLegalMarketingUpdateRequiredEventArgs> OnLegalMarketingUpdateRequired = delegate
		{
		};

		public event EventHandler<AbstractAuthenticationLostEventArgs> OnAuthenticationLost = delegate
		{
		};

		public GuestControllerClient(IWwwCallFactory wwwCallFactory, string spoofedIpAddress, IDatabase database, string swid, string host, string clientId, AbstractLogger logger)
		{
			this.wwwCallFactory = wwwCallFactory;
			this.spoofedIpAddress = spoofedIpAddress;
			this.database = database;
			this.swid = swid;
			this.host = host;
			this.clientId = clientId;
			this.logger = logger;
			queue = new List<QueueItem>();
			wwwCalls = new List<IWwwCall>();
		}

		public void LogIn(LogInRequest request, Action<GuestControllerResult<LogInResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/login?expand=profile,displayName,marketing", HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void Refresh(Action<GuestControllerResult<RefreshResponse>> callback)
		{
			QueueItem item = CreateRefreshQueueItem(callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		private void RefreshImmediately(Action<GuestControllerResult<RefreshResponse>> callback)
		{
			QueueItem item = CreateRefreshQueueItem(callback);
			queue.Insert(0, item);
			ExecuteNextCall();
		}

		private QueueItem CreateRefreshQueueItem(Action<GuestControllerResult<RefreshResponse>> callback)
		{
			Action<GuestControllerResult<RefreshResponse>> failureCallback = delegate(GuestControllerResult<RefreshResponse> r)
			{
				callback(new GuestControllerResult<RefreshResponse>(false, r.Response, r.ResponseHeaders));
			};
			QueueItem queueItem = CreateQueueItem("/client/{client-id}/guest/refresh-auth/{refresh-token}", HttpMethod.POST, new EmptyRequest(), GuestControllerAuthenticationType.None, delegate(GuestControllerResult<RefreshResponse> r)
			{
				if (!r.Success)
				{
					AuthenticationUnavailableEventArgs e = new AuthenticationUnavailableEventArgs();
					this.OnAuthenticationLost(this, e);
					failureCallback(r);
				}
				else
				{
					GuestApiErrorCollection guestApiErrorCollection = r.Response.error;
					RefreshData data = r.Response.data;
					IRefreshGuestControllerTokenError refreshGuestControllerTokenError = GuestControllerErrorParser.GetGuestControllerTokenRefreshError(guestApiErrorCollection);
					if (refreshGuestControllerTokenError is IRefreshRequiresLegalMarketingUpdateError)
					{
						refreshGuestControllerTokenError = null;
						guestApiErrorCollection = null;
						this.OnLegalMarketingUpdateRequired(this, new LegalMarketingUpdateRequiredEventArgs());
					}
					if (refreshGuestControllerTokenError is IRefreshTokenGatedLocationError)
					{
						logger.Critical("Location gated during Guest Controller token refresh");
						AuthenticationLostGatedCountryEventArgs e2 = new AuthenticationLostGatedCountryEventArgs();
						this.OnAuthenticationLost(this, e2);
						failureCallback(r);
					}
					else if (refreshGuestControllerTokenError is IRefreshProfileDisabledError || refreshGuestControllerTokenError is IRefreshTemporaryBanError)
					{
						logger.Critical("Banned during Guest Controller token refresh");
						AccountBannedEventArgs e3 = new AccountBannedEventArgs(logger, null);
						this.OnAuthenticationLost(this, e3);
						failureCallback(r);
					}
					else if (refreshGuestControllerTokenError != null)
					{
						logger.Critical("Guest Controller token refresh error: " + refreshGuestControllerTokenError);
						AuthenticationRevokedEventArgs e4 = new AuthenticationRevokedEventArgs();
						this.OnAuthenticationLost(this, e4);
						failureCallback(r);
					}
					else if (guestApiErrorCollection != null)
					{
						logger.Critical("Unhandled error during Guest Controller token refresh: " + JsonParser.ToJson(guestApiErrorCollection));
						AuthenticationUnavailableEventArgs e = new AuthenticationUnavailableEventArgs();
						this.OnAuthenticationLost(this, e);
						failureCallback(r);
					}
					else if (data == null || data.token == null || data.token.access_token == null || data.token.refresh_token == null)
					{
						logger.Critical("Refreshing Guest Controller token returned invalid refresh data: " + JsonParser.ToJson(data));
						AuthenticationUnavailableEventArgs e = new AuthenticationUnavailableEventArgs();
						this.OnAuthenticationLost(this, e);
						failureCallback(r);
					}
					else
					{
						Token token = data.token;
						database.UpdateGuestControllerToken(token, data.etag);
						this.OnAccessTokenChanged(this, new GuestControllerAccessTokenChangedEventArgs(token.access_token));
						callback(r);
					}
				}
			});
			queueItem.RefreshedAccessToken = true;
			return queueItem;
		}

		public void GetSiteConfiguration(Action<GuestControllerResult<SiteConfigurationResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/configuration/site", HttpMethod.GET, null, GuestControllerAuthenticationType.None, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetSiteConfiguration(string countryCode, Action<GuestControllerResult<SiteConfigurationResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/configuration/site?countryCode=" + countryCode, HttpMethod.GET, null, GuestControllerAuthenticationType.None, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void Validate(ValidateRequest request, Action<GuestControllerResult<ValidateResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/validate", HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void Register(RegisterRequest request, Action<GuestControllerResult<LogInResponse>> callback)
		{
			string text = "/client/{client-id}/guest/register";
			if (request.profile == null || request.profile.username == null)
			{
				text += "?autogenerateUsername=true";
			}
			text = AddLanguagePreference(text, (request.profile != null) ? request.profile.languagePreference : null);
			QueueItem item = CreateQueueItem(text, HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void UpdateProfile(UpdateProfileRequest request, Action<GuestControllerResult<ProfileResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/{swid}", HttpMethod.POST, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetProfile(Action<GuestControllerResult<ProfileResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/{swid}", HttpMethod.GET, null, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void RecoverPassword(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem(AddLanguagePreference("/client/{client-id}/notification/recover-password", languageCode), HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void RecoverUsername(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem(AddLanguagePreference("/client/{client-id}/notification/recover-username", languageCode), HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void ResolveMase(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem(AddLanguagePreference("/client/{client-id}/notification/resolve-mase", languageCode), HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void UpgradeNrt(RecoverRequest request, string languageCode, Action<GuestControllerResult<NotificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem(AddLanguagePreference("/client/{client-id}/notification/upgrade-nrt", languageCode), HttpMethod.POST, request, GuestControllerAuthenticationType.ApiKey, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void SendParentalApprovalEmail(string languageCode, Action<GuestControllerResult<NotificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem(AddLanguagePreference("/client/{client-id}/notification/parental-approval/{swid}", languageCode), HttpMethod.POST, new EmptyRequest(), GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void SendVerificationEmail(string languageCode, Action<GuestControllerResult<NotificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem(AddLanguagePreference("/client/{client-id}/notification/email-verification", languageCode), HttpMethod.POST, new EmptyRequest(), GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetAdultVerificationStatus(Action<GuestControllerResult<AdultVerificationStatusResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/{swid}/adult-verification-status", HttpMethod.GET, null, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void VerifyAdultUnitedStates(AdultVerificationRequest request, Action<GuestControllerResult<AdultVerificationResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/{swid}/adult-verification-ssn", HttpMethod.POST, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void SendAdultVerificationQuiz(AdultVerificationQuizRequest request, Action<GuestControllerResult<AdultVerificationQuizResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/{swid}/adult-verification-quiz", HttpMethod.POST, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetClaimableChildren(Action<GuestControllerResult<ChildrenResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guardian/{swid}/children", HttpMethod.GET, null, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetLinkedChildren(Action<GuestControllerResult<ChildrenResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guardian/{swid}/children/linked", HttpMethod.GET, null, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void LinkChild(LinkChildRequest request, string childSwid, Action<GuestControllerResult<GuestControllerWebCallResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guardian/{swid}/children/child/" + childSwid, HttpMethod.POST, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void LinkClaimableChildren(LinkClaimableChildrenRequest request, Action<GuestControllerResult<GuestControllerWebCallResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guardian/{swid}/children", HttpMethod.PUT, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetGuardians(Action<GuestControllerResult<GuardiansResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guardian/guardians/child/{swid}", HttpMethod.GET, null, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void RequestPermission(RequestPermissionRequest request, Action<GuestControllerResult<PermissionResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/activity-permission/{swid}", HttpMethod.POST, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void RequestPermissionForChild(RequestPermissionRequest request, string childSwid, Action<GuestControllerResult<PermissionResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/activity-permission/" + childSwid, HttpMethod.POST, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void ApprovePermission(ApprovePermissionRequest request, string childSwid, Action<GuestControllerResult<PermissionResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/activity-permission/" + childSwid, HttpMethod.PUT, request, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void GetPermission(string childSwid, Action<GuestControllerResult<GetPermissionsResponse>> callback)
		{
			QueueItem item = CreateQueueItem("/client/{client-id}/guest/activity-permission/" + childSwid, HttpMethod.GET, null, GuestControllerAuthenticationType.AccessToken, callback);
			queue.Add(item);
			ExecuteNextCall();
		}

		public void Dispose()
		{
			queue.Clear();
			foreach (IWwwCall wwwCall in wwwCalls)
			{
				wwwCall.Dispose();
			}
			wwwCalls.Clear();
		}

		private void GetApiKeyImmediately(Action<GuestControllerResult<GuestControllerWebCallResponse>> callback)
		{
			QueueItem queueItem = CreateQueueItem("/client/{client-id}/api-key", HttpMethod.POST, new EmptyRequest(), GuestControllerAuthenticationType.None, delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
			{
				string value = default(string);
				if (r.Success && (r.ResponseHeaders.TryGetValue("api-key", out value) || r.ResponseHeaders.TryGetValue("API-KEY", out value)) && !string.IsNullOrEmpty(value))
				{
					database.SetGuestControllerApiKey(value);
				}
				callback(r);
			});
			queueItem.RefreshedApiKey = true;
			queue.Insert(0, queueItem);
			ExecuteNextCall();
		}

		private QueueItem CreateQueueItem<TResponse>(string path, HttpMethod httpMethod, AbstractGuestControllerWebCallRequest request, GuestControllerAuthenticationType authType, Action<GuestControllerResult<TResponse>> callback) where TResponse : GuestControllerWebCallResponse
		{
			string text;
			byte[] requestBody;
			if (request == null)
			{
				text = null;
				requestBody = null;
			}
			else
			{
				text = JsonParser.ToJson(request);
				requestBody = stringEncoding.GetBytes(text);
			}
			if (path.Contains("{client-id}"))
			{
				path = path.Replace("{client-id}", clientId);
			}
			if (path.Contains("{swid}"))
			{
				path = path.Replace("{swid}", swid);
			}
			if (path.Contains("{refresh-token}"))
			{
				SessionDocument sessionDocument = database.GetSessionDocument(swid);
				string guestControllerRefreshToken = sessionDocument.GuestControllerRefreshToken;
				path = path.Replace("{refresh-token}", guestControllerRefreshToken);
			}
			Uri uri = new Uri(host + "/jgc/v5" + path);
			QueueItem queueItem = new QueueItem();
			queueItem.Uri = uri;
			queueItem.HttpMethod = httpMethod;
			queueItem.RequestBodyJson = text;
			queueItem.RequestBody = requestBody;
			queueItem.AuthType = authType;
			queueItem.ResponseParser = ((string b) => JsonParser.FromJson<TResponse>(b));
			queueItem.Callback = delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
			{
				callback(new GuestControllerResult<TResponse>(r.Success, (TResponse)r.Response, r.ResponseHeaders));
			};
			return queueItem;
		}

		private void ExecuteNextCall()
		{
			if (isExecuting || queue.Count == 0)
			{
				return;
			}
			QueueItem item = queue[0];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Content-Type", "application/json");
			dictionary.Add("Accept", "application/json");
			dictionary.Add("Cache-Control", "no-cache,no-store,must-revalidate");
			Dictionary<string, string> dictionary2 = dictionary;
			if (spoofedIpAddress != null)
			{
				dictionary2["X-Forwarded-For"] = spoofedIpAddress;
			}
			switch (item.AuthType)
			{
			case GuestControllerAuthenticationType.ApiKey:
			{
				string guestControllerApiKey = database.GetGuestControllerApiKey();
				if (guestControllerApiKey == null)
				{
					GetApiKeyImmediately(delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
					{
						if (!r.Success)
						{
							logger.Critical("Getting API key failed");
							queue.Remove(item);
							CallCallbackWithFailureResult(item, null, null);
						}
						ExecuteNextCall();
					});
					return;
				}
				dictionary2["Authorization"] = "APIKEY " + guestControllerApiKey;
				break;
			}
			case GuestControllerAuthenticationType.AccessToken:
			{
				SessionDocument sessionDocument = database.GetSessionDocument(swid);
				if (sessionDocument == null)
				{
					logger.Critical("Guest Controller web call requires access token, but no session document found");
					queue.Remove(item);
					CallCallbackWithFailureResult(item, null, null);
					ExecuteNextCall();
					return;
				}
				string guestControllerAccessToken = sessionDocument.GuestControllerAccessToken;
				if (guestControllerAccessToken == null)
				{
					logger.Critical("Guest Controller web call requires access token, session document doesn't have one");
					queue.Remove(item);
					CallCallbackWithFailureResult(item, null, null);
					ExecuteNextCall();
					return;
				}
				dictionary2["Authorization"] = "BEARER " + guestControllerAccessToken;
				break;
			}
			}
			item.NumAttempts++;
			isExecuting = true;
			IWwwCall wwwCall = wwwCallFactory.Create(item.Uri, item.HttpMethod, item.RequestBody, dictionary2, 60000L, 70000L);
			logger.Debug(HttpLogBuilder.BuildRequestLog(wwwCall.RequestId, item.Uri, item.HttpMethod, dictionary2, item.RequestBodyJson));
			wwwCalls.Add(wwwCall);
			wwwCall.OnDone += delegate
			{
				isExecuting = false;
				Dictionary<string, string> responseHeaders = new Dictionary<string, string>(wwwCall.ResponseHeaders);
				string error = wwwCall.Error;
				int requestId = wwwCall.RequestId;
				long timeToStartUpload = wwwCall.TimeToStartUpload;
				long timeToFinishUpload = wwwCall.TimeToFinishUpload;
				float percentUploaded = wwwCall.PercentUploaded;
				long timeToStartDownload = wwwCall.TimeToStartDownload;
				long timeToFinishDownload = wwwCall.TimeToFinishDownload;
				float percentDownloaded = wwwCall.PercentDownloaded;
				string timeoutReason = wwwCall.TimeoutReason;
				long timeoutTime = wwwCall.TimeoutTime;
				uint statusCode = wwwCall.StatusCode;
				string responseText = (wwwCall.ResponseBody == null) ? string.Empty : stringEncoding.GetString(wwwCall.ResponseBody);
				GuestControllerWebCallResponse response = item.ResponseParser(responseText);
				if (response == null)
				{
					CallCallbackWithFailureResult(item, null, responseHeaders);
				}
				else
				{
					wwwCalls.Remove(wwwCall);
					wwwCall.Dispose();
					if (!string.IsNullOrEmpty(error))
					{
						string text = error.ToLower();
						if (text == "couldn't connect to host" || text.Contains("timedout") || text.Contains("timed out"))
						{
							string text2 = HttpLogBuilder.BuildTimeoutLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, timeToStartUpload, timeToFinishUpload, percentUploaded, timeToStartDownload, timeToFinishDownload, percentDownloaded, timeoutReason, timeoutTime);
							item.TimeoutLogs.Append(text2);
							item.TimeoutLogs.Append("\n\n");
							logger.Error(text2);
							if (item.NumAttempts > 3)
							{
								logger.Critical("Too many timeouts: " + item.Uri.AbsoluteUri + "\nPrevious logs:\n" + item.TimeoutLogs);
								queue.Remove(item);
								CallCallbackWithFailureResult(item, response, responseHeaders);
							}
							ExecuteNextCall();
							return;
						}
					}
					if (statusCode >= 500 && statusCode <= 599)
					{
						string text2 = HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode);
						if (item.NumAttempts > 1)
						{
							logger.Critical(text2);
							logger.Critical("Too many server errors");
							queue.Remove(item);
							CallCallbackWithFailureResult(item, response, responseHeaders);
						}
						else
						{
							logger.Error(text2);
						}
						ExecuteNextCall();
					}
					else if (HasErrorCode(response, "API_KEY_INVALID"))
					{
						logger.Error(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
						if (item.RefreshedApiKey)
						{
							logger.Critical("Invalid API key. Already got new token once. Giving up.");
							queue.Remove(item);
							CallCallbackWithFailureResult(item, response, responseHeaders);
							ExecuteNextCall();
						}
						else
						{
							logger.Error("Invalid API key. Getting new API key...");
							GetApiKeyImmediately(delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
							{
								if (!r.Success)
								{
									logger.Critical(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
									logger.Critical("Getting new API key failed.");
									queue.Remove(item);
									CallCallbackWithFailureResult(item, response, responseHeaders);
								}
								ExecuteNextCall();
							});
						}
					}
					else if (HasErrorCode(response, "RATE_LIMITED"))
					{
						logger.Error(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
						if (item.RefreshedApiKey)
						{
							logger.Critical("Couldn't get new API key. Already got new API key once. Giving up.");
							queue.Remove(item);
							CallCallbackWithFailureResult(item, response, responseHeaders);
							ExecuteNextCall();
						}
						else
						{
							logger.Error("Rate limited. Getting new API key...");
							GetApiKeyImmediately(delegate(GuestControllerResult<GuestControllerWebCallResponse> r)
							{
								if (!r.Success || HasErrorCode(r.Response, "RATE_LIMITED"))
								{
									logger.Critical(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
									logger.Critical("Getting new API key failed.");
									queue.Remove(item);
									CallCallbackWithFailureResult(item, response, responseHeaders);
								}
								ExecuteNextCall();
							});
						}
					}
					else if (HasErrorCode(response, "ACCESS_TOKEN_NOT_FOUND", "AUTHORIZATION_INVALID_OR_EXPIRED_TOKEN"))
					{
						logger.Error(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
						if (item.RefreshedAccessToken)
						{
							logger.Critical("Invalid access token. Already tried to refresh. Giving up.");
							queue.Remove(item);
							CallCallbackWithFailureResult(item, response, responseHeaders);
							ExecuteNextCall();
						}
						else
						{
							logger.Error("Invalid access token. Refreshing...");
							RefreshImmediately(delegate(GuestControllerResult<RefreshResponse> r)
							{
								if (!r.Success)
								{
									logger.Critical(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
									logger.Critical("Refreshing access token failed");
									queue.Remove(item);
									CallCallbackWithFailureResult(item, response, responseHeaders);
								}
								ExecuteNextCall();
							});
						}
					}
					else if (statusCode >= 401 && statusCode <= 499)
					{
						logger.Critical(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
						queue.Remove(item);
						CallCallbackWithFailureResult(item, response, responseHeaders);
						ExecuteNextCall();
					}
					else if (statusCode == 400 || (statusCode >= 200 && statusCode <= 299))
					{
						logger.Debug(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
						queue.Remove(item);
						item.Callback(new GuestControllerResult<GuestControllerWebCallResponse>(true, response, responseHeaders));
						ExecuteNextCall();
					}
					else
					{
						if (!string.IsNullOrEmpty(error))
						{
							string text = error.ToLower();
							if (text.Contains("connection appears to be offline") || text.Contains("connection was lost") || text.Contains("network is unreachable"))
							{
								logger.Critical(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
								logger.Critical("Offline error = " + error);
							}
							else
							{
								logger.Critical(HttpLogBuilder.BuildResponseLog(requestId, item.Uri, item.HttpMethod, responseHeaders, responseText, statusCode));
								logger.Critical("Other web call error = " + error);
							}
						}
						queue.Remove(item);
						CallCallbackWithFailureResult(item, response, responseHeaders);
						ExecuteNextCall();
					}
				}
			};
			wwwCall.Execute();
		}

		private static void CallCallbackWithFailureResult(QueueItem item, GuestControllerWebCallResponse response, Dictionary<string, string> responseHeaders)
		{
			item.Callback(new GuestControllerResult<GuestControllerWebCallResponse>(false, response, responseHeaders));
		}

		private static bool HasErrorCode(GuestControllerWebCallResponse response, params string[] errorCodes)
		{
			GuestApiErrorCollection error = response.error;
			if (error == null)
			{
				return false;
			}
			List<GuestApiError> errors = error.errors;
			if (errors == null || errors.Count == 0)
			{
				return false;
			}
			if (errors.Any((GuestApiError e) => e.code != null && !errorCodes.Any((string c) => c == e.code)))
			{
				return false;
			}
			return true;
		}

		private static string AddLanguagePreference(string path, string languageCode)
		{
			if (languageCode != null)
			{
				if (path.Contains("?"))
				{
					return path + "&langPref=" + languageCode;
				}
				return path + "?langPref=" + languageCode;
			}
			return path;
		}
	}
}
