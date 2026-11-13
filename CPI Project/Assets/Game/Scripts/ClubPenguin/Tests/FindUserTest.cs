using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;

namespace ClubPenguin.Tests
{
	public class FindUserTest : BaseFriendsIntegrationTest
	{
		private bool? isFindUserSuccessful;

		protected override IEnumerator runTest()
		{
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			friendsService.FindUser(bobSession.LocalUser.DisplayName.Text, aliceSession.LocalUser);
			while (!isFindUserSuccessful.HasValue)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(!isFindUserSuccessful.Value, "Find user did not succeed");
			DataEntityHandle handle = dataEntityCollection.FindEntity<DisplayNameData, string>(bobSession.LocalUser.DisplayName.Text);
			IntegrationTestEx.FailIf(handle.IsNull, "Data entity handle with the correct display name was not created");
			SearchedUserData searchedUserData = dataEntityCollection.GetComponent<SearchedUserData>(handle);
			IntegrationTestEx.FailIf(searchedUserData == null, "Data entity handle did not have a SearchedUserData component added");
		}

		private bool onFindUserSent(FriendsServiceEvents.FindUserSent evt)
		{
			isFindUserSuccessful = evt.Success;
			return false;
		}
	}
}
