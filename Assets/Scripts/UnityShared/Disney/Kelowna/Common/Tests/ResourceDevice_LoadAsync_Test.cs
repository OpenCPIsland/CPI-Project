using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class ResourceDevice_LoadAsync_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "small_text?dl=res&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager manager = new DeviceManager();
			ResourceDevice device = new ResourceDevice(manager);
			manager.Mount(device);
			AssetRequest<TextAsset> request = manager.LoadAsync<TextAsset>(entry.DeviceList, ref entry);
			if (request == null)
			{
				IntegrationTest.Fail("request == null");
				yield break;
			}
			if (request.Finished)
			{
				IntegrationTest.Fail("Asset did not load async");
			}
			else if (request.Cancelled)
			{
				IntegrationTest.Fail("request should not be cancelled");
			}
			yield return request;
			IntegrationTestEx.FailIf(!request.Finished);
			IntegrationTestEx.FailIf(request.Cancelled);
			IntegrationTestEx.FailIf(request.Asset == null);
			IntegrationTestEx.FailIf(request.Asset.GetType() != typeof(TextAsset));
			if (request.Asset != null)
			{
				IntegrationTestEx.FailIf(!request.Asset.text.StartsWith("hello world"));
			}
			IntegrationTest.Pass();
		}
	}
}
