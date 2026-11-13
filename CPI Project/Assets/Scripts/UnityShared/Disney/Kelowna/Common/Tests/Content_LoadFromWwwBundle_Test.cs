using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class Content_LoadFromWwwBundle_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			AssetRequest<GameObject> request = Content.LoadAsync<GameObject>("test/test_cube");
			IntegrationTestEx.FailIf(request == null);
			yield return request;
			IntegrationTest.Assert(request.Finished);
			IntegrationTest.Assert(!request.Cancelled);
			IntegrationTest.Assert(request.Asset != null);
			IntegrationTest.Assert(request.Asset.GetType() == typeof(GameObject));
		}
	}
}
