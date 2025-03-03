using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleMount_DependenciesCreate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			BundleMount bundleMount = loadAndMount("DependencyBundlesTest/main.unity3d", null, false);
			BundleMount bundleMount2 = loadAndMount("DependencyBundlesTest/dep1.unity3d", new HashSet<BundleMount>
			{
				bundleMount
			}, true);
			BundleMount bundleMount3 = loadAndMount("DependencyBundlesTest/dep2.unity3d", new HashSet<BundleMount>
			{
				bundleMount2,
				bundleMount
			}, true);
			bundleMount.IsPinned = true;
			IntegrationTest.Assert(bundleMount.IsPinned, "DependencyBundlesTest/main.unity3d");
			IntegrationTest.Assert(bundleMount2.IsPinned, "DependencyBundlesTest/dep1.unity3d");
			IntegrationTest.Assert(bundleMount3.IsPinned, "DependencyBundlesTest/dep2.unity3d");
			bundleMount.IsPinned = false;
			IntegrationTest.Assert(!bundleMount.IsPinned, "DependencyBundlesTest/main.unity3d");
			IntegrationTest.Assert(bundleMount2.IsPinned, "DependencyBundlesTest/dep1.unity3d");
			IntegrationTest.Assert(bundleMount3.IsPinned, "DependencyBundlesTest/dep2.unity3d");
			bundleMount3.IsPinned = true;
			IntegrationTest.Assert(!bundleMount.IsPinned, "DependencyBundlesTest/main.unity3d");
			IntegrationTest.Assert(bundleMount2.IsPinned, "DependencyBundlesTest/dep1.unity3d");
			IntegrationTest.Assert(bundleMount3.IsPinned, "DependencyBundlesTest/dep2.unity3d");
			bundleMount.Unload(true);
			bundleMount2.Unload(true);
			bundleMount3.Unload(true);
			yield break;
		}

		private BundleMount loadAndMount(string key, HashSet<BundleMount> dependentMounts, bool isDependency)
		{
			if (dependentMounts == null)
			{
				dependentMounts = new HashSet<BundleMount>();
			}
			TextAsset textAsset = Resources.Load<TextAsset>(key);
			AssetBundle assetBundle = AssetBundle.LoadFromMemory(textAsset.bytes);
			BundleMount bundleMount = new BundleMount(assetBundle, key, dependentMounts);
			IntegrationTest.Assert(bundleMount.IsMounted, key);
			IntegrationTest.Assert(bundleMount.Key == key, key);
			IntegrationTest.Assert(bundleMount.DependentMounts == dependentMounts, key);
			IntegrationTest.Assert(bundleMount.ActiveRequestCount == 0, key);
			IntegrationTest.Assert(bundleMount.AssetCount == assetBundle.GetAllAssetNames().Length, key);
			IntegrationTest.Assert(bundleMount.Bundle == assetBundle, key);
			IntegrationTest.Assert(bundleMount.IsDependency == isDependency, key);
			IntegrationTest.Assert(bundleMount.IsPinned == isDependency, key);
			return bundleMount;
		}
	}
}
