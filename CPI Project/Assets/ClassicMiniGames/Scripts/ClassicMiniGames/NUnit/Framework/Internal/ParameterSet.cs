using NUnit.Framework.Api;
using System;

namespace NUnit.Framework.Internal
{
	public class ParameterSet : ITestCaseData, IApplyToTest
	{
		private object[] arguments;

		private object[] originalArguments;

		private object result;

		private bool hasExpectedResult;

		private ExpectedExceptionData exceptionData;

		private IPropertyBag properties;

		private RunState runState;

		private string testName;

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

		public object[] Arguments
		{
			get
			{
				return arguments;
			}
			set
			{
				arguments = value;
				if (originalArguments == null)
				{
					originalArguments = value;
				}
			}
		}

		public object[] OriginalArguments
		{
			get
			{
				return originalArguments;
			}
		}

		public bool ExceptionExpected
		{
			get
			{
				return exceptionData.ExpectedExceptionName != null;
			}
		}

		public ExpectedExceptionData ExceptionData
		{
			get
			{
				return exceptionData;
			}
		}

		public object ExpectedResult
		{
			get
			{
				return result;
			}
			set
			{
				result = value;
				hasExpectedResult = true;
			}
		}

		public bool HasExpectedResult
		{
			get
			{
				return hasExpectedResult;
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

		public ParameterSet(Exception exception)
		{
			RunState = RunState.NotRunnable;
			Properties.Set(PropertyNames.SkipReason, ExceptionHelper.BuildMessage(exception));
			Properties.Set(PropertyNames.ProviderStackTrace, ExceptionHelper.BuildStackTrace(exception));
		}

		public ParameterSet()
		{
			RunState = RunState.Runnable;
		}

		public ParameterSet(ITestCaseData data)
		{
			TestName = data.TestName;
			RunState = data.RunState;
			Arguments = data.Arguments;
			exceptionData = data.ExceptionData;
			if (data.HasExpectedResult)
			{
				ExpectedResult = data.ExpectedResult;
			}
			foreach (string key in data.Properties.Keys)
			{
				Properties[key] = data.Properties[key];
			}
		}

		public void ApplyToTest(ITest test)
		{
			if (RunState == RunState.Ignored || RunState == RunState.Explicit)
			{
				test.RunState = RunState;
			}
			foreach (string key in Properties.Keys)
			{
				foreach (object item in Properties[key])
				{
					test.Properties.Add(key, item);
				}
			}
			TestMethod testMethod = test as TestMethod;
			if (testMethod != null && exceptionData.ExpectedExceptionName != null)
			{
				testMethod.CustomDecorators.Add(new ExpectedExceptionDecorator(ExceptionData));
			}
		}
	}
}
