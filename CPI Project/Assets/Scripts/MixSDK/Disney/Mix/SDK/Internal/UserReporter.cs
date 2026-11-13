using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class UserReporter
	{
		public static void Report(AbstractLogger logger, IMixWebCallFactory mixWebCallFactory, string userId, ReportUserReason reason, Action successCallback, Action failureCallback)
		{
			try
			{
				ReportPlayerRequest reportPlayerRequest = new ReportPlayerRequest();
				reportPlayerRequest.ReportedUserId = userId;
				reportPlayerRequest.Reason = reason.ToString();
				ReportPlayerRequest request = reportPlayerRequest;
				IWebCall<ReportPlayerRequest, BaseResponse> webCall = mixWebCallFactory.ModerationReportPlayerPut(request);
				webCall.OnResponse += delegate
				{
					successCallback();
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
