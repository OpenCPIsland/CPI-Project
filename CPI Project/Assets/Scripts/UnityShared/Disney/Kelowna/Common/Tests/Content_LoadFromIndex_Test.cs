using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class Content_LoadFromIndex_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			TextAsset x = Content.LoadImmediate<TextAsset>("small_text");
			TextAsset y = Content.LoadImmediate<TextAsset>("small_text");
			IntegrationTest.Assert(x == y);
			yield break;
		}
	}
}
