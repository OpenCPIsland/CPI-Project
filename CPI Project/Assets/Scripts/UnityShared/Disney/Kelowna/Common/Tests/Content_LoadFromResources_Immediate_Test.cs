using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class Content_LoadFromResources_Immediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			TextAsset textAsset = Content.LoadImmediate<TextAsset>("small_text");
			IntegrationTest.Assert(textAsset != null);
			IntegrationTest.Assert(textAsset.text.StartsWith("hello world"));
			yield break;
		}
	}
}
