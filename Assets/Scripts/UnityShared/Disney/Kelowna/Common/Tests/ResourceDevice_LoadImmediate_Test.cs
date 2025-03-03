using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class ResourceDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "small_text?dl=res&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			ResourceDevice device = new ResourceDevice(deviceManager);
			deviceManager.Mount(device);
			TextAsset textAsset = deviceManager.LoadImmediate<TextAsset>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(textAsset == null);
			IntegrationTestEx.FailIf(textAsset.GetType() != typeof(TextAsset));
			IntegrationTestEx.FailIf(!textAsset.text.StartsWith("hello world"));
			IntegrationTest.Pass();
			yield break;
		}
	}
}
