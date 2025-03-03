using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleDevice_LoadAsyncWhileBundleLoading_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("embeddedasseta?dl=bundle:mock-create-bundle&b=embedded_asset_test&x=.txt");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			BundleManager bundleManager = new BundleManager(null);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new BundleDevice(deviceManager, bundleManager));
			AssetRequest<TextAsset> requestA = deviceManager.LoadAsync<TextAsset>(entry.DeviceList, ref entry);
			AssetRequest<TextAsset> requestB = deviceManager.LoadAsync<TextAsset>(entry.DeviceList, ref entry);
			if (requestA == null || requestB == null)
			{
				IntegrationTest.Fail("requestA == null");
			}
			else
			{
				yield return requestA;
				yield return requestB;
				IntegrationTestEx.FailIf(requestA.Asset == null);
				IntegrationTestEx.FailIf(requestB.Asset == null);
				IntegrationTest.Pass();
			}
			bundleManager.UnmountAllBundles();
		}
	}
}
