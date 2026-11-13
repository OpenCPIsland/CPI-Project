using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace Fabric
{
	public static class Serialization
	{
		public interface IField
		{
			string FieldName
			{
				get;
			}

			Type Type
			{
				get;
			}

			object GetValue();

			void SetValue(object value);
		}

		public struct PrimitiveField : IField
		{
			private readonly string _fieldName;

			private readonly FieldInfo _fieldInfo;

			private readonly object _instance;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return _fieldInfo.FieldType;
				}
			}

			public object GetValue()
			{
				return _fieldInfo.GetValue(_instance);
			}

			public void SetValue(object value)
			{
				_fieldInfo.SetValue(_instance, value);
			}

			public PrimitiveField(string fieldName, FieldInfo fieldInfo, object instance)
			{
				_fieldName = fieldName;
				_fieldInfo = fieldInfo;
				_instance = instance;
			}
		}

		public struct EnumField : IField
		{
			private readonly string _fieldName;

			private readonly FieldInfo _fieldInfo;

			private readonly object _instance;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return _fieldInfo.FieldType;
				}
			}

			public object GetValue()
			{
				return _fieldInfo.GetValue(_instance).ToString();
			}

			public void SetValue(object value)
			{
				string b = value as string;
				string[] names = Enum.GetNames(Type);
				int num = 0;
				while (true)
				{
					if (num < names.Length)
					{
						if (names[num] == b)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				_fieldInfo.SetValue(_instance, Enum.GetValues(Type).GetValue(num));
			}

			public EnumField(string fieldName, FieldInfo fieldInfo, object instance)
			{
				_fieldName = fieldName;
				_fieldInfo = fieldInfo;
				_instance = instance;
			}
		}

		public struct ArraySizeVirtualField : IField
		{
			private readonly string _fieldName;

			private readonly FieldInfo _fieldInfo;

			private readonly object _instance;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return typeof(int);
				}
			}

			public object GetValue()
			{
				Array array = (Array)_fieldInfo.GetValue(_instance);
				return (array != null) ? array.Length : 0;
			}

			public void SetValue(object value)
			{
				Array array = Array.CreateInstance(_fieldInfo.FieldType.GetElementType(), (int)value);
				Array array2 = (Array)_fieldInfo.GetValue(_instance);
				if (array2 != null)
				{
					Array.Copy(array2, array, Math.Min(array2.Length, array.Length));
				}
				_fieldInfo.SetValue(_instance, array);
			}

			public ArraySizeVirtualField(string fieldName, FieldInfo fieldInfo, object instance)
			{
				_fieldName = fieldName;
				_fieldInfo = fieldInfo;
				_instance = instance;
			}
		}

		public struct PrimitiveArrayElementField : IField
		{
			private readonly string _fieldName;

			private readonly Array _array;

			private readonly int _index;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return _array.GetType().GetElementType();
				}
			}

			public object GetValue()
			{
				return _array.GetValue(_index);
			}

			public void SetValue(object value)
			{
				_array.SetValue(value, _index);
			}

			public PrimitiveArrayElementField(string fieldName, Array array, int index)
			{
				_fieldName = fieldName;
				_array = array;
				_index = index;
			}
		}

		public struct EnumArrayElementField : IField
		{
			private readonly string _fieldName;

			private readonly Array _array;

			private readonly int _index;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return _array.GetType().GetElementType();
				}
			}

			public object GetValue()
			{
				return _array.GetValue(_index).ToString();
			}

			public void SetValue(object value)
			{
				string b = value as string;
				string[] names = Enum.GetNames(Type);
				int num = 0;
				while (true)
				{
					if (num < names.Length)
					{
						if (names[num] == b)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				_array.SetValue(Enum.GetValues(Type).GetValue(num), _index);
			}

			public EnumArrayElementField(string fieldName, Array array, int index)
			{
				_fieldName = fieldName;
				_array = array;
				_index = index;
			}
		}

		public struct ListSizeVirtualField : IField
		{
			private readonly string _fieldName;

			private readonly IList _list;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return typeof(int);
				}
			}

			public object GetValue()
			{
				return _list.Count;
			}

			public void SetValue(object value)
			{
				int num = (int)value;
				int count = _list.Count;
				if (count == num)
				{
					return;
				}
				if (count > num)
				{
					for (int num2 = count - 1; num2 >= num; num2--)
					{
						_list.RemoveAt(num2);
					}
					return;
				}
				Type type = _list.GetType().GetGenericArguments()[0];
				object value2 = CreateInstance(type);
				for (int i = count; i < num; i++)
				{
					_list.Add(value2);
				}
			}

			public ListSizeVirtualField(string fieldName, IList list)
			{
				_fieldName = fieldName;
				_list = list;
			}
		}

		public struct PrimitiveListElementField : IField
		{
			private readonly string _fieldName;

			private readonly IList _list;

			private readonly int _index;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return _list.GetType().GetGenericArguments()[0];
				}
			}

			public object GetValue()
			{
				return _list[_index];
			}

			public void SetValue(object value)
			{
				_list[_index] = value;
			}

			public PrimitiveListElementField(string fieldName, IList list, int index)
			{
				_fieldName = fieldName;
				_list = list;
				_index = index;
			}
		}

		public struct EnumListElementField : IField
		{
			private readonly string _fieldName;

			private readonly IList _list;

			private readonly int _index;

			public string FieldName
			{
				get
				{
					return _fieldName;
				}
			}

			public Type Type
			{
				get
				{
					return _list.GetType().GetGenericArguments()[0];
				}
			}

			public object GetValue()
			{
				return _list[_index].ToString();
			}

			public void SetValue(object value)
			{
				string b = value as string;
				string[] names = Enum.GetNames(Type);
				int num = 0;
				while (true)
				{
					if (num < names.Length)
					{
						if (names[num] == b)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				_list[_index] = Enum.GetValues(Type).GetValue(num);
			}

			public EnumListElementField(string fieldName, IList list, int index)
			{
				_fieldName = fieldName;
				_list = list;
				_index = index;
			}
		}

		private struct EnumerateFieldsTodo
		{
			public string Prefix;

			public object Instance;

			public Type Type;
		}

		public class CachedFieldScan
		{
			private class CachedField
			{
				public readonly int ScanNumber;

				public readonly IField Field;

				public object Value;

				public CachedField(int scanNumber, IField field, object value = null)
				{
					ScanNumber = scanNumber;
					Field = field;
					Value = value;
				}

				public bool Update()
				{
					object value = Field.GetValue();
					if ((value == null && Value == null) || (value != null && value.Equals(Value)))
					{
						return false;
					}
					Value = value;
					return true;
				}
			}

			private readonly SortedDictionary<string, CachedField> _fields;

			private readonly object _instance;

			private int _scanNumber;

			public CachedFieldScan(object instance)
			{
				_fields = new SortedDictionary<string, CachedField>();
				_instance = instance;
				Scan();
			}

			public void Scan()
			{
				_scanNumber++;
				foreach (IField item in EnumerateFields(_instance))
				{
					if (_fields.ContainsKey(item.FieldName))
					{
						object value = _fields[item.FieldName].Value;
						_fields[item.FieldName] = new CachedField(_scanNumber, item, value);
					}
					else
					{
						_fields[item.FieldName] = new CachedField(_scanNumber, item);
					}
				}
				List<string> list = new List<string>();
				foreach (string key in _fields.Keys)
				{
					if (_fields[key].ScanNumber != _scanNumber)
					{
						list.Add(key);
					}
				}
				foreach (string item2 in list)
				{
					_fields.Remove(item2);
				}
			}

			public IEnumerable<KeyValuePair<string, object>> Update()
			{
				for (int pass = 0; pass < 2; pass++)
				{
					bool needRescan = false;
					foreach (CachedField field in _fields.Values)
					{
						if (field.Update())
						{
							if (pass == 0 && field.Field.FieldName.EndsWith("#"))
							{
								needRescan = true;
								break;
							}
							yield return new KeyValuePair<string, object>(field.Field.FieldName, field.Value);
						}
					}
					if (needRescan)
					{
						Scan();
						continue;
					}
					break;
				}
			}
		}

		public static IEnumerable<FieldInfo> GetSerializableFields(Type type)
		{
			try
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo field in fields)
				{
					bool include = false;
					if (field.IsDefined(typeof(SerializeField), true))
					{
						include = true;
					}
					if (include)
					{
						yield return field;
					}
				}
			}
			finally
			{
			}
		}

		public static IEnumerable<IField> EnumerateFields(object instance)
		{
			Queue<EnumerateFieldsTodo> todo = new Queue<EnumerateFieldsTodo>();
			todo.Enqueue(new EnumerateFieldsTodo
			{
				Prefix = "",
				Instance = instance,
				Type = instance.GetType()
			});
			while (todo.Count > 0)
			{
				EnumerateFieldsTodo current = todo.Dequeue();
				foreach (FieldInfo field in GetSerializableFields(current.Type))
				{
					if (field.FieldType.IsArray)
					{
						yield return new ArraySizeVirtualField(current.Prefix + field.Name + "#", field, current.Instance);
						Array array = (Array)field.GetValue(current.Instance);
						if (array != null)
						{
							Type elementType2 = field.FieldType.GetElementType();
							if (elementType2.IsSubclassOf(typeof(UnityEngine.Object)) || elementType2 == typeof(UnityEngine.Object))
							{
								for (int l = 0; l < array.Length; l++)
								{
									yield return new PrimitiveArrayElementField(current.Prefix + field.Name + "[" + l + "]", array, l);
								}
							}
							else if (elementType2.IsClass && elementType2 != typeof(string))
							{
								for (int num = 0; num < array.Length; num++)
								{
									object obj = array.GetValue(num);
									if (obj == null)
									{
										obj = CreateInstance(elementType2);
										array.SetValue(obj, num);
									}
									todo.Enqueue(new EnumerateFieldsTodo
									{
										Prefix = current.Prefix + field.Name + "[" + num + "].",
										Instance = obj,
										Type = elementType2
									});
								}
							}
							else if (elementType2.IsEnum)
							{
								for (int n = 0; n < array.Length; n++)
								{
									yield return new EnumArrayElementField(current.Prefix + field.Name + "[" + n + "]", array, n);
								}
							}
							else
							{
								for (int m = 0; m < array.Length; m++)
								{
									yield return new PrimitiveArrayElementField(current.Prefix + field.Name + "[" + m + "]", array, m);
								}
							}
						}
					}
					else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
					{
						IList list = (IList)field.GetValue(current.Instance);
						if (list == null)
						{
							list = (IList)field.FieldType.GetConstructor(Type.EmptyTypes).Invoke(null);
							field.SetValue(current.Instance, list);
						}
						yield return new ListSizeVirtualField(current.Prefix + field.Name + "#", list);
						Type elementType = field.FieldType.GetGenericArguments()[0];
						if (elementType.IsSubclassOf(typeof(UnityEngine.Object)) || elementType == typeof(UnityEngine.Object))
						{
							for (int i = 0; i < list.Count; i++)
							{
								yield return new PrimitiveListElementField(current.Prefix + field.Name + "[" + i + "]", list, i);
							}
						}
						else if (elementType.IsClass && elementType != typeof(string))
						{
							for (int num2 = 0; num2 < list.Count; num2++)
							{
								object obj2 = list[num2];
								if (obj2 == null)
								{
									obj2 = (list[num2] = CreateInstance(elementType));
								}
								todo.Enqueue(new EnumerateFieldsTodo
								{
									Prefix = current.Prefix + field.Name + "[" + num2 + "].",
									Instance = obj2,
									Type = elementType
								});
							}
						}
						else if (elementType.IsEnum)
						{
							for (int k = 0; k < list.Count; k++)
							{
								yield return new EnumListElementField(current.Prefix + field.Name + "[" + k + "]", list, k);
							}
						}
						else
						{
							for (int j = 0; j < list.Count; j++)
							{
								yield return new PrimitiveListElementField(current.Prefix + field.Name + "[" + j + "]", list, j);
							}
						}
					}
					else if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) || field.FieldType == typeof(UnityEngine.Object))
					{
						yield return new PrimitiveField(current.Prefix + field.Name, field, current.Instance);
					}
					else if (field.FieldType == typeof(AnimationCurve))
					{
						yield return new PrimitiveField(current.Prefix + field.Name, field, current.Instance);
					}
					else if (field.FieldType.IsClass && field.FieldType != typeof(string))
					{
						object obj4 = field.GetValue(current.Instance);
						if (obj4 == null)
						{
							obj4 = CreateInstance(field.FieldType);
							field.SetValue(current.Instance, obj4);
						}
						todo.Enqueue(new EnumerateFieldsTodo
						{
							Prefix = current.Prefix + field.Name + ".",
							Instance = obj4,
							Type = field.FieldType
						});
					}
					else if (field.FieldType.IsEnum)
					{
						yield return new EnumField(current.Prefix + field.Name, field, current.Instance);
					}
					else
					{
						yield return new PrimitiveField(current.Prefix + field.Name, field, current.Instance);
					}
				}
				if (current.Type.BaseType != null)
				{
					todo.Enqueue(new EnumerateFieldsTodo
					{
						Prefix = current.Prefix + "base.",
						Instance = current.Instance,
						Type = current.Type.BaseType
					});
				}
			}
		}

		public static object CreateInstance(Type type)
		{
			if (!type.IsClass)
			{
				return Activator.CreateInstance(type);
			}
			if (type == typeof(string))
			{
				return null;
			}
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor != null)
			{
				return constructor.Invoke(null);
			}
			return FormatterServices.GetUninitializedObject(type);
		}
	}
}
