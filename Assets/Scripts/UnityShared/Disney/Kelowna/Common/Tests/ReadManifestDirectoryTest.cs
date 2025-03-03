using Disney.Kelowna.Common.Environment;
using Disney.Kelowna.Common.Manifest;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class ReadManifestDirectoryTest : BaseIntegrationTest
	{
		[Serializable]
		public class SampleAssetEntry
		{
			public string Key;

			public string Uri;

			public string Scheme;
		}

		[Serializable]
		public class ExpectedResultType
		{
			public string TestName;

			public string ClientVersion;

			public string Platform;

			public string ContentDate;

			public Disney.Kelowna.Common.Environment.Environment Environment;

			[Space]
			public bool RequiresAppUgrade;

			public int AssetEntriesSize;

			public int BundleEntriesSize;

			public SampleAssetEntry[] SampleAssetEntries;

			public string BaseUri;

			public string Validate(ContentManifest mergedManifest, bool requiresAppUgrade)
			{
				if (RequiresAppUgrade != requiresAppUgrade)
				{
					return string.Format("'{0}': RequiresAppUgrade was expected to be '{1}' but was '{2}'!", TestName, RequiresAppUgrade, requiresAppUgrade);
				}
				if (AssetEntriesSize != mergedManifest.AssetEntryMap.Count)
				{
					return string.Format("'{0}': The Asset Entries Count was expected to be '{1}' but was '{2}'!", TestName, AssetEntriesSize, mergedManifest.AssetEntryMap.Count);
				}
				if (BundleEntriesSize != mergedManifest.BundleEntryMap.Count)
				{
					return string.Format("'{0}': The Bundle Entries Count was expected to be '{1}' but was '{2}'!", TestName, BundleEntriesSize, mergedManifest.BundleEntryMap.Count);
				}
				if (BaseUri != mergedManifest.BaseUri)
				{
					return string.Format("'{0}': The Base URI was expected to be '{1}' but was '{2}'!", TestName, BaseUri, mergedManifest.BaseUri);
				}
				return null;
			}
		}

		public ExpectedResultType ExpectedResult;

		protected override IEnumerator setup()
		{
			IGcsAccessTokenService gcsAccessTokenService = new GcsAccessTokenService(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountName"), new GcsP12AssetFileLoader(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountFile")));
			Service.Set(gcsAccessTokenService);
			ICPipeManifestService instance = new MockCPipeManifestServices(gcsAccessTokenService);
			Service.Set(instance);
			yield break;
		}

		protected override void tearDown()
		{
		}

		protected override IEnumerator runTest()
		{
			InitializeManifestDefinitionCommand initializeManifestDefinitionCommand = new InitializeManifestDefinitionCommand(new MockManifestService2(ExpectedResult.ClientVersion, ExpectedResult.Platform, CommonDateTime.Deserialize(ExpectedResult.ContentDate), ExpectedResult.Environment), onInitializeManifestCommandComplete);
			wait();
			initializeManifestDefinitionCommand.Execute();
			yield break;
		}

		private void onInitializeManifestCommandComplete(ContentManifest mergedManifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest, bool requiresAppUgrade, bool appUpgradeAvailable)
		{
			string text = ExpectedResult.Validate(mergedManifest, requiresAppUgrade);
			if (!string.IsNullOrEmpty(text))
			{
				Log.LogError(this, text);
				IntegrationTest.Fail(text);
			}
			else
			{
				IntegrationTest.Pass();
				Log.LogErrorFormatted(this, "ReadManifestDirectoryTest: TEST '{0}' SUCCEEDED.", ExpectedResult.TestName);
			}
			doneWaiting();
		}
	}
}
