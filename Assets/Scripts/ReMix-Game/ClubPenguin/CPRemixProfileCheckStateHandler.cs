using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class CPRemixProfileCheckStateHandler : AbstractAccountStateHandler
	{
		public string FirstTimePlayerEvent;

		public string ReturnPlayerEvent;

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
			ProfileData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				MembershipService membershipService = Service.Get<MembershipService>();
				string text;
				if (component.IsFirstTimePlayer && !membershipService.LoginViaMembership && !membershipService.LoginViaRestore)
				{
					QuestService questService = Service.Get<QuestService>();
					GameStateController gameStateController = Service.Get<GameStateController>();
					text = ((questService.ActiveQuest == null || !(questService.ActiveQuest.Id == gameStateController.FTUEConfig.FtueQuestId)) ? FirstTimePlayerEvent : ReturnPlayerEvent);
				}
				else
				{
					if (membershipService.LoginViaRestore)
					{
						SessionManager sessionManager = Service.Get<SessionManager>();
						if (sessionManager.HasSession)
						{
							sessionManager.ReturnToRestorePurchases();
						}
						Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "21", "check_cpremixprofile", "ReturnToSettings");
						AccountPopupController componentInParent = GetComponentInParent<AccountPopupController>();
						componentInParent.OnClosePopup();
						return;
					}
					text = ReturnPlayerEvent;
				}
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "21", "check_cpremixprofile", text);
				rootStateMachine.SendEvent(text);
				return;
			}
			throw new MissingReferenceException("No profile data found for local player");
		}
	}
}
