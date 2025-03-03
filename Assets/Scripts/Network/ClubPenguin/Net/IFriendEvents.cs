using Disney.Mix.SDK;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public interface IFriendEvents
	{
		void OnFriendsListReady(List<IFriend> friends);

		void OnIncomingInvitationsListReady(List<IIncomingFriendInvitation> incomingFriendInvitations);

		void OnOutgoingInvitationsListReady(List<IOutgoingFriendInvitation> outgoingFriendInvitations);

		void OnFindUserSent(bool success, IUnidentifiedUser searchedUser);

		void OnReceivedIncomingFriendInvitation(IIncomingFriendInvitation incomingFriendInvitation);

		void OnReceivedOutgoingFriendInvitation(IOutgoingFriendInvitation outgoingFriendInvitation);

		void OnUnfriended(IFriend exfriend);
	}
}
