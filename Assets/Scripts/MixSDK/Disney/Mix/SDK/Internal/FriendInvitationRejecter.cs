using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class FriendInvitationRejecter
	{
		public static void Reject(AbstractLogger logger, INotificationQueue notificationQueue, IMixWebCallFactory mixWebCallFactory, long invitationId, Action<RemoveFriendshipInvitationNotification> successCallback, Action failureCallback)
		{
			try
			{
				RemoveFriendshipInvitationRequest removeFriendshipInvitationRequest = new RemoveFriendshipInvitationRequest();
				removeFriendshipInvitationRequest.InvitationId = invitationId;
				RemoveFriendshipInvitationRequest request = removeFriendshipInvitationRequest;
				IWebCall<RemoveFriendshipInvitationRequest, RemoveFriendshipInvitationResponse> webCall = mixWebCallFactory.FriendshipInvitationDeletePost(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<RemoveFriendshipInvitationResponse> e)
				{
					RemoveFriendshipInvitationResponse response = e.Response;
					if (NotificationValidator.Validate(response.Notification))
					{
						RemoveFriendshipInvitationNotification notification = response.Notification;
						notificationQueue.Dispatch(notification, delegate
						{
							successCallback(notification);
						}, failureCallback);
					}
					else
					{
						logger.Critical("Failed to validate invitation response: " + JsonParser.ToJson(response));
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
