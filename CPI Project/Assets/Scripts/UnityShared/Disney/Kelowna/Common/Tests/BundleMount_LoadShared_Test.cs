using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleMount_LoadShared_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string key = "embedded_asset_test";
			TextAsset bundleTxt = Resources.Load<TextAsset>(key + ".unity3d");
			AssetBundle bundle = AssetBundle.LoadFromMemory(bundleTxt.bytes);
			BundleMount mount = new BundleMount(bundle, "embedded_asset_test", null);
			if (mount.ActiveRequestCount != 0)
			{
				IntegrationTest.Fail("ActiveRequestCount should start at 0.");
			}
			AsyncAssetBundleRequest<TextAsset> requestA = mount.LoadAsync<TextAsset>("embeddedasseta", "embeddedasseta.txt");
			AsyncAssetBundleRequest<TextAsset> requestB = mount.LoadAsync<TextAsset>("embeddedasseta", "embeddedasseta.txt");
			if (mount.ActiveRequestCount != 1)
			{
				IntegrationTest.Fail("ActiveRequestCount should be at 1 after LoadAsync even if shared.");
			}
			bool finishedLoadingWasCalled = false;
			mount.EFinishedLoading += delegate
			{
				finishedLoadingWasCalled = true;
			};
			yield return requestA;
			yield return null;
			IntegrationTest.Assert(requestA == requestB);
			IntegrationTest.Assert(finishedLoadingWasCalled);
			TextAsset asset = requestA.Asset;
			IntegrationTest.Assert(asset != null);
			IntegrationTest.Assert(asset.text.StartsWith("embeddedasseta"));
			IntegrationTest.Assert(mount.ActiveRequestCount == 0);
			mount.Unload(false);
		}
	}
}
