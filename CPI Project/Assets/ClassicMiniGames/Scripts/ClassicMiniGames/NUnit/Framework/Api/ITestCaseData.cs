namespace NUnit.Framework.Api
{
	public interface ITestCaseData
	{
		string TestName
		{
			get;
		}

		RunState RunState
		{
			get;
		}

		object[] Arguments
		{
			get;
		}

		object ExpectedResult
		{
			get;
		}

		bool HasExpectedResult
		{
			get;
		}

		ExpectedExceptionData ExceptionData
		{
			get;
		}

		IPropertyBag Properties
		{
			get;
		}
	}
}
