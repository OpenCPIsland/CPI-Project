using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public static class StateGetter
	{
		public static void GetState(AbstractLogger logger, IEpochTime epochTime, string clientVersion, IDatabase database, IUserDatabase userDatabase, INotificationQueue notificationQueue, IMixWebCallFactory mixWebCallFactory, string localUserId, long lastNotificationTime, Action<GetStateResponse> successCallback, Action failureCallback)
		{
			try
			{
				GetStateRequest getStateRequest = new GetStateRequest();
				getStateRequest.UserId = localUserId;
				getStateRequest.ClientVersion = clientVersion;
				GetStateRequest request = getStateRequest;
				IWebCall<GetStateRequest, GetStateResponse> webCall = mixWebCallFactory.StatePost(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<GetStateResponse> e)
				{
					HandleGetStateSuccess(logger, epochTime, database, userDatabase, notificationQueue, e.Response, mixWebCallFactory, successCallback, failureCallback);
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

		private static void HandleGetStateSuccess(AbstractLogger logger, IEpochTime epochTime, IDatabase database, IUserDatabase userDatabase, INotificationQueue notificationQueue, GetStateResponse response, IMixWebCallFactory mixWebCallFactory, Action<GetStateResponse> successCallback, Action failureCallback)
		{
			try
			{
				if (ValidateResponse(response))
				{
					epochTime.ReferenceTime = response.Timestamp.Value;
					database.SetServerTimeOffsetMillis((long)epochTime.Offset.TotalMilliseconds);
					logger.Debug("New time offset: " + epochTime.Offset);
					successCallback(response);
					notificationQueue.LatestSequenceNumber = response.NotificationSequenceCounter.Value;
				}
				else
				{
					logger.Critical("Error validating get state response: " + JsonParser.ToJson(response));
					failureCallback();
					notificationQueue.Clear();
				}
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				failureCallback();
				notificationQueue.Clear();
			}
		}

		private static bool ValidateResponse(GetStateResponse response)
		{
			if (response.FriendshipInvitations != null && response.FriendshipInvitations.Any((FriendshipInvitation i) => !i.IsTrusted.HasValue || !i.FriendshipInvitationId.HasValue))
			{
				return false;
			}
			if (response.Friendships != null && response.Friendships.Any((Friendship f) => !f.IsTrusted.HasValue))
			{
				return false;
			}
			if (response.PollIntervals != null && response.PollIntervals.Any((int? p) => !p.HasValue))
			{
				return false;
			}
			if (response.PokeIntervals != null && response.PokeIntervals.Any((int? p) => !p.HasValue))
			{
				return false;
			}
			if (!response.Timestamp.HasValue)
			{
				return false;
			}
			if (!response.NotificationSequenceCounter.HasValue)
			{
				return false;
			}
			if (response.Users == null || response.Users.Any((User u) => u.HashedUserId == null))
			{
				return false;
			}
			return true;
		}
	}
}
