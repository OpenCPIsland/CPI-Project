using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class Content_LoadFromResources_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			AssetRequest<TextAsset> request = Content.LoadAsync<TextAsset>("small_text");
			if (request == null)
			{
				IntegrationTest.Fail("request == null");
				yield break;
			}
			yield return request;
			IntegrationTest.Assert(request.Finished);
			IntegrationTest.Assert(!request.Cancelled);
			IntegrationTest.Assert(request.Asset != null);
			IntegrationTest.Assert(request.Asset.GetType() == typeof(TextAsset));
			if (request.Asset != null)
			{
				IntegrationTest.Assert(request.Asset.text.StartsWith("hello world"));
			}
		}
	}
}
