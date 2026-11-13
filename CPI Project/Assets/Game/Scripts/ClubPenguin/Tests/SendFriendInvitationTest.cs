using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;

namespace ClubPenguin.Tests
{
	public class SendFriendInvitationTest : BaseFriendsIntegrationTest
	{
		private bool? sendFriendInvitationSuccess;

		private SearchedUserData searchedUserData;

		protected override IEnumerator setup()
		{
			yield return base.setup();
			friendsService.FindUser(bobSession.LocalUser.DisplayName.Text, aliceSession.LocalUser);
			while (dataEntityCollection.GetEntityByType<SearchedUserData>().IsNull)
			{
				yield return null;
			}
			DataEntityHandle searchedUserHandle = dataEntityCollection.GetEntityByType<SearchedUserData>();
			searchedUserData = dataEntityCollection.GetComponent<SearchedUserData>(searchedUserHandle);
		}

		protected override IEnumerator runTest()
		{
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.SendFriendInvitationSent>(onSendFriendInvitationSent);
			friendsService.SendFriendInvitation(searchedUserData.SearchedUser, aliceSession.LocalUser);
			while (!sendFriendInvitationSuccess.HasValue)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!sendFriendInvitationSuccess.Value, "Send friend invitation did not succeed");
			while (dataEntityCollection.GetEntityByType<OutgoingFriendInvitationData>().IsNull)
			{
				yield return null;
			}
			DataEntityHandle displayNameHandle = dataEntityCollection.FindEntity<DisplayNameData, string>(bobSession.LocalUser.DisplayName.Text);
			IntegrationTestEx.FailIf(displayNameHandle.IsNull, "Alice doesn't have Bob's display name");
			IntegrationTestEx.FailIf(dataEntityCollection.GetComponent<SearchedUserData>(displayNameHandle) == null, "Alice doesn't have a searched user data for Bob");
			IntegrationTestEx.FailIf(dataEntityCollection.GetComponent<OutgoingFriendInvitationData>(displayNameHandle) == null, "Alice still has the friend invitation to Bob");
		}

		private bool onSendFriendInvitationSent(FriendsServiceEvents.SendFriendInvitationSent evt)
		{
			sendFriendInvitationSuccess = evt.Success;
			return false;
		}
	}
}
