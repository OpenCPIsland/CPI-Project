using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class AsyncResourceLoader_Load_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "small_text.txt?dl=asset:res&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			AssetRequest<TextAsset> request = AsycnResourceLoader<TextAsset>.Load(ref entry);
			yield return request;
			IntegrationTest.Assert(request.Asset != null);
			IntegrationTest.Assert(request.Asset.text.StartsWith("hello world"));
		}
	}
}
