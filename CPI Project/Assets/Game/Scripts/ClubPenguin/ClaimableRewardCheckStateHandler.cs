using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class ClaimableRewardCheckStateHandler : AbstractAccountStateHandler
	{
		private struct ClaimableRewardData
		{
			public string RewardTitle;

			public Reward Reward;

			public ClaimableRewardData(string rewardTitle, Reward reward)
			{
				RewardTitle = rewardTitle;
				Reward = reward;
			}
		}

		public string ContinueEvent;

		private Queue<ClaimableRewardData> rewardsToShow;

		private Queue<int> rewardsToClaimById;

		private Dictionary<int, ClaimableRewardDefinition> claimableRewardDefinition;

		private CPDataEntityCollection dataEntityCollection;

		private ContentSchedulerService contentSchedulerService;

		private ClaimedRewardIdsData claimedRewardIdsData;

		public new void Start()
		{
			base.Start();
			claimableRewardDefinition = Service.Get<GameData>().Get<Dictionary<int, ClaimableRewardDefinition>>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			contentSchedulerService = Service.Get<ContentSchedulerService>();
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimedReward>(onClaimedReward);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimableRewardFail>(onClaimableRewardFail);
		}

		public void onDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimedReward>(onClaimedReward);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimableRewardFail>(onClaimableRewardFail);
		}

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				ProfileData component = dataEntityCollection.GetComponent<ProfileData>(dataEntityCollection.LocalPlayerHandle);
				if (component != null && component.IsFirstTimePlayer)
				{
					showRewardsComplete();
					return;
				}
				rewardsToShow = new Queue<ClaimableRewardData>();
				rewardsToClaimById = new Queue<int>();
				claimedRewardIdsData = dataEntityCollection.GetComponent<ClaimedRewardIdsData>(dataEntityCollection.LocalPlayerHandle);
				parseUnlockDefinitions();
			}
		}

		private bool isMember()
		{
			MembershipData component = dataEntityCollection.GetComponent<MembershipData>(dataEntityCollection.LocalPlayerHandle);
			return component != null && component.IsMember;
		}

		private void parseUnlockDefinitions()
		{
			foreach (ClaimableRewardDefinition value in claimableRewardDefinition.Values)
			{
				if (value.ClaimOnLogin && (!value.IsMemberOnly || isMember()))
				{
					DateTime target = contentSchedulerService.ScheduledEventDate();
					ScheduledEventDateDefinition definitionById = Service.Get<IGameData>().GetDefinitionById(value.DateDefinitionKey);
					if (DateTimeUtils.DoesDateFallBetween(target, definitionById.Dates.StartDate, definitionById.Dates.EndDate) && (claimedRewardIdsData == null || !claimedRewardIdsData.RewardIds.Contains(value.Id)))
					{
						rewardsToClaimById.Enqueue(value.Id);
					}
				}
			}
			if (rewardsToClaimById.Count > 0)
			{
				claimNextReward();
			}
			else
			{
				showRewardsComplete();
			}
		}

		private void claimNextReward()
		{
			if (rewardsToClaimById.Count > 0)
			{
				int rewardId = rewardsToClaimById.Peek();
				Service.Get<INetworkServicesManager>().RewardService.ClaimReward(rewardId);
			}
			else if (rewardsToShow.Count > 0)
			{
				Service.Get<EventDispatcher>().AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
				showNextReward();
			}
			else
			{
				showRewardsComplete();
			}
		}

		private bool onClaimedReward(RewardServiceEvents.ClaimedReward evt)
		{
			int num = rewardsToClaimById.Dequeue();
			ClaimableRewardDefinition claimableRewardDefinition = this.claimableRewardDefinition[num];
			ClaimableRewardData item = new ClaimableRewardData(claimableRewardDefinition.TitleToken, claimableRewardDefinition.Reward.ToReward());
			rewardsToShow.Enqueue(item);
			ClaimedRewardIdsData claimedRewardIdsData = dataEntityCollection.GetComponent<ClaimedRewardIdsData>(dataEntityCollection.LocalPlayerHandle);
			if (claimedRewardIdsData == null)
			{
				claimedRewardIdsData = dataEntityCollection.AddComponent<ClaimedRewardIdsData>(dataEntityCollection.LocalPlayerHandle);
			}
			claimedRewardIdsData.RewardIds.Add(num);
			claimNextReward();
			return false;
		}

		private bool onClaimableRewardFail(RewardServiceEvents.ClaimableRewardFail evt)
		{
			rewardsToClaimById.Dequeue();
			claimNextReward();
			return false;
		}

		private void showNextReward()
		{
			ClaimableRewardData claimableRewardData = rewardsToShow.Dequeue();
			ShowRewardPopup.Builder builder = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.generic, claimableRewardData.Reward).setRewardSource("ClaimableReward");
			builder.setHeaderText(claimableRewardData.RewardTitle);
			ShowRewardPopup showRewardPopup = builder.Build();
			showRewardPopup.Execute();
			Service.Get<ICPSwrveService>().Action("game.marketing_promotion", claimableRewardData.RewardTitle);
		}

		private bool onRewardPopupComplete(RewardEvents.RewardPopupComplete evt)
		{
			if (rewardsToShow.Count > 0)
			{
				showNextReward();
			}
			else
			{
				Service.Get<EventDispatcher>().RemoveListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
				showRewardsComplete();
			}
			return false;
		}

		private void showRewardsComplete()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "26", "check_claimable_rewards", ContinueEvent);
			rootStateMachine.SendEvent(ContinueEvent);
		}
	}
}
