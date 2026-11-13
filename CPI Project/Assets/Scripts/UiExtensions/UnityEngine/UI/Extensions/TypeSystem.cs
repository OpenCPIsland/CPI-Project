using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	internal static class TypeSystem
	{
		internal static Type GetElementType(Type seqType)
		{
			Type type = FindIEnumerable(seqType);
			if (type == null)
			{
				return seqType;
			}
			return type.GetGenericArguments()[0];
		}

		private static Type FindIEnumerable(Type seqType)
		{
			if (seqType == null || seqType == typeof(string))
			{
				return null;
			}
			if (seqType.IsArray)
			{
				return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
			}
			if (seqType.IsGenericType)
			{
				Type[] genericArguments = seqType.GetGenericArguments();
				foreach (Type type in genericArguments)
				{
					Type type2 = typeof(IEnumerable<>).MakeGenericType(type);
					if (type2.IsAssignableFrom(seqType))
					{
						return type2;
					}
				}
			}
			Type[] interfaces = seqType.GetInterfaces();
			if (interfaces != null && interfaces.Length > 0)
			{
				Type[] array = interfaces;
				foreach (Type seqType2 in array)
				{
					Type type3 = FindIEnumerable(seqType2);
					if (type3 != null)
					{
						return type3;
					}
				}
			}
			if (seqType.BaseType != null && seqType.BaseType != typeof(object))
			{
				return FindIEnumerable(seqType.BaseType);
			}
			return null;
		}

		public static bool IsEnumerableType(Type type)
		{
			return type.GetInterface("IEnumerable") != null;
		}

		public static bool IsCollectionType(Type type)
		{
			return type.GetInterface("ICollection") != null;
		}
	}
}
