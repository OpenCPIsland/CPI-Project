using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class Untruster
	{
		public static void Untrust(AbstractLogger logger, INotificationQueue notificationQueue, IMixWebCallFactory mixWebCallFactory, string friendSwid, Action<RemoveFriendshipTrustNotification> successCallback, Action failureCallback)
		{
			try
			{
				RemoveFriendshipTrustRequest removeFriendshipTrustRequest = new RemoveFriendshipTrustRequest();
				removeFriendshipTrustRequest.FriendUserId = friendSwid;
				RemoveFriendshipTrustRequest request = removeFriendshipTrustRequest;
				IWebCall<RemoveFriendshipTrustRequest, RemoveFriendshipTrustResponse> webCall = mixWebCallFactory.FriendshipTrustDeletePost(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<RemoveFriendshipTrustResponse> e)
				{
					RemoveFriendshipTrustResponse response = e.Response;
					if (NotificationValidator.Validate(response.Notification))
					{
						notificationQueue.Dispatch(response.Notification, delegate
						{
							successCallback(response.Notification);
						}, failureCallback);
					}
					else
					{
						logger.Critical("Failed to validate remove friendship trust response: " + JsonParser.ToJson(response));
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
	}
}
