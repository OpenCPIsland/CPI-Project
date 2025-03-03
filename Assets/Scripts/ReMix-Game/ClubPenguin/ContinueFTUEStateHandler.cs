using ClubPenguin.Analytics;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class ContinueFTUEStateHandler : AbstractAccountStateHandler
	{
		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "08", "ftue_continue");
				Service.Get<ICPSwrveService>().Action("game.completed_registration");
				Service.Get<GameStateController>().ContinueFTUE();
				AccountPopupController componentInParent = GetComponentInParent<AccountPopupController>();
				componentInParent.OnClosePopup();
			}
		}
	}
}
