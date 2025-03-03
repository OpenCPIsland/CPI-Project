using NUnit.Framework.Api;
using NUnit.Framework.Builders;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Extensibility
{
	internal class TestCaseProviders : ITestCaseProvider
	{
		private List<ITestCaseProvider> Extensions = new List<ITestCaseProvider>();

		public TestCaseProviders()
		{
			Extensions.Add(new DataAttributeTestCaseProvider());
			Extensions.Add(new CombinatorialTestCaseProvider());
		}

		public bool HasTestCasesFor(MethodInfo method)
		{
			foreach (ITestCaseProvider extension in Extensions)
			{
				if (extension.HasTestCasesFor(method))
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			foreach (ITestCaseProvider extension in Extensions)
			{
				try
				{
					if (extension.HasTestCasesFor(method))
					{
						foreach (ITestCaseData item in extension.GetTestCasesFor(method))
						{
							list.Add(item);
						}
					}
				}
				catch (TargetInvocationException ex)
				{
					list.Add(new ParameterSet(ex.InnerException));
				}
				catch (Exception exception)
				{
					list.Add(new ParameterSet(exception));
				}
			}
			return list;
		}
	}
}
