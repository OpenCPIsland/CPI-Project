using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityTest
{
	public class MemberResolver
	{
		private object m_CallingObjectRef;

		private MemberInfo[] m_Callstack;

		private readonly GameObject m_GameObject;

		private readonly string m_Path;

		public MemberResolver(GameObject gameObject, string path)
		{
			path = path.Trim();
			ValidatePath(path);
			m_GameObject = gameObject;
			m_Path = path.Trim();
		}

		public object GetValue(bool useCache)
		{
			if (useCache && m_CallingObjectRef != null)
			{
				object obj = m_CallingObjectRef;
				for (int i = 0; i < m_Callstack.Length; i++)
				{
					obj = GetValueFromMember(obj, m_Callstack[i]);
				}
				return obj;
			}
			object obj2 = GetBaseObject();
			MemberInfo[] callstack = GetCallstack();
			m_CallingObjectRef = obj2;
			List<MemberInfo> list = new List<MemberInfo>();
			foreach (MemberInfo memberInfo in callstack)
			{
				obj2 = GetValueFromMember(obj2, memberInfo);
				list.Add(memberInfo);
				if (obj2 == null)
				{
					return null;
				}
				Type type = obj2.GetType();
				if (!IsValueType(type) && type != typeof(string))
				{
					list.Clear();
					m_CallingObjectRef = obj2;
				}
			}
			m_Callstack = list.ToArray();
			return obj2;
		}

		public Type GetMemberType()
		{
			MemberInfo[] callstack = GetCallstack();
			if (callstack.Length == 0)
			{
				return GetBaseObject().GetType();
			}
			MemberInfo memberInfo = callstack[callstack.Length - 1];
			if (memberInfo is FieldInfo)
			{
				return (memberInfo as FieldInfo).FieldType;
			}
			if (memberInfo is MethodInfo)
			{
				return (memberInfo as MethodInfo).ReturnType;
			}
			return null;
		}

		public static bool TryGetMemberType(GameObject gameObject, string path, out Type value)
		{
			try
			{
				MemberResolver memberResolver = new MemberResolver(gameObject, path);
				value = memberResolver.GetMemberType();
				return true;
			}
			catch (InvalidPathException)
			{
				value = null;
				return false;
			}
		}

		public static bool TryGetValue(GameObject gameObject, string path, out object value)
		{
			try
			{
				MemberResolver memberResolver = new MemberResolver(gameObject, path);
				value = memberResolver.GetValue(false);
				return true;
			}
			catch (InvalidPathException)
			{
				value = null;
				return false;
			}
		}

		private object GetValueFromMember(object obj, MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo)
			{
				return (memberInfo as FieldInfo).GetValue(obj);
			}
			if (memberInfo is MethodInfo)
			{
				return (memberInfo as MethodInfo).Invoke(obj, null);
			}
			throw new InvalidPathException(memberInfo.Name);
		}

		private object GetBaseObject()
		{
			if (string.IsNullOrEmpty(m_Path))
			{
				return m_GameObject;
			}
			string type = m_Path.Split('.')[0];
			Component component = m_GameObject.GetComponent(type);
			if (component != null)
			{
				return component;
			}
			return m_GameObject;
		}

		private MemberInfo[] GetCallstack()
		{
			if (m_Path == "")
			{
				return new MemberInfo[0];
			}
			Queue<string> queue = new Queue<string>(m_Path.Split('.'));
			Type type = GetBaseObject().GetType();
			if (type != typeof(GameObject))
			{
				queue.Dequeue();
			}
			List<MemberInfo> list = new List<MemberInfo>();
			while (queue.Count != 0)
			{
				string text = queue.Dequeue();
				FieldInfo field = GetField(type, text);
				if (field != null)
				{
					type = field.FieldType;
					list.Add(field);
					continue;
				}
				PropertyInfo property = GetProperty(type, text);
				if (property != null)
				{
					type = property.PropertyType;
					MethodInfo getMethod = GetGetMethod(property);
					list.Add(getMethod);
					continue;
				}
				throw new InvalidPathException(text);
			}
			return list.ToArray();
		}

		private void ValidatePath(string path)
		{
			bool flag = false;
			if (path.StartsWith(".") || path.EndsWith("."))
			{
				flag = true;
			}
			if (path.IndexOf("..") >= 0)
			{
				flag = true;
			}
			if (Regex.IsMatch(path, "\\s"))
			{
				flag = true;
			}
			if (flag)
			{
				throw new InvalidPathException(path);
			}
		}

		private static bool IsValueType(Type type)
		{
			return type.IsValueType;
		}

		private static FieldInfo GetField(Type type, string fieldName)
		{
			return type.GetField(fieldName);
		}

		private static PropertyInfo GetProperty(Type type, string propertyName)
		{
			return type.GetProperty(propertyName);
		}

		private static MethodInfo GetGetMethod(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetGetMethod();
		}
	}
}
