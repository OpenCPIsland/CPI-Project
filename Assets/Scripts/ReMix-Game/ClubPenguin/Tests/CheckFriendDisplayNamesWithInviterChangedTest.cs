using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System.Collections;
using System.Linq;

namespace ClubPenguin.Tests
{
	public class CheckFriendDisplayNamesWithInviterChangedTest : BaseFriendsIntegrationTest
	{
		protected override IEnumerator setup()
		{
			yield return base.setup();
			IFindUserResult findUserResult = null;
			bobSession.LocalUser.FindUser(aliceSession.LocalUser.DisplayName.Text, delegate(IFindUserResult r)
			{
				findUserResult = r;
			});
			while (findUserResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!findUserResult.Success, "Find user failed");
			ISendFriendInvitationResult sendInviteResult = null;
			bobSession.LocalUser.SendFriendInvitation(findUserResult.User, false, delegate(ISendFriendInvitationResult r)
			{
				sendInviteResult = r;
			});
			while (sendInviteResult == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!sendInviteResult.Success, "Send friend invitation failed");
		}

		protected override IEnumerator runTest()
		{
			while (dataEntityCollection.GetEntityByType<IncomingFriendInvitationData>().IsNull)
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
			friendsService.AcceptFriendInvitation(aliceSession.LocalUser.IncomingFriendInvitations.First(), aliceSession.LocalUser);
			while (dataEntityCollection.GetEntityByType<FriendData>().IsNull)
			{
				yield return null;
			}
			DataEntityHandle aliceFriendHandle = dataEntityCollection.GetEntityByType<FriendData>();
			FriendData aliceFriendData = dataEntityCollection.GetComponent<FriendData>(aliceFriendHandle);
			DisplayNameData aliceDisplayNameData = dataEntityCollection.GetComponent<DisplayNameData>(aliceFriendHandle);
			IntegrationTestEx.FailIf(aliceFriendData.Friend.DisplayName.Text != bobNewDisplayName, string.Format("The invitee friend data display name did not match changed display name 1. Actual: {0}. Expected: {1}", aliceFriendData.Friend.DisplayName.Text, bobNewDisplayName));
			IntegrationTestEx.FailIf(aliceDisplayNameData.DisplayName != bobNewDisplayName, string.Format("The invitee display name data display name did not match changed display name 1. Actual: {0}. Expected: {1}", aliceDisplayNameData.DisplayName, bobNewDisplayName));
		}
	}
}
