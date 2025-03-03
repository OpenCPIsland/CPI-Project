using System;

namespace Disney.Mix.SDK.Internal
{
	public interface INrtUpgradeSender
	{
		void Send(string lookupValue, string languageCode, Action<ISendNonRegisteredTransactorUpgradeResult> callback);
	}
}
