using Disney.Mix.SDK.Internal.MixDomain;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IGetStateResponseParser
	{
		IList<IInternalFriend> ParseFriendships(GetStateResponse response, IUserDatabase userDatabase);

		void ParseFriendshipInvitations(GetStateResponse response, IUserDatabase userDatabase, IInternalLocalUser localUser, out IList<IInternalIncomingFriendInvitation> incomingFriendInvitations, out IList<IInternalOutgoingFriendInvitation> outgoingFriendInvitations);

		void ReconcileWithLocalUser(IMixWebCallFactory mixWebCallFactory, GetStateResponse response, IInternalLocalUser localUser, IUserDatabase userDatabase);

		void ParsePollIntervals(GetStateResponse response, out int[] pollIntervals, out int[] pokeIntervals);

		IList<IInternalAlert> ParseAlerts(GetStateResponse response);

		List<BaseNotification> CollectNotifications(GetNotificationsResponse response);
	}
}
