using Disney.Kelowna.Common.Tests;
using Disney.Mix.SDK;
using System.Collections;

namespace ClubPenguin.Tests
{
	public class MixSDKLoginTests : BaseMixIntegrationTest
	{
		protected override IEnumerator setup()
		{
			yield return StartCoroutine(base.setup());
			mixLoginCreateService.OnLoginSuccess += HandleOnLoginSuccess;
			mixLoginCreateService.OnLoginFailed += HandleOnLoginFailed;
			login();
			while (!sessionManager.HasSession)
			{
				yield return null;
			}
		}

		private void login()
		{
			mixLoginCreateService.Login("cptest0042", "123qwe");
		}

		private void softLogin()
		{
			mixLoginCreateService.SoftLogin();
		}

		private void HandleOnLoginSuccess(ISession session)
		{
			sessionManager.AddMixSession(session);
		}

		private void HandleOnLoginFailed(ILoginResult result)
		{
			IntegrationTest.Fail("Login Failed : " + result);
		}

		protected override IEnumerator runTest()
		{
			sessionManager.DisposeSession();
			softLogin();
			while (!sessionManager.HasSession)
			{
				yield return null;
			}
		}

		protected override void tearDown()
		{
		}
	}
}
