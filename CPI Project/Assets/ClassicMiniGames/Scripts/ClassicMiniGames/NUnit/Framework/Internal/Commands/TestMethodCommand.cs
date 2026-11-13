using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
	public class TestMethodCommand : TestCommand
	{
		private readonly TestMethod testMethod;

		private readonly object[] arguments;

		public TestMethodCommand(Test test)
			: base(test)
		{
			testMethod = (test as TestMethod);
			arguments = test.arguments;
		}

		public override TestResult Execute(TestExecutionContext context)
		{
			object actual = Reflect.InvokeMethod(testMethod.Method, context.TestObject, arguments);
			if (testMethod.hasExpectedResult)
			{
				Assert.AreEqual(testMethod.expectedResult, actual);
			}
			context.CurrentResult.SetResult(ResultState.Success);
			return context.CurrentResult;
		}
	}
}
