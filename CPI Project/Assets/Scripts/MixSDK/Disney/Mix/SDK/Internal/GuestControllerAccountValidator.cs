using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public static class GuestControllerAccountValidator
	{
		public static void ValidateAdultAccount(AbstractLogger logger, IGuestControllerClient guestControllerClient, string email, string password, Action<IValidateNewAccountResult> callback)
		{
			Send(logger, guestControllerClient, new ValidateRequest
			{
				email = email,
				password = password
			}, callback);
		}

		public static void ValidateChildAccount(AbstractLogger logger, IGuestControllerClient guestControllerClient, string username, string password, Action<IValidateNewAccountResult> callback)
		{
			Send(logger, guestControllerClient, new ValidateRequest
			{
				username = username,
				password = password
			}, callback);
		}

		private static void Send(AbstractLogger logger, IGuestControllerClient guestControllerClient, ValidateRequest request, Action<IValidateNewAccountResult> callback)
		{
			try
			{
				guestControllerClient.Validate(request, delegate(GuestControllerResult<ValidateResponse> r)
				{
					if (!r.Success)
					{
						callback(new ValidateNewAccountResult(false, null));
					}
					else
					{
						ValidateResponse response = r.Response;
						if (response.error == null)
						{
							callback(new ValidateNewAccountResult(true, null));
						}
						else
						{
							IValidateNewAccountResult validateResult = GuestControllerErrorParser.GetValidateResult(response.error);
							if (validateResult != null)
							{
								callback(validateResult);
							}
							else
							{
								IEnumerable<IValidateNewAccountError> validationErrors = GuestControllerErrorParser.GetValidationErrors(response.error);
								callback(new ValidateNewAccountResult(false, validationErrors));
							}
						}
					}
				});
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				callback(new ValidateNewAccountResult(false, null));
			}
		}
	}
}
