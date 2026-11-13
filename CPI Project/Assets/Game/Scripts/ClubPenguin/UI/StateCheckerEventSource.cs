using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class StateCheckerEventSource : MonoBehaviour
	{
		[Serializable]
		private struct StateToEvent
		{
			public string State;

			public string Event;

			public StateToEvent(string state, string eventName)
			{
				State = state;
				Event = eventName;
			}
		}

		[SerializeField]
		private string StateMachineToCheck = string.Empty;

		[SerializeField]
		private string Target = string.Empty;

		[SerializeField]
		private StateToEvent[] StatesToEvents = null;

		private IEnumerator Start()
		{
			if (!string.IsNullOrEmpty(StateMachineToCheck))
			{
				StateMachineContext stateMachineContext = GetComponentInParent<StateMachineContext>();
				while (!stateMachineContext.ContainsStateMachine(StateMachineToCheck))
				{
					yield return null;
				}
				string state = stateMachineContext.GetStateMachineState(StateMachineToCheck);
				int num = 0;
				while (true)
				{
					if (num < StatesToEvents.Length)
					{
						if (StatesToEvents[num].State == state)
						{
							break;
						}
						num++;
						continue;
					}
					yield break;
				}
				stateMachineContext.SendEvent(new ExternalEvent(Target, StatesToEvents[num].Event));
			}
			else
			{
				Log.LogError(this, "StateMachineToCheck was null or empty");
			}
		}
	}
}
