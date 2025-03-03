using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class MaseResolutionSender : IMaseResolutionSender
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClient guestControllerClient;

		public MaseResolutionSender(AbstractLogger logger, IGuestControllerClient guestControllerClient)
		{
			this.logger = logger;
			this.guestControllerClient = guestControllerClient;
		}

		public void Send(string lookupValue, string languageCode, Action<ISendMultipleAccountsResolutionResult> callback)
		{
			try
			{
				guestControllerClient.ResolveMase(new RecoverRequest
				{
					lookupValue = lookupValue
				}, languageCode, delegate(GuestControllerResult<NotificationResponse> r)
				{
					if (!r.Success)
					{
						callback(new SendMultipleAccountsResolutionResult(false));
					}
					else
					{
						ISendMultipleAccountsResolutionResult resolveMaseResult = GuestControllerErrorParser.GetResolveMaseResult(r.Response.error);
						if (resolveMaseResult != null)
						{
							callback(resolveMaseResult);
						}
						else
						{
							callback(new SendMultipleAccountsResolutionResult(r.Response.data != null));
						}
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new SendMultipleAccountsResolutionResult(false));
			}
		}
	}
}
