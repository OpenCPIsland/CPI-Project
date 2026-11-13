namespace NUnit.Framework.Api
{
	public interface ITestListener
	{
		void TestStarted(ITest test);

		void TestFinished(ITestResult result);

		void TestOutput(TestOutput testOutput);
	}
}
