using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.NPC;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	public class MarketplaceEventItem
	{
		public ClaimableRewardDefinition EventItemDefinition;

		private List<DReward> rewards;

		public event System.Action CollectItemSucceeded;

		public event System.Action CollectItemFailed;

		public List<DReward> GetRewards()
		{
			if (rewards == null)
			{
				rewards = RewardUtils.GetDRewardFromReward(EventItemDefinition.Reward.ToReward());
			}
			return rewards;
		}

		public int GetCoinReward()
		{
			CoinReward rewardable;
			if (EventItemDefinition.Reward.ToReward().TryGetValue(out rewardable))
			{
				return rewardable.Coins;
			}
			return 0;
		}

		public Dictionary<string, int> GetXpReward()
		{
			MascotXPReward rewardable;
			if (EventItemDefinition.Reward.ToReward().TryGetValue(out rewardable))
			{
				return rewardable.XP;
			}
			return null;
		}

		public bool IsAvailable()
		{
			DateTime target = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			ScheduledEventDateDefinition definitionById = Service.Get<IGameData>().GetDefinitionById(EventItemDefinition.DateDefinitionKey);
			return DateTimeUtils.DoesDateFallBetween(target, definitionById.Dates.StartDate, definitionById.Dates.EndDate);
		}

		public bool HasItem()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			ClaimedRewardIdsData component = cPDataEntityCollection.GetComponent<ClaimedRewardIdsData>(cPDataEntityCollection.LocalPlayerHandle);
			return component != null && component.RewardIds.Contains(EventItemDefinition.Id);
		}

		public void CollectItem()
		{
			if (!EventItemDefinition.ClaimOnLogin && !HasItem() && IsAvailable())
			{
				Service.Get<INetworkServicesManager>().RewardService.ClaimReward(EventItemDefinition.Id);
				Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimedReward>(onClaimedReward);
				Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimableRewardFail>(onClaimableRewardFail);
			}
		}

		private bool onClaimedReward(RewardServiceEvents.ClaimedReward evt)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			ClaimedRewardIdsData component;
			if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component = cPDataEntityCollection.AddComponent<ClaimedRewardIdsData>(cPDataEntityCollection.LocalPlayerHandle);
			}
			component.RewardIds.Add(EventItemDefinition.Id);
			if (this.CollectItemSucceeded != null)
			{
				this.CollectItemSucceeded();
				this.CollectItemSucceeded = null;
			}
			return false;
		}

		private bool onClaimableRewardFail(RewardServiceEvents.ClaimableRewardFail evt)
		{
			if (this.CollectItemFailed != null)
			{
				this.CollectItemFailed();
				this.CollectItemFailed = null;
			}
			return false;
		}
	}
}
