using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IMaseResolutionSender
	{
		void Send(string lookupValue, string languageCode, Action<ISendMultipleAccountsResolutionResult> callback);
	}
}
