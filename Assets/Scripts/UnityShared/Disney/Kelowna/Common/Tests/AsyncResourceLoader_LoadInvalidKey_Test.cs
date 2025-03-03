using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class AsyncResourceLoader_LoadInvalidKey_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			string payload = "invalid?dl=asset:res&x=txt";
			ContentManifest.AssetEntry entry = ContentManifest.AssetEntry.Parse(payload);
			try
			{
				AsycnResourceLoader<TextAsset>.Load(ref entry);
				IntegrationTest.Fail("Expected exception");
			}
			catch (ArgumentException)
			{
				IntegrationTest.Pass();
			}
			yield break;
		}
	}
}
