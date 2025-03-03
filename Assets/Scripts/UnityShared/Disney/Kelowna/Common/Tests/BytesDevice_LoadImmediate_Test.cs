using System.Collections;
using System.Linq;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BytesDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}?dl=bytes:mock-text-asset", "Configuration/embedded_content_manifest");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new MockTextAssetDevice(deviceManager));
			deviceManager.Mount(new BytesDevice(deviceManager));
			byte[] array = deviceManager.LoadImmediate<byte[]>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(array == null);
			IntegrationTestEx.FailIf(array == null);
			string key = entry.Key;
			byte[] bytes = Resources.Load<TextAsset>(key).bytes;
			IntegrationTestEx.FailIf(!array.SequenceEqual(bytes));
			IntegrationTest.Pass();
			yield break;
		}
	}
}
