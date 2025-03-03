using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Api
{
	public interface ITestCaseSource
	{
		IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method);
	}
}
