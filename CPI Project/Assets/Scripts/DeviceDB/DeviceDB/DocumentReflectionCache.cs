using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeviceDB
{
	internal static class DocumentReflectionCache
	{
		private static readonly Dictionary<Type, DocumentTypeReflection> typeReflections = new Dictionary<Type, DocumentTypeReflection>();

		internal static DocumentTypeReflection GetTypeReflection<T>()
		{
			Type typeFromHandle = typeof(T);
			DocumentTypeReflection value;
			if (!typeReflections.TryGetValue(typeFromHandle, out value))
			{
				List<DocumentFieldReflection> list = new List<DocumentFieldReflection>();
				FieldInfo[] fields = typeFromHandle.GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(IndexedAttribute), true);
					if (customAttributes.Length > 0)
					{
						DocumentFieldReflection item = new DocumentFieldReflection(fieldInfo);
						list.Add(item);
					}
				}
				value = new DocumentTypeReflection(list.ToArray());
				typeReflections.Add(typeFromHandle, value);
			}
			return value;
		}
	}
}
