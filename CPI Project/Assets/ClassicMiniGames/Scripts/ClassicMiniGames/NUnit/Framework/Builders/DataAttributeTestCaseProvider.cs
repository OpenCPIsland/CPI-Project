using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Builders
{
	public class DataAttributeTestCaseProvider : ITestCaseProvider
	{
		public bool HasTestCasesFor(MethodInfo method)
		{
			return method.IsDefined(typeof(DataAttribute), false);
		}

		public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			object[] customAttributes = method.GetCustomAttributes(typeof(DataAttribute), false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				DataAttribute dataAttribute = (DataAttribute)customAttributes[i];
				ITestCaseSource testCaseSource = dataAttribute as ITestCaseSource;
				if (testCaseSource != null)
				{
					foreach (ITestCaseData item in ((ITestCaseSource)dataAttribute).GetTestCasesFor(method))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}
	}
}
