using NUnit.Framework.Api;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Extensibility
{
	public interface ITestCaseProvider
	{
		bool HasTestCasesFor(MethodInfo method);

		IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method);
	}
}
