using NUnit.Framework.Api;
using NUnit.Framework.Extensibility;
using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Builders
{
	public class ParameterDataProvider : IParameterDataProvider
	{
		public bool HasDataFor(ParameterInfo parameter)
		{
			return parameter.IsDefined(typeof(DataAttribute), false);
		}

		public IEnumerable GetDataFor(ParameterInfo parameter)
		{
			ObjectList objectList = new ObjectList();
			object[] customAttributes = parameter.GetCustomAttributes(typeof(DataAttribute), false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Attribute attribute = (Attribute)customAttributes[i];
				IParameterDataSource parameterDataSource = attribute as IParameterDataSource;
				if (parameterDataSource != null)
				{
					foreach (object datum in parameterDataSource.GetData(parameter))
					{
						objectList.Add(datum);
					}
				}
			}
			return objectList;
		}
	}
}
