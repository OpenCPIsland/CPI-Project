using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class SessionRefresher : ISessionRefresher
	{
		private readonly IMixWebCallQueue webCallQueue;

		private readonly IGuestControllerClient guestControllerClient;

		private readonly IMixSessionStarter mixSessionStarter;

		public SessionRefresher(IMixWebCallQueue webCallQueue, IGuestControllerClient guestControllerClient, IMixSessionStarter mixSessionStarter)
		{
			this.webCallQueue = webCallQueue;
			this.guestControllerClient = guestControllerClient;
			this.mixSessionStarter = mixSessionStarter;
		}

		public void RefreshGuestControllerToken(string swid, Action successCallback, Action failureCallback)
		{
			webCallQueue.HandleRefreshing();
			EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> accessTokenChangedHandler = null;
			accessTokenChangedHandler = delegate(object sender, AbstractGuestControllerAccessTokenChangedEventArgs e)
			{
				OnGuestControllerAccessTokenChanged(e.GuestControllerAccessToken, swid, successCallback, null, null, accessTokenChangedHandler);
			};
			guestControllerClient.OnAccessTokenChanged += accessTokenChangedHandler;
			guestControllerClient.Refresh(delegate(GuestControllerResult<RefreshResponse> r)
			{
				if (!r.Success)
				{
					failureCallback();
					webCallQueue.HandleGuestControllerTokenRefreshFailure();
				}
			});
		}

		public void RefreshAll(string swid, Action<IWebCallEncryptor> successCallback, Action failureCallback)
		{
			webCallQueue.HandleRefreshing();
			EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> accessTokenChangedHandler = null;
			accessTokenChangedHandler = delegate(object sender, AbstractGuestControllerAccessTokenChangedEventArgs e)
			{
				OnGuestControllerAccessTokenChanged(e.GuestControllerAccessToken, swid, null, successCallback, failureCallback, accessTokenChangedHandler);
			};
			guestControllerClient.OnAccessTokenChanged += accessTokenChangedHandler;
			guestControllerClient.Refresh(delegate(GuestControllerResult<RefreshResponse> r)
			{
				if (!r.Success)
				{
					failureCallback();
					webCallQueue.HandleGuestControllerTokenRefreshFailure();
				}
			});
		}

		private void OnGuestControllerAccessTokenChanged(string guestControllerAccessToken, string swid, Action tokenRefreshSuccessCallback, Action<IWebCallEncryptor> sessionRefreshSuccessCallback, Action failureCallback, EventHandler<AbstractGuestControllerAccessTokenChangedEventArgs> accessTokenChangedHandler)
		{
			guestControllerClient.OnAccessTokenChanged -= accessTokenChangedHandler;
			if (tokenRefreshSuccessCallback != null)
			{
				tokenRefreshSuccessCallback();
				webCallQueue.HandleGuestControllerTokenRefreshSuccess(guestControllerAccessToken);
			}
			else if (sessionRefreshSuccessCallback != null)
			{
				RefreshSession(guestControllerAccessToken, swid, sessionRefreshSuccessCallback, failureCallback);
			}
		}

		public void RefreshSession(string guestControllerAccessToken, string swid, Action<IWebCallEncryptor> successCallback, Action failureCallback)
		{
			webCallQueue.HandleRefreshing();
			mixSessionStarter.Start(swid, guestControllerAccessToken, delegate(MixSessionStartResult result)
			{
				IWebCallEncryptor webCallEncryptor = result.WebCallEncryptor;
				successCallback(webCallEncryptor);
				webCallQueue.HandleCombinedRefreshSuccess(guestControllerAccessToken, webCallEncryptor);
			}, delegate
			{
				failureCallback();
				webCallQueue.HandleSessionRefreshFailure();
			});
		}
	}
}
