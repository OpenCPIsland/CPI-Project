using ClubPenguin.UI;
using Disney.MobileNetwork;
using System.Collections;

namespace Disney.Kelowna.Common.Tests
{
	public abstract class BaseBackButtonControllerIntegrationTest : BaseIntegrationTest
	{
		protected override IEnumerator setup()
		{
			BackButtonController backButtonController = base.gameObject.AddComponent<BackButtonController>();
			Service.Set(backButtonController);
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			yield break;
		}

		protected override void tearDown()
		{
			Service.ResetAll();
		}
	}
}
