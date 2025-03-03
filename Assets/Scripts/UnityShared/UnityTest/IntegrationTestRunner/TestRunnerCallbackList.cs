using System.Collections.Generic;

namespace UnityTest.IntegrationTestRunner
{
	public class TestRunnerCallbackList : ITestRunnerCallback
	{
		private readonly List<ITestRunnerCallback> m_CallbackList = new List<ITestRunnerCallback>();

		public void Add(ITestRunnerCallback callback)
		{
			m_CallbackList.Add(callback);
		}

		public void Remove(ITestRunnerCallback callback)
		{
			m_CallbackList.Remove(callback);
		}

		public void RunStarted(string platform, List<TestComponent> testsToRun)
		{
			foreach (ITestRunnerCallback callback in m_CallbackList)
			{
				callback.RunStarted(platform, testsToRun);
			}
		}

		public void RunFinished(List<TestResult> testResults)
		{
			foreach (ITestRunnerCallback callback in m_CallbackList)
			{
				callback.RunFinished(testResults);
			}
		}

		public void AllScenesFinished()
		{
			foreach (ITestRunnerCallback callback in m_CallbackList)
			{
				callback.AllScenesFinished();
			}
		}

		public void TestStarted(TestResult test)
		{
			foreach (ITestRunnerCallback callback in m_CallbackList)
			{
				callback.TestStarted(test);
			}
		}

		public void TestFinished(TestResult test)
		{
			foreach (ITestRunnerCallback callback in m_CallbackList)
			{
				callback.TestFinished(test);
			}
		}

		public void TestRunInterrupted(List<ITestComponent> testsNotRun)
		{
			foreach (ITestRunnerCallback callback in m_CallbackList)
			{
				callback.TestRunInterrupted(testsNotRun);
			}
		}
	}
}
