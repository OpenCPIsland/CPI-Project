using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin.UI
{
	public abstract class AbstractEventSource : MonoBehaviour
	{
		public string Target;

		public string Event;

		protected void sendEvent()
		{
			if (!string.IsNullOrEmpty(Event))
			{
				if (!string.IsNullOrEmpty(Target))
				{
					StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
					componentInParent.SendEvent(new ExternalEvent(Target, Event));
				}
				else
				{
					StateMachine componentInParent2 = GetComponentInParent<StateMachine>();
					componentInParent2.SendEvent(Event);
				}
			}
		}
	}
}
