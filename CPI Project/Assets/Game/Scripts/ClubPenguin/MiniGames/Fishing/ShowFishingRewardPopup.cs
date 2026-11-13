using ClubPenguin.NPC;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	public class ShowFishingRewardPopup
	{
		private readonly string prizeName;

		private readonly MinigameService minigameService;

		private readonly bool isQuestPrize;

		private static PrefabContentKey halfItemContentKey = new PrefabContentKey("Rewards/RewardPopup/HalfItemRewardPopup");

		private ItemRewardPopupHalf itemRewardPopup;

		public event System.Action PopupDismissed;

		public ShowFishingRewardPopup(string prizeName, MinigameService minigameService, bool isQuestPrize)
		{
			this.prizeName = prizeName;
			this.minigameService = minigameService;
			this.isQuestPrize = isQuestPrize;
		}

		public void Init()
		{
			CoroutineRunner.Start(loadPopup(), this, "loadPopup");
		}

		private IEnumerator loadPopup()
		{
			AssetRequest<GameObject> assetRequestRewarUI = Content.LoadAsync(halfItemContentKey);
			yield return assetRequestRewarUI;
			LootTableRewardDefinition lootDefinition = minigameService.GetLootRewardDefinition(prizeName);
			List<MascotXPRewardDefinition> xp = (!(lootDefinition.Reward != null)) ? new List<MascotXPRewardDefinition>() : lootDefinition.Reward.GetDefinitions<MascotXPRewardDefinition>();
			DItemRewardPopup rewardPopupData = new DItemRewardPopup();
			rewardPopupData.HeaderText = "You fished up an item!";
			rewardPopupData.ItemName = Service.Get<Localizer>().GetTokenTranslation(lootDefinition.DisplayName);
			rewardPopupData.CoinReward = CoinRewardableDefinition.Coins(lootDefinition.Reward);
			rewardPopupData.XpReward = ((xp.Count > 0) ? xp[0].XP : 0);
			rewardPopupData.MascotDefinition = ((xp.Count > 0) ? xp[0].Mascot : null);
			rewardPopupData.MascotAbbreviatedName = ((xp.Count > 0) ? xp[0].Mascot.AbbreviatedName : "");
			rewardPopupData.RewardCategory = RewardCategory.genericSprite;
			rewardPopupData.RewardData = new DReward();
			rewardPopupData.RewardData.UnlockID = lootDefinition.RewardImage;
			rewardPopupData.IsQuestItem = isQuestPrize;
			rewardPopupData.RewardIcon = lootDefinition.RewardImage;
			GameObject popup = UnityEngine.Object.Instantiate(assetRequestRewarUI.Asset);
			itemRewardPopup = popup.GetComponent<ItemRewardPopupHalf>();
			if (itemRewardPopup != null)
			{
				itemRewardPopup.SetData(rewardPopupData);
				itemRewardPopup.DoneClose += onPopupClosed;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup));
		}

		private void onPopupClosed()
		{
			itemRewardPopup.DoneClose -= onPopupClosed;
			if (this.PopupDismissed != null)
			{
				this.PopupDismissed();
				this.PopupDismissed = null;
			}
		}
	}
}
