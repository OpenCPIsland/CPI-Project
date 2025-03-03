using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnitLite
{
	public class ResultSummary
	{
		private int testCount;

		private int errorCount;

		private int failureCount;

		private int notRunCount;

		public int TestCount
		{
			get
			{
				return testCount;
			}
		}

		public int ErrorCount
		{
			get
			{
				return errorCount;
			}
		}

		public int FailureCount
		{
			get
			{
				return failureCount;
			}
		}

		public int NotRunCount
		{
			get
			{
				return notRunCount;
			}
		}

		public ResultSummary(NUnit.Framework.Api.ITestResult result)
		{
			Visit(result);
		}

		private void Visit(NUnit.Framework.Api.ITestResult result)
		{
			if (result.Test is TestSuite)
			{
				foreach (NUnit.Framework.Api.ITestResult child in result.Children)
				{
					Visit(child);
				}
				return;
			}
			testCount++;
			switch (result.ResultState.Status)
			{
			case TestStatus.Passed:
				break;
			case TestStatus.Skipped:
				notRunCount++;
				break;
			case TestStatus.Failed:
				if (result.ResultState == ResultState.Failure)
				{
					failureCount++;
				}
				else
				{
					errorCount++;
				}
				break;
			}
		}
	}
}
