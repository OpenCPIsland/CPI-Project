using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common.Tests;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace ClubPenguin.Tests
{
	public class BaseFriendsIntegrationTest : BaseMixIntegrationTest
	{
		protected CPDataEntityCollection dataEntityCollection;

		protected IFriendsService friendsService;

		protected ISession aliceSession;

		protected ISession bobSession;

		protected override IEnumerator setup()
		{
			yield return base.setup();
			TestGuestBuilder testGuestBuilder = new TestGuestBuilder(networkServicesConfig.GuestControllerHostUrl, networkServicesConfig.DisneyIdClientId);
			TestGuest aliceGuest = null;
			TestGuest bobGuest = null;
			foreach (object item in testGuestBuilder.CreateChildAccount(delegate(TestGuest g)
			{
				aliceGuest = g;
			}))
			{
				object obj = item;
				yield return null;
			}
			foreach (object item2 in testGuestBuilder.CreateChildAccount(delegate(TestGuest g)
			{
				bobGuest = g;
			}))
			{
				object obj2 = item2;
				yield return null;
			}
			foreach (object item3 in login(aliceGuest, delegate(ISession s)
			{
				aliceSession = s;
			}))
			{
				object obj3 = item3;
				yield return null;
			}
			foreach (object item4 in login(bobGuest, delegate(ISession s)
			{
				bobSession = s;
			}))
			{
				object obj4 = item4;
				yield return null;
			}
			dataEntityCollection = createDataEntityCollection(aliceSession.LocalUser.DisplayName.Text);
			Service.Set(dataEntityCollection);
			friendsService = new FriendsService();
			createFriendsDataModelService(dataEntityCollection, friendsService);
			sessionManager.AddMixSession(aliceSession);
			friendsService.SetLocalUser(aliceSession.LocalUser);
		}

		private IEnumerable login(TestGuest guest, Action<ISession> callback)
		{
			ISession session = null;
			bool done = false;
			Action<ISession> onSuccess = null;
			Action<ILoginResult> onFailed = null;
			onSuccess = delegate(ISession s)
			{
				mixLoginCreateService.OnLoginSuccess -= onSuccess;
				mixLoginCreateService.OnLoginFailed -= onFailed;
				session = s;
				done = true;
			};
			onFailed = delegate
			{
				mixLoginCreateService.OnLoginSuccess -= onSuccess;
				mixLoginCreateService.OnLoginFailed -= onFailed;
				IntegrationTest.Fail("Couldn't create a session");
				done = true;
			};
			mixLoginCreateService.OnLoginSuccess += onSuccess;
			mixLoginCreateService.OnLoginFailed += onFailed;
			mixLoginCreateService.Login(guest.Username, guest.Password);
			while (!done)
			{
				yield return null;
			}
			callback(session);
		}

		private void createFriendsDataModelService(CPDataEntityCollection dataEntityCollection, IFriendsService friendsService)
		{
			FriendsDataModelService friendsDataModelService = base.gameObject.AddComponent<FriendsDataModelService>();
			friendsDataModelService.enabled = false;
			friendsDataModelService.SetDataEntityCollection(dataEntityCollection);
			friendsService.AddMixFriendEventsListener(friendsDataModelService);
		}

		protected override void tearDown()
		{
			if (aliceSession != null)
			{
				aliceSession.Dispose();
			}
			if (bobSession != null)
			{
				bobSession.Dispose();
			}
		}

		protected static string CreateRandomName()
		{
			string text;
			do
			{
				text = Regex.Replace(Path.GetRandomFileName(), "[^0-9a-zA-Z]", "");
			}
			while (text.Length < 6);
			return text;
		}
	}
}
