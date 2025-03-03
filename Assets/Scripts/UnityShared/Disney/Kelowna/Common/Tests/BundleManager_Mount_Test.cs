using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleManager_Mount_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string text = "embedded_asset_test";
			TextAsset textAsset = Resources.Load<TextAsset>(text + ".unity3d");
			AssetBundle bundle = AssetBundle.LoadFromMemory(textAsset.bytes);
			BundleManager bundleManager = Content.BundleManager;
			BundleMount bundleMount = bundleManager.MountBundle(text, bundle);
			IntegrationTest.Assert(bundleMount != null);
			bundle = bundleMount.Bundle;
			string[] allAssetNames = bundle.GetAllAssetNames();
			foreach (string text2 in allAssetNames)
			{
				Debug.LogWarningFormat("bundle asset {0}", text2);
			}
			IntegrationTest.Assert(bundle != null);
			IntegrationTest.Assert(bundle.GetAllAssetNames().Length == 2);
			IntegrationTest.Assert(bundle.Contains("assets/rootassets/embeddedasseta.txt"));
			IntegrationTest.Assert(bundle.Contains("assets/rootassets/embeddedassetb.txt"));
			IntegrationTest.Assert(bundleManager.IsMounted(text));
			bundleManager.UnmountBundle(text, false);
			IntegrationTest.Assert(!bundleManager.IsMounted(text));
			yield break;
		}
	}
}
