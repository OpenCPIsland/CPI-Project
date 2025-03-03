using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class PlatformAttribute : IncludeExcludeAttribute, IApplyToTest
	{
		private PlatformHelper platformHelper = new PlatformHelper();

		public PlatformAttribute()
		{
		}

		public PlatformAttribute(string platforms)
			: base(platforms)
		{
		}

		public void ApplyToTest(ITest test)
		{
			if (test.RunState != 0 && !platformHelper.IsPlatformSupported(this))
			{
				test.RunState = RunState.Skipped;
				test.Properties.Add(PropertyNames.SkipReason, platformHelper.Reason);
			}
		}
	}
}
