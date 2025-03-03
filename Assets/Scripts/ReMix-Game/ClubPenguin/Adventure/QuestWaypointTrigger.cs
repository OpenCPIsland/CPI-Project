using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Collider))]
	public class QuestWaypointTrigger : MonoBehaviour
	{
		public void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.QuestWaypointTriggerEntered(base.transform.parent.name));
			}
		}
	}
}
