using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class StateMachineProxyLoader : MonoBehaviour
	{
		[Serializable]
		public struct Binding
		{
			public string Target;

			public string TargetStateMachine;
		}

		public Binding[] Bindings;

		private void Start()
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
			Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				dictionary[componentsInChildren[i].name] = componentsInChildren[i].gameObject;
			}
			for (int i = 0; i < Bindings.Length; i++)
			{
				string target = Bindings[i].Target;
				GameObject value;
				if (!dictionary.TryGetValue(target, out value))
				{
					value = new GameObject(target);
					value.transform.SetParent(base.transform, false);
				}
				StateMachineProxy stateMachineProxy = value.AddComponent<StateMachineProxy>();
				stateMachineProxy.TargetStateMachine = Bindings[i].TargetStateMachine;
			}
		}
	}
}
