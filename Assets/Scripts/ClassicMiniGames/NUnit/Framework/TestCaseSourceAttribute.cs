using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class TestCaseSourceAttribute : DataAttribute, ITestCaseSource
	{
		private readonly string sourceName;

		private readonly Type sourceType;

		private string category;

		public string SourceName
		{
			get
			{
				return sourceName;
			}
		}

		public Type SourceType
		{
			get
			{
				return sourceType;
			}
		}

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		public TestCaseSourceAttribute(string sourceName)
		{
			this.sourceName = sourceName;
		}

		public TestCaseSourceAttribute(Type sourceType, string sourceName)
		{
			this.sourceType = sourceType;
			this.sourceName = sourceName;
		}

		public TestCaseSourceAttribute(Type sourceType)
		{
			this.sourceType = sourceType;
		}

		public IEnumerable<ITestCaseData> GetTestCasesFor(MethodInfo method)
		{
			List<ITestCaseData> list = new List<ITestCaseData>();
			IEnumerable testCaseSource = GetTestCaseSource(method);
			if (testCaseSource != null)
			{
				ParameterInfo[] parameters = method.GetParameters();
				foreach (object item in testCaseSource)
				{
					ParameterSet parameterSet = new ParameterSet();
					ITestCaseData testCaseData = item as ITestCaseData;
					if (testCaseData != null)
					{
						parameterSet = new ParameterSet(testCaseData);
					}
					else if (item is object[])
					{
						object[] array = item as object[];
						parameterSet.Arguments = ((array.Length == parameters.Length) ? array : new object[1]
						{
							item
						});
					}
					else if (item is Array)
					{
						Array array2 = item as Array;
						if (array2.Rank == 1 && array2.Length == parameters.Length)
						{
							parameterSet.Arguments = new object[array2.Length];
							for (int i = 0; i < array2.Length; i++)
							{
								parameterSet.Arguments[i] = array2.GetValue(i);
							}
						}
						else
						{
							parameterSet.Arguments = new object[1]
							{
								item
							};
						}
					}
					else
					{
						parameterSet.Arguments = new object[1]
						{
							item
						};
					}
					if (Category != null)
					{
						string[] array3 = Category.Split(',');
						foreach (string value in array3)
						{
							parameterSet.Properties.Add(PropertyNames.Category, value);
						}
					}
					list.Add(parameterSet);
				}
			}
			return list;
		}

		private IEnumerable GetTestCaseSource(MethodInfo method)
		{
			IEnumerable result = null;
			Type reflectedType = sourceType;
			if (reflectedType == null)
			{
				reflectedType = method.ReflectedType;
			}
			if (sourceName == null)
			{
				return Reflect.Construct(reflectedType) as IEnumerable;
			}
			MemberInfo[] member = reflectedType.GetMember(sourceName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (member.Length == 1)
			{
				MemberInfo memberInfo = member[0];
				object obj = Reflect.Construct(reflectedType);
				switch (memberInfo.MemberType)
				{
				case MemberTypes.Field:
				{
					FieldInfo fieldInfo = memberInfo as FieldInfo;
					result = (IEnumerable)fieldInfo.GetValue(obj);
					break;
				}
				case MemberTypes.Property:
				{
					PropertyInfo propertyInfo = memberInfo as PropertyInfo;
					result = (IEnumerable)propertyInfo.GetValue(obj, null);
					break;
				}
				case MemberTypes.Method:
				{
					MethodInfo methodInfo = memberInfo as MethodInfo;
					result = (IEnumerable)methodInfo.Invoke(obj, null);
					break;
				}
				}
			}
			return result;
		}
	}
}
