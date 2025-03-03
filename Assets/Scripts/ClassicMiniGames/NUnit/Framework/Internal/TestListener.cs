using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	public class TestListener : ITestListener
	{
		public static ITestListener NULL
		{
			get
			{
				return new TestListener();
			}
		}

		public void TestStarted(ITest test)
		{
		}

		public void TestFinished(NUnit.Framework.Api.ITestResult result)
		{
		}

		public void TestOutput(TestOutput testOutput)
		{
		}

		private TestListener()
		{
		}
	}
}
