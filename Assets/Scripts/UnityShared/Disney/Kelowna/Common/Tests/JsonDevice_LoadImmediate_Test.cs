using System.Collections;

namespace Disney.Kelowna.Common.Tests
{
	public class JsonDevice_LoadImmediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}?dl=json:mock-json-res&x=txt", "Configuration/embedded_content_manifest");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager deviceManager = new DeviceManager();
			deviceManager.Mount(new MockJsonResourceDevice(deviceManager));
			deviceManager.Mount(new JsonDevice(deviceManager));
			ContentManifest contentManifest = deviceManager.LoadImmediate<ContentManifest>(entry.DeviceList, ref entry);
			IntegrationTestEx.FailIf(contentManifest == null);
			if (contentManifest != null)
			{
			}
			IntegrationTest.Pass();
			yield break;
		}
	}
}
