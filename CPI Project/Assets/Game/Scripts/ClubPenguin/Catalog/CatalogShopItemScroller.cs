using ClubPenguin.Avatar;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.ClothingDesigner;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogShopItemScroller : MonoBehaviour
	{
		public const string PURCHASE_ERROR_PROMPT_ID = "CataogPurchaseErrorPrompt";

		public const string INSUFFICIENT_FUNDS_ERROR_PROMPT_ID = "InsufficientFundsErrorPrompt";

		public int IndexOffset = 0;

		[HideInInspector]
		public int lastNumShopRows = 0;

		[HideInInspector]
		public int numShopRows = 0;

		[HideInInspector]
		public int lastRowNumItems = 0;

		[HideInInspector]
		public bool isShopScrollInitialized = false;

		[HideInInspector]
		public int CatalogShopItemsPerRow = 3;

		public GameObject ClothingPurchase;

		public bool HideItemName = false;

		public int StandaloneCatalogShopItemsPerRow = 8;

		public VerticalScrollingLayoutElementPool Scroller;

		[HideInInspector]
		public CatalogShopBuyPanelController buyPanel;

		[HideInInspector]
		public List<CatalogItemData> items;

		[HideInInspector]
		public List<CatalogItemData> filteredItems;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private Color backgroundColor = CatalogItemIcon.ITEM_BACKGROUND_COLOR;

		private Color foregroundColor = CatalogItemIcon.PENGUIN_COLOR;

		private CustomEquipment? itemForPurchase;

		private CatalogItemData selectedItem;

		private CatalogItemData purchaseItem;

		private EventChannel catalogEventChannel;

		private int buyPanelIndex = -1;

		private void Start()
		{
			setupListeners();
			if (PlatformUtils.GetPlatformType() == PlatformType.Standalone)
			{
				CatalogShopItemsPerRow = StandaloneCatalogShopItemsPerRow;
			}
		}

		protected virtual void setupListeners()
		{
			catalogEventChannel = new EventChannel(CatalogContext.EventBus);
			catalogEventChannel.AddListener<CatalogUIEvents.ShopItemClickedEvent>(onShopItemClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelCloseButtonClickedEvent>(onBuyPanelCloseButtonClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent>(onBuyPanelPurchaseButtonClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelWearItButtonClickedEvent>(onBuyPanelWearItButtonClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelLearnMoreButtonClickedEvent>(onBuyPanelLearnMoreButtonClickedEvent);
			catalogEventChannel.AddListener<CatalogUIEvents.BuyPanelRequiredLevelButtonClickedEvent>(onBuyPanelRequiredLevelButtonClickedEvent);
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.ItemPurchaseCompleteEvent>(onPurchaseComplete);
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.ItemPurchaseErrorEvent>(onPurchaseError);
			VerticalScrollingLayoutElementPool scroller = Scroller;
			scroller.OnElementShown = (Action<int, GameObject>)Delegate.Combine(scroller.OnElementShown, new Action<int, GameObject>(onElementShown));
			VerticalScrollingLayoutElementPool scroller2 = Scroller;
			scroller2.OnElementHidden = (Action<int, GameObject>)Delegate.Combine(scroller2.OnElementHidden, new Action<int, GameObject>(onElementHidden));
		}

		public void SetScrollerPrefabs(GameObject[] prefabsToInstance)
		{
			Scroller.SetPrefabsToInstance(prefabsToInstance);
		}

		protected virtual void OnDestroy()
		{
			catalogEventChannel.RemoveAllListeners();
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.ItemPurchaseCompleteEvent>(onPurchaseComplete);
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.ItemPurchaseErrorEvent>(onPurchaseError);
			VerticalScrollingLayoutElementPool scroller = Scroller;
			scroller.OnElementShown = (Action<int, GameObject>)Delegate.Remove(scroller.OnElementShown, new Action<int, GameObject>(onElementShown));
			VerticalScrollingLayoutElementPool scroller2 = Scroller;
			scroller2.OnElementHidden = (Action<int, GameObject>)Delegate.Remove(scroller2.OnElementHidden, new Action<int, GameObject>(onElementHidden));
			CoroutineRunner.StopAllForOwner(this);
		}

		public void ClearData()
		{
			ClearScroller();
			lastNumShopRows = 0;
			numShopRows = 0;
			lastRowNumItems = 0;
			items = null;
			filteredItems = null;
		}

		private bool onBuyPanelCloseButtonClickedEvent(CatalogUIEvents.BuyPanelCloseButtonClickedEvent evt)
		{
			CloseBuyPanel();
			return false;
		}

		private bool onBuyPanelPurchaseButtonClickedEvent(CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent evt)
		{
			int coins = Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
			long cost = evt.ItemData.cost;
			if (coins < cost)
			{
				Service.Get<PromptManager>().ShowPrompt("InsufficientFundsErrorPrompt", null);
			}
			else
			{
				purchaseItem = evt.ItemData;
				itemForPurchase = evt.ItemData.equipment;
				Service.Get<INetworkServicesManager>().CatalogService.PurchaseCatalogItem(evt.ItemData.clothingCatalogItemId);
			}
			return false;
		}

		private bool onBuyPanelWearItButtonClickedEvent(CatalogUIEvents.BuyPanelWearItButtonClickedEvent evt)
		{
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.HideCatalog));
			return false;
		}

		private bool onBuyPanelLearnMoreButtonClickedEvent(CatalogUIEvents.BuyPanelLearnMoreButtonClickedEvent evt)
		{
			Service.Get<GameStateController>().ShowAccountSystemMembership("Catalog");
			return false;
		}

		private bool onBuyPanelRequiredLevelButtonClickedEvent(CatalogUIEvents.BuyPanelRequiredLevelButtonClickedEvent evt)
		{
			return false;
		}

		public virtual void ClearScroller()
		{
			if (isShopScrollInitialized)
			{
				try
				{
					for (int i = 0; i < lastNumShopRows; i++)
					{
						Scroller.RemoveElement(0);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public virtual void GenerateScrollData(List<CatalogItemData> scrollItems)
		{
			if (scrollItems != null && scrollItems.Count > 0)
			{
				numShopRows = Mathf.CeilToInt((float)scrollItems.Count / (float)CatalogShopItemsPerRow);
				lastRowNumItems = scrollItems.Count % CatalogShopItemsPerRow;
				if (lastRowNumItems == 0)
				{
					lastRowNumItems = CatalogShopItemsPerRow;
				}
			}
			else
			{
				lastNumShopRows = 0;
			}
		}

		public virtual void CloseBuyPanel()
		{
			buyPanelIndex = -1;
			if (!(buyPanel != null))
			{
				return;
			}
			GameObject gameObject = null;
			try
			{
				gameObject = Scroller.GetElementAtIndex(buyPanel.ElementIndex + IndexOffset);
			}
			catch (Exception)
			{
			}
			if (gameObject != null)
			{
				LayoutElement component = gameObject.transform.parent.gameObject.GetComponent<LayoutElement>();
				if (component != null)
				{
					component.preferredHeight = gameObject.GetComponent<LayoutElement>().preferredHeight;
				}
			}
			buyPanel.ClosePanel();
			buyPanel = null;
		}

		public void SetItemTintColors(Color foreground, Color background)
		{
			foregroundColor = foreground;
			backgroundColor = background;
		}

		public virtual void ShowShopItemPanel(int scrollIndex, int rowIndex, bool isMemberUnlocked, bool isRecentlyPurchased)
		{
			int index = scrollIndex * CatalogShopItemsPerRow + (rowIndex - 1);
			CatalogItemData itemData = filteredItems[index];
			int arrowIndex = rowIndex - 1;
			try
			{
				GameObject elementAtIndex = Scroller.GetElementAtIndex(scrollIndex + IndexOffset);
				if (elementAtIndex != null)
				{
					CatalogShopRowItem component = elementAtIndex.GetComponent<CatalogShopRowItem>();
					if (component != null)
					{
						int count = component.GetShopItems().Count;
						if (count == 1)
						{
							arrowIndex = 1;
						}
						if (count == 2)
						{
							arrowIndex = CatalogShopItemsPerRow + rowIndex - 1;
						}
						CatalogShopItem catalogShopItem = component.GetShopItems()[rowIndex - 1];
						catalogShopItem.AddListeners();
					}
				}
			}
			catch (Exception)
			{
			}
			selectedItem = itemData;
			bool flag = false;
			if (buyPanel != null)
			{
				if (buyPanel.ElementIndex == scrollIndex)
				{
					buyPanel.SetPanel(itemData, scrollIndex, arrowIndex, isMemberUnlocked, isRecentlyPurchased);
					flag = true;
				}
				else
				{
					CloseBuyPanel();
				}
			}
			if (flag)
			{
				return;
			}
			GameObject elementAtIndex2 = Scroller.GetElementAtIndex(scrollIndex + IndexOffset);
			if (elementAtIndex2 != null)
			{
				LayoutElement component2 = elementAtIndex2.transform.parent.gameObject.GetComponent<LayoutElement>();
				if (component2 != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(ClothingPurchase);
					DefaultValuesPooledLayoutElement component3 = gameObject.GetComponent<DefaultValuesPooledLayoutElement>();
					component2.preferredHeight = component3.DefaultHeight + elementAtIndex2.GetComponent<LayoutElement>().preferredHeight;
					gameObject.transform.SetParent(elementAtIndex2.transform, false);
					buyPanel = gameObject.GetComponent<CatalogShopBuyPanelController>();
					buyPanelIndex = scrollIndex;
					buyPanel.SetPanel(itemData, scrollIndex, arrowIndex, isMemberUnlocked, isRecentlyPurchased);
					CoroutineRunner.Start(ScrollToElement(scrollIndex + IndexOffset), this, "ScrollToElement");
				}
			}
		}

		private IEnumerator ScrollToElement(int scrollItemIndex)
		{
			yield return new WaitForSeconds(0.1f);
			ScrollElementToTop(scrollItemIndex);
		}

		private bool onShopItemClickedEvent(CatalogUIEvents.ShopItemClickedEvent evt)
		{
			ShowShopItemPanel(evt.ScrollIndex, evt.RowIndex, evt.IsMemberUnlocked, evt.IsAlreadyOwned);
			return false;
		}

		private bool onPurchaseComplete(CatalogServiceEvents.ItemPurchaseCompleteEvent e)
		{
			int coins = Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
			if (e.Response.newCoinTotal < coins)
			{
				coins -= (int)e.Response.newCoinTotal;
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).RemoveCoins(coins);
			}
			if (selectedItem.clothingCatalogItemId == purchaseItem.clothingCatalogItemId && buyPanel != null)
			{
				buyPanel.SetState(CatalogShopBuyPanelState.Success);
			}
			if (itemForPurchase.HasValue)
			{
				DCustomEquipment data = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(itemForPurchase.Value);
				data.Id = e.Response.equipmentId;
				data.DateTimeCreated = DateTime.UtcNow.GetTimeInMilliseconds();
				DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
				InventoryData component = Service.Get<CPDataEntityCollection>().GetComponent<InventoryData>(localPlayerHandle);
				if (component != null)
				{
					InventoryIconModel<DCustomEquipment> value = new InventoryIconModel<DCustomEquipment>(data.Id, data, false, true);
					component.Inventory.Add(data.Id, value);
					Service.Get<NotificationBreadcrumbController>().AddPersistentBreadcrumb(BreadcrumbType, data.Id.ToString());
				}
				else
				{
					Log.LogError(this, "Unable to locate InventoryData object.");
				}
			}
			Service.Get<CatalogServiceProxy>().cache.ClearCache();
			return false;
		}

		private bool onPurchaseError(CatalogServiceEvents.ItemPurchaseErrorEvent e)
		{
			Service.Get<PromptManager>().ShowPrompt("CataogPurchaseErrorPrompt", null);
			CloseBuyPanel();
			return false;
		}

		private void ScrollElementToTop(int elementIndex)
		{
			float num = Scroller.ElementLayoutGroup.padding.top;
			for (int i = 0; i < Scroller.ElementLayoutGroup.transform.childCount && i < elementIndex; i++)
			{
				RectTransform rectTransform = Scroller.ElementLayoutGroup.transform.GetChild(i) as RectTransform;
				num += rectTransform.rect.height;
			}
			Scroller.ElementScrollRect.content.anchoredPosition = new Vector2(Scroller.ElementScrollRect.content.anchoredPosition.x, num);
		}

		protected virtual void onElementShown(int index, GameObject element)
		{
			if (!(element != null))
			{
				return;
			}
			if (buyPanel == null)
			{
				LayoutElement component = element.transform.parent.gameObject.GetComponent<LayoutElement>();
				if (component != null)
				{
					component.preferredHeight = element.GetComponent<LayoutElement>().preferredHeight;
				}
			}
			CatalogShopRowItem component2 = element.GetComponent<CatalogShopRowItem>();
			if (component2 != null)
			{
				component2.ClearShopItems();
				int catalogShopItemsPerRow = CatalogShopItemsPerRow;
				if (index + 1 - IndexOffset == numShopRows)
				{
					catalogShopItemsPerRow = lastRowNumItems;
				}
				int num = 1;
				while (num <= catalogShopItemsPerRow)
				{
					int index2 = (index - IndexOffset) * CatalogShopItemsPerRow + (num - 1);
					CatalogShopItem catalogShopItem = component2.AddShopItem(index - IndexOffset, num);
					num++;
					catalogShopItem.SetUpButton(filteredItems[index2], foregroundColor, backgroundColor, HideItemName);
				}
			}
		}

		protected virtual void onElementHidden(int index, GameObject element)
		{
			if (buyPanel != null && index == buyPanelIndex)
			{
				buyPanelIndex = -1;
				buyPanel.ClosePanel();
			}
		}
	}
}
