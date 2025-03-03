using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class ResourceLoader_Load_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "small_text?dl=asset:res&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			TextAsset textAsset = ResourceLoader<TextAsset>.Load(ref entry);
			IntegrationTest.Assert(textAsset != null);
			IntegrationTest.Assert(textAsset.text.StartsWith("hello world"));
			yield break;
		}
	}
}
