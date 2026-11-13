using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class FileStringDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string arg = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt");
			string payload = string.Format("{0}?dl=file-string&x=txt", arg);
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new FileStringDevice(deviceManager));
			string text = deviceManager.LoadImmediate<string>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(string.IsNullOrEmpty(text));
			string path = "Configuration/embedded_content_manifest.json";
			string text2 = Resources.Load<TextAsset>(path).text;
			IntegrationTestEx.FailIf(!text.SequenceEqual(text2));
			IntegrationTest.Pass();
			yield break;
		}
	}
}
