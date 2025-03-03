using ClubPenguin.Analytics;
using ClubPenguin.Rewards;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public abstract class AbstractDisneyStoreConfirmation : MonoBehaviour
	{
		private const int IGLOO_ICON_INDEX = 3;

		private const int TRAY_ICON_DEFAULT_INDEX = 0;

		private const string PURCHASEACTION_USEIT_TOKEN = "GlobalUI.Buttons.Use";

		private const string PURCHASEACTION_WEARIT_TOKEN = "Marketplace.DisneyShop.WearButton";

		public SpriteSelector TrayIconImageSelector;

		public Text PurchasedActionText;

		protected RewardCategory[] TrayIconImageSelectorCategoryIndexes = new RewardCategory[4]
		{
			RewardCategory.equipmentInstances,
			RewardCategory.durables,
			RewardCategory.consumables,
			RewardCategory.decorationInstances
		};

		public abstract void SetItem(DisneyStoreItemData item, Sprite icon, DisneyStoreFranchise storeFranchise, IDisneyStoreController storeController, DisneyStoreFranchiseItem shopItem, RectTransform scrollRectTransform);

		protected void selectTrayIcons(DisneyStoreItemData item)
		{
			List<DReward> rewards = item.GetRewards();
			RewardCategory category = rewards[0].Category;
			int num = DisneyStoreUtils.IsIglooReward(item) ? 3 : Array.IndexOf(TrayIconImageSelectorCategoryIndexes, category);
			if (num != -1 || num >= TrayIconImageSelector.Sprites.Length)
			{
				TrayIconImageSelector.SelectSprite(num);
			}
			else
			{
				TrayIconImageSelector.SelectSprite(0);
			}
		}

		protected void logItemViewed(DisneyStoreItemData item)
		{
			Service.Get<ICPSwrveService>().Action("game.disney_store_item_view", item.Definition.name, item.GetRewards()[0].Category.ToString());
		}

		protected void setPurchaseActionText(DisneyStoreItemData item)
		{
			List<DReward> rewards = item.GetRewards();
			RewardCategory category = rewards[0].Category;
			string token = "GlobalUI.Buttons.Use";
			if (category == RewardCategory.equipmentInstances)
			{
				token = "Marketplace.DisneyShop.WearButton";
			}
			PurchasedActionText.text = Service.Get<Localizer>().GetTokenTranslation(token);
		}
	}
}
