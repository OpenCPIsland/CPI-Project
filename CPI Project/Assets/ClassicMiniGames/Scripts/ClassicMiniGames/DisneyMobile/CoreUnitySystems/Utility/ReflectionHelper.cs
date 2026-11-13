using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DisneyMobile.CoreUnitySystems.Utility
{
	public static class ReflectionHelper
	{
		public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
		{
			return from t in assembly.GetTypes()
				where baseType.IsAssignableFrom(t)
				select t;
		}

		public static List<Type> FindDerivedTypesInAllAssemblies(Type baseType, bool includeAbstractTypes, bool ignoreTestNamespace)
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				foreach (Type item in FindDerivedTypes(assembly, baseType))
				{
					if (item != null && (!ignoreTestNamespace || item.Namespace == null || !item.Namespace.Contains(".Test")) && (!item.IsAbstract || includeAbstractTypes))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public static Type GetTypeInAllAssemblies(string typeName)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type type in types)
				{
					if (type.ToString() == typeName)
					{
						return type;
					}
				}
			}
			return null;
		}

		public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(bool inherit) where TAttribute : Attribute
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				from t in a.GetTypes()
				where t.IsDefined(typeof(TAttribute), inherit)
				select t;
		}

		public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>() where TAttribute : Attribute
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				from t in a.GetTypes()
				from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public)
				from method in m.GetCustomAttributes(typeof(TAttribute), false)
				select m;
		}
	}
}
