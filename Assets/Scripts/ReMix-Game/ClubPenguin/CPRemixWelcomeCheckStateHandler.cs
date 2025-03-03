using ClubPenguin.Analytics;
using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class CPRemixWelcomeCheckStateHandler : AbstractAccountStateHandler
	{
		public string ShowWelcomeEvent;

		public string SkipWelcomeEvent;

		public void OnStateChanged(string state)
		{
			if (!base.gameObject.IsDestroyed() && state == HandledState && rootStateMachine != null)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				if (cPDataEntityCollection == null || DataEntityHandle.IsNullValue(cPDataEntityCollection.LocalPlayerHandle))
				{
					Log.LogError(this, "Unable to resolve data entity collection");
				}
				ProfileData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
					string text = (component.PenguinAgeInDays >= 1 || accountFlowData.SkipWelcome || !component.IsFirstTimePlayer) ? SkipWelcomeEvent : ShowWelcomeEvent;
					Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "23", "check_showwelcome", text);
					rootStateMachine.SendEvent(text);
				}
				else
				{
					Log.LogError(this, "No profile data found for local player");
				}
			}
		}
	}
}
