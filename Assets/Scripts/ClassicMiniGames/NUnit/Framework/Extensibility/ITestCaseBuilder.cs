using NUnit.Framework.Internal;
using System.Reflection;

namespace NUnit.Framework.Extensibility
{
	public interface ITestCaseBuilder
	{
		bool CanBuildFrom(MethodInfo method);

		Test BuildFrom(MethodInfo method);
	}
}
