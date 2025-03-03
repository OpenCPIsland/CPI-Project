using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class StartEventSource : MonoBehaviour
	{
		public string Target;

		public string Event;

		public bool AllowMissingStateMachine = false;

		private void Start()
		{
			if (!string.IsNullOrEmpty(Target))
			{
				StateMachineContext stateMachineContext = GetComponentInParent<StateMachineContext>();
				if (stateMachineContext == null)
				{
					GameObject gameObject = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root);
					if (gameObject != null)
					{
						stateMachineContext = gameObject.GetComponent<StateMachineContext>();
					}
				}
				if (stateMachineContext != null)
				{
					if (stateMachineContext.ContainsStateMachine(Target))
					{
						stateMachineContext.SendEvent(new ExternalEvent(Target, Event));
					}
					else
					{
						CoroutineRunner.Start(sendEventAtEndOfFrame(stateMachineContext, new ExternalEvent(Target, Event)), this, "Start Event Delay");
					}
				}
				else if (!AllowMissingStateMachine)
				{
					Log.LogError(this, "Could not find a StateMachineContext");
				}
			}
			else
			{
				StateMachine component = GetComponent<StateMachine>();
				if (component != null)
				{
					component.SendEvent(Event);
				}
				else
				{
					Log.LogError(this, "Could not find a StateMachine");
				}
			}
		}

		private IEnumerator sendEventAtEndOfFrame(StateMachineContext stateMachineContext, ExternalEvent externalEvent)
		{
			yield return new WaitForEndOfFrame();
			stateMachineContext.SendEvent(externalEvent);
		}
	}
}
