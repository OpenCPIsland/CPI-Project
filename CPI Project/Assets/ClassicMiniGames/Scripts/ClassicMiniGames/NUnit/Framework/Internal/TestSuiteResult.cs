using NUnit.Framework.Api;
using System;

namespace NUnit.Framework.Internal
{
	public class TestSuiteResult : TestResult
	{
		private int passCount = 0;

		private int failCount = 0;

		private int skipCount = 0;

		private int inconclusiveCount = 0;

		public override int FailCount
		{
			get
			{
				return failCount;
			}
		}

		public override int PassCount
		{
			get
			{
				return passCount;
			}
		}

		public override int SkipCount
		{
			get
			{
				return skipCount;
			}
		}

		public override int InconclusiveCount
		{
			get
			{
				return inconclusiveCount;
			}
		}

		public TestSuiteResult(TestSuite suite)
			: base(suite)
		{
		}

		public void AddResult(TestResult result)
		{
			base.Children.Add(result);
			assertCount += result.AssertCount;
			passCount += result.PassCount;
			failCount += result.FailCount;
			skipCount += result.SkipCount;
			inconclusiveCount += result.InconclusiveCount;
			switch (result.ResultState.Status)
			{
			case TestStatus.Inconclusive:
				break;
			case TestStatus.Passed:
				if (resultState.Status == TestStatus.Inconclusive)
				{
					resultState = ResultState.Success;
				}
				break;
			case TestStatus.Failed:
				if (resultState.Status != TestStatus.Failed)
				{
					resultState = ResultState.Failure;
					message = "One or more child tests had errors";
				}
				break;
			case TestStatus.Skipped:
				switch (result.ResultState.Label)
				{
				case "Invalid":
					if (base.ResultState != ResultState.NotRunnable && base.ResultState.Status != TestStatus.Failed)
					{
						resultState = ResultState.Failure;
						message = "One or more child tests had errors";
					}
					break;
				case "Ignored":
					if (base.ResultState.Status == TestStatus.Inconclusive || base.ResultState.Status == TestStatus.Passed)
					{
						resultState = ResultState.Ignored;
						message = "One or more child tests were ignored";
					}
					break;
				}
				break;
			}
		}

		public void RecordException(Exception ex, FailureSite site)
		{
			RecordException(ex);
			if (site == FailureSite.SetUp)
			{
				switch (base.ResultState.Status)
				{
				case TestStatus.Passed:
					break;
				case TestStatus.Skipped:
					skipCount = test.TestCaseCount;
					break;
				case TestStatus.Failed:
					failCount = test.TestCaseCount;
					break;
				}
			}
		}
	}
}
