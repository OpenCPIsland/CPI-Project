using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class ProximityBroadcaster : MonoBehaviour
	{
		public string ProximityGroup;

		public float Distance;

		public Vector3 Offset;

		private Color HighlightColor = Color.green;

		public virtual void OnProximityEnter(ProximityListener other)
		{
		}

		public virtual void OnProximityStay(ProximityListener other)
		{
		}

		public virtual void OnProximityExit(ProximityListener other)
		{
		}

		public virtual void Awake()
		{
		}

		public virtual void Start()
		{
			Service.Get<ProximityService>().AddBroadcaster(this);
		}

		public virtual void OnDestroy()
		{
			Service.Get<ProximityService>().RemoveBroadcaster(this);
		}

		public virtual void OnDrawGizmos()
		{
		}

		public virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = HighlightColor;
			Gizmos.DrawWireSphere(base.transform.position + Offset, Distance);
		}
	}
}
