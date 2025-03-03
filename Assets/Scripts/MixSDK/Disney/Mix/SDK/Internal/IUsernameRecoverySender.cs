using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IUsernameRecoverySender
	{
		void Send(string lookupValue, string languageCode, Action<ISendUsernameRecoveryResult> callback);
	}
}
