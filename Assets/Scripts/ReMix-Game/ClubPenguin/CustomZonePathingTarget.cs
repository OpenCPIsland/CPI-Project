using UnityEngine;

namespace ClubPenguin
{
	public class CustomZonePathingTarget : MonoBehaviour
	{
		public GameObject WaypointPosition;

		public CustomZonePathingTrigger OverrideTrigger;

		public bool IsActiveInTrigger = false;

		public bool IsCustomWaypointActive()
		{
			if (OverrideTrigger == null)
			{
				return false;
			}
			if ((IsActiveInTrigger && OverrideTrigger.IsInTrigger) || (!IsActiveInTrigger && !OverrideTrigger.IsInTrigger))
			{
				return true;
			}
			return false;
		}
	}
}
