using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.Catalog
{
	public class IglooCatalogPurchaseConfirmation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler, IIglooCatalogPurchaseErrorHandler
	{
		private enum ConfirmationState
		{
			Pending,
			Complete,
			Animating,
			Failed
		}

		private const int MIN_PURCHASE_COUNT = 1;

		private const int MAX_PURCHASE_COUNT = 99;

		private const string COINS_TOKEN = "GlobalUI.Notification.Coins";

		private ConfirmationState currentState;

		public StoreItemConfirmationPlacement ConfirmationPlacement;

		[SerializeField]
		private Image iconImage;

		[SerializeField]
		private Text titleText;

		[SerializeField]
		private Text descriptionText;

		[SerializeField]
		[Header("Purchase Confirmation Panel")]
		private GameObject purchasePanel;

		[SerializeField]
		private GameObject buyButton;

		[SerializeField]
		private GameObject disabledBuyButton;

		[SerializeField]
		private GameObject needMoreCoinsTooltip;

		[SerializeField]
		private Text costText;

		[SerializeField]
		private Text countText;

		[SerializeField]
		[Header("Purchase Complete Panel")]
		private GameObject successPanel;

		[SerializeField]
		private GameObject usePurchasedItemButton;

		[SerializeField]
		private Text usePurchasedItemText;

		[SerializeField]
		[Header("Breadcrumbs")]
		private PersistentBreadcrumbTypeDefinitionKey decorationTypeBreadcrumb;

		[SerializeField]
		private StaticBreadcrumbDefinitionKey decorationBreadcrumb;

		[SerializeField]
		private PersistentBreadcrumbTypeDefinitionKey structureTypeBreadcrumb;

		[SerializeField]
		private StaticBreadcrumbDefinitionKey structureBreadcrumb;

		private IglooCatalogController catalog;

		private IglooCatalogItem catalogItem;

		private IglooCatalogItemData item;

		private NotificationBreadcrumbController notificationBreadcrumbController;

		private int purchaseCount;

		public void SetItem(IglooCatalogItemData item, Sprite icon, IglooCatalogController catalog, IglooCatalogItem catalogItem, RectTransform scrollRectTransform)
		{
			this.catalog = catalog;
			this.item = item;
			this.catalogItem = catalogItem;
			titleText.text = Service.Get<Localizer>().GetTokenTranslation(item.TitleToken);
			descriptionText.text = Service.Get<Localizer>().GetTokenTranslation(item.DescriptionToken);
			StartCoroutine(waitForItemIcon());
			setState(ConfirmationState.Pending);
			setPurchaseCount(1);
			ConfirmationPlacement.PositionConfirmation((RectTransform)catalogItem.transform, scrollRectTransform);
			logItemViewed(item);
			if (notificationBreadcrumbController == null)
			{
				notificationBreadcrumbController = Service.Get<NotificationBreadcrumbController>();
			}
			Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.DecorationPurchaseFailed>(onDecorationPurchaseFailed);
		}

		private IEnumerator waitForItemIcon()
		{
			if (iconImage != null)
			{
				while (!item.IconSpriteLoaded)
				{
					yield return null;
				}
				iconImage.sprite = item.IconSprite;
				iconImage.enabled = true;
			}
		}

		public void OnKeepShoppingClicked()
		{
			Object.Destroy(base.gameObject);
		}

		public void OnUseItClicked()
		{
			if (currentState == ConfirmationState.Complete)
			{
				RecentDecorationsService recentDecorationsService = Service.Get<RecentDecorationsService>();
				recentDecorationsService.ShouldShowMostRecentPurchase = true;
				recentDecorationsService.SetRecentPurchaseData(item.ItemType, item.ID);
				if (Service.Get<ZoneTransitionService>().IsInIgloo)
				{
					catalog.OnCloseClicked();
					return;
				}
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(IglooCatalogPurchaseConfirmation), null, sendPlayerToIgloo);
				Object.Destroy(base.gameObject);
			}
		}

		public void OnBuyClicked()
		{
			if ((Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() || !item.IsMemberOnly) && currentState == ConfirmationState.Pending)
			{
				if (getCoinAmount() < item.Cost * purchaseCount)
				{
					CoroutineRunner.Start(showCoinTooltip(), this, "");
					logInsufficientCoins();
				}
				else
				{
					purchaseItem();
					logItemPurchased();
				}
			}
		}

		private void sendPlayerToIgloo()
		{
			SceneStateData.SceneState sceneState = SceneStateData.SceneState.Edit;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			ProfileData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				Service.Get<ZoneTransitionService>().LoadIgloo(component.ZoneId, Service.Get<Localizer>().Language, sceneState);
			}
			else
			{
				Log.LogError(this, "Unable to find profileData to load into local players igloo.");
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
			if (countText != null)
			{
				countText.text = purchaseCount.ToString();
			}
			costText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Notification.Coins"), item.Cost * purchaseCount);
			showCoinsStatus();
		}

		private int getCoinAmount()
		{
			return Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
		}

		private void purchaseItem()
		{
			Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.DecorationUpdated>(onPurchaseComplete);
			Service.Get<INetworkServicesManager>().IglooService.PurchaseDecoration(item.ID, item.ItemType, purchaseCount, this);
			showPurchaseModal();
		}

		private bool onDecorationPurchaseFailed(IglooServiceEvents.DecorationPurchaseFailed evt)
		{
			setState(ConfirmationState.Failed);
			Service.Get<PromptManager>().ShowPrompt("DisneyStorePurchaseErrorPrompt", onDecorationFailedPromptDismissed);
			return false;
		}

		private void onDecorationFailedPromptDismissed(DPrompt.ButtonFlags buttonFlags)
		{
			setState(ConfirmationState.Pending);
			hidePurchaseModal();
		}

		private bool onPurchaseComplete(IglooServiceEvents.DecorationUpdated evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<IglooServiceEvents.DecorationUpdated>(onPurchaseComplete);
			setState(ConfirmationState.Complete);
			hidePurchaseModal();
			PersistentBreadcrumbTypeDefinitionKey type = null;
			StaticBreadcrumbDefinitionKey breadcrumbKey = null;
			string id = "";
			if (evt.DecorationId.type == DecorationType.Decoration)
			{
				type = decorationTypeBreadcrumb;
				id = evt.DecorationId.definitionId.ToString();
				breadcrumbKey = decorationBreadcrumb;
			}
			else if (evt.DecorationId.type == DecorationType.Structure)
			{
				type = structureTypeBreadcrumb;
				id = evt.DecorationId.definitionId.ToString();
				breadcrumbKey = structureBreadcrumb;
			}
			if (notificationBreadcrumbController.GetPersistentBreadcrumbCount(type, id) == 0)
			{
				notificationBreadcrumbController.AddPersistentBreadcrumb(type, id);
			}
			notificationBreadcrumbController.AddBreadcrumb(breadcrumbKey);
			return false;
		}

		private void showPurchaseModal()
		{
			catalog.ShowLoadingModal();
		}

		private void hidePurchaseModal()
		{
			catalog.HideLoadingModal();
			catalogItem.ShowItemStatus();
		}

		private void showCoinsStatus()
		{
			if (getCoinAmount() < item.Cost * purchaseCount)
			{
				buyButton.SetActive(false);
				disabledBuyButton.SetActive(true);
			}
			else
			{
				buyButton.SetActive(true);
				disabledBuyButton.SetActive(false);
			}
		}

		private void setState(ConfirmationState newState)
		{
			switch (newState)
			{
			case ConfirmationState.Pending:
				purchasePanel.SetActive(true);
				successPanel.SetActive(false);
				break;
			case ConfirmationState.Complete:
				purchasePanel.SetActive(false);
				successPanel.SetActive(true);
				break;
			}
			currentState = newState;
		}

		private IEnumerator showCoinTooltip()
		{
			needMoreCoinsTooltip.SetActive(true);
			needMoreCoinsTooltip.GetComponent<Animator>().SetBool("IsOpen", true);
			yield return new WaitForSeconds(2f);
			needMoreCoinsTooltip.GetComponent<Animator>().SetBool("IsOpen", false);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		protected void logItemViewed(IglooCatalogItemData item)
		{
		}

		private void logItemPurchased()
		{
			string text = item.TitleToken;
			string[] array = item.TitleToken.Split('.');
			switch (item.ItemType)
			{
			case DecorationType.Decoration:
				if (array.Length == 3)
				{
					text = array[1];
				}
				break;
			case DecorationType.Structure:
				if (array.Length == 4)
				{
					text = array[2];
				}
				break;
			}
			Service.Get<ICPSwrveService>().PurchaseIglooItem(text, item.Cost, purchaseCount);
			Service.Get<ICPSwrveService>().Action("igloo", "purchase", item.ItemType.ToString(), text);
		}

		private void logInsufficientCoins()
		{
		}

		private void logUseItem()
		{
		}

		public void OnPurchaseDecorationError()
		{
			Log.LogError(this, "OnPurchaseDecorationError");
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooServiceEvents.DecorationPurchaseFailed));
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
		}

		public void OnDrag(PointerEventData eventData)
		{
		}

		public void OnEndDrag(PointerEventData eventData)
		{
		}
	}
}
