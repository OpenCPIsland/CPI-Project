using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Tests;
using Disney.Mix.SDK;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ClubPenguin.Tests
{
	public class MixSDKCreateTest : BaseMixIntegrationTest
	{
		private IRegistrationConfiguration config;

		protected static IGetRegistrationConfigurationResult configResult;

		protected static IRegistrationConfiguration registrationConfig;

		private ISession session;

		private IAgeBand ageBand;

		protected override IEnumerator setup()
		{
			yield return StartCoroutine(base.setup());
			mixLoginCreateService.GetRegistrationConfig(OnConfigResponse);
			mixLoginCreateService.OnCreateSuccess += HandleOnCreateSuccess;
			mixLoginCreateService.OnCreateFailed += HandleOnCreateFailed;
			while (ageBand == null)
			{
				yield return null;
			}
		}

		private void OnConfigResponse(IGetRegistrationConfigurationResult result)
		{
			if (result.Success)
			{
				config = result.Configuration;
				config.GetRegistrationAgeBand(1, "en-US", onAgeBandReceived);
			}
			else
			{
				IntegrationTest.Fail("Registration Config could not be retrieved.");
			}
		}

		private void onAgeBandReceived(IGetAgeBandResult result)
		{
			if (result.Success)
			{
				ageBand = result.AgeBand;
			}
			else
			{
				IntegrationTest.Fail("AgeBand could not be retrieved.");
			}
		}

		protected override IEnumerator runTest()
		{
			CreateChildAccountTest();
			while (session == null)
			{
				yield return null;
			}
			IntegrationTestEx.FailIf(string.IsNullOrEmpty(session.LocalUser.DisplayName.Text));
		}

		public void CreateChildAccountTest()
		{
			Dictionary<IMarketingItem, bool> marketing = new Dictionary<IMarketingItem, bool>();
			IEnumerable<ILegalDocument> legalDocuments = ageBand.LegalDocuments;
			mixLoginCreateService.CreateChildAccount("Dan", "cpTest_" + Path.GetRandomFileName(), Path.GetRandomFileName() + "@dispostable.com", "CpIntTestPassword1", "en-US", marketing, legalDocuments);
		}

		private void HandleOnCreateSuccess(ISession session)
		{
			this.session = session;
		}

		private void HandleOnCreateFailed(IRegisterResult result)
		{
			IntegrationTest.Fail("Create Request Failed");
		}

		protected override void tearDown()
		{
			mixLoginCreateService.OnCreateSuccess -= HandleOnCreateSuccess;
			mixLoginCreateService.OnCreateFailed -= HandleOnCreateFailed;
			mixLoginCreateService = null;
		}
	}
}
