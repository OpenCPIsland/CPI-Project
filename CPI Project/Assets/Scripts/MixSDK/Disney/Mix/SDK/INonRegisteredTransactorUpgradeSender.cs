using System;

namespace Disney.Mix.SDK
{
	public interface INonRegisteredTransactorUpgradeSender
	{
		void Send(string lookupValue, string languageCode, Action<ISendNonRegisteredTransactorUpgradeResult> callback);
	}
}
