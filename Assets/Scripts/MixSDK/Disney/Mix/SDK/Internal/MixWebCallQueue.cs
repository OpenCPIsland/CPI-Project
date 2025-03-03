using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class MixWebCallQueue : IMixWebCallQueue
	{
		private const string SessionHeaderKey = "X-Mix-UserSessionId";

		private const string OneIdTokenHeaderKey = "X-Mix-OneIdToken";

		private bool isRefreshing;

		private readonly IDictionary<object, Action<bool, string, bool, IWebCallEncryptor>> webCalls;

		private readonly IDictionary<object, Action<bool, string, bool, IWebCallEncryptor>> queuedWebCalls;

		public MixWebCallQueue()
		{
			webCalls = new Dictionary<object, Action<bool, string, bool, IWebCallEncryptor>>();
			queuedWebCalls = new Dictionary<object, Action<bool, string, bool, IWebCallEncryptor>>();
		}

		public void AddWebCall<TRequest, TResponse>(IWebCall<TRequest, TResponse> webCall) where TRequest : BaseUserRequest where TResponse : BaseResponse, new()
		{
			webCall.OnUnqueued += delegate
			{
				RemoveWebCall(webCall);
			};
			Action<bool, string, bool, IWebCallEncryptor> value = delegate(bool needAuthToken, string authToken, bool needSession, IWebCallEncryptor encryptor)
			{
				HandleRefreshCallback(webCall, needAuthToken, authToken, needSession, encryptor);
			};
			if (isRefreshing)
			{
				webCall.RefreshStatus = WebCallRefreshStatus.WaitingForRefreshCallback;
				queuedWebCalls[webCall] = value;
			}
			else
			{
				webCalls[webCall] = value;
			}
		}

		private static void HandleRefreshCallback<TRequest, TResponse>(IWebCall<TRequest, TResponse> webCall, bool needAuthToken, string authToken, bool needSession, IWebCallEncryptor webCallEncryptor) where TRequest : BaseUserRequest where TResponse : BaseResponse, new()
		{
			if (needAuthToken && authToken != null)
			{
				webCall.SetHeader("X-Mix-OneIdToken", authToken);
			}
			if (needSession && webCallEncryptor != null)
			{
				webCall.WebCallEncryptor = webCallEncryptor;
				webCall.SetHeader("X-Mix-UserSessionId", webCallEncryptor.SessionId);
			}
			if (webCall.RefreshStatus == WebCallRefreshStatus.WaitingForRefreshCallback)
			{
				webCall.RefreshStatus = WebCallRefreshStatus.NotRefreshing;
				bool flag = true;
				if (needAuthToken && authToken == null)
				{
					flag = false;
					webCall.DispatchError("GuestController token expired and couldn't get a new token");
				}
				if (needSession && webCallEncryptor == null)
				{
					flag = false;
					webCall.DispatchError("Session expired and couldn't start a new session");
				}
				if (flag)
				{
					webCall.Execute();
				}
			}
			else
			{
				webCall.RefreshStatus = WebCallRefreshStatus.RefreshedWhileWaitingForCallback;
			}
		}

		public void RemoveWebCall<TRequest, TResponse>(IWebCall<TRequest, TResponse> webCall) where TRequest : BaseUserRequest where TResponse : BaseResponse, new()
		{
			webCalls.Remove(webCall);
			queuedWebCalls.Remove(webCall);
		}

		public void HandleRefreshing()
		{
			if (!isRefreshing)
			{
				isRefreshing = true;
				foreach (KeyValuePair<object, Action<bool, string, bool, IWebCallEncryptor>> webCall in webCalls)
				{
					queuedWebCalls[webCall.Key] = webCall.Value;
				}
				webCalls.Clear();
			}
		}

		public void HandleGuestControllerTokenRefreshSuccess(string guestControllerAccessToken)
		{
			HandleRefreshResult(true, guestControllerAccessToken, false, null);
		}

		public void HandleGuestControllerTokenRefreshFailure()
		{
			HandleRefreshResult(true, null, false, null);
		}

		public void HandleSessionRefreshSuccess(IWebCallEncryptor encryptor)
		{
			HandleRefreshResult(false, null, true, encryptor);
		}

		public void HandleSessionRefreshFailure()
		{
			HandleRefreshResult(false, null, true, null);
		}

		public void HandleCombinedRefreshSuccess(string guestControllerAccessToken, IWebCallEncryptor encryptor)
		{
			HandleRefreshResult(true, guestControllerAccessToken, true, encryptor);
		}

		private void HandleRefreshResult(bool needAuthToken, string authToken, bool needSession, IWebCallEncryptor webCallEncryptor)
		{
			isRefreshing = false;
			Action<bool, string, bool, IWebCallEncryptor>[] array = queuedWebCalls.Values.ToArray();
			foreach (KeyValuePair<object, Action<bool, string, bool, IWebCallEncryptor>> queuedWebCall in queuedWebCalls)
			{
				webCalls[queuedWebCall.Key] = queuedWebCall.Value;
			}
			queuedWebCalls.Clear();
			Action<bool, string, bool, IWebCallEncryptor>[] array2 = array;
			foreach (Action<bool, string, bool, IWebCallEncryptor> action in array2)
			{
				action(needAuthToken, authToken, needSession, webCallEncryptor);
			}
		}
	}
}
