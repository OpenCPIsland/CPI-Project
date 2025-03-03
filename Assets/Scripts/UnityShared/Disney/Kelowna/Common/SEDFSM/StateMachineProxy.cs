using System.Collections.Generic;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class StateMachineProxy : MonoBehaviour
	{
		public string TargetStateMachine;

		private Queue<string> events;

		private void Awake()
		{
			StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
			componentInParent.AddStateMachineProxy(TargetStateMachine, this);
			events = new Queue<string>();
		}

		public void AddEvent(string evt)
		{
			events.Enqueue(evt);
		}

		public IEnumerable<string> DequeueEvents()
		{
			while (events.Count > 0)
			{
				yield return events.Dequeue();
			}
		}
	}
}
