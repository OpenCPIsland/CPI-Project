using ClubPenguin;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public abstract class BaseMixIntegrationTest : BaseIntegrationTest
	{
		protected MixLoginCreateService mixLoginCreateService;

		protected SessionManager sessionManager;

		protected NetworkServicesConfig networkServicesConfig;

		protected override IEnumerator setup()
		{
			base.gameObject.AddComponent<KeyChainManager>();
			GameSettings gameSettings = new GameSettings();
			Service.Set(gameSettings);
			ContentManifest definition = ContentManifestUtility.FromDefinitionFile("Configuration/embedded_content_manifest");
			GcsAccessTokenService gcsAccessTokenService = new GcsAccessTokenService(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountName"), new GcsP12AssetFileLoader(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountFile")));
			Service.Set((IGcsAccessTokenService)gcsAccessTokenService);
			string cdnUrl = ContentHelper.GetCdnUrl();
			string cpipeMappingFilename = ContentHelper.GetCpipeMappingFilename();
			CPipeManifestService cpipeManifestService = new CPipeManifestService(cdnUrl, cpipeMappingFilename, gcsAccessTokenService);
			Service.Set((ICPipeManifestService)cpipeManifestService);
			NullCPSwrveService cpSwrveService = new NullCPSwrveService();
			Service.Set((ICPSwrveService)cpSwrveService);
			Content content = new Content(definition);
			Service.Set(content);
			networkServicesConfig = NetworkController.GenerateNetworkServiceConfig(Disney.Kelowna.Common.Environment.Environment.LOCAL);
			ConnectionManager connectionManager = base.gameObject.AddComponent<ConnectionManager>();
			Service.Set(connectionManager);
			initEnvironmentManager();
			sessionManager = new SessionManager();
			Service.Set(sessionManager);
			Localizer localizer = Localizer.Instance;
			Service.Set(localizer);
			Language language = LocalizationLanguage.GetLanguage();
			localizer.Language = language;
			localizer.LanguageString = LocalizationLanguage.GetLanguageString(language);
			AppTokensFilePath tokensFilePath = new AppTokensFilePath(Localizer.DEFAULT_TOKEN_LOCATION, Platform.global);
			bool tokensLoaded = false;
			Service.Get<Localizer>().LoadTokensFromLocalJSON(tokensFilePath, delegate
			{
				Debug.Log("Tokens for " + localizer.LanguageString + " have been loaded.");
				tokensLoaded = true;
			});
			while (!tokensLoaded)
			{
				yield return null;
			}
			mixLoginCreateService = new MixLoginCreateService();
			Service.Set(mixLoginCreateService);
			IntegrationTestEx.FailIf(!mixLoginCreateService.NetworkConfigIsNotSet);
			mixLoginCreateService.SetNetworkConfig(networkServicesConfig);
			IntegrationTestEx.FailIf(mixLoginCreateService.NetworkConfigIsNotSet);
			yield return null;
		}

		protected CPDataEntityCollection createDataEntityCollection(string dataModelName)
		{
			return new CPDataEntityCollectionImpl();
		}

		protected override IEnumerator runTest()
		{
			yield break;
		}

		protected override void tearDown()
		{
		}

		private void initEnvironmentManager()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = typeof(EnvironmentManager).Name;
			GameObject gameObject2 = gameObject;
			gameObject2.transform.SetParent(base.gameObject.transform);
			EnvironmentManagerStandalone environmentManagerStandalone = gameObject2.AddComponent<EnvironmentManagerStandalone>();
			Service.Set(environmentManagerStandalone);
			environmentManagerStandalone.SetLogger(LoggerDelegate);
			environmentManagerStandalone.Initialize();
		}

		private static void LoggerDelegate(object sourceObject, string message, LogType logType)
		{
			switch (logType)
			{
			case LogType.Warning:
				break;
			case LogType.Log:
				break;
			case LogType.Exception:
				Log.LogFatal(sourceObject, message);
				break;
			case LogType.Error:
			case LogType.Assert:
				Log.LogError(sourceObject, message);
				break;
			}
		}
	}
}
