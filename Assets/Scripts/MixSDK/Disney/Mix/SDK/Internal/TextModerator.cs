using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class TextModerator
	{
		public static void ModerateText(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, string text, bool isTrusted, Action<ModerateTextResponse> successCallback, Action failureCallback)
		{
			try
			{
				ModerateTextRequest moderateTextRequest = new ModerateTextRequest();
				moderateTextRequest.Text = text;
				moderateTextRequest.ModerationPolicy = (isTrusted ? "Trusted" : "UnTrusted");
				ModerateTextRequest request = moderateTextRequest;
				IWebCall<ModerateTextRequest, ModerateTextResponse> webCall = mixWebCallFactory.ModerationTextPut(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<ModerateTextResponse> e)
				{
					ModerateTextResponse response = e.Response;
					if (ValidateModerateTextResponse(response))
					{
						successCallback(response);
					}
					else
					{
						logger.Critical("Failed to validate moderate text response: " + JsonParser.ToJson(response));
						failureCallback();
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

		private static bool ValidateModerateTextResponse(ModerateTextResponse response)
		{
			return response.Moderated.HasValue && response.Text != null;
		}
	}
}
