using NUnit.Framework.Api;
using System;
using System.Reflection;
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
	public class SetUpTearDownCommand : DelegatingTestCommand
	{
		private readonly MethodInfo[] setUpMethods;

		private readonly MethodInfo[] tearDownMethods;

		public SetUpTearDownCommand(TestCommand innerCommand)
			: base(innerCommand)
		{
			setUpMethods = base.Test.SetUpMethods;
			tearDownMethods = base.Test.TearDownMethods;
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			try
			{
				RunSetUpMethods(context);
				context.CurrentResult = innerCommand.Execute(context);
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
				{
					Thread.ResetAbort();
				}
				context.CurrentResult.RecordException(ex);
			}
			finally
			{
				RunTearDownMethods(context);
			}
			return context.CurrentResult;
		}

		private void RunSetUpMethods(TestExecutionContext context)
		{
			if (setUpMethods != null)
			{
				MethodInfo[] array = setUpMethods;
				foreach (MethodInfo methodInfo in array)
				{
					Reflect.InvokeMethod(methodInfo, methodInfo.IsStatic ? null : context.TestObject);
				}
			}
		}

		private void RunTearDownMethods(TestExecutionContext context)
		{
			try
			{
				if (tearDownMethods != null)
				{
					int num = tearDownMethods.Length;
					while (--num >= 0)
					{
						Reflect.InvokeMethod(tearDownMethods[num], tearDownMethods[num].IsStatic ? null : context.TestObject);
					}
				}
			}
			catch (Exception innerException)
			{
				if (innerException is NUnitException)
				{
					innerException = innerException.InnerException;
				}
				ResultState resultState = (context.CurrentResult.ResultState == ResultState.Cancelled) ? ResultState.Cancelled : ResultState.Error;
				string text = "TearDown : " + ExceptionHelper.BuildMessage(innerException);
				if (context.CurrentResult.Message != null)
				{
					text = context.CurrentResult.Message + Env.NewLine + text;
				}
				string text2 = "--TearDown" + Env.NewLine + ExceptionHelper.BuildStackTrace(innerException);
				if (context.CurrentResult.StackTrace != null)
				{
					text2 = context.CurrentResult.StackTrace + Env.NewLine + text2;
				}
				context.CurrentResult.SetResult(resultState, text, text2);
			}
		}
	}
}
