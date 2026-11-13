using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class ConnectToOfflineZoneStateHandler : AbstractStateHandler
	{
		protected override void OnEnter()
		{
			Service.Get<ZoneTransitionService>().ConnectToZone(Service.Get<GameStateController>().GetZoneToLoad());
			rootStateMachine.SendEvent("zoneconnectingevent");
		}
	}
}
