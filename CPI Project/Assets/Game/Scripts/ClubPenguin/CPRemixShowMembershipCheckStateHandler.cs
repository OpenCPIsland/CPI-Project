using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class CPRemixShowMembershipCheckStateHandler : AbstractAccountStateHandler
	{
		public string ShowMembershipEvent;

		public string SkipMembershipEvent;

		public string EndMembershipFlowEvent;

		public string ShowAllAccessEvent;

		public string ShowAllAccessOverEvent;

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
				{
					throw new Exception("Unable to resolve data entity collection");
				}
				bool flag = false;
				ProfileData component;
				MembershipData component2;
				if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component) || !cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component2))
				{
					throw new MissingReferenceException("No profile data or membership data found for local player");
				}
				MembershipService membershipService = Service.Get<MembershipService>();
				AccountFlowData accountFlowData = membershipService.GetAccountFlowData();
				bool flag2 = Service.Get<SessionManager>().LocalUser.RegistrationProfile.AccountStatus == AccountStatus.AwaitingParentalConsent;
				bool flag3 = component2.MembershipType == MembershipType.Member;
				string text;
				if (!membershipService.IsPurchaseFunnelAvailable())
				{
					text = SkipMembershipEvent;
				}
				else if (!flag3 && showAllAccess() && !membershipService.LoginViaMembership)
				{
					text = ShowAllAccessEvent;
				}
				else if (!flag3 && showAllAccessOver() && !membershipService.LoginViaMembership)
				{
					text = ShowAllAccessOverEvent;
				}
				else if (flag || (membershipService.LoginViaMembership && !flag3 && !flag2) || (component.IsFirstTimePlayer && !component2.IsMember && !accountFlowData.SkipMembership && !flag2))
				{
					accountFlowData.SkipMembership = true;
					text = ((!component.IsFirstTimePlayer || membershipService.LoginViaMembership) ? ShowMembershipEvent : EndMembershipFlowEvent);
				}
				else
				{
					text = SkipMembershipEvent;
				}
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "22", "check_showmembership", text);
				rootStateMachine.SendEvent(text);
			}
		}

		private bool showAllAccess()
		{
			AllAccessService allAccessService = Service.Get<AllAccessService>();
			if (allAccessService.IsAllAccessActive())
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				DisplayNameData component;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
				{
					string allAccessEventKey = allAccessService.GetAllAccessEventKey();
					if (!AllAccessHelper.HasSeenAllAccessFlow(allAccessEventKey, component.DisplayName))
					{
						AllAccessHelper.SetHasSeenAllAccessFlow(allAccessEventKey, component.DisplayName);
						DataEntityHandle handle = cPDataEntityCollection.AddEntity("AllAccessCelebrationData");
						AllAccessCelebrationData allAccessCelebrationData = cPDataEntityCollection.AddComponent<AllAccessCelebrationData>(handle);
						allAccessCelebrationData.ShowAllAccessCelebration = true;
						return true;
					}
				}
			}
			return false;
		}

		private bool showAllAccessOver()
		{
			if (!Service.Get<AllAccessService>().IsAllAccessActive() && AllAccessHelper.ShowAccessEndedFlow())
			{
				AllAccessHelper.SetHasSeenAllAccessEndedFlow();
				return true;
			}
			return false;
		}
	}
}
