using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using Disney.Native;
using System;

namespace ClubPenguin
{
	public class EndFlowStateHandler : AbstractAccountStateHandler
	{
		public string NormalLoginExitEvent;

		public string StartFTUEExitEvent;

		public string ContinueFTUEExitEvent;

		public string InGameExitEvent;

		public string NotLoggedInExitEvent;

		public string ParentalConsentRequiredEvent;

		public void OnStateChanged(string state)
		{
			if (!(state == HandledState) || !(rootStateMachine != null))
			{
				return;
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(AccountSystemEvents.AccountSystemEnded));
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
			{
				throw new Exception("Unable to resolve data entity collection");
			}
			SessionManager sessionManager = Service.Get<SessionManager>();
			if (sessionManager.HasSession)
			{
				Service.Get<GameSettings>().FirstSession = false;
				ProfileData component;
				MembershipData component2;
				if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component) || !cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component2))
				{
					return;
				}
				bool flag = true;
				if (sessionManager.LocalUser.RegistrationProfile.AccountStatus == AccountStatus.AwaitingParentalConsent)
				{
					NormalLoginExitEvent = ParentalConsentRequiredEvent;
					ContinueFTUEExitEvent = ParentalConsentRequiredEvent;
					InGameExitEvent = ParentalConsentRequiredEvent;
				}
				QuestService questService = Service.Get<QuestService>();
				bool flag2 = false;
				if (questService.ActiveQuest != null && questService.ActiveQuest.Id == Service.Get<GameStateController>().FTUEConfig.FtueQuestId)
				{
					if (component.IsFirstTimePlayer)
					{
						if (flag)
						{
							Service.Get<EventDispatcher>().DispatchEvent(default(SessionEvents.FTUENameObjectiveCompleteEvent));
						}
						else
						{
							Service.Get<EventDispatcher>().DispatchEvent(default(SessionEvents.FTUENameObjectiveCancelledEvent));
						}
					}
					else
					{
						Service.Get<EventDispatcher>().DispatchEvent(default(SessionEvents.FTUENameObjectiveAlreadyDoneEvent));
					}
					flag2 = true;
				}
				LocalPlayerInZoneData component3;
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component3))
				{
					recordBI(InGameExitEvent);
					rootStateMachine.SendEvent(InGameExitEvent);
					return;
				}
				bool flag3 = false;
				flag3 |= MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled;
				if (component.IsFirstTimePlayer && questService.ActiveQuest == null && !flag3)
				{
					recordBI(StartFTUEExitEvent);
					rootStateMachine.SendEvent(StartFTUEExitEvent);
				}
				else if ((!component.IsFTUEComplete && !flag3) || flag2)
				{
					recordBI(ContinueFTUEExitEvent);
					rootStateMachine.SendEvent(ContinueFTUEExitEvent);
				}
				else
				{
					recordBI(NormalLoginExitEvent);
					rootStateMachine.SendEvent(NormalLoginExitEvent);
				}
			}
			else
			{
				recordBI(NotLoggedInExitEvent);
				rootStateMachine.SendEvent(NotLoggedInExitEvent);
			}
		}

		private void recordBI(string nextEvent)
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "27", "end_flow", nextEvent);
		}
	}
}
