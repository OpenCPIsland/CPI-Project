using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Collider))]
	public class CustomZonePathingTrigger : MonoBehaviour
	{
		public bool DoesOverrideWaypointPosition = false;

		public Transform OverrideWaypointTarget;

		public bool OverrideInTrigger = true;

		public GameObject[] OverrideIgnoreTargets;

		public bool IsInTrigger
		{
			get;
			private set;
		}

		private void OnTriggerEnter(Collider other)
		{
			IsInTrigger = true;
			Service.Get<ZonePathing>().RecalculateWaypoint();
		}

		private void OnTriggerExit(Collider other)
		{
			IsInTrigger = false;
			Service.Get<ZonePathing>().RecalculateWaypoint();
		}

		public bool IsOverrideActive()
		{
			if (DoesOverrideWaypointPosition && ((OverrideInTrigger && IsInTrigger) || (!OverrideInTrigger && !IsInTrigger)))
			{
				return true;
			}
			return false;
		}
	}
}
