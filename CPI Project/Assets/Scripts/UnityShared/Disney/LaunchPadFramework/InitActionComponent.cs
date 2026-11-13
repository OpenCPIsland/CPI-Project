using Disney.LaunchPadFramework.Utility.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public abstract class InitActionComponent : MonoBehaviour, ITopologicalNode
	{
		private List<string> dependencies = new List<string>();

		public abstract bool HasSecondPass
		{
			get;
		}

		public abstract bool HasCompletedPass
		{
			get;
		}

		public string TopologicalIdentifier
		{
			get
			{
				return GetType().ToString();
			}
		}

		public List<string> TopologicalDependencies
		{
			get
			{
				return dependencies;
			}
		}

		public abstract IEnumerator PerformFirstPass();

		public virtual IEnumerator PerformSecondPass()
		{
			yield break;
		}

		public virtual void OnInitializationComplete()
		{
		}

		public void Awake()
		{
			MemberInfo type = GetType();
			object[] customAttributes = type.GetCustomAttributes(true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				RequireComponent requireComponent = customAttributes[i] as RequireComponent;
				if (requireComponent != null)
				{
					addDependency(requireComponent.m_Type0);
					addDependency(requireComponent.m_Type1);
					addDependency(requireComponent.m_Type2);
				}
			}
		}

		public void Start()
		{
		}

		private void addDependency(Type type)
		{
			if (type != null)
			{
				string item = type.ToString();
				if (!dependencies.Contains(item))
				{
					dependencies.Add(item);
				}
			}
		}
	}
}
