using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class ParentalApprovalEmailSender
	{
		public static void SendParentalApprovalEmail(AbstractLogger logger, string languageCode, IGuestControllerClient guestControllerClient, Action<ISendParentalApprovalEmailResult> callback)
		{
			try
			{
				guestControllerClient.SendParentalApprovalEmail(languageCode, delegate(GuestControllerResult<NotificationResponse> r)
				{
					callback(new SendParentalApprovalEmailResult(r.Success && r.Response.data != null));
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new SendParentalApprovalEmailResult(false));
			}
		}
	}
}
