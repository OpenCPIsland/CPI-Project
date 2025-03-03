using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class FileBytesDevice_LoadAsync_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string testPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt");
			string payload = string.Format("{0}?dl=file-bytes&x=txt", testPath);
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			DeviceManager manager = new DeviceManager();
			manager.Mount(new FileBytesDevice(manager));
			AssetRequest<byte[]> request = manager.LoadAsync<byte[]>(entry.DeviceList, ref entry);
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
				IntegrationTestEx.FailIf(request.Asset == null);
				string path = "Configuration/embedded_content_manifest.json";
				IntegrationTestEx.FailIf(!Enumerable.SequenceEqual(second: Resources.Load<TextAsset>(path).bytes, first: request.Asset));
			}
			IntegrationTest.Pass();
		}
	}
}
