using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DestroyEventSource : MonoBehaviour
	{
		public string Target;

		public string Event;

		public bool AllowMissingStateMachine = false;

		private void OnDestroy()
		{
			if (!string.IsNullOrEmpty(Target))
			{
				StateMachineContext stateMachineContext = GetComponentInParent<StateMachineContext>();
				if (stateMachineContext == null)
				{
					GameObject gameObject = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root);
					if (!gameObject.IsDestroyed())
					{
						stateMachineContext = gameObject.GetComponent<StateMachineContext>();
					}
				}
				if (stateMachineContext != null)
				{
					stateMachineContext.SendEvent(new ExternalEvent(Target, Event));
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
	}
}
