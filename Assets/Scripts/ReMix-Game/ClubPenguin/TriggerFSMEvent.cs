using UnityEngine;

namespace ClubPenguin
{
	public class TriggerFSMEvent : MonoBehaviour
	{
		public enum TriggerTime
		{
			OnEnter,
			OnExit
		}

		[Tooltip("Leave blank for all colliders.")]
		public string ColliderTag;

		public string EventName;

		public TriggerTime TriggerOn = TriggerTime.OnEnter;

		private void execute(Collider trigger)
		{
			if (string.IsNullOrEmpty(ColliderTag) || trigger.CompareTag(ColliderTag))
			{
				PlayMakerFSM[] components = trigger.gameObject.GetComponents<PlayMakerFSM>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].SendEvent(EventName);
				}
			}
		}

		private void OnTriggerEnter(Collider trigger)
		{
			if (TriggerOn == TriggerTime.OnEnter)
			{
				execute(trigger);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			OnTriggerEnter(collision.collider);
		}

		private void OnTriggerExit(Collider trigger)
		{
			if (TriggerOn == TriggerTime.OnExit)
			{
				execute(trigger);
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			OnTriggerExit(collision.collider);
		}
	}
}
