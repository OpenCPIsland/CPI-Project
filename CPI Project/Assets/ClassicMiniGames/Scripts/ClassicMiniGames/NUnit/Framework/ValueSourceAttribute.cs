using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
	public class ValueSourceAttribute : DataAttribute, IParameterDataSource
	{
		private readonly string sourceName;

		private readonly Type sourceType;

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

		public ValueSourceAttribute(string sourceName)
		{
			this.sourceName = sourceName;
		}

		public ValueSourceAttribute(Type sourceType, string sourceName)
		{
			this.sourceType = sourceType;
			this.sourceName = sourceName;
		}

		public IEnumerable GetData(ParameterInfo parameter)
		{
			ObjectList objectList = new ObjectList();
			IEnumerable dataSource = GetDataSource(parameter);
			if (dataSource != null)
			{
				foreach (object item in dataSource)
				{
					objectList.Add(item);
				}
			}
			return dataSource;
		}

		private IEnumerable GetDataSource(ParameterInfo parameter)
		{
			IEnumerable result = null;
			Type reflectedType = sourceType;
			if (reflectedType == null)
			{
				reflectedType = parameter.Member.ReflectedType;
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
