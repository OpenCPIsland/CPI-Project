using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TestAttribute : TestModificationAttribute, IApplyToTest
	{
		private string description;

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public void ApplyToTest(ITest test)
		{
			if (!test.Properties.ContainsKey(PropertyNames.Description) && description != null)
			{
				test.Properties.Set(PropertyNames.Description, description);
			}
		}
	}
}
