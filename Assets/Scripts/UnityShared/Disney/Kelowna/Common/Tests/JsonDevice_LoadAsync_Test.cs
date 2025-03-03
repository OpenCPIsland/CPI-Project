using System.Collections;

namespace Disney.Kelowna.Common.Tests
{
	public class JsonDevice_LoadAsync_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}?dl=json:mock-json-res&x=txt", "Configuration/embedded_content_manifest");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager manager = new DeviceManager();
			manager.Mount(new MockJsonResourceDevice(manager));
			manager.Mount(new JsonDevice(manager));
			AssetRequest<ContentManifest> request = manager.LoadAsync<ContentManifest>(entry.DeviceList, ref entry);
			if (request == null)
			{
				IntegrationTest.Fail("request == null");
			}
			else
			{
				yield return request;
				IntegrationTestEx.FailIf(!request.Finished);
				IntegrationTestEx.FailIf(request.Cancelled);
				IntegrationTestEx.FailIf(request.Asset == null);
				if (request.Asset == null)
				{
				}
			}
			IntegrationTest.Pass();
		}
	}
}
