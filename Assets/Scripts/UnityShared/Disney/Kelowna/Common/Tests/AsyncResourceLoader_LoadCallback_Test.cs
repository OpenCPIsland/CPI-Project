using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class AsyncResourceLoader_LoadCallback_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "small_text?dl=asset:res&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			bool finished = false;
			AsycnResourceLoader<TextAsset>.Load(ref entry, delegate(string s, TextAsset a)
			{
				IntegrationTest.Assert(s != null);
				IntegrationTest.Assert(a != null);
				IntegrationTest.Assert(a.text.StartsWith("hello world"));
				finished = true;
			});
			while (!finished)
			{
				yield return null;
			}
		}
	}
}
