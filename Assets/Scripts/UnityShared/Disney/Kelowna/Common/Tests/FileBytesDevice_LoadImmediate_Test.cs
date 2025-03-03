using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class FileBytesDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string arg = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt");
			string payload = string.Format("{0}?dl=file-bytes&x=txt", arg);
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new FileBytesDevice(deviceManager));
			byte[] array = deviceManager.LoadImmediate<byte[]>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(array == null);
			string path = "Configuration/embedded_content_manifest.json";
			byte[] bytes = Resources.Load<TextAsset>(path).bytes;
			IntegrationTestEx.FailIf(!array.SequenceEqual(bytes));
			IntegrationTest.Pass();
			yield break;
		}
	}
}
