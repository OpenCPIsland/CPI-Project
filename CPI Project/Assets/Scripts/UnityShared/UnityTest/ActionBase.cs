using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityTest
{
	public abstract class ActionBase : ScriptableObject
	{
		public GameObject go;

		protected object m_ObjVal;

		private MemberResolver m_MemberResolver;

		public string thisPropertyPath = "";

		protected virtual bool UseCache
		{
			get
			{
				return false;
			}
		}

		public virtual Type[] GetAccepatbleTypesForA()
		{
			return null;
		}

		public virtual int GetDepthOfSearch()
		{
			return 2;
		}

		public virtual string[] GetExcludedFieldNames()
		{
			return new string[0];
		}

		public bool Compare()
		{
			if (m_MemberResolver == null)
			{
				m_MemberResolver = new MemberResolver(go, thisPropertyPath);
			}
			m_ObjVal = m_MemberResolver.GetValue(UseCache);
			return Compare(m_ObjVal);
		}

		protected abstract bool Compare(object objVal);

		public virtual Type GetParameterType()
		{
			return typeof(object);
		}

		public virtual string GetConfigurationDescription()
		{
			string text = "";
			foreach (FieldInfo item in from info in GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
				where info.FieldType.IsSerializable
				select info)
			{
				object obj = item.GetValue(this);
				if (obj is double)
				{
					obj = ((double)obj).ToString("0.########");
				}
				if (obj is float)
				{
					obj = ((float)obj).ToString("0.########");
				}
				text = string.Concat(text, obj, " ");
			}
			return text;
		}

		private IEnumerable<FieldInfo> GetFields(Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		}

		public ActionBase CreateCopy(GameObject oldGameObject, GameObject newGameObject)
		{
			ActionBase actionBase = ScriptableObject.CreateInstance(GetType()) as ActionBase;
			IEnumerable<FieldInfo> fields = GetFields(GetType());
			foreach (FieldInfo item in fields)
			{
				object obj = item.GetValue(this);
				if (obj is GameObject && obj as GameObject == oldGameObject)
				{
					obj = newGameObject;
				}
				item.SetValue(actionBase, obj);
			}
			return actionBase;
		}

		public virtual void Fail(AssertionComponent assertion)
		{
			Debug.LogException(new AssertionException(assertion), assertion.GetFailureReferenceObject());
		}

		public virtual string GetFailureMessage()
		{
			return string.Concat(GetType().Name, " assertion failed.\n(", go, ").", thisPropertyPath, " failed. Value: ", m_ObjVal);
		}
	}
}
