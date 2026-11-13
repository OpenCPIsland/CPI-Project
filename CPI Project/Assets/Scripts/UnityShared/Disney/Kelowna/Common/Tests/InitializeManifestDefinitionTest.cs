using Disney.Kelowna.Common.Manifest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class InitializeManifestDefinitionTest : BaseIntegrationTest
	{
		private ContentManifest mockResultingManifest;

		protected override IEnumerator setup()
		{
			mockResultingManifest = new ContentManifest(Resources.Load<TextAsset>("fixtures/mock_content_manifest"));
			yield break;
		}

		protected override IEnumerator runTest()
		{
			InitializeManifestDefinitionCommand initializeManifestDefinitionCommand = new InitializeManifestDefinitionCommand(new MockManifestService(), onInitializeManifestCommandComplete);
			wait();
			initializeManifestDefinitionCommand.Execute();
			yield break;
		}

		private void onInitializeManifestCommandComplete(ContentManifest mergedManifest, ScenePrereqContentBundlesManifest scenePrereqBundlesManifest, bool requiresAppUgrade, bool appUpgradeAvailable)
		{
			IntegrationTest.Assert(manifestsAreIdentical(mergedManifest));
			doneWaiting();
		}

		private bool manifestsAreIdentical(ContentManifest mergedManifest)
		{
			bool flag = mockResultingManifest.BaseUri == mergedManifest.BaseUri;
			bool flag2 = checkAssetsAreIndentical(mergedManifest);
			bool flag3 = checkAssetsAreIndentical(mergedManifest);
			return flag && flag2 && flag3;
		}

		private bool checkAssetsAreIndentical(ContentManifest mergedManifest)
		{
			Dictionary<string, ContentManifest.AssetEntry>.Enumerator enumerator = mergedManifest.AssetEntryMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ContentManifest.AssetEntry value = enumerator.Current.Value;
				ContentManifest.AssetEntry lhs = mockResultingManifest.AssetEntryMap[enumerator.Current.Key];
				if (lhs != value)
				{
					return false;
				}
			}
			return true;
		}

		private bool checkBundleAreIndentical(ContentManifest mergedManifest)
		{
			Dictionary<string, ContentManifest.BundleEntry>.Enumerator enumerator = mergedManifest.BundleEntryMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ContentManifest.BundleEntry value = enumerator.Current.Value;
				ContentManifest.BundleEntry lhs = mockResultingManifest.BundleEntryMap[enumerator.Current.Key];
				if (lhs != value)
				{
					return false;
				}
			}
			return true;
		}
	}
}
