using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	public class TestCaseData : ITestCaseData
	{
		private object[] arguments;

		private object expectedResult;

		private ExpectedExceptionData exceptionData;

		private IPropertyBag properties;

		private bool hasExpectedResult;

		private string testName;

		private RunState runState;

		public object[] Arguments
		{
			get
			{
				return arguments;
			}
		}

		public object ExpectedResult
		{
			get
			{
				return expectedResult;
			}
			set
			{
				expectedResult = value;
				HasExpectedResult = true;
			}
		}

		public bool HasExpectedResult
		{
			get
			{
				return hasExpectedResult;
			}
			set
			{
				hasExpectedResult = value;
			}
		}

		public ExpectedExceptionData ExceptionData
		{
			get
			{
				return exceptionData;
			}
		}

		public string TestName
		{
			get
			{
				return testName;
			}
			set
			{
				testName = value;
			}
		}

		public RunState RunState
		{
			get
			{
				return runState;
			}
			set
			{
				runState = value;
			}
		}

		public IPropertyBag Properties
		{
			get
			{
				if (properties == null)
				{
					properties = new PropertyBag();
				}
				return properties;
			}
		}

		public TestCaseData(params object[] args)
		{
			RunState = RunState.Runnable;
			if (args == null)
			{
				object[] array = arguments = new object[1];
			}
			else
			{
				arguments = args;
			}
		}

		public TestCaseData(object arg)
		{
			RunState = RunState.Runnable;
			arguments = new object[1]
			{
				arg
			};
		}

		public TestCaseData(object arg1, object arg2)
		{
			RunState = RunState.Runnable;
			arguments = new object[2]
			{
				arg1,
				arg2
			};
		}

		public TestCaseData(object arg1, object arg2, object arg3)
		{
			RunState = RunState.Runnable;
			arguments = new object[3]
			{
				arg1,
				arg2,
				arg3
			};
		}

		public TestCaseData Returns(object result)
		{
			ExpectedResult = result;
			return this;
		}

		public TestCaseData Throws(Type exceptionType)
		{
			exceptionData.ExpectedExceptionName = exceptionType.FullName;
			return this;
		}

		public TestCaseData Throws(string exceptionName)
		{
			exceptionData.ExpectedExceptionName = exceptionName;
			return this;
		}

		public TestCaseData SetName(string name)
		{
			TestName = name;
			return this;
		}

		public TestCaseData SetDescription(string description)
		{
			Properties.Set(PropertyNames.Description, description);
			return this;
		}

		public TestCaseData SetCategory(string category)
		{
			Properties.Add(PropertyNames.Category, category);
			return this;
		}

		public TestCaseData SetProperty(string propName, string propValue)
		{
			Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData SetProperty(string propName, int propValue)
		{
			Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData SetProperty(string propName, double propValue)
		{
			Properties.Add(propName, propValue);
			return this;
		}

		public TestCaseData Ignore()
		{
			RunState = RunState.Ignored;
			return this;
		}

		public TestCaseData Explicit()
		{
			RunState = RunState.Explicit;
			return this;
		}

		public TestCaseData Explicit(string reason)
		{
			RunState = RunState.Explicit;
			Properties.Set(PropertyNames.SkipReason, reason);
			return this;
		}

		public TestCaseData Ignore(string reason)
		{
			RunState = RunState.Ignored;
			Properties.Set(PropertyNames.SkipReason, reason);
			return this;
		}
	}
}
