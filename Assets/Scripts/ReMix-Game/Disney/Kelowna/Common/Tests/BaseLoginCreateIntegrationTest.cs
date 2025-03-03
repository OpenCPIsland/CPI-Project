using ClubPenguin;
using ClubPenguin.Analytics;
using ClubPenguin.Commerce;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common.Environment;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public abstract class BaseLoginCreateIntegrationTest : BaseIntegrationTest
	{
		public Disney.Kelowna.Common.Environment.Environment TestEnvironment;

		public GameObject TestCanvas;

		protected override IEnumerator setup()
		{
			GcsAccessTokenService gcsAccessTokenService = new GcsAccessTokenService(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountName"), new GcsP12AssetFileLoader(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountFile")));
			Service.Set((IGcsAccessTokenService)gcsAccessTokenService);
			string cdnUrl = ContentHelper.GetCdnUrl();
			string cpipeMappingFilename = ContentHelper.GetCpipeMappingFilename();
			CPipeManifestService cpipeManifestService = new CPipeManifestService(cdnUrl, cpipeMappingFilename, gcsAccessTokenService);
			Service.Set((ICPipeManifestService)cpipeManifestService);
			base.gameObject.AddComponent<KeyChainManager>();
			GameSettings gameSettings = new GameSettings();
			Service.Set(gameSettings);
			ContentManifest definition = ContentManifestUtility.FromDefinitionFile("Configuration/embedded_content_manifest");
			Service.Set(new Content(definition));
			Localizer localizer = Localizer.Instance;
			Service.Set(localizer);
			NullCPSwrveService cpSwrveService = new NullCPSwrveService();
			Service.Set((ICPSwrveService)cpSwrveService);
			NetworkServicesConfig networkConfig = NetworkController.GenerateNetworkServiceConfig(TestEnvironment);
			Service.Set((INetworkServicesManager)new NetworkServicesManager(this, networkConfig, false));
			CommerceService commerceService = new CommerceService();
			commerceService.Setup();
			Service.Set(commerceService);
			Service.Set(new MembershipService(null));
			ConnectionManager connectionManager = base.gameObject.AddComponent<ConnectionManager>();
			Service.Set(connectionManager);
			Service.Set(new SessionManager());
			Service.Set(new MixLoginCreateService());
			Service.Set((CPDataEntityCollection)new CPDataEntityCollectionImpl());
			LocalPlayerData localPlayerData = new LocalPlayerData
			{
				name = "TestPlayer",
				tutorialData = new List<sbyte>()
			};
			PlayerId playerId = localPlayerData.id = new PlayerId
			{
				id = "999999999999999",
				type = PlayerId.PlayerIdType.SESSION_ID
			};
			Service.Get<CPDataEntityCollection>().ResetLocalPlayerHandle();
			PlayerDataEntityFactory.AddLocalPlayerProfileDataComponents(Service.Get<CPDataEntityCollection>(), localPlayerData);
			LoginController loginController = new LoginController();
			Service.Set(loginController);
			loginController.SetNetworkConfig(networkConfig);
			IntegrationTestEx.FailIf(Service.Get<MixLoginCreateService>().NetworkConfigIsNotSet);
			yield return null;
		}

		protected override IEnumerator runTest()
		{
			TestCanvas.SetActive(true);
			yield break;
		}

		protected override void tearDown()
		{
			Service.ResetAll();
		}
	}
}
