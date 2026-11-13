using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class NrtUpgradeSender : INrtUpgradeSender
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClient guestControllerClient;

		public NrtUpgradeSender(AbstractLogger logger, IGuestControllerClient guestControllerClient)
		{
			this.logger = logger;
			this.guestControllerClient = guestControllerClient;
		}

		public void Send(string lookupValue, string languageCode, Action<ISendNonRegisteredTransactorUpgradeResult> callback)
		{
			try
			{
				guestControllerClient.UpgradeNrt(new RecoverRequest
				{
					lookupValue = lookupValue
				}, languageCode, delegate(GuestControllerResult<NotificationResponse> r)
				{
					if (!r.Success)
					{
						callback(new SendNonRegisteredTransactorUpgradeResult(false));
					}
					else
					{
						ISendNonRegisteredTransactorUpgradeResult upgradeNrtResult = GuestControllerErrorParser.GetUpgradeNrtResult(r.Response.error);
						if (upgradeNrtResult != null)
						{
							callback(upgradeNrtResult);
						}
						else
						{
							callback(new SendNonRegisteredTransactorUpgradeResult(r.Response.data != null));
						}
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new SendNonRegisteredTransactorUpgradeResult(false));
			}
		}
	}
}
