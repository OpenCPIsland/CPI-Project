using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
	public class SkipCommand : TestCommand
	{
		public SkipCommand(Test test)
			: base(test)
		{
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			TestResult testResult = base.Test.MakeTestResult();
			switch (base.Test.RunState)
			{
			default:
				testResult.SetResult(ResultState.Skipped, GetSkipReason());
				break;
			case RunState.Ignored:
				testResult.SetResult(ResultState.Ignored, GetSkipReason());
				break;
			case RunState.NotRunnable:
				testResult.SetResult(ResultState.NotRunnable, GetSkipReason(), GetProviderStackTrace());
				break;
			}
			return testResult;
		}

		private string GetSkipReason()
		{
			return (string)base.Test.Properties.Get(PropertyNames.SkipReason);
		}

		private string GetProviderStackTrace()
		{
			return (string)base.Test.Properties.Get(PropertyNames.ProviderStackTrace);
		}
	}
}
