using NUnit;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NUnitLite.Runner
{
	public class TextUI
	{
		private CommandLineOptions commandLineOptions;

		private int reportCount = 0;

		private ObjectList assemblies = new ObjectList();

		private TextWriter writer;

		private ITestAssemblyRunner runner;

		public TextUI()
			: this(ConsoleWriter.Out)
		{
		}

		public TextUI(TextWriter writer)
		{
			this.writer = writer;
			runner = new NUnitLiteTestAssemblyRunner(new NUnitLiteTestAssemblyBuilder());
		}

		public void Execute(string[] args)
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			commandLineOptions = ProcessArguments(args);
			if (!commandLineOptions.ShowHelp && !commandLineOptions.Error)
			{
				if (commandLineOptions.Wait && commandLineOptions.OutFile != null)
				{
					writer.WriteLine("Ignoring /wait option - only valid for Console");
				}
				IDictionary settings = new Hashtable();
				IDictionary dictionary = new Hashtable();
				if (commandLineOptions.TestCount > 0)
				{
					dictionary["RUN"] = commandLineOptions.Tests;
				}
				try
				{
					string[] parameters = commandLineOptions.Parameters;
					foreach (string assemblyString in parameters)
					{
						assemblies.Add(Assembly.Load(assemblyString));
					}
					if (assemblies.Count == 0)
					{
						assemblies.Add(callingAssembly);
					}
					Assembly assembly = assemblies[0] as Assembly;
					if (!runner.Load(assembly, settings))
					{
						Console.WriteLine("No tests found in assembly {0}", assembly.GetName().Name);
					}
					else if (commandLineOptions.Explore)
					{
						ExploreTests();
					}
					else
					{
						RunTests();
					}
				}
				catch (FileNotFoundException ex)
				{
					writer.WriteLine(ex.Message);
				}
				catch (Exception ex2)
				{
					writer.WriteLine(ex2.ToString());
				}
				finally
				{
					if (commandLineOptions.OutFile == null)
					{
						if (commandLineOptions.Wait)
						{
							Console.WriteLine("Press Enter key to continue . . .");
							Console.ReadLine();
						}
					}
					else
					{
						writer.Close();
					}
				}
			}
		}

		private void RunTests()
		{
			NUnit.Framework.Api.ITestResult testResult = runner.Run(TestListener.NULL, TestFilter.Empty);
			ReportResults(testResult);
			string resultFile = commandLineOptions.ResultFile;
			if (resultFile != null)
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(resultFile, Encoding.UTF8);
				xmlTextWriter.Formatting = Formatting.Indented;
				testResult.ToXml(true).WriteTo(xmlTextWriter);
				xmlTextWriter.Close();
			}
		}

		private void ExploreTests()
		{
			XmlNode xmlNode = runner.LoadedTest.ToXml(true);
			string exploreFile = commandLineOptions.ExploreFile;
			XmlTextWriter xmlTextWriter = (exploreFile != null && exploreFile.Length > 0) ? new XmlTextWriter(exploreFile, Encoding.UTF8) : new XmlTextWriter(Console.Out);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlNode.WriteTo(xmlTextWriter);
			xmlTextWriter.Close();
		}

		private void ReportResults(NUnit.Framework.Api.ITestResult result)
		{
			ResultSummary resultSummary = new ResultSummary(result);
			writer.WriteLine("{0} Tests : {1} Failures, {2} Errors, {3} Not Run", resultSummary.TestCount, resultSummary.FailureCount, resultSummary.ErrorCount, resultSummary.NotRunCount);
			if (resultSummary.FailureCount > 0 || resultSummary.ErrorCount > 0)
			{
				PrintErrorReport(result);
			}
			if (resultSummary.NotRunCount > 0)
			{
				PrintNotRunReport(result);
			}
			if (commandLineOptions.Full)
			{
				PrintFullReport(result);
			}
		}

		private CommandLineOptions ProcessArguments(string[] args)
		{
			commandLineOptions = new CommandLineOptions();
			commandLineOptions.Parse(args);
			if (commandLineOptions.OutFile != null)
			{
				writer = new StreamWriter(commandLineOptions.OutFile);
			}
			else
			{
				writer = ConsoleWriter.Out;
			}
			if (!commandLineOptions.NoHeader)
			{
				WriteCopyright();
			}
			if (commandLineOptions.ShowHelp)
			{
				writer.Write(commandLineOptions.HelpText);
			}
			else if (commandLineOptions.Error)
			{
				writer.WriteLine(commandLineOptions.ErrorMessage);
			}
			return commandLineOptions;
		}

		private void WriteCopyright()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string arg = "NUNit Framework";
			Version version = executingAssembly.GetName().Version;
			string value = "Copyright (C) 2012, Charlie Poole";
			string arg2 = "";
			object[] customAttributes = executingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
			if (customAttributes.Length > 0)
			{
				AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
				arg = assemblyTitleAttribute.Title;
			}
			customAttributes = executingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			if (customAttributes.Length > 0)
			{
				AssemblyCopyrightAttribute assemblyCopyrightAttribute = (AssemblyCopyrightAttribute)customAttributes[0];
				value = assemblyCopyrightAttribute.Copyright;
			}
			customAttributes = executingAssembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
			if (customAttributes.Length > 0)
			{
				AssemblyConfigurationAttribute assemblyConfigurationAttribute = (AssemblyConfigurationAttribute)customAttributes[0];
				arg2 = string.Format("({0})", assemblyConfigurationAttribute.Configuration);
			}
			writer.WriteLine(string.Format("{0} {1} {2}", arg, version.ToString(3), arg2));
			writer.WriteLine(value);
			writer.WriteLine();
			string arg3 = (Type.GetType("Mono.Runtime", false) == null) ? ".NET" : "Mono";
			writer.WriteLine("Runtime Environment -");
			writer.WriteLine("    OS Version: {0}", Environment.OSVersion);
			writer.WriteLine("  {0} Version: {1}", arg3, Environment.Version);
			writer.WriteLine();
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

		private void PrintTestProperties(ITest test)
		{
			foreach (PropertyEntry property in test.Properties)
			{
				writer.WriteLine("  {0}: {1}", property.Name, property.Value);
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
