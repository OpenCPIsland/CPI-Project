using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Builders
{
	public class CombinatorialTestCaseProvider : ITestCaseProvider
	{
		private static IParameterDataProvider dataPointProvider = new ParameterDataProvider();

		public bool HasTestCasesFor(MethodInfo method)
		{
			if (method.GetParameters().Length == 0)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			foreach (ParameterInfo parameter in parameters)
			{
				if (!dataPointProvider.HasDataFor(parameter))
				{
					return false;
				}
			}
			return true;
		}

		public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
		{
			return GetStrategy(method).GetTestCases();
		}

		private CombiningStrategy GetStrategy(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			IEnumerable[] array = new IEnumerable[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = dataPointProvider.GetDataFor(parameters[i]);
			}
			if (method.IsDefined(typeof(SequentialAttribute), false))
			{
				return new SequentialStrategy(array);
			}
			if (method.IsDefined(typeof(PairwiseAttribute), false) && method.GetParameters().Length > 2)
			{
				return new PairwiseStrategy(array);
			}
			return new CombinatorialStrategy(array);
		}
	}
}
