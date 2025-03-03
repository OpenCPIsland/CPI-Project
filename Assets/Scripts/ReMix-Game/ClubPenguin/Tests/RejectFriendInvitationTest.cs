using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System.Collections;
using System.Linq;

namespace ClubPenguin.Tests
{
	public class RejectFriendInvitationTest : BaseFriendsIntegrationTest
	{
		private bool outgoingFriendInvitationRejected;

		private bool incomingFriendInvitationComponentRemoved;

		private bool outgoingFriendInvitationComponentRemoved;

		private IncomingFriendInvitationData incomingFriendInvitationData;

		protected override IEnumerator setup()
		{
			yield return base.setup();
			friendsService.FindUser(bobSession.LocalUser.DisplayName.Text, aliceSession.LocalUser);
			while (dataEntityCollection.GetEntityByType<SearchedUserData>().IsNull)
			{
				yield return null;
			}
			DataEntityHandle searchedUserHandle = dataEntityCollection.GetEntityByType<SearchedUserData>();
			SearchedUserData searchedUserData = dataEntityCollection.GetComponent<SearchedUserData>(searchedUserHandle);
			friendsService.SendFriendInvitation(searchedUserData.SearchedUser, aliceSession.LocalUser);
			while (dataEntityCollection.GetEntityByType<OutgoingFriendInvitationData>().IsNull)
			{
				yield return null;
			}
			DataEntityHandle outgoingInvitationHandle = dataEntityCollection.GetEntityByType<OutgoingFriendInvitationData>();
			OutgoingFriendInvitationData outgoingFriendInvitationData = dataEntityCollection.GetComponent<OutgoingFriendInvitationData>(outgoingInvitationHandle);
			outgoingFriendInvitationData.OnRejected += onOutgoingFriendInvitationRejected;
			while (!bobSession.LocalUser.IncomingFriendInvitations.Any())
			{
				yield return null;
			}
		}

		protected override IEnumerator runTest()
		{
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentRemovedEvent>(onOutgoingFriendInvitationComponentRemoved);
			IRejectFriendInvitationResult rejectResult = null;
			bobSession.LocalUser.RejectFriendInvitation(bobSession.LocalUser.IncomingFriendInvitations.First(), delegate(IRejectFriendInvitationResult r)
			{
				rejectResult = r;
			});
			while (rejectResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!rejectResult.Success, "Reject friend invitation failed");
			while (!outgoingFriendInvitationComponentRemoved)
			{
				yield return null;
			}
			while (!outgoingFriendInvitationRejected)
			{
				yield return null;
			}
		}

		private void onOutgoingFriendInvitationRejected(string displayName)
		{
			outgoingFriendInvitationRejected = true;
		}

		private bool onOutgoingFriendInvitationComponentRemoved(DataEntityEvents.ComponentRemovedEvent evt)
		{
			if (evt.Component is OutgoingFriendInvitationData)
			{
				outgoingFriendInvitationComponentRemoved = true;
			}
			return false;
		}
	}
}
