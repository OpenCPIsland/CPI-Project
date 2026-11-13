using Disney.Kelowna.Common.SEDFSM;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Analytics
{
	public class SwrveABTestStateMachineLoader : StateMachineLoader
	{
		[Serializable]
		public struct SwrveABTest
		{
			[Tooltip("Swrve Resource Unique ID must match Binding being tested")]
			public string Binding;

			[Tooltip("Swrve Resource Attribute containing names of State Machine Definitions being varied")]
			public string Attribute;

			[Tooltip("Include all state machine definitions that are referenced in the AB Test Variants in Swrve including control/default")]
			public StateMachineDefinition[] Definitions;
		}

		[Header("Swrve Resource AB Test Settings")]
		public SwrveABTest ABTest;

		public new void Start()
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
			Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				dictionary[componentsInChildren[i].name] = componentsInChildren[i].gameObject;
			}
			for (int i = 0; i < Bindings.Length; i++)
			{
				string name = Bindings[i].Name;
				GameObject value;
				if (!dictionary.TryGetValue(name, out value))
				{
					value = new GameObject(name);
					value.transform.SetParent(base.transform, false);
				}
				StateMachine.Create(value, Bindings[i].Definition);
			}
		}
	}
}
