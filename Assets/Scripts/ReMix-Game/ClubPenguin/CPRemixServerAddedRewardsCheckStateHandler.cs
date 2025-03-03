using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class CPRemixServerAddedRewardsCheckStateHandler : AbstractAccountStateHandler
	{
		public string ContinueEvent;

		public PrefabContentKey FriendshipPromptPrefabContentKey;

		private Queue<ServerAddedReward> rewardsToShow;

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimServerAddedRewardsFound>(onClaimServerAddedRewardsFound);
				Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimServerAddedRewardsNotFound>(onClaimServerAddedRewardsNotFound);
				Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "28", "check_server_added_rewards", ContinueEvent);
				Service.Get<INetworkServicesManager>().RewardService.ClaimServerAddedRewards();
			}
		}

		private bool onClaimServerAddedRewardsFound(RewardServiceEvents.ClaimServerAddedRewardsFound evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimServerAddedRewardsFound>(onClaimServerAddedRewardsFound);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimServerAddedRewardsNotFound>(onClaimServerAddedRewardsNotFound);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "09", "server_added_rewards_recieved");
			Service.Get<EventDispatcher>().AddListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			rewardsToShow = new Queue<ServerAddedReward>(evt.ServerAddedRewards);
			ShowGiftMessage(evt.ServerAddedRewards);
			return false;
		}

		public void ShowGiftMessage(List<ServerAddedReward> rewards)
		{
			string text = "";
			string bodyText = "";
			string i18nText = "";
			string tier = "";
			if (rewards.Count > 1)
			{
				bodyText = Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referrer.B2");
				foreach (ServerAddedReward reward in rewards)
				{
					string str = string.Format(Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referrer.B2.Header"), reward.instanceId);
					text = text + str + "\n";
				}
				i18nText = Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referrer.B3");
				tier = "thankyou_multiple";
			}
			else if (rewards.Count == 1)
			{
				ServerAddedReward current = rewards[0];
				if (current.instanceId == getCurrentDisplayName())
				{
					AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
					text = string.Format(Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referee.A"), accountFlowData.Referrer);
					bodyText = Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referee.A1");
					i18nText = Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.PopUp.A");
					tier = "welcome";
				}
				else
				{
					text = string.Format(Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referrer.B.Header"), current.instanceId);
					bodyText = Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referrer.B.Body");
					i18nText = Service.Get<Localizer>().GetTokenTranslation("Playercard.FriendInvite.Referrer.B3");
					tier = "thankyou_single";
				}
			}
			DPrompt data = new DPrompt(text, bodyText, DPrompt.ButtonFlags.OK, null, true, true, true);
			data.SetText(DPrompt.PROMPT_TEXT_INFO, i18nText, true);
			Service.Get<ICPSwrveService>().Action("refer_gift_award", tier);
			Content.LoadAsync(delegate(string path, GameObject prefab)
			{
				onFriendshipPromptLoaded(data, prefab);
			}, FriendshipPromptPrefabContentKey);
		}

		private string getCurrentDisplayName()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				return component.DisplayName;
			}
			return "";
		}

		private void onFriendshipPromptLoaded(DPrompt data, GameObject membershipPromptPrefab)
		{
			PromptController component = membershipPromptPrefab.GetComponent<PromptController>();
			Service.Get<PromptManager>().ShowPrompt(data, onPromptFinished, component);
		}

		private void onPromptFinished(DPrompt.ButtonFlags buttonFlags)
		{
			showNextReward();
		}

		private bool onClaimServerAddedRewardsNotFound(RewardServiceEvents.ClaimServerAddedRewardsNotFound evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimServerAddedRewardsFound>(onClaimServerAddedRewardsFound);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.ClaimServerAddedRewardsNotFound>(onClaimServerAddedRewardsNotFound);
			rootStateMachine.SendEvent(ContinueEvent);
			return false;
		}

		private void showNextReward()
		{
			ServerAddedReward serverAddedReward = rewardsToShow.Dequeue();
			ShowRewardPopup.Builder builder = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.generic, serverAddedReward.reward).setRewardSource("ServerAddedReward");
			builder.setHeaderText("Rewards.ServerAdded.GiftTitle");
			ShowRewardPopup showRewardPopup = builder.Build();
			showRewardPopup.Execute();
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
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.RewardPopupComplete>(onRewardPopupComplete);
			rootStateMachine.SendEvent(ContinueEvent);
		}
	}
}
