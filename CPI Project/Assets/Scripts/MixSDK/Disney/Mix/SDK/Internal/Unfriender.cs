using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class Unfriender
	{
		public static void Unfriend(AbstractLogger logger, INotificationQueue notificationQueue, IMixWebCallFactory mixWebCallFactory, string friendSwid, Action successCallback, Action failureCallback)
		{
			try
			{
				RemoveFriendshipRequest removeFriendshipRequest = new RemoveFriendshipRequest();
				removeFriendshipRequest.FriendUserId = friendSwid;
				RemoveFriendshipRequest request = removeFriendshipRequest;
				IWebCall<RemoveFriendshipRequest, RemoveFriendshipResponse> webCall = mixWebCallFactory.FriendshipDeletePost(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<RemoveFriendshipResponse> e)
				{
					RemoveFriendshipResponse response = e.Response;
					RemoveFriendshipNotification notification = response.Notification;
					if (NotificationValidator.Validate(notification))
					{
						notificationQueue.Dispatch(notification, successCallback, failureCallback);
					}
					else
					{
						logger.Critical("Failed to validate remove friendship response: " + JsonParser.ToJson(response));
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
