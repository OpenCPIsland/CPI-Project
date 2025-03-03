using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class IgnoreAttribute : TestModificationAttribute, IApplyToTest
	{
		private string reason;

		public IgnoreAttribute()
		{
			reason = "";
		}

		public IgnoreAttribute(string reason)
		{
			this.reason = reason;
		}

		public void ApplyToTest(ITest test)
		{
			if (test.RunState != 0)
			{
				test.RunState = RunState.Ignored;
				test.Properties.Set(PropertyNames.SkipReason, reason);
			}
		}
	}
}
