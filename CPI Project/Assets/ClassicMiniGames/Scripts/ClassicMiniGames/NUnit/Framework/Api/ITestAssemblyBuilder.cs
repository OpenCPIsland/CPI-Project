using NUnit.Framework.Internal;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Api
{
	public interface ITestAssemblyBuilder
	{
		TestSuite Build(Assembly assembly, IDictionary options);

		TestSuite Build(string assemblyName, IDictionary options);
	}
}
