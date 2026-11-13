using NUnit.Framework.Api;
using System;

namespace NUnit.Framework.Internal.Commands
{
	public class MaxTimeCommand : DelegatingTestCommand
	{
		private int maxTime;

		public MaxTimeCommand(TestCommand innerCommand)
			: base(innerCommand)
		{
			maxTime = base.Test.Properties.GetSetting(PropertyNames.MaxTime, 0);
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			DateTime now = DateTime.Now;
			TestResult testResult = innerCommand.Execute(context);
			testResult.Time = (double)DateTime.Now.Subtract(now).Ticks / 10000000.0;
			if (testResult.ResultState == ResultState.Success)
			{
				int num = (int)Math.Round(testResult.Time * 1000.0);
				if (num > maxTime)
				{
					testResult.SetResult(ResultState.Failure, string.Format("Elapsed time of {0}ms exceeds maximum of {1}ms", num, maxTime));
				}
			}
			return testResult;
		}
	}
}
