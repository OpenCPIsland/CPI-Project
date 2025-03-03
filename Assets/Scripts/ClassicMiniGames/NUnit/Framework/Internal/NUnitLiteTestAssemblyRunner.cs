using NUnit.Framework.Api;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class NUnitLiteTestAssemblyRunner : ITestAssemblyRunner
	{
		private IDictionary settings;

		private ITestAssemblyBuilder builder;

		private TestSuite loadedTest;

		public ITest LoadedTest
		{
			get
			{
				return loadedTest;
			}
		}

		public NUnitLiteTestAssemblyRunner(ITestAssemblyBuilder builder)
		{
			this.builder = builder;
		}

		public bool Load(string assemblyName, IDictionary settings)
		{
			this.settings = settings;
			loadedTest = builder.Build(assemblyName, settings);
			if (loadedTest == null)
			{
				return false;
			}
			return true;
		}

		public bool Load(Assembly assembly, IDictionary settings)
		{
			this.settings = settings;
			loadedTest = builder.Build(assembly, settings);
			if (loadedTest == null)
			{
				return false;
			}
			return true;
		}

		public NUnit.Framework.Api.ITestResult Run(ITestListener listener, ITestFilter filter)
		{
			if (settings.Contains("WorkDirectory"))
			{
				TestExecutionContext.CurrentContext.WorkDirectory = (string)settings["WorkDirectory"];
			}
			else
			{
				TestExecutionContext.CurrentContext.WorkDirectory = Environment.CurrentDirectory;
			}
			TestCommand testCommand = loadedTest.GetTestCommand(filter);
			return CommandRunner.Execute(testCommand);
		}
	}
}
