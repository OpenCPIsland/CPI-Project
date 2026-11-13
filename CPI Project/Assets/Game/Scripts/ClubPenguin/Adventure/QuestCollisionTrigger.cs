using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Collider))]
	public class QuestCollisionTrigger : MonoBehaviour
	{
		public string ColliderTag = "";

		private string EventName;

		public void Start()
		{
			EventName = base.gameObject.name + "Collided";
		}

		public void OnTriggerEnter(Collider collider)
		{
			SendCollisionEvent(collider);
		}

		public void OnCollisionEnter(Collision collision)
		{
			SendCollisionEvent(collision.collider);
		}

		private void SendCollisionEvent(Collider collider)
		{
			if (ColliderTag == "" || collider.gameObject.CompareTag(ColliderTag))
			{
				Service.Get<QuestService>().SendEvent(EventName);
			}
		}
	}
}
