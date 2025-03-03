using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class ClearWaypointAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Service.Get<ZonePathing>().ClearWaypoint();
			Finish();
		}
	}
}
