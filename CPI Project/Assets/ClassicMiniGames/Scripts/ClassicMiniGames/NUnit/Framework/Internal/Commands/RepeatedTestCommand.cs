using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
	public class RepeatedTestCommand : DelegatingTestCommand
	{
		private int repeatCount;

		public RepeatedTestCommand(TestCommand innerCommand)
			: base(innerCommand)
		{
			repeatCount = base.Test.Properties.GetSetting(PropertyNames.RepeatCount, 1);
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			int num = repeatCount;
			while (num-- > 0)
			{
				context.CurrentResult = innerCommand.Execute(context);
				if (context.CurrentResult.ResultState == ResultState.Failure || context.CurrentResult.ResultState == ResultState.Error || context.CurrentResult.ResultState == ResultState.Cancelled)
				{
					break;
				}
			}
			return context.CurrentResult;
		}
	}
}
