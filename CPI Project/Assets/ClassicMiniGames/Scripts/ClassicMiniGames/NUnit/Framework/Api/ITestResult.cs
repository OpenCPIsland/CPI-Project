using System.Collections.Generic;

namespace NUnit.Framework.Api
{
	public interface ITestResult : IXmlNodeBuilder
	{
		ResultState ResultState
		{
			get;
		}

		string Name
		{
			get;
		}

		string FullName
		{
			get;
		}

		double Time
		{
			get;
		}

		string Message
		{
			get;
		}

		string StackTrace
		{
			get;
		}

		int AssertCount
		{
			get;
		}

		int FailCount
		{
			get;
		}

		int PassCount
		{
			get;
		}

		int SkipCount
		{
			get;
		}

		int InconclusiveCount
		{
			get;
		}

		bool HasChildren
		{
			get;
		}

		IList<ITestResult> Children
		{
			get;
		}

		ITest Test
		{
			get;
		}
	}
}
