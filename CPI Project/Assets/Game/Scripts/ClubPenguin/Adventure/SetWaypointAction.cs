using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SetWaypointAction : FsmStateAction
	{
		public string WaypointName;

		public FsmGameObject WaypointObject;

		public string WaypointZone;

		public bool ShowOnscreenIndicator = true;

		public FsmVector3 OnscreenIndicatorOffset;

		public override void Reset()
		{
			WaypointName = "";
			WaypointObject = null;
			WaypointZone = "";
			OnscreenIndicatorOffset = new FsmVector3
			{
				UseVariable = true
			};
			ShowOnscreenIndicator = true;
		}

		public override void OnEnter()
		{
			DWaypoint waypoint = new DWaypoint(WaypointName, WaypointObject.Value, WaypointZone, ShowOnscreenIndicator, OnscreenIndicatorOffset.Value);
			Service.Get<ZonePathing>().SetWaypoint(waypoint);
			Finish();
		}
	}
}
