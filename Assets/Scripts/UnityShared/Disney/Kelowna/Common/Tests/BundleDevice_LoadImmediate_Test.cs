using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BundleDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}embeddedasseta.txt?dl=bundle:mock-create-bundle&b=embedded_asset_test&x=.txt", "assets/rootassets/");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			BundleManager bundleManager = new BundleManager(null);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new BundleDevice(deviceManager, bundleManager));
			TextAsset textAsset = deviceManager.LoadImmediate<TextAsset>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(textAsset == null, "asset was null");
			if (textAsset != null)
			{
				IntegrationTestEx.FailIf(!textAsset.text.StartsWith("embeddedasset"));
			}
			bundleManager.UnmountAllBundles();
			IntegrationTest.Pass();
			yield break;
		}
	}
}
