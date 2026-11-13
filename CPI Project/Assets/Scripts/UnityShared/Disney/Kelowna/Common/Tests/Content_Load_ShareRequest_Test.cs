using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class Content_Load_ShareRequest_Test : BaseContentIntegrationTest
	{
		private AssetRequest<TextAsset> request_a;

		private AssetRequest<TextAsset> request_b;

		protected override IEnumerator runTest()
		{
			request_a = Content.LoadAsync<TextAsset>("small_text");
			request_b = Content.LoadAsync<TextAsset>("small_text");
			if (request_a != request_b)
			{
				IntegrationTest.Fail("The request objects should have been shared.");
				yield break;
			}
			yield return new CompositeCoroutineReturn(request_a, request_b);
			IntegrationTest.Assert(request_a.Asset != null);
			IntegrationTest.Assert(request_b.Asset != null);
			IntegrationTest.Assert(request_a.Asset == request_b.Asset);
			IntegrationTest.Assert(request_a.Asset.text == request_b.Asset.text);
		}
	}
}
