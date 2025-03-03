using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public class PasswordRecoverySender : IPasswordRecoverySender
	{
		private readonly AbstractLogger logger;

		private readonly IGuestControllerClient guestControllerClient;

		public PasswordRecoverySender(AbstractLogger logger, IGuestControllerClient guestControllerClient)
		{
			this.logger = logger;
			this.guestControllerClient = guestControllerClient;
		}

		public void Send(string lookupValue, string languageCode, Action<ISendPasswordRecoveryResult> callback)
		{
			try
			{
				guestControllerClient.RecoverPassword(new RecoverRequest
				{
					lookupValue = lookupValue
				}, languageCode, delegate(GuestControllerResult<NotificationResponse> r)
				{
					if (!r.Success)
					{
						callback(new SendPasswordRecoveryResult(false));
					}
					else
					{
						ISendPasswordRecoveryResult recoverPasswordResult = GuestControllerErrorParser.GetRecoverPasswordResult(r.Response.error);
						if (recoverPasswordResult != null)
						{
							callback(recoverPasswordResult);
						}
						else
						{
							callback(new SendPasswordRecoveryResult(r.Response.data != null));
						}
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new SendPasswordRecoveryResult(false));
			}
		}
	}
}
