using NUnit.Framework.Api;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public class ValuesAttribute : DataAttribute, IParameterDataSource
	{
		protected object[] data;

		public ValuesAttribute(object arg1)
		{
			data = new object[1]
			{
				arg1
			};
		}

		public ValuesAttribute(object arg1, object arg2)
		{
			data = new object[2]
			{
				arg1,
				arg2
			};
		}

		public ValuesAttribute(object arg1, object arg2, object arg3)
		{
			data = new object[3]
			{
				arg1,
				arg2,
				arg3
			};
		}

		public ValuesAttribute(params object[] args)
		{
			data = args;
		}

		public IEnumerable GetData(ParameterInfo parameter)
		{
			Type parameterType = parameter.ParameterType;
			for (int i = 0; i < data.Length; i++)
			{
				object obj = data[i];
				if (obj == null)
				{
					continue;
				}
				if (obj.GetType().FullName == "NUnit.Framework.SpecialValue" && obj.ToString() == "Null")
				{
					data[i] = null;
				}
				else
				{
					if (parameterType.IsAssignableFrom(obj.GetType()))
					{
						continue;
					}
					if (obj is DBNull)
					{
						data[i] = null;
						continue;
					}
					bool flag = false;
					if (parameterType == typeof(short) || parameterType == typeof(byte) || parameterType == typeof(sbyte))
					{
						flag = (obj is int);
					}
					else if (parameterType == typeof(decimal))
					{
						flag = (obj is double || obj is string || obj is int);
					}
					else if (parameterType == typeof(DateTime) || parameterType == typeof(TimeSpan))
					{
						flag = (obj is string);
					}
					if (flag)
					{
						data[i] = Convert.ChangeType(obj, parameterType, CultureInfo.InvariantCulture);
					}
				}
			}
			return data;
		}
	}
}
