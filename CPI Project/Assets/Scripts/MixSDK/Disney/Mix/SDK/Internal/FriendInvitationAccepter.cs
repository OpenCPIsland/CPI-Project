using Disney.Mix.SDK.Internal.MixDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class FriendInvitationAccepter
	{
		public static void Accept(AbstractLogger logger, INotificationQueue notificationQueue, IMixWebCallFactory mixWebCallFactory, long invitationId, bool isTrusted, Action<AddFriendshipNotification> successCallback, Action failureCallback)
		{
			try
			{
				AddFriendshipRequest addFriendshipRequest = new AddFriendshipRequest();
				addFriendshipRequest.InvitationId = invitationId;
				addFriendshipRequest.IsTrusted = isTrusted;
				AddFriendshipRequest request = addFriendshipRequest;
				IWebCall<AddFriendshipRequest, AddFriendshipResponse> webCall = mixWebCallFactory.FriendshipPut(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<AddFriendshipResponse> e)
				{
					AddFriendshipResponse response = e.Response;
					if (NotificationValidator.Validate(response.Notification))
					{
						notificationQueue.Dispatch(response.Notification, delegate
						{
							successCallback(response.Notification);
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
