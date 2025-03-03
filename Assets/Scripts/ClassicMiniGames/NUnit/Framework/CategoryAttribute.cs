using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class CategoryAttribute : TestModificationAttribute, IApplyToTest
	{
		protected string categoryName;

		public string Name
		{
			get
			{
				return categoryName;
			}
		}

		public CategoryAttribute(string name)
		{
			categoryName = name.Trim();
		}

		protected CategoryAttribute()
		{
			categoryName = GetType().Name;
			if (categoryName.EndsWith("Attribute"))
			{
				categoryName = categoryName.Substring(0, categoryName.Length - 9);
			}
		}

		public void ApplyToTest(ITest test)
		{
			test.Properties.Add(PropertyNames.Category, Name);
			if (Name.IndexOfAny(new char[4]
			{
				',',
				'!',
				'+',
				'-'
			}) >= 0)
			{
				test.RunState = RunState.NotRunnable;
				test.Properties.Set(PropertyNames.SkipReason, "Category name must not contain ',', '!', '+' or '-'");
			}
		}
	}
}
