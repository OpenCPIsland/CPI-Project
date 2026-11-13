using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System.Collections;
using System.Linq;

namespace ClubPenguin.Tests
{
	public class AcceptFriendInvitationTest : BaseFriendsIntegrationTest
	{
		private bool? acceptFriendInvitationSuccess;

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
		}

		protected override IEnumerator runTest()
		{
			while (!bobSession.LocalUser.IncomingFriendInvitations.Any())
			{
				yield return null;
			}
			IAcceptFriendInvitationResult rejectResult = null;
			bobSession.LocalUser.AcceptFriendInvitation(bobSession.LocalUser.IncomingFriendInvitations.First(), false, delegate(IAcceptFriendInvitationResult r)
			{
				rejectResult = r;
			});
			while (rejectResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!rejectResult.Success, "Accept friend invitation failed");
			while (dataEntityCollection.FindEntity<DisplayNameData, string>(bobSession.LocalUser.DisplayName.Text).IsNull)
			{
				yield return null;
			}
			DataEntityHandle handle = dataEntityCollection.FindEntity<DisplayNameData, string>(bobSession.LocalUser.DisplayName.Text);
			while (dataEntityCollection.GetComponent<OutgoingFriendInvitationData>(handle) != null)
			{
				yield return null;
			}
			while (dataEntityCollection.GetComponent<FriendData>(handle) == null)
			{
				yield return null;
			}
		}
	}
}
