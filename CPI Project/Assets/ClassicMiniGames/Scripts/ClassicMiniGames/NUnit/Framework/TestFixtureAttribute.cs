using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TestFixtureAttribute : TestModificationAttribute, IApplyToTest
	{
		private string description;

		private object[] originalArgs;

		private object[] constructorArgs;

		private Type[] typeArgs;

		private bool argsInitialized;

		private bool isIgnored;

		private string ignoreReason;

		private string category;

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

		public object[] Arguments
		{
			get
			{
				if (!argsInitialized)
				{
					InitializeArgs();
				}
				return constructorArgs;
			}
		}

		public bool Ignore
		{
			get
			{
				return isIgnored;
			}
			set
			{
				isIgnored = value;
			}
		}

		public string IgnoreReason
		{
			get
			{
				return ignoreReason;
			}
			set
			{
				ignoreReason = value;
				isIgnored = (ignoreReason != null && ignoreReason != string.Empty);
			}
		}

		public Type[] TypeArgs
		{
			get
			{
				if (!argsInitialized)
				{
					InitializeArgs();
				}
				return typeArgs;
			}
			set
			{
				typeArgs = value;
				argsInitialized = true;
			}
		}

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		public IList Categories
		{
			get
			{
				return (category == null) ? null : category.Split(',');
			}
		}

		public TestFixtureAttribute()
			: this(null)
		{
		}

		public TestFixtureAttribute(params object[] arguments)
		{
			originalArgs = ((arguments == null) ? new object[0] : arguments);
			constructorArgs = originalArgs;
			typeArgs = new Type[0];
		}

		private void InitializeArgs()
		{
			int num = 0;
			if (originalArgs != null)
			{
				object[] array = originalArgs;
				foreach (object obj in array)
				{
					if (obj is Type)
					{
						num++;
						continue;
					}
					break;
				}
			}
			typeArgs = new Type[num];
			for (int j = 0; j < num; j++)
			{
				typeArgs[j] = (Type)originalArgs[j];
			}
			int num2 = originalArgs.Length - num;
			constructorArgs = new object[num2];
			for (int j = 0; j < num2; j++)
			{
				constructorArgs[j] = originalArgs[num + j];
			}
			argsInitialized = true;
		}

		public void ApplyToTest(ITest test)
		{
			if (!test.Properties.ContainsKey(PropertyNames.Description) && description != null)
			{
				test.Properties.Set(PropertyNames.Description, description);
			}
			if (category != null)
			{
				string[] array = category.Split(',');
				foreach (string value in array)
				{
					test.Properties.Add(PropertyNames.Category, value);
				}
			}
		}
	}
}
