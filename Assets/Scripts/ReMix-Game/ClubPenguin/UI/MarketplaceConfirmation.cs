using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceConfirmation : MonoBehaviour
	{
		private enum ConfirmationState
		{
			Pending,
			Purchasing,
			Complete,
			AnimatingItem
		}

		private const int MIN_PURCHASE_COUNT = 1;

		private const int MAX_PURCHASE_COUNT = 99;

		private const string COINS_TOKEN = "GlobalUI.Notification.Coins";

		private const string PURCHASE_ERROR_PROMPT_ID = "DisneyStorePurchaseErrorPrompt";

		private const string SALE_TEXT_TOKEN = "GoGuide.ShopSale.Discount";

		public RawImage IconImage;

		public Text TitleText;

		public Text DescriptionText;

		public Text CostText;

		public Text CountText;

		public GameObject PurchasePanel;

		public GameObject SuccessPanel;

		public GameObject CoinsPanel;

		public GameObject BuyButton;

		public GameObject DisabledButton;

		public GameObject SalePanel;

		public Text SaleText;

		protected PropDefinition prop;

		private MarketplaceScreenController marketplaceController;

		private int purchaseCount;

		private int singleItemCost;

		private ConfirmationState currentState;

		private StoreItemConfirmationPlacement confirmationPlacement;

		private void Awake()
		{
			SuccessPanel.SetActive(false);
			PurchasePanel.SetActive(true);
			confirmationPlacement = GetComponent<StoreItemConfirmationPlacement>();
		}

		public virtual void SetItem(PropDefinition propDefinition, Texture icon, MarketplaceScreenController marketplaceController, RectTransform itemTransform, RectTransform scrollRectTransform)
		{
			this.marketplaceController = marketplaceController;
			confirmationPlacement.PositionConfirmation(itemTransform, scrollRectTransform);
			IconImage.texture = icon;
			prop = propDefinition;
			TitleText.text = Service.Get<Localizer>().GetTokenTranslation(propDefinition.Name);
			DescriptionText.text = Service.Get<Localizer>().GetTokenTranslation(propDefinition.Description);
			singleItemCost = getItemCost(propDefinition);
			setState(ConfirmationState.Pending);
			setPurchaseCount(1);
		}

		public void OnKeepShoppingClicked()
		{
			if (currentState != ConfirmationState.AnimatingItem)
			{
				setState(ConfirmationState.AnimatingItem);
				marketplaceController.TrayAnimator.TweenToTray(marketplaceController.TrayAnimator.ToyboxDestination, IconImage.transform);
				marketplaceController.KeepShoppingButtonPressed();
			}
		}

		public void OnUseItClicked()
		{
			if (currentState != ConfirmationState.AnimatingItem)
			{
				setState(ConfirmationState.AnimatingItem);
				Service.Get<PropService>().LocalPlayerRetrieveProp(prop.GetNameOnServer());
				marketplaceController.UseItemButtonPressed();
			}
		}

		public void OnCloseClicked()
		{
			marketplaceController.HideConfirmation();
		}

		public void OnBuyClicked()
		{
			if (currentState == ConfirmationState.Pending)
			{
				if (getCoinAmount() < singleItemCost * purchaseCount)
				{
					CoroutineRunner.Start(showCoinTooltip(), this, "");
				}
				else
				{
					purchaseItem();
				}
			}
		}

		public void OnCountIncreaseClick()
		{
			if (currentState == ConfirmationState.Pending)
			{
				setPurchaseCount(purchaseCount + 1);
			}
		}

		public void OnCountDecreaseClick()
		{
			if (currentState == ConfirmationState.Pending)
			{
				setPurchaseCount(purchaseCount - 1);
			}
		}

		private void setPurchaseCount(int newPurchaseCount)
		{
			purchaseCount = Mathf.Clamp(newPurchaseCount, 1, 99);
			CountText.text = purchaseCount.ToString();
			CostText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Notification.Coins"), singleItemCost * purchaseCount);
			showCoinsStatus();
		}

		private int getCoinAmount()
		{
			return Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
		}

		private void purchaseItem()
		{
			Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).OnConsumableInventoryChanged += onInventoryChanged;
			Service.Get<EventDispatcher>().AddListener<ConsumableServiceErrors.NotEnoughCoins>(onNotEnoughCoinsPurchaseError);
			Service.Get<EventDispatcher>().AddListener<ConsumableServiceErrors.Unknown>(onUnkownPurchaseError);
			Service.Get<INetworkServicesManager>().ConsumableService.PurchaseConsumable(prop.GetNameOnServer(), purchaseCount);
			showPurchaseModal();
			logPurchaseBI();
			setState(ConfirmationState.Purchasing);
		}

		private bool onNotEnoughCoinsPurchaseError(ConsumableServiceErrors.NotEnoughCoins evt)
		{
			removePurchaseListeners();
			hidePurchaseModal();
			Service.Get<PromptManager>().ShowPrompt("DisneyStorePurchaseErrorPrompt", null);
			setState(ConfirmationState.Pending);
			return false;
		}

		private bool onUnkownPurchaseError(ConsumableServiceErrors.Unknown evt)
		{
			removePurchaseListeners();
			hidePurchaseModal();
			Service.Get<PromptManager>().ShowPrompt("DisneyStorePurchaseErrorPrompt", null);
			setState(ConfirmationState.Pending);
			return false;
		}

		private void onInventoryChanged(ConsumableInventory inventory)
		{
			if (prop.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby)
			{
				marketplaceController.SetPurchasedPartyGame();
			}
			removePurchaseListeners();
			hidePurchaseModal();
			Service.Get<EventDispatcher>().DispatchEvent(new MarketplaceEvents.ItemPurchased(prop, purchaseCount));
			setState(ConfirmationState.Complete);
		}

		private void removePurchaseListeners()
		{
			Service.Get<EventDispatcher>().RemoveListener<ConsumableServiceErrors.NotEnoughCoins>(onNotEnoughCoinsPurchaseError);
			Service.Get<EventDispatcher>().RemoveListener<ConsumableServiceErrors.Unknown>(onUnkownPurchaseError);
			ConsumableInventoryData component = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			if (component != null)
			{
				component.OnConsumableInventoryChanged -= onInventoryChanged;
			}
		}

		private void showPurchaseModal()
		{
			marketplaceController.ShowLoadingModal();
		}

		private void hidePurchaseModal()
		{
			marketplaceController.HideLoadingModal();
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
			removePurchaseListeners();
		}

		private void logPurchaseBI()
		{
			Service.Get<ICPSwrveService>().PurchaseConsumable(prop.name, singleItemCost, purchaseCount, Service.Get<ProgressionService>().Level, null);
		}

		private int getItemCost(PropDefinition definition)
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

		private bool isItemInSale(CellPhoneSaleActivityDefinition sale, PropDefinition item)
		{
			bool result = false;
			for (int i = 0; i < sale.Consumables.Length; i++)
			{
				if (sale.Consumables[i].Id == item.Id)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
