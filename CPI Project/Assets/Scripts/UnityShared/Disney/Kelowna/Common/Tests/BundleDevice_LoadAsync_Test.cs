using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleDevice_LoadAsync_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}embeddedasseta.txt?dl=bundle:mock-create-bundle&b=embedded_asset_test&x=.txt", "assets/rootassets/");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			BundleManager bundleManager = new BundleManager(null);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new BundleDevice(deviceManager, bundleManager));
			AssetRequest<TextAsset> request = deviceManager.LoadAsync<TextAsset>(entry.DeviceList, ref entry);
			if (request == null)
			{
				IntegrationTest.Fail("request == null");
			}
			else
			{
				if (request.Finished)
				{
					IntegrationTest.Fail("Asset did not load async");
				}
				else if (request.Cancelled)
				{
					IntegrationTest.Fail("request should not be cancelled");
				}
				yield return request;
				IntegrationTestEx.FailIf(!request.Finished);
				IntegrationTestEx.FailIf(request.Cancelled);
				IntegrationTestEx.FailIf(request.Asset == null);
				if (request.Asset != null)
				{
					IntegrationTestEx.FailIf(!request.Asset.text.StartsWith("embeddedasset"));
				}
				IntegrationTest.Pass();
			}
			bundleManager.UnmountAllBundles();
		}
	}
}
