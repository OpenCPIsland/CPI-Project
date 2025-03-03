using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleManager_MountPinned_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string key = "embedded_asset_test";
			TextAsset bundleTxt = Resources.Load<TextAsset>(key + ".unity3d");
			AssetBundle bundle = AssetBundle.LoadFromMemory(bundleTxt.bytes);
			BundleManager manager = Content.BundleManager;
			BundleMount mount = manager.MountBundle(key, bundle);
			mount.IsPinned = true;
			if (!mount.IsPinned)
			{
				IntegrationTest.Fail("mount should be pinned");
			}
			yield return new WaitForFrame(Math.Max(manager.UnmountFrameCount, manager.MonitorFrameFrequency) + 1);
			IntegrationTest.Assert(mount.IsMounted);
			IntegrationTest.Assert(manager.IsMounted(key));
			mount.Unload(false);
		}
	}
}
