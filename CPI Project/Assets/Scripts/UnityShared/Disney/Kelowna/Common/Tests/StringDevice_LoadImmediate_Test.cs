using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class StringDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}?dl=str:mock-text-asset&x=txt", "Configuration/embedded_content_manifest");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new MockTextAssetDevice(deviceManager));
			deviceManager.Mount(new StringDevice(deviceManager));
			string text = deviceManager.LoadImmediate<string>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(text == null);
			IntegrationTestEx.FailIf(text == null);
			string key = entry.Key;
			string text2 = Resources.Load<TextAsset>(key).text;
			IntegrationTestEx.FailIf(text != text2);
			IntegrationTest.Pass();
			yield break;
		}
	}
}
