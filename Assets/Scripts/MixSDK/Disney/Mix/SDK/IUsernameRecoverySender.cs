using System;

namespace Disney.Mix.SDK
{
	public interface IUsernameRecoverySender
	{
		void Send(string lookupValue, string languageCode, Action<ISendUsernameRecoveryResult> callback);
	}
}
