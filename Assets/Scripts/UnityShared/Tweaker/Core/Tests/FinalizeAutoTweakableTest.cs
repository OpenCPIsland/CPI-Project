using System;
using System.Collections;
using Tweaker.AssemblyScanner;
using UnityEngine;

namespace Tweaker.Core.Tests
{
	public class FinalizeAutoTweakableTest : MonoBehaviour
	{
		public class TestClass : IDisposable
		{
			[Tweakable("TestClass.AutoInt")]
			[IsTest]
			[TweakerRange(0, 10)]
			public Tweakable<int> AutoInt = new Tweakable<int>();

			private bool disposeTweakable;

			public TestClass(bool disposeTweakable)
			{
				this.disposeTweakable = disposeTweakable;
				AutoTweakable.Bind(this);
			}

			public void Dispose()
			{
				if (disposeTweakable && AutoInt != null)
				{
					AutoInt.Dispose();
				}
				else
				{
					AutoInt = null;
				}
			}
		}

		private Tweaker tweaker;

		private IEnumerator Start()
		{
			IScanner scanner = new Scanner();
			tweaker = new Tweaker();
			TweakerOptions options = TweakerOptions.GetDefaultWithAdditionalFlags(TweakerOptionFlags.IncludeTests);
			tweaker.Init(options, scanner);
			AutoTweakable.Manager = tweaker.Tweakables;
			new TestClass(false);
			ITweakable tweakable = tweaker.Tweakables.GetTweakable(new SearchOptions("TestClass.AutoInt#"));
			IntegrationTest.Assert(tweakable != null);
			uint counter = 0u;
			while (tweaker.Tweakables.GetTweakable(new SearchOptions("TestClass.AutoInt#")) != null)
			{
				GC.Collect();
				counter++;
				if (counter > 1000)
				{
					IntegrationTest.Fail("Failed to finalize AutoTweakable after " + counter + " frames.");
					yield break;
				}
				yield return null;
			}
			IntegrationTest.Pass();
		}
	}
}
