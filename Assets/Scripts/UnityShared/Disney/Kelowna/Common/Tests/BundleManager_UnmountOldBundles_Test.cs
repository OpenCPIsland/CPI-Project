using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleManager_UnmountOldBundles_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string key = "embedded_asset_test";
			TextAsset bundleTxt = Resources.Load<TextAsset>(key + ".unity3d");
			AssetBundle bundle = AssetBundle.LoadFromMemory(bundleTxt.bytes);
			BundleManager manager = Content.BundleManager;
			BundleMount mount = manager.MountBundle(key, bundle);
			if (mount.IsPinned)
			{
				IntegrationTest.Fail("mount should not be pinned");
			}
			if (!mount.IsMounted)
			{
				IntegrationTest.Fail("mount should be mounted.");
			}
			while (mount.IsMounted || manager.IsMounted(key))
			{
				yield return null;
			}
			IntegrationTest.Pass();
		}
	}
}
