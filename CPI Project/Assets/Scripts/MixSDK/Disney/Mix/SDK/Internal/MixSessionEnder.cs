using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class MixSessionEnder
	{
		public static void End(AbstractLogger logger, IDatabase database, IKeychain keychain, IMixWebCallFactory mixWebCallFactory, string swid, Action successCallback, Action failureCallback)
		{
			try
			{
				keychain.PushNotificationKey = null;
				BaseUserRequest request = new BaseUserRequest();
				IWebCall<BaseUserRequest, BaseResponse> webCall = mixWebCallFactory.SessionUserDeletePost(request);
				webCall.OnResponse += delegate
				{
					database.LogOutSession(swid);
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
