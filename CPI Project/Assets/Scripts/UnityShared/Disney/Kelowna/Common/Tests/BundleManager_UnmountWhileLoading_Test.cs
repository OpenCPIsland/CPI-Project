using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleManager_UnmountWhileLoading_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string key = "embedded_asset_test";
			TextAsset bundleTxt = Resources.Load<TextAsset>(key + ".unity3d");
			AssetBundle bundle = AssetBundle.LoadFromMemory(bundleTxt.bytes);
			BundleManager manager = Content.BundleManager;
			BundleMount mount = manager.MountBundle(key, bundle);
			AsyncAssetBundleRequest<TextAsset> assetRequest = mount.LoadAsync<TextAsset>("embeddedasseta", "embeddedasseta.txt");
			manager.UnmountBundle(key, false);
			if (manager.IsMounted(key))
			{
				IntegrationTest.Fail("Bundle should not be considered mounted while waiting to unmount.");
			}
			if (!manager.IsUnmounting(key))
			{
				IntegrationTest.Fail("Bundle should be in the process of unmounting.");
			}
			yield return assetRequest;
			if (assetRequest.Asset == null)
			{
				IntegrationTest.Fail("Failed to load asset");
			}
			yield return null;
			IntegrationTest.Assert(!manager.IsMounted(key));
			IntegrationTest.Assert(!manager.IsUnmounting(key));
		}
	}
}
