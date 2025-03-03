using System.Collections;
using System.Linq;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class BytesDevice_LoadAsync_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}?dl=bytes:mock-text-asset", "Configuration/embedded_content_manifest");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager manager = new DeviceManager();
			manager.Mount(new MockTextAssetDevice(manager));
			manager.Mount(new BytesDevice(manager));
			AssetRequest<byte[]> request = manager.LoadAsync<byte[]>(entry.DeviceList, ref entry);
			if (request == null)
			{
				IntegrationTest.Fail("request == null");
				yield break;
			}
			yield return request;
			IntegrationTestEx.FailIf(!request.Finished);
			IntegrationTestEx.FailIf(request.Cancelled);
			IntegrationTestEx.FailIf(request.Asset == null);
			IntegrationTestEx.FailIf(request.Asset == null);
			if (request.Asset != null)
			{
				string key = entry.Key;
				byte[] bytes = Resources.Load<TextAsset>(key).bytes;
				IntegrationTestEx.FailIf(!request.Asset.SequenceEqual(bytes));
			}
			IntegrationTest.Pass();
		}
	}
}
