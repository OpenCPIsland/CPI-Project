using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class Content_LoadFromBundle_Immediate_Test : BaseContentIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			TextAsset textAsset = Content.LoadImmediate<TextAsset>("embeddedasseta");
			IntegrationTest.Assert(textAsset != null);
			IntegrationTest.Assert(textAsset.text.StartsWith("embeddedasseta"));
			yield break;
		}
	}
}
