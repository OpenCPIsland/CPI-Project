using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System.Collections;
using System.Linq;

namespace ClubPenguin.Tests
{
	public class CheckFriendDisplayNamesWithInviteeChangedTest : BaseFriendsIntegrationTest
	{
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
		}

		protected override IEnumerator runTest()
		{
			while (!bobSession.LocalUser.IncomingFriendInvitations.Any())
			{
				yield return null;
			}
			string bobNewDisplayName = BaseFriendsIntegrationTest.CreateRandomName();
			IUpdateDisplayNameResult updateDisplayNameResult = null;
			bobSession.LocalUser.UpdateDisplayName(bobNewDisplayName, delegate(IUpdateDisplayNameResult r)
			{
				updateDisplayNameResult = r;
			});
			while (updateDisplayNameResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!updateDisplayNameResult.Success, "Failed to change Bob's display name to " + bobNewDisplayName);
			IAcceptFriendInvitationResult acceptResult = null;
			bobSession.LocalUser.AcceptFriendInvitation(bobSession.LocalUser.IncomingFriendInvitations.First(), false, delegate(IAcceptFriendInvitationResult r)
			{
				acceptResult = r;
			});
			while (acceptResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!acceptResult.Success, "Accept friend invitation failed");
			while (dataEntityCollection.GetEntityByType<FriendData>().IsNull)
			{
				yield return null;
			}
			DataEntityHandle inviteeFriend = dataEntityCollection.GetEntityByType<FriendData>();
			FriendData inviteeFriendData = dataEntityCollection.GetComponent<FriendData>(inviteeFriend);
			DisplayNameData inviteeDisplayNameData = dataEntityCollection.GetComponent<DisplayNameData>(inviteeFriend);
			IntegrationTestEx.FailIf(inviteeFriendData.Friend.DisplayName.Text != bobNewDisplayName, string.Format("The invitee friend data display name did not match changed display name 1. Actual: {0}. Expected: {1}", inviteeFriendData.Friend.DisplayName.Text, bobNewDisplayName));
			IntegrationTestEx.FailIf(inviteeDisplayNameData.DisplayName != bobNewDisplayName, string.Format("The invitee display name data display name did not match changed display name 1. Actual: {0}. Expected: {1}", inviteeDisplayNameData.DisplayName, bobNewDisplayName));
		}
	}
}
