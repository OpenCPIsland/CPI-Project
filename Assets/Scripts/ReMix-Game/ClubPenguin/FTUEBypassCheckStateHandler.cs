using ClubPenguin.Analytics;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class FTUEBypassCheckStateHandler : AbstractAccountStateHandler
	{
		public string ShowFTUEEvent;

		public string SkipFTUEEvent;

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				if (skipFTUE())
				{
					recordBI(SkipFTUEEvent);
					rootStateMachine.SendEvent(SkipFTUEEvent);
				}
				else
				{
					recordBI(ShowFTUEEvent);
					rootStateMachine.SendEvent(ShowFTUEEvent);
				}
			}
		}

		private bool skipFTUE()
		{
			bool result = false;
			if (Service.Get<MembershipService>().LoginViaRestore || SceneManager.GetActiveScene().name != "Home")
			{
				result = true;
			}
			if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
			{
				result = true;
			}
			return result;
		}

		private void recordBI(string nextEvent)
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "27", "check_skipftue", nextEvent);
			rootStateMachine.SendEvent(nextEvent);
		}
	}
}
