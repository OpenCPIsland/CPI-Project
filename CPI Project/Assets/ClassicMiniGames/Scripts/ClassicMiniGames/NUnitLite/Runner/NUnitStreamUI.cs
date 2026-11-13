using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace NUnitLite.Runner
{
	public class NUnitStreamUI
	{
		private int reportCount = 0;

		private TextWriter writer;

		private ITestAssemblyRunner runner;

		public ResultSummary Summary
		{
			get;
			private set;
		}

		public NUnitStreamUI(TextWriter writer)
		{
			this.writer = writer;
			runner = new NUnitLiteTestAssemblyRunner(new NUnitLiteTestAssemblyBuilder());
		}

		public void Execute(Assembly assembly)
		{
			try
			{
				IDictionary settings = new Hashtable();
				if (!runner.Load(assembly, settings))
				{
					writer.WriteLine("No tests found in assembly {0}", assembly.GetName().Name);
				}
				else
				{
					writer.Write(assembly.GetName().Name + ": ");
					RunTests();
				}
			}
			catch (NullReferenceException)
			{
			}
			catch (Exception ex2)
			{
				writer.WriteLine(ex2.Message);
			}
			finally
			{
				writer.Close();
			}
		}

		private void RunTests()
		{
			NUnit.Framework.Api.ITestResult result = runner.Run(TestListener.NULL, TestFilter.Empty);
			ReportResults(result);
		}

		private void ReportResults(NUnit.Framework.Api.ITestResult result)
		{
			Summary = new ResultSummary(result);
			writer.WriteLine("{0} Tests : {1} Failures, {2} Errors, {3} Not Run", Summary.TestCount, Summary.FailureCount, Summary.ErrorCount, Summary.NotRunCount);
			if (Summary.FailureCount > 0 || Summary.ErrorCount > 0)
			{
				PrintErrorReport(result);
			}
			if (Summary.NotRunCount > 0)
			{
				PrintNotRunReport(result);
			}
		}

		private void PrintErrorReport(NUnit.Framework.Api.ITestResult result)
		{
			reportCount = 0;
			writer.WriteLine();
			writer.WriteLine("Errors and Failures:");
			PrintErrorResults(result);
		}

		private void PrintErrorResults(NUnit.Framework.Api.ITestResult result)
		{
			if (result.HasChildren)
			{
				foreach (NUnit.Framework.Api.ITestResult child in result.Children)
				{
					PrintErrorResults(child);
				}
			}
			else if (result.ResultState == ResultState.Error || result.ResultState == ResultState.Failure)
			{
				writer.WriteLine();
				writer.WriteLine("{0}) {1} ({2})", ++reportCount, result.Name, result.FullName);
				writer.WriteLine(result.Message);
				writer.WriteLine(result.StackTrace);
			}
		}

		private void PrintNotRunReport(NUnit.Framework.Api.ITestResult result)
		{
			reportCount = 0;
			writer.WriteLine();
			writer.WriteLine("Tests Not Run:");
			PrintNotRunResults(result);
		}

		private void PrintNotRunResults(NUnit.Framework.Api.ITestResult result)
		{
			if (result.HasChildren)
			{
				foreach (NUnit.Framework.Api.ITestResult child in result.Children)
				{
					PrintNotRunResults(child);
				}
			}
			else if (result.ResultState == ResultState.Ignored || result.ResultState == ResultState.NotRunnable || result.ResultState == ResultState.Skipped)
			{
				writer.WriteLine();
				writer.WriteLine("{0}) {1} ({2}) : {3}", ++reportCount, result.Name, result.FullName, result.Message);
			}
		}

		private void PrintFullReport(NUnit.Framework.Api.ITestResult result)
		{
			writer.WriteLine();
			writer.WriteLine("All Test Results:");
			PrintAllResults(result, " ");
		}

		private void PrintAllResults(NUnit.Framework.Api.ITestResult result, string indent)
		{
			string value = null;
			switch (result.ResultState.Status)
			{
			case TestStatus.Failed:
				value = "FAIL";
				break;
			case TestStatus.Skipped:
				value = "SKIP";
				break;
			case TestStatus.Inconclusive:
				value = "INC ";
				break;
			case TestStatus.Passed:
				value = "OK  ";
				break;
			}
			writer.Write(value);
			writer.Write(indent);
			writer.WriteLine(result.Name);
			if (result.HasChildren)
			{
				foreach (NUnit.Framework.Api.ITestResult child in result.Children)
				{
					PrintAllResults(child, indent + "  ");
				}
			}
		}
	}
}
