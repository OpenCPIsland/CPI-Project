using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System.Collections;
using System.Linq;

namespace ClubPenguin.Tests
{
	public class UnfriendTest : BaseFriendsIntegrationTest
	{
		private bool? unfriendSuccess;

		private bool isFriendComponentRemoved;

		private bool isOutgoingFriendInvitationComponentRemoved;

		private FriendData friendData;

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
			while (dataEntityCollection.GetEntityByType<FriendData>().IsNull)
			{
				yield return null;
			}
		}

		protected override IEnumerator runTest()
		{
			IUnfriendResult unfriendResult = null;
			bobSession.LocalUser.Unfriend(bobSession.LocalUser.Friends.First(), delegate(IUnfriendResult r)
			{
				unfriendResult = r;
			});
			while (unfriendResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!unfriendResult.Success, "Unfriend did not succeed");
			while (!dataEntityCollection.GetEntityByType<FriendData>().IsNull)
			{
				yield return null;
			}
		}
	}
}
