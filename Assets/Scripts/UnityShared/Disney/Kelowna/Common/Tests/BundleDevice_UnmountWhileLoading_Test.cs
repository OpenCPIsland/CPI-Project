using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleDevice_UnmountWhileLoading_Test : BaseContentIntegrationTest
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
			else if (bundleManager.IsMounted(entry.BundleKey))
			{
				IntegrationTest.Fail("Bundle will not be mounted untill after the AssetBundle is created internally.");
			}
			else
			{
				yield return request;
				IntegrationTest.Assert(bundleManager.IsMounted(entry.BundleKey));
				IntegrationTest.Assert(request.Asset != null);
				bundleManager.UnmountBundle(entry.BundleKey, false);
			}
			bundleManager.UnmountAllBundles();
		}
	}
}
