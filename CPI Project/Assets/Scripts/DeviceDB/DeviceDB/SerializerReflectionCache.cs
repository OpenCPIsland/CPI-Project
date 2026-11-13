using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeviceDB
{
	internal static class SerializerReflectionCache
	{
		private static readonly Dictionary<Type, SerializedTypeReflection> typeReflectionsByType = new Dictionary<Type, SerializedTypeReflection>();

		private static readonly Dictionary<byte, SerializedTypeReflection> typeReflectionsById = new Dictionary<byte, SerializedTypeReflection>();

		internal static void Clear()
		{
			typeReflectionsById.Clear();
			typeReflectionsByType.Clear();
		}

		internal static SerializedTypeReflection GetTypeReflection(Type type)
		{
			SerializedTypeReflection value;
			if (!typeReflectionsByType.TryGetValue(type, out value))
			{
				throw new CorruptionException(string.Concat("Unknown type: ", type, ". Are you sure you added the [Serialized] attribute to it?"));
			}
			return value;
		}

		internal static SerializedTypeReflection GetTypeReflection(byte typeId)
		{
			SerializedTypeReflection value;
			if (!typeReflectionsById.TryGetValue(typeId, out value))
			{
				throw new CorruptionException("Unknown type ID: " + typeId + ". Are you sure you have a class where the [Serialized] attribute has this ID?");
			}
			return value;
		}

		internal static void AddTypes(params Type[] types)
		{
			foreach (Type uncachedType in GetUncachedTypes(types))
			{
				AddType(uncachedType);
			}
		}

		private static IEnumerable<Type> GetUncachedTypes(params Type[] types)
		{
			return types.Where((Type type) => !typeReflectionsByType.ContainsKey(type));
		}

		private static void AddType(Type type)
		{
			SerializedAttribute serializedAttribute = GetSerializedAttribute(type);
			if (serializedAttribute != null)
			{
				AssertNewType(type, serializedAttribute.PrimaryId);
				AssertTypeExtendsAbstractDocument(type);
				SerializedTypeReflection value = AddType(type, serializedAttribute.PrimaryId);
				byte[] alternateIds = serializedAttribute.AlternateIds;
				foreach (byte key in alternateIds)
				{
					typeReflectionsById.Add(key, value);
				}
			}
		}

		private static void AssertTypeExtendsAbstractDocument(Type type)
		{
			if (type.BaseType != typeof(AbstractDocument))
			{
				throw new FormatException(string.Concat("[Serialized] type ", type, " does not directly extend AbstractDocument"));
			}
		}

		private static SerializedAttribute GetSerializedAttribute(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(SerializedAttribute), true);
			return (customAttributes.Length <= 0) ? null : ((SerializedAttribute)customAttributes[0]);
		}

		private static SerializedTypeReflection AddType(Type type, byte id)
		{
			SerializedFieldReflection[] fieldReflections = GetFieldReflections(type);
			AssertUniqueFieldIds(type, fieldReflections);
			return CacheTypeReflection(id, type, fieldReflections);
		}

		private static SerializedFieldReflection[] GetFieldReflections(Type type)
		{
			List<SerializedFieldReflection> list = new List<SerializedFieldReflection>();
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				AddFieldReflection(fieldInfo, list);
			}
			return list.ToArray();
		}

		private static void AssertUniqueFieldIds(Type type, SerializedFieldReflection[] fieldReflections)
		{
			if ((from r in fieldReflections
				group r by r.Id).Any((IGrouping<byte, SerializedFieldReflection> g) => g.Count() > 1))
			{
				throw new FormatException(string.Concat("[Serialized] type ", type, " has duplicate IDs"));
			}
		}

		private static void AssertNewType(Type type, byte id)
		{
			if (typeReflectionsById.ContainsKey(id))
			{
				throw new FormatException(string.Concat("[Serialized] type ID ", id, " can't be used by ", type, " because ", typeReflectionsById[id].Type, " is already using it"));
			}
		}

		private static void AddFieldReflection(FieldInfo fieldInfo, ICollection<SerializedFieldReflection> fieldReflections)
		{
			object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(SerializedAttribute), true);
			if (customAttributes.Length > 0)
			{
				SerializedAttribute serializedAttribute = (SerializedAttribute)customAttributes[0];
				AddFieldReflection(fieldInfo, fieldReflections, serializedAttribute.PrimaryId);
				byte[] alternateIds = serializedAttribute.AlternateIds;
				foreach (byte id in alternateIds)
				{
					AddFieldReflection(fieldInfo, fieldReflections, id);
				}
			}
		}

		private static void AddFieldReflection(FieldInfo fieldInfo, ICollection<SerializedFieldReflection> fieldReflections, byte id)
		{
			fieldReflections.Add(new SerializedFieldReflection(fieldInfo, id));
		}

		private static SerializedTypeReflection CacheTypeReflection(byte id, Type type, SerializedFieldReflection[] fieldReflections)
		{
			SerializedTypeReflection serializedTypeReflection = new SerializedTypeReflection(type, id, fieldReflections);
			typeReflectionsByType.Add(type, serializedTypeReflection);
			typeReflectionsById.Add(id, serializedTypeReflection);
			return serializedTypeReflection;
		}
	}
}
