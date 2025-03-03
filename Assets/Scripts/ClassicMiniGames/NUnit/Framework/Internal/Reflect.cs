using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal
{
	public class Reflect
	{
		private class BaseTypesFirstComparer : IComparer<MethodInfo>
		{
			public int Compare(MethodInfo m1, MethodInfo m2)
			{
				if (m1 == null || m2 == null)
				{
					return 0;
				}
				Type declaringType = m1.DeclaringType;
				Type declaringType2 = m2.DeclaringType;
				if (declaringType == declaringType2)
				{
					return 0;
				}
				if (declaringType.IsAssignableFrom(declaringType2))
				{
					return -1;
				}
				return 1;
			}
		}

		private class MethodInfoList : List<MethodInfo>
		{
		}

		private static readonly BindingFlags AllMembers = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		private static readonly Type[] EmptyTypes = new Type[0];

		public static MethodInfo[] GetMethodsWithAttribute(Type fixtureType, Type attributeType, bool inherit)
		{
			MethodInfoList methodInfoList = new MethodInfoList();
			MethodInfo[] methods = fixtureType.GetMethods(AllMembers);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.IsDefined(attributeType, inherit))
				{
					methodInfoList.Add(methodInfo);
				}
			}
			methodInfoList.Sort(new BaseTypesFirstComparer());
			return methodInfoList.ToArray();
		}

		public static bool HasMethodWithAttribute(Type fixtureType, Type attributeType, bool inherit)
		{
			MethodInfo[] methods = fixtureType.GetMethods(AllMembers);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.IsDefined(attributeType, inherit))
				{
					return true;
				}
			}
			return false;
		}

		public static object Construct(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(EmptyTypes);
			if (constructor == null)
			{
				throw new InvalidTestFixtureException(type.FullName + " does not have a default constructor");
			}
			return constructor.Invoke(null);
		}

		public static object Construct(Type type, object[] arguments)
		{
			if (arguments == null)
			{
				return Construct(type);
			}
			Type[] typeArray = GetTypeArray(arguments);
			ConstructorInfo constructor = type.GetConstructor(typeArray);
			if (constructor == null)
			{
				throw new InvalidTestFixtureException(type.FullName + " does not have a suitable constructor");
			}
			return constructor.Invoke(arguments);
		}

		private static Type[] GetTypeArray(object[] objects)
		{
			Type[] array = new Type[objects.Length];
			int num = 0;
			foreach (object obj in objects)
			{
				array[num++] = obj.GetType();
			}
			return array;
		}

		public static object InvokeMethod(MethodInfo method, object fixture)
		{
			return InvokeMethod(method, fixture, null);
		}

		public static object InvokeMethod(MethodInfo method, object fixture, params object[] args)
		{
			if (method != null)
			{
				try
				{
					return method.Invoke(fixture, args);
				}
				catch (Exception ex)
				{
					if (ex is TargetInvocationException)
					{
						throw new NUnitException("Rethrown", ex.InnerException);
					}
					throw new NUnitException("Rethrown", ex);
				}
			}
			return null;
		}

		private Reflect()
		{
		}
	}
}
