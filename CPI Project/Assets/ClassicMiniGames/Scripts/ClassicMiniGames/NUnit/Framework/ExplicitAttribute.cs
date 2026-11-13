using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExplicitAttribute : TestModificationAttribute, IApplyToTest
	{
		private string reason;

		public ExplicitAttribute()
		{
			reason = "";
		}

		public ExplicitAttribute(string reason)
		{
			this.reason = reason;
		}

		public void ApplyToTest(ITest test)
		{
			if (test.RunState != 0)
			{
				test.RunState = RunState.Explicit;
				test.Properties.Set(PropertyNames.SkipReason, reason);
			}
		}
	}
}
