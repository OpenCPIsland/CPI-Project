using System;

namespace Disney.Mix.SDK.Internal
{
	public class NoOpSessionRefresher : ISessionRefresher
	{
		public void RefreshGuestControllerToken(string swid, Action successCallback, Action failureCallback)
		{
			successCallback();
		}

		public void RefreshSession(string guestControllerAccessToken, string swid, Action<IWebCallEncryptor> successCallback, Action failureCallback)
		{
			successCallback(null);
		}

		public void RefreshAll(string swid, Action<IWebCallEncryptor> successCallback, Action failureCallback)
		{
			successCallback(null);
		}
	}
}
