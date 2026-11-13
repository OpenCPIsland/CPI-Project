using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class AdultVerificationRequirementsGetter
	{
		private const string PersonalInformationUsageCode = "PUBLIC_DISCLOSURE";

		public static void GetRequirements(AbstractLogger logger, string countryCode, IMixWebCallFactory mixWebCallFactory, Action<bool, bool> successCallback, Action failureCallback)
		{
			try
			{
				PilCheckRequest pilCheckRequest = new PilCheckRequest();
				pilCheckRequest.PersonalInformationUsage = "PUBLIC_DISCLOSURE";
				pilCheckRequest.CountryCode = countryCode;
				PilCheckRequest request = pilCheckRequest;
				IWebCall<PilCheckRequest, PilCheckResponse> webCall = mixWebCallFactory.PilCheckPost(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<PilCheckResponse> e)
				{
					bool? adultVerificationRequired = e.Response.AdultVerificationRequired;
					bool? adultVerificationAvailable = e.Response.AdultVerificationAvailable;
					if (!adultVerificationRequired.HasValue || !adultVerificationAvailable.HasValue)
					{
						failureCallback();
					}
					else
					{
						successCallback(adultVerificationRequired.Value, adultVerificationAvailable.Value);
					}
				};
				webCall.OnError += delegate
				{
					failureCallback();
				};
				webCall.Execute();
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				failureCallback();
			}
		}
	}
}
