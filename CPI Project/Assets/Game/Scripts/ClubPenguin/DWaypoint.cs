using UnityEngine;

namespace ClubPenguin
{
	public class DWaypoint
	{
		public string WaypointName = "";

		public GameObject WaypointObject = null;

		public string WaypointZone = "";

		public bool ShowOnScreenIndicator = true;

		public Vector3 OnScreenIndicatorOffset = Vector3.zero;

		public DWaypoint(string waypointName, GameObject waypointObject, string waypointZone, bool showOnScreenIndicator, Vector3 onScreenIndicatorOffset)
		{
			WaypointName = waypointName;
			WaypointObject = waypointObject;
			WaypointZone = waypointZone;
			ShowOnScreenIndicator = showOnScreenIndicator;
			OnScreenIndicatorOffset = onScreenIndicatorOffset;
		}
	}
}
