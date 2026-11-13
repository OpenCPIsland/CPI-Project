using System;

namespace Disney.Mix.SDK
{
	public interface IPasswordRecoverySender
	{
		void Send(string lookupValue, string languageCode, Action<ISendPasswordRecoveryResult> callback);
	}
}
