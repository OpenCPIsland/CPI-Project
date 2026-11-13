using System;

namespace Disney.Mix.SDK
{
	public interface IMultipleAccountsResolutionSender
	{
		void Send(string lookupValue, string languageCode, Action<ISendMultipleAccountsResolutionResult> callback);
	}
}
