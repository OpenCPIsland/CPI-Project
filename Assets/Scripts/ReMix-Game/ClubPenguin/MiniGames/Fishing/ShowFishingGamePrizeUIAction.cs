using ClubPenguin.NPC;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	[ActionCategory("Fishing")]
	public class ShowFishingGamePrizeUIAction : FsmStateAction
	{
		public FsmString PrizeName = "";

		private static PrefabContentKey prefabContentKey = new PrefabContentKey("Rewards/RewardPopup/ItemRewardPopup");

		private GameObject popup;

		private ItemRewardPopup itemRewardPopup;

		public override void OnEnter()
		{
			Content.LoadAsync(onPrefabLoaded, prefabContentKey);
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			MinigameService minigameService = Service.Get<MinigameService>();
			LootTableRewardDefinition lootRewardDefinition = minigameService.GetLootRewardDefinition(PrizeName.Value);
			List<MascotXPRewardDefinition> list = (!(lootRewardDefinition.Reward != null)) ? new List<MascotXPRewardDefinition>() : lootRewardDefinition.Reward.GetDefinitions<MascotXPRewardDefinition>();
			DItemRewardPopup dItemRewardPopup = new DItemRewardPopup();
			dItemRewardPopup.HeaderText = "You fished up an item!";
			dItemRewardPopup.ItemName = Service.Get<Localizer>().GetTokenTranslation(lootRewardDefinition.DisplayName);
			dItemRewardPopup.CoinReward = CoinRewardableDefinition.Coins(lootRewardDefinition.Reward);
			dItemRewardPopup.XpReward = ((list.Count > 0) ? list[0].XP : 0);
			dItemRewardPopup.MascotDefinition = ((list.Count > 0) ? list[0].Mascot : null);
			dItemRewardPopup.MascotAbbreviatedName = ((list.Count > 0) ? list[0].Mascot.AbbreviatedName : "");
			dItemRewardPopup.RewardCategory = RewardCategory.genericSprite;
			dItemRewardPopup.RewardData = new DReward();
			dItemRewardPopup.RewardData.UnlockID = lootRewardDefinition.RewardImage;
			popup = Object.Instantiate(prefab);
			itemRewardPopup = popup.GetComponent<ItemRewardPopup>();
			if (itemRewardPopup != null)
			{
				itemRewardPopup.SetData(dItemRewardPopup);
				itemRewardPopup.DoneClose += onPopupClosed;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup, false, true, "Accessibility.Popup.Title.FishingReward"));
		}

		public override void OnExit()
		{
			if (itemRewardPopup != null)
			{
				itemRewardPopup.DoneClose -= onPopupClosed;
			}
		}

		private void onPopupClosed()
		{
			Finish();
		}
	}
}
