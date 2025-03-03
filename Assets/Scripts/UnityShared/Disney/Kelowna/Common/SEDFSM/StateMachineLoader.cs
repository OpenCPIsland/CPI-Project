using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class StateMachineLoader : MonoBehaviour
	{
		[Serializable]
		public struct Binding
		{
			public string Name;

			public StateMachineDefinition Definition;
		}

		public Binding[] Bindings;

		public void Start()
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

		public bool ContainsBindingName(string bindingName)
		{
			for (int i = 0; i < Bindings.Length; i++)
			{
				if (Bindings[i].Name == bindingName)
				{
					return true;
				}
			}
			return false;
		}

		public void OnDestroy()
		{
			StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
			if (componentInParent != null)
			{
				for (int i = 0; i < Bindings.Length; i++)
				{
					string name = Bindings[i].Name;
					componentInParent.RemoveStateMachine(name);
				}
			}
		}
	}
}
