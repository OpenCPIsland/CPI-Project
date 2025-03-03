using System;

namespace Disney.Mix.SDK.Internal
{
	public interface ISessionRefresher
	{
		void RefreshGuestControllerToken(string swid, Action successCallback, Action failureCallback);

		void RefreshSession(string guestControllerAccessToken, string swid, Action<IWebCallEncryptor> successCallback, Action failureCallback);

		void RefreshAll(string swid, Action<IWebCallEncryptor> successCallback, Action failureCallback);
	}
}
