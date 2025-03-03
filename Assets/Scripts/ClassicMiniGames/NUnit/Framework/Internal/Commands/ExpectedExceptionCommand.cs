using NUnit.Framework.Api;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
	public class ExpectedExceptionCommand : DelegatingTestCommand
	{
		private ExpectedExceptionData exceptionData;

		public ExpectedExceptionCommand(TestCommand innerCommand, ExpectedExceptionData exceptionData)
			: base(innerCommand)
		{
			this.exceptionData = exceptionData;
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			try
			{
				context.CurrentResult = innerCommand.Execute(context);
				if (context.CurrentResult.ResultState == ResultState.Success)
				{
					ProcessNoException(context);
				}
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
				{
					Thread.ResetAbort();
				}
				ProcessException(ex, context);
			}
			return context.CurrentResult;
		}

		public void ProcessNoException(TestExecutionContext context)
		{
			context.CurrentResult.SetResult(ResultState.Failure, NoExceptionMessage());
		}

		public void ProcessException(Exception exception, TestExecutionContext context)
		{
			if (exception is NUnitException)
			{
				exception = exception.InnerException;
			}
			if (IsExpectedExceptionType(exception))
			{
				if (IsExpectedMessageMatch(exception))
				{
					MethodInfo exceptionHandler = exceptionData.GetExceptionHandler(context.TestObject.GetType());
					if (exceptionHandler != null)
					{
						Reflect.InvokeMethod(exceptionHandler, context.TestObject, exception);
					}
					else
					{
						IExpectException ex = context.TestObject as IExpectException;
						if (ex != null)
						{
							ex.HandleException(exception);
						}
					}
					context.CurrentResult.SetResult(ResultState.Success);
				}
				else
				{
					context.CurrentResult.SetResult(ResultState.Failure, WrongTextMessage(exception), GetStackTrace(exception));
				}
			}
			else
			{
				context.CurrentResult.RecordException(exception);
				if (context.CurrentResult.ResultState == ResultState.Error)
				{
					context.CurrentResult.SetResult(ResultState.Failure, WrongTypeMessage(exception), GetStackTrace(exception));
				}
			}
		}

		private bool IsExpectedExceptionType(Exception exception)
		{
			return exceptionData.ExpectedExceptionName == null || exceptionData.ExpectedExceptionName.Equals(exception.GetType().FullName);
		}

		private bool IsExpectedMessageMatch(Exception exception)
		{
			if (exceptionData.ExpectedMessage == null)
			{
				return true;
			}
			switch (exceptionData.MatchType)
			{
			default:
				return exceptionData.ExpectedMessage.Equals(exception.Message);
			case MessageMatch.Contains:
				return exception.Message.IndexOf(exceptionData.ExpectedMessage) >= 0;
			case MessageMatch.Regex:
				return Regex.IsMatch(exception.Message, exceptionData.ExpectedMessage);
			case MessageMatch.StartsWith:
				return exception.Message.StartsWith(exceptionData.ExpectedMessage);
			}
		}

		private string NoExceptionMessage()
		{
			string str = (exceptionData.ExpectedExceptionName == null) ? "An Exception" : exceptionData.ExpectedExceptionName;
			return CombineWithUserMessage(str + " was expected");
		}

		private string WrongTypeMessage(Exception exception)
		{
			return CombineWithUserMessage("An unexpected exception type was thrown" + Env.NewLine + "Expected: " + exceptionData.ExpectedExceptionName + Env.NewLine + " but was: " + exception.GetType().FullName + " : " + exception.Message);
		}

		private string WrongTextMessage(Exception exception)
		{
			string text;
			switch (exceptionData.MatchType)
			{
			default:
				text = "Expected: ";
				break;
			case MessageMatch.Contains:
				text = "Expected message containing: ";
				break;
			case MessageMatch.Regex:
				text = "Expected message matching: ";
				break;
			case MessageMatch.StartsWith:
				text = "Expected message starting: ";
				break;
			}
			return CombineWithUserMessage("The exception message text was incorrect" + Env.NewLine + text + exceptionData.ExpectedMessage + Env.NewLine + " but was: " + exception.Message);
		}

		private string CombineWithUserMessage(string message)
		{
			if (exceptionData.UserMessage == null)
			{
				return message;
			}
			return exceptionData.UserMessage + Env.NewLine + message;
		}

		private string GetStackTrace(Exception exception)
		{
			try
			{
				return exception.StackTrace;
			}
			catch (Exception)
			{
				return "No stack trace available";
			}
		}
	}
}
