using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(StateMachineContext))]
	public class SEDFSMStartEventTrigger : MonoBehaviour
	{
		public string FSMName;

		private void Start()
		{
			string startEvent = GetComponentInParent<SEDFSMStartEventSource>().StartEvent;
			StateMachineContext component = GetComponent<StateMachineContext>();
			component.SendEvent(new ExternalEvent(FSMName, startEvent));
		}
	}
}
