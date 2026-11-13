using ClubPenguin.Analytics;
using ClubPenguin.Net;
using Disney.Mix.SDK;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class OneIdDisplayNameCheckStateHandler : AbstractAccountStateHandler
	{
		public string RejectedEvent;

		public string AcceptedPendingEvent;

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				string text = (Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayNameProposedStatus != DisplayNameProposedStatus.Rejected && Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayNameProposedStatus != DisplayNameProposedStatus.None && (Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayNameProposedStatus != 0 || !Service.Get<SessionManager>().LocalUser.RegistrationProfile.ProposedDisplayName.StartsWith("DNAME-REJ-"))) ? AcceptedPendingEvent : RejectedEvent;
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "24", "check_displayname", text);
				rootStateMachine.SendEvent(text);
			}
		}
	}
}
