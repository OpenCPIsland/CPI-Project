#define DEBUG
using NUnit.Framework.Api;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
	public class CommandRunner
	{
		public static TestResult Execute(TestCommand command)
		{
			TestExecutionContext.Save();
			TestExecutionContext currentContext = TestExecutionContext.CurrentContext;
			currentContext.CurrentTest = command.Test;
			currentContext.CurrentResult = command.Test.MakeTestResult();
			currentContext.Listener.TestStarted(command.Test);
			long ticks = DateTime.Now.Ticks;
			TestResult testResult;
			try
			{
				TestSuiteCommand testSuiteCommand = command as TestSuiteCommand;
				testResult = ((testSuiteCommand == null) ? command.Execute(currentContext) : ExecuteSuiteCommand(testSuiteCommand, currentContext));
				testResult.AssertCount = currentContext.AssertCount;
				long ticks2 = DateTime.Now.Ticks;
				double num2 = testResult.Time = (double)(ticks2 - ticks) / 10000000.0;
				currentContext.Listener.TestFinished(testResult);
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
				{
					Thread.ResetAbort();
				}
				currentContext.CurrentResult.RecordException(ex);
				return currentContext.CurrentResult;
			}
			finally
			{
				TestExecutionContext.Restore();
			}
			return testResult;
		}

		private static TestResult ExecuteSuiteCommand(TestSuiteCommand command, TestExecutionContext context)
		{
			TestSuiteResult testSuiteResult = context.CurrentResult as TestSuiteResult;
			Debug.Assert(testSuiteResult != null);
			bool flag = false;
			try
			{
				ApplyTestSettingsToExecutionContext(command.Test, context);
				command.DoOneTimeSetUp(context);
				flag = true;
				context.Update();
				testSuiteResult = RunChildCommands(command, context);
			}
			catch (Exception innerException)
			{
				if (innerException is NUnitException || innerException is TargetInvocationException)
				{
					innerException = innerException.InnerException;
				}
				if (flag)
				{
					testSuiteResult.RecordException(innerException);
				}
				else
				{
					testSuiteResult.RecordException(innerException, FailureSite.SetUp);
				}
			}
			finally
			{
				command.DoOneTimeTearDown(context);
			}
			return testSuiteResult;
		}

		public static TestSuiteResult RunChildCommands(TestSuiteCommand command, TestExecutionContext context)
		{
			TestSuiteResult testSuiteResult = TestExecutionContext.CurrentContext.CurrentResult as TestSuiteResult;
			testSuiteResult.SetResult(ResultState.Success);
			foreach (TestCommand child in command.Children)
			{
				TestResult testResult = Execute(child);
				testSuiteResult.AddResult(testResult);
				if (testResult.ResultState == ResultState.Cancelled || (testResult.ResultState.Status == TestStatus.Failed && TestExecutionContext.CurrentContext.StopOnError))
				{
					break;
				}
			}
			return testSuiteResult;
		}

		public static void ApplyTestSettingsToExecutionContext(Test test, TestExecutionContext context)
		{
			string text = (string)test.Properties.Get(PropertyNames.SetCulture);
			if (text != null)
			{
				context.CurrentCulture = new CultureInfo(text);
			}
			string text2 = (string)test.Properties.Get(PropertyNames.SetUICulture);
			if (text2 != null)
			{
				context.CurrentUICulture = new CultureInfo(text2);
			}
		}
	}
}
