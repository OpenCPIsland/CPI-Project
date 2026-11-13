using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class StringDevice_LoadAsync_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = string.Format("{0}?dl=str:mock-text-asset&x=txt", "Configuration/embedded_content_manifest");
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager manager = new DeviceManager();
			manager.Mount(new MockTextAssetDevice(manager));
			manager.Mount(new StringDevice(manager));
			AssetRequest<string> request = manager.LoadAsync<string>(entry.DeviceList, ref entry);
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
				string text = Resources.Load<TextAsset>(key).text;
				IntegrationTestEx.FailIf(request.Asset != text);
			}
			IntegrationTest.Pass();
		}
	}
}
