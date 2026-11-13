using UnityEngine;
using UnityEngine.Events;

namespace ClubPenguin
{
	public class SnowballTarget : MonoBehaviour
	{
		public int Id;

		public UnityEvent HitEventVoid;

		public SnowballHitTargetEvent HitEvent;

		public void OnHit(long playerId)
		{
			HitEventVoid.Invoke();
			HitEvent.Invoke(playerId, Id);
		}

		protected void OnCollisionEnter(Collision collision)
		{
			SnowballController component = collision.gameObject.GetComponent<SnowballController>();
			if (component != null)
			{
				OnHit(component.OwnerId);
			}
		}
	}
}
