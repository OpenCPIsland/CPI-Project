using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IInternalLocalUser : ILocalUser
	{
		string Swid
		{
			get;
		}

		IEnumerable<IInternalFriend> InternalFriends
		{
			get;
		}

		IEnumerable<IInternalOutgoingFriendInvitation> InternalOutgoingFriendInvitations
		{
			get;
		}

		IEnumerable<IInternalIncomingFriendInvitation> InternalIncomingFriendInvitations
		{
			get;
		}

		IInternalRegistrationProfile InternalRegistrationProfile
		{
			get;
		}

		event Action<bool> OnPushNotificationsToggled;

		event Action<bool> OnPushNotificationReceived;

		event Action<string> OnDisplayNameUpdated;

		void AddFriend(IInternalFriend friend);

		void RemoveFriend(IInternalFriend friend);

		void UntrustFriend(IInternalFriend friend);

		void AddIncomingFriendInvitation(IInternalIncomingFriendInvitation invitation);

		void AddOutgoingFriendInvitation(IInternalOutgoingFriendInvitation invitation);

		void RemoveIncomingFriendInvitation(IInternalIncomingFriendInvitation invitation);

		void RemoveOutgoingFriendInvitation(IInternalOutgoingFriendInvitation invitation);

		void AddFriendshipInvitation(FriendshipInvitation invitation, User friend);

		void RemoveFriendshipInvitation(long invitationId);

		void AddFriend(User domainFriend, bool isTrusted, long invitationId);

		void RemoveFriend(string friendSwid, bool sendEvent);

		void UntrustFriend(string friendSwid);

		void DispatchOnAlertsAdded(IAlert alert);

		void DispatchOnAlertsCleared(IEnumerable<IAlert> alerts);
	}
}
