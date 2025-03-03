using NUnit.Framework.Internal;
using System.Reflection;

namespace NUnit.Framework.Extensibility
{
	public interface ITestCaseBuilder2 : ITestCaseBuilder
	{
		bool CanBuildFrom(MethodInfo method, Test suite);

		Test BuildFrom(MethodInfo method, Test suite);
	}
}
