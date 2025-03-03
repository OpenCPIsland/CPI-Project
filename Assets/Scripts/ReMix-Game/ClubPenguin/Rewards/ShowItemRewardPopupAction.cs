using ClubPenguin.Adventure;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	[ActionCategory("GUI")]
	public class ShowItemRewardPopupAction : FsmStateAction
	{
		public FsmString ItemName = "Item Reward";

		public string i18nItemName = "";

		public FsmString HeaderText = "You got an item!";

		public string i18nHeaderText = "";

		public FsmInt CoinReward = 0;

		public FsmInt XpReward = 0;

		public MascotDefinition MascotDefinition;

		public RewardCategory RewardCategory;

		public string RewardID;

		public float OpenDelay;

		private static PrefabContentKey prefabContentKey = new PrefabContentKey("Rewards/RewardPopup/ItemRewardPopup");

		private GameObject popup;

		private ItemRewardPopup itemRewardPopup;

		public override void OnEnter()
		{
			Content.LoadAsync(onPrefabLoaded, prefabContentKey);
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			DItemRewardPopup dItemRewardPopup = new DItemRewardPopup();
			dItemRewardPopup.HeaderText = Service.Get<Localizer>().GetTokenTranslation(i18nHeaderText);
			dItemRewardPopup.ItemName = Service.Get<Localizer>().GetTokenTranslation(i18nItemName);
			dItemRewardPopup.RewardCategory = RewardCategory;
			dItemRewardPopup.CoinReward = CoinReward.Value;
			dItemRewardPopup.XpReward = XpReward.Value;
			dItemRewardPopup.MascotDefinition = MascotDefinition;
			DReward dReward = new DReward();
			if (RewardCategory == RewardCategory.equipmentInstances)
			{
				dReward.EquipmentRequest.definitionId = Convert.ToInt32(RewardID);
			}
			dReward.UnlockID = RewardID;
			dItemRewardPopup.RewardData = dReward;
			popup = UnityEngine.Object.Instantiate(prefab);
			itemRewardPopup = popup.GetComponent<ItemRewardPopup>();
			if (itemRewardPopup != null)
			{
				itemRewardPopup.SetData(dItemRewardPopup);
				itemRewardPopup.OpenDelay = OpenDelay;
				itemRewardPopup.DoneClose += onPopupClosed;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup, false, true, "Accessibility.Popup.Title.ItemReward"));
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
