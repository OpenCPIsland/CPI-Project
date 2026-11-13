using NUnit.Framework.Api;
using System;
using System.Reflection;

namespace NUnit.Framework.Internal.Commands
{
	public class TestSuiteCommand : TestCommand
	{
		private readonly TestSuite suite;

		private readonly Type fixtureType;

		private readonly object[] arguments;

		public TestSuiteCommand(TestSuite test)
			: base(test)
		{
			suite = test;
			fixtureType = test.FixtureType;
			arguments = test.arguments;
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			throw new NotImplementedException("Execute is not implemented for TestSuiteCommand");
		}

		public virtual void DoOneTimeSetUp(TestExecutionContext context)
		{
			if (fixtureType == null)
			{
				return;
			}
			if (context.TestObject == null && !IsStaticClass(fixtureType))
			{
				context.TestObject = Reflect.Construct(fixtureType, arguments);
			}
			if (suite.OneTimeSetUpMethods != null)
			{
				MethodInfo[] oneTimeSetUpMethods = suite.OneTimeSetUpMethods;
				foreach (MethodInfo methodInfo in oneTimeSetUpMethods)
				{
					Reflect.InvokeMethod(methodInfo, methodInfo.IsStatic ? null : context.TestObject);
				}
			}
		}

		public virtual void DoOneTimeTearDown(TestExecutionContext context)
		{
			if (fixtureType != null)
			{
				TestSuiteResult testSuiteResult = context.CurrentResult as TestSuiteResult;
				try
				{
					if (suite.OneTimeTearDownMethods != null)
					{
						int num = suite.OneTimeTearDownMethods.Length;
						while (--num >= 0)
						{
							MethodInfo methodInfo = suite.OneTimeTearDownMethods[num];
							Reflect.InvokeMethod(methodInfo, methodInfo.IsStatic ? null : context.TestObject);
						}
					}
					IDisposable disposable = context.TestObject as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				catch (Exception innerException)
				{
					NUnitException ex = innerException as NUnitException;
					if (ex != null)
					{
						innerException = ex.InnerException;
					}
					string text = "TearDown : " + ExceptionHelper.BuildMessage(innerException);
					if (testSuiteResult.Message != null)
					{
						text = testSuiteResult.Message + Env.NewLine + text;
					}
					string text2 = "--TearDown" + Env.NewLine + ExceptionHelper.BuildStackTrace(innerException);
					if (testSuiteResult.StackTrace != null)
					{
						text2 = testSuiteResult.StackTrace + Env.NewLine + text2;
					}
					testSuiteResult.SetResult(ResultState.Error, text, text2);
				}
			}
		}

		private static bool IsStaticClass(Type type)
		{
			return type.IsAbstract && type.IsSealed;
		}
	}
}
