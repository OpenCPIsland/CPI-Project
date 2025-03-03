using ClubPenguin.Analytics;
using ClubPenguin.Net;
using ClubPenguin.Rewards;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class CPRemixMigrationRewardsCheckStateHandler : AbstractAccountStateHandler
	{
		public string ContinueEvent;

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimPreregistrationRewardFound>(onClaimPreregistrationRewardFound);
				Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimPreregistrationRewardNotFound>(onClaimPreregistrationRewardNotFound);
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "25", "check_preregistration_rewards", ContinueEvent);
				Service.Get<INetworkServicesManager>().RewardService.ClaimPreregistrationRewards();
			}
		}

		private bool onClaimPreregistrationRewardFound(RewardServiceEvents.ClaimPreregistrationRewardFound evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimPreregistrationRewardFound>(onClaimPreregistrationRewardFound);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimPreregistrationRewardNotFound>(onClaimPreregistrationRewardNotFound);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "07", "migration_rewards");
			Service.Get<EventDispatcher>().AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			ShowRewardPopup.Builder builder = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.generic, evt.Reward).setRewardSource("PreRegistrationReward");
			builder.setHeaderText("GlobalUI.RegistrationRewards.Title");
			ShowRewardPopup showRewardPopup = builder.Build();
			showRewardPopup.Execute();
			return false;
		}

		private bool onClaimPreregistrationRewardNotFound(RewardServiceEvents.ClaimPreregistrationRewardNotFound evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimPreregistrationRewardFound>(onClaimPreregistrationRewardFound);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimPreregistrationRewardNotFound>(onClaimPreregistrationRewardNotFound);
			rootStateMachine.SendEvent(ContinueEvent);
			return false;
		}

		private bool onRewardPopupComplete(RewardEvents.RewardPopupComplete evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			rootStateMachine.SendEvent(ContinueEvent);
			return false;
		}
	}
}
