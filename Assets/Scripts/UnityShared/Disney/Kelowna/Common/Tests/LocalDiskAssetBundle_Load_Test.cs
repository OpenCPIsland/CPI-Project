using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class LocalDiskAssetBundle_Load_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string arg = PathUtil.Combine(Directory.GetCurrentDirectory(), "Assets/External/UnityShared/KelownaCommon/Core/Tests/Integration/ContentSystem/Resources/embedded_asset_test.unity3d.txt");
			string payload = string.Format("{0}embeddedasseta.txt?dl=bundle:create-bundle:file-bytes&b={1}&x=txt", "assets/rootassets/", arg);
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			BundleManager bundleManager = new BundleManager(null);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new BundleDevice(deviceManager, bundleManager));
			deviceManager.Mount(new FileBytesDevice(deviceManager));
			TextAsset textAsset = deviceManager.LoadImmediate<TextAsset>(entry.DeviceList, ref entry);
			IntegrationTest.Assert(textAsset != null);
			IntegrationTest.Assert(textAsset.text.StartsWith("embeddedasseta"));
			bundleManager.UnmountAllBundles();
			yield break;
		}
	}
}
