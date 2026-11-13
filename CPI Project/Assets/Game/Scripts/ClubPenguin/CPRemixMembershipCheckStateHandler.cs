using ClubPenguin.Analytics;
using ClubPenguin.Commerce;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin
{
	public class CPRemixMembershipCheckStateHandler : AbstractAccountStateHandler
	{
		public string HasMembershipEvent;

		public string NoMembershipEvent;

		public string ExpiredMembershipEvent;

		public void OnStateChanged(string state)
		{
			if (!(state == HandledState) || !(rootStateMachine != null))
			{
				return;
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
			{
				throw new Exception("Unable to resolve data entity collection");
			}
			string text = "membership_offer";
			string text2;
			MembershipData component;
			if (!Service.Get<MembershipService>().IsPurchaseFunnelAvailable())
			{
				text2 = HasMembershipEvent;
			}
			else if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				if (component.MembershipType == MembershipType.Member)
				{
					text2 = HasMembershipEvent;
				}
				else if (Service.Get<CommerceService>().IsPurchaseInProgress())
				{
					Service.Get<PromptManager>().ShowPrompt("MembershipInProcessPrompt", null);
					text2 = HasMembershipEvent;
				}
				else if (component.MembershipTrialAvailable)
				{
					text2 = NoMembershipEvent;
				}
				else
				{
					text = "membership_expired";
					text2 = ExpiredMembershipEvent;
				}
			}
			else
			{
				text2 = NoMembershipEvent;
			}
			Service.Get<ICPSwrveService>().Funnel(text, "00", "check_membership", text2);
			rootStateMachine.SendEvent(text2);
			Service.Get<MembershipService>().MembershipFunnelName = text;
		}
	}
}
