using ClubPenguin.Adventure;
using ClubPenguin.Props;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Collider))]
	public class SwordTrigger : MonoBehaviour
	{
		public GameObject Particles;

		public string QuestEvent;

		public void OnTriggerEnter(Collider otherCollider)
		{
			if (otherCollider.CompareTag("SwordCollider"))
			{
				if (Particles != null)
				{
					Vector3 a = Vector3.Normalize(base.transform.position - otherCollider.transform.position);
					Object.Instantiate(Particles, otherCollider.transform.position + a * 0.15f, Quaternion.LookRotation(-a));
				}
				PropUser propUserRef = otherCollider.transform.parent.GetComponent<Prop>().PropUserRef;
				if (!string.IsNullOrEmpty(QuestEvent) && propUserRef.CompareTag("Player"))
				{
					Service.Get<QuestService>().SendEvent(QuestEvent);
				}
			}
		}
	}
}
