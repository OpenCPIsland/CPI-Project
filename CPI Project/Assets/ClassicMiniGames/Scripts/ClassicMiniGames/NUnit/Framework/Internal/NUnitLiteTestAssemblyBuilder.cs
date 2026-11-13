using NUnit.Framework.Api;
using NUnit.Framework.Builders;
using NUnit.Framework.Extensibility;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class NUnitLiteTestAssemblyBuilder : ITestAssemblyBuilder
	{
		private Assembly assembly;

		public TestSuite Build(Assembly assembly, IDictionary options)
		{
			this.assembly = assembly;
			IList names = options["LOAD"] as IList;
			IList fixtures = GetFixtures(assembly, names);
			if (fixtures.Count > 0)
			{
				return BuildTestAssembly(assembly.GetName().Name, fixtures);
			}
			return null;
		}

		public TestSuite Build(string assemblyName, IDictionary options)
		{
			assembly = Load(assemblyName);
			if (assembly == null)
			{
				return null;
			}
			IList names = options["LOAD"] as IList;
			IList fixtures = GetFixtures(assembly, names);
			if (fixtures.Count > 0)
			{
				return BuildTestAssembly(assemblyName, fixtures);
			}
			return null;
		}

		private Assembly Load(string path)
		{
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(Path.GetFileName(path));
			return Assembly.Load(assemblyName);
		}

		private IList GetFixtures(Assembly assembly, IList names)
		{
			ObjectList objectList = new ObjectList();
			IList candidateFixtureTypes = GetCandidateFixtureTypes(assembly, names);
			foreach (Type item in candidateFixtureTypes)
			{
				if (TestFixtureBuilder.CanBuildFrom(item))
				{
					objectList.Add(TestFixtureBuilder.BuildFrom(item));
				}
			}
			return objectList;
		}

		private IList GetCandidateFixtureTypes(Assembly assembly, IList names)
		{
			IList types = assembly.GetTypes();
			if (names == null || names.Count == 0)
			{
				return types;
			}
			ObjectList objectList = new ObjectList();
			foreach (string name in names)
			{
				Type type = assembly.GetType(name);
				if (type != null)
				{
					objectList.Add(type);
				}
				else
				{
					string value = name + ".";
					foreach (Type item in types)
					{
						if (item.FullName.StartsWith(value))
						{
							objectList.Add(item);
						}
					}
				}
			}
			return objectList;
		}

		private TestSuite BuildFromFixtureType(string assemblyName, Type testType)
		{
			ISuiteBuilder suiteBuilder = new NUnitTestFixtureBuilder();
			if (suiteBuilder.CanBuildFrom(testType))
			{
				return BuildTestAssembly(assemblyName, new Test[1]
				{
					suiteBuilder.BuildFrom(testType)
				});
			}
			return null;
		}

		private TestSuite BuildTestAssembly(string assemblyName, IList fixtures)
		{
			TestSuite testSuite = new TestAssembly(assembly, assemblyName);
			foreach (Test fixture in fixtures)
			{
				testSuite.Add(fixture);
			}
			if (fixtures.Count == 0)
			{
				testSuite.RunState = RunState.NotRunnable;
				testSuite.Properties.Set(PropertyNames.SkipReason, "Has no TestFixtures");
			}
			testSuite.ApplyCommonAttributes(assembly);
			testSuite.Properties.Set(PropertyNames.ProcessID, Process.GetCurrentProcess().Id);
			testSuite.Properties.Set(PropertyNames.AppDomain, AppDomain.CurrentDomain.FriendlyName);
			testSuite.Sort();
			return testSuite;
		}
	}
}
