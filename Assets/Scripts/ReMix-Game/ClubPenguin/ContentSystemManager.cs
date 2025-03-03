using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Environment;
using Disney.Kelowna.Common.Manifest;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class ContentSystemManager
	{
		public struct ContentSystemResponse
		{
			public readonly ContentManifest Manifest;

			public readonly ScenePrereqContentBundlesManifest ScenePrereq;

			public readonly bool RequiresUpgrade;

			public readonly bool AppUpgradeAvailable;

			public ContentSystemResponse(ContentManifest manifest, ScenePrereqContentBundlesManifest scenePrereq, bool requiresUpgrade, bool appUpgradeAvailable)
			{
				Manifest = manifest;
				ScenePrereq = scenePrereq;
				RequiresUpgrade = requiresUpgrade;
				AppUpgradeAvailable = appUpgradeAvailable;
			}
		}

		private const int EXPIRY_TIME_IN_SECS = 60;

		private string currentContentVersion;

		private string currentContentManifestHash;

		private ContentSystemResponse? cachedContentSystemResponse;

		private DateTime lastUpdate;

		private readonly ApplicationService appService;

		private Content contentSystem;

		public ContentSystemManager(ApplicationService applicationService)
		{
			currentContentVersion = "";
			currentContentManifestHash = "";
			appService = applicationService;
		}

		private bool loadRequestTimeoutExpired()
		{
			return (DateTime.Now - lastUpdate).TotalSeconds > 60.0;
		}

		public IEnumerator InitContentSystem()
		{
			bool completed = false;
			InitializeManifestDefinitionCommand command = new InitializeManifestDefinitionCommand(getManifestServiceForEnvironment(), delegate(ContentManifest manifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest, bool requiresUpgrade, bool appUpgradeAvailable)
			{
				onCommandComplete(manifest, scenePrereqBundlesManifest, requiresUpgrade, appUpgradeAvailable);
				completed = true;
			});
			command.Execute();
			yield return new WaitUntil(() => completed && Service.IsSet<BundlePrecacheManager>() && Service.Get<BundlePrecacheManager>().IsReady);
		}

		private IManifestService getManifestServiceForEnvironment()
		{
			Disney.Kelowna.Common.Environment.Environment environment = Disney.Kelowna.Common.Environment.Environment.PRODUCTION;
			return new ManifestService();
		}

		public IEnumerator UpdateContentSystem(bool forceUpdate)
		{
			if (!forceUpdate && !loadRequestTimeoutExpired())
			{
				yield break;
			}
			if (!Service.IsSet<Content>())
			{
				Log.LogError(this, "The Content system is not available, skipping check to update the ContentManifest");
				yield break;
			}
			bool completed = false;
			if (cachedContentSystemResponse.HasValue)
			{
				onCommandComplete(cachedContentSystemResponse.Value.Manifest, cachedContentSystemResponse.Value.ScenePrereq, cachedContentSystemResponse.Value.RequiresUpgrade, cachedContentSystemResponse.Value.AppUpgradeAvailable);
				cachedContentSystemResponse = null;
				completed = true;
			}
			else
			{
				InitializeManifestDefinitionCommand initializeManifestDefinitionCommand = new InitializeManifestDefinitionCommand(getManifestServiceForEnvironment(), delegate(ContentManifest manifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest, bool requiresUpgrade, bool appUpgradeAvailable)
				{
					onCommandComplete(manifest, scenePrereqBundlesManifest, requiresUpgrade, appUpgradeAvailable);
					completed = true;
				});
				initializeManifestDefinitionCommand.Execute();
			}
			yield return new WaitUntil(() => completed && Service.IsSet<BundlePrecacheManager>() && Service.Get<BundlePrecacheManager>().IsReady);
		}

		private void onCommandComplete(ContentManifest manifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest, bool requiresUpgrade, bool appUpgradeAvailable)
		{
			appService.RequiresUpdate = requiresUpgrade;
			appService.UpdateAvailable = appUpgradeAvailable;
			string languageString = Service.Get<Localizer>().LanguageString;
			if (!Service.IsSet<Content>())
			{
				contentSystem = new Content(manifest, languageString);
				Service.Set(contentSystem);
				Service.Set(new BundlePrecacheManager(manifest));
				Service.Set(new ScenePrereqContentManager(Service.Get<BundlePrecacheManager>(), scenePrereqBundlesManifest));
				currentContentVersion = manifest.ContentVersion;
				currentContentManifestHash = manifest.ContentManifestHash;
			}
			else if (Service.Get<INetworkServicesManager>().IsGameServerConnected())
			{
				cachedContentSystemResponse = new ContentSystemResponse(manifest, scenePrereqBundlesManifest, requiresUpgrade, appUpgradeAvailable);
			}
			else if (!requiresUpgrade && (!currentContentVersion.Equals(manifest.ContentVersion) || !currentContentManifestHash.Equals(manifest.ContentManifestHash)))
			{
				if (!currentContentVersion.Equals(manifest.ContentVersion))
				{
					currentContentVersion = manifest.ContentVersion;
				}
				if (!currentContentManifestHash.Equals(manifest.ContentManifestHash))
				{
					currentContentManifestHash = manifest.ContentManifestHash;
				}
				contentSystem.UpdateContentManifest(manifest);
				Service.Get<BundlePrecacheManager>().SetManifest(manifest);
				Service.Get<ScenePrereqContentManager>().SetPrereqs(Service.Get<BundlePrecacheManager>(), scenePrereqBundlesManifest);
			}
			lastUpdate = DateTime.Now;
		}
	}
}
