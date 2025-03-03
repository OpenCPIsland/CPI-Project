using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.CellPhone;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using ClubPenguin.DisneyStore;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using ClubPenguin.Rewards;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DisneyStorePurchaseConfirmation : AbstractDisneyStoreConfirmation
	{
		private enum ConfirmationState
		{
			Pending,
			Complete,
			Animating
		}

		private const int MIN_PURCHASE_COUNT = 1;

		private const int MAX_PURCHASE_COUNT = 99;

		private const string COINS_TOKEN = "GlobalUI.Notification.Coins";

		private const string PURCHASE_ERROR_PROMPT_ID = "DisneyStorePurchaseErrorPrompt";

		private const string SALE_TEXT_TOKEN = "GoGuide.ShopSale.Discount";

		public Image IconImage;

		public Text TitleText;

		public Text DescriptionText;

		public Text CostText;

		public Text CountText;

		public GameObject PurchasePanel;

		public GameObject SuccessPanel;

		public GameObject EquipPanel;

		public GameObject CoinsPanel;

		public GameObject BuyButton;

		public GameObject DisabledButton;

		public GameObject SalePanel;

		public Text SaleText;

		public StaticBreadcrumbDefinitionKey Breadcrumb;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private DisneyStoreFranchise storeFranchise;

		private DisneyStoreItemData item;

		private IDisneyStoreController storeController;

		private DisneyStoreFranchiseItem shopItem;

		private int purchaseCount;

		private int singleItemCost;

		private ConfirmationState currentState;

		private void Awake()
		{
			SuccessPanel.SetActive(false);
			PurchasePanel.SetActive(true);
		}

		public override void SetItem(DisneyStoreItemData item, Sprite icon, DisneyStoreFranchise storeFranchise, IDisneyStoreController storeController, DisneyStoreFranchiseItem shopItem, RectTransform scrollRectTransform)
		{
			IconImage.sprite = icon;
			this.storeFranchise = storeFranchise;
			this.storeController = storeController;
			this.item = item;
			this.shopItem = shopItem;
			TitleText.text = Service.Get<Localizer>().GetTokenTranslation(item.Definition.TitleToken);
			DescriptionText.text = Service.Get<Localizer>().GetTokenTranslation(item.Definition.DescriptionToken);
			setState(ConfirmationState.Pending);
			setPurchaseCount(1);
			EquipPanel.SetActive(isItemEquippable(item));
			selectTrayIcons(item);
			setPurchaseActionText(item);
			GetComponent<StoreItemConfirmationPlacement>().PositionConfirmation((RectTransform)shopItem.transform, scrollRectTransform);
			logItemViewed(item);
			singleItemCost = getItemCost(item.Definition);
			CostText.text = singleItemCost.ToString();
		}

		public void OnKeepShoppingClicked()
		{
			switch (currentState)
			{
			case ConfirmationState.Complete:
				CoroutineRunner.Start(tweenIconToTray(), this, "");
				break;
			case ConfirmationState.Pending:
				storeFranchise.HideConfirmation();
				break;
			}
		}

		public void OnUseItClicked()
		{
			if (currentState == ConfirmationState.Complete)
			{
				List<DReward> rewards = item.GetRewards();
				RewardCategory category = rewards[0].Category;
				if (category == RewardCategory.durables || category == RewardCategory.consumables)
				{
					equipProp(rewards[0]);
				}
				else
				{
					jumpToClothingDesigner(rewards[0]);
				}
				logUseItem();
			}
		}

		private bool isItemEquippable(DisneyStoreItemData itemData)
		{
			List<DReward> rewards = itemData.GetRewards();
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].Category == RewardCategory.equipmentInstances || rewards[i].Category == RewardCategory.durables || rewards[i].Category == RewardCategory.equipmentTemplates || rewards[i].Category == RewardCategory.fabrics || rewards[i].Category == RewardCategory.decals || rewards[i].Category == RewardCategory.colourPacks || rewards[i].Category == RewardCategory.consumables)
				{
					return true;
				}
			}
			return false;
		}

		private IEnumerator tweenIconToTray()
		{
			currentState = ConfirmationState.Animating;
			DisneyStoreTrayAnimator animator = storeController.GetTrayAnimator();
			RewardCategory category = DisneyStoreUtils.GetItemRewardCategory(item);
			Transform tweenDestination = animator.MyPenguinDestination;
			if (category == RewardCategory.consumables)
			{
				tweenDestination = animator.ToyboxDestination;
			}
			animator.TweenToTray(tweenDestination, IconImage.transform);
			yield return new WaitForSeconds(animator.TweenTime);
			storeFranchise.HideConfirmation();
			yield return null;
		}

		private void equipProp(DReward propReward)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			PropDefinition value;
			if (dictionary.TryGetValue((int)propReward.UnlockID, out value))
			{
				Service.Get<PropService>().LocalPlayerRetrieveProp(value.GetNameOnServer());
			}
			if (purchaseCount == 1)
			{
				Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(BreadcrumbType, value.Id.ToString());
				Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(Breadcrumb);
			}
			storeController.OnCloseClicked();
		}

		private void jumpToClothingDesigner(DReward equipmentReward)
		{
			Service.Get<SceneTransitionService>().LoadScene("ClothingDesigner", "Loading");
		}

		public void OnBuyClicked()
		{
			if (!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && item.Definition.IsMemberOnly)
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership("DisneyShop");
			}
			else if (getCoinAmount() < singleItemCost * purchaseCount)
			{
				CoroutineRunner.Start(showCoinTooltip(), this, "");
				logInsufficientCoins();
			}
			else
			{
				logItemPurchased();
				purchaseItem();
			}
		}

		public void OnCountIncreaseClick()
		{
			setPurchaseCount(purchaseCount + 1);
		}

		public void OnCountDecreaseClick()
		{
			setPurchaseCount(purchaseCount - 1);
		}

		private void setPurchaseCount(int newPurchaseCount)
		{
			purchaseCount = Mathf.Clamp(newPurchaseCount, 1, 99);
			if (CountText != null)
			{
				CountText.text = purchaseCount.ToString();
			}
			CostText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Notification.Coins"), singleItemCost * purchaseCount);
			showCoinsStatus();
		}

		private int getCoinAmount()
		{
			return Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
		}

		private void purchaseItem()
		{
			Service.Get<EventDispatcher>().AddListener<DisneyStoreServiceEvents.DisneyStorePurchaseComplete>(onPurchaseComplete);
			Service.Get<INetworkServicesManager>().DisneyStoreService.PurchaseDisneyStoreItem(item.Definition.Id, purchaseCount);
			showPurchaseModal();
		}

		private bool onPurchaseComplete(DisneyStoreServiceEvents.DisneyStorePurchaseComplete evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<DisneyStoreServiceEvents.DisneyStorePurchaseComplete>(onPurchaseComplete);
			if (evt.Result == DisneyStoreServiceEvents.DisneyStorePurchaseResult.Success)
			{
				PlayerPrefs.SetInt("DisneyStoreShowTutorial", 0);
				setState(ConfirmationState.Complete);
				if (DisneyStoreUtils.DoesItemContainEquipmentInstance(item))
				{
					GetLatestInventoryCMD getLatestInventoryCMD = new GetLatestInventoryCMD(onGetInventoryComplete);
					getLatestInventoryCMD.Execute();
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(new DisneyStoreEvents.PurchaseComplete(item.Definition.Reward.ToReward()));
					hidePurchaseModal();
				}
			}
			else if (evt.Result == DisneyStoreServiceEvents.DisneyStorePurchaseResult.Error)
			{
				Service.Get<PromptManager>().ShowPrompt("DisneyStorePurchaseErrorPrompt", null);
				hidePurchaseModal();
			}
			return false;
		}

		private void onGetInventoryComplete()
		{
			Reward reward = item.Definition.Reward.ToReward();
			EquipmentInstanceReward rewardable;
			if (reward.TryGetValue(out rewardable))
			{
				CustomEquipment[] array = rewardable.EquipmentInstances.ToArray();
				reward.ClearReward(typeof(EquipmentInstanceReward));
				for (int i = 0; i < array.Length; i++)
				{
					DCustomEquipment equipmentData;
					if (InventoryUtils.TryGetDCustomEquipment(array[i], out equipmentData))
					{
						array[i].equipmentId = equipmentData.Id;
					}
					reward.Add(new EquipmentInstanceReward(array[i]));
				}
			}
			Service.Get<EventDispatcher>().DispatchEvent(new DisneyStoreEvents.PurchaseComplete(reward));
			hidePurchaseModal();
		}

		private void showPurchaseModal()
		{
			storeController.ShowLoadingModal();
		}

		private void hidePurchaseModal()
		{
			storeController.HideLoadingModal();
			shopItem.ShowItemStatus();
		}

		private void showCoinsStatus()
		{
			if (getCoinAmount() < singleItemCost * purchaseCount)
			{
				BuyButton.SetActive(false);
				DisabledButton.SetActive(true);
			}
			else
			{
				BuyButton.SetActive(true);
				DisabledButton.SetActive(false);
			}
		}

		private void setState(ConfirmationState newState)
		{
			switch (newState)
			{
			case ConfirmationState.Pending:
				PurchasePanel.SetActive(true);
				SuccessPanel.SetActive(false);
				break;
			case ConfirmationState.Complete:
				PurchasePanel.SetActive(false);
				SuccessPanel.SetActive(true);
				break;
			}
			currentState = newState;
		}

		private IEnumerator showCoinTooltip()
		{
			CoinsPanel.SetActive(true);
			CoinsPanel.GetComponent<Animator>().SetBool("IsOpen", true);
			yield return new WaitForSeconds(2f);
			CoinsPanel.GetComponent<Animator>().SetBool("IsOpen", false);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private void logItemPurchased()
		{
			Service.Get<ICPSwrveService>().Action("game.disney_store_item_purchase", item.Definition.name, item.GetRewards()[0].Category.ToString(), purchaseCount.ToString());
			Service.Get<ICPSwrveService>().PurchaseGeneral("disney_store", item.Definition.name, purchaseCount * singleItemCost, 1, "durable", null, item.GetRewards()[0].Category.ToString());
		}

		private void logInsufficientCoins()
		{
			Service.Get<ICPSwrveService>().Action("game.disney_store_item_purchase_need_more_coins", item.Definition.name);
		}

		private void logUseItem()
		{
			Service.Get<ICPSwrveService>().Action("game.disney_store_use_now", item.Definition.name, item.GetRewards()[0].Category.ToString());
		}

		private int getItemCost(DisneyStoreItemDefinition definition)
		{
			int itemCost = MarketPlaceUtils.GetItemCost(definition, definition.Cost, isItemInSale);
			if (itemCost != definition.Cost)
			{
				SalePanel.SetActive(true);
				SaleText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GoGuide.ShopSale.Discount"), MarketPlaceUtils.GetItemDiscountPercentage(definition, isItemInSale));
			}
			else
			{
				SalePanel.SetActive(false);
			}
			return itemCost;
		}

		private bool isItemInSale(CellPhoneSaleActivityDefinition sale, DisneyStoreItemDefinition item)
		{
			bool result = false;
			for (int i = 0; i < sale.DisneyStoreItems.Length; i++)
			{
				if (sale.DisneyStoreItems[i].Id == item.Id)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
