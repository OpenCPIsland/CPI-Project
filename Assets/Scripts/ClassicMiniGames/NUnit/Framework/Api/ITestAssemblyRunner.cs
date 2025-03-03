using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Api
{
	public interface ITestAssemblyRunner
	{
		ITest LoadedTest
		{
			get;
		}

		bool Load(string assemblyName, IDictionary settings);

		bool Load(Assembly assembly, IDictionary settings);

		ITestResult Run(ITestListener listener, ITestFilter filter);
	}
}
