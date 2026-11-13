using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogShopItem : ACatalogController
	{
		public const int ITEM_NOT_OWNED = 0;

		public Button ButtonComponent;

		public GameObject PriceTag;

		public Text PriceTagText;

		public GameObject NameTag;

		public GameObject MemberLock;

		public GameObject ItemOwned;

		public Text CreatorNameText;

		private int scrollIndex = 0;

		private int rowIndex = 0;

		private bool isMemberUnlocked;

		private bool isAlreadyOwned = false;

		private CatalogItemData itemData;

		private EventChannel catalogChannel;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (catalogChannel != null)
			{
				catalogChannel.RemoveAllListeners();
			}
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.ItemPurchaseCompleteEvent>(onPurchaseComplete);
		}

		public void SetUpButton(CatalogItemData catalogItemData, Color foreground, Color background, bool isCreatorNameHidden = false)
		{
			itemData = catalogItemData;
			DCustomEquipment equipment = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(itemData.equipment);
			if (isCreatorNameHidden)
			{
				DisableName();
			}
			CreatorNameText.text = itemData.creatorName;
			CatalogItemIcon component = GetComponent<CatalogItemIcon>();
			AbstractImageBuilder.CallbackToken callbackToken = default(AbstractImageBuilder.CallbackToken);
			callbackToken.Id = itemData.clothingCatalogItemId;
			callbackToken.DefinitionId = equipment.DefinitionId;
			base.itemImageBuilder.RequestImage(equipment, component.SetIcon, callbackToken, background, foreground);
			if (!isAlreadyOwned)
			{
				isAlreadyOwned = ((itemData.equipment.equipmentId != 0) ? true : false);
			}
			ItemOwned.SetActive(isAlreadyOwned);
			PriceTagText.text = itemData.cost.ToString();
			PriceTag.SetActive(!isAlreadyOwned);
			int definitionId = equipment.DefinitionId;
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			TemplateDefinition templateDefinition = dictionary.Values.ToList().First((TemplateDefinition x) => x.Id == definitionId);
			isMemberUnlocked = (!templateDefinition.IsMemberOnly || Service.Get<CPDataEntityCollection>().IsLocalPlayerMember());
			if (!isMemberUnlocked)
			{
				MemberLock.SetActive(true);
			}
		}

		public void SetScrollIndex(int scrollElementIndex)
		{
			scrollIndex = scrollElementIndex;
		}

		public void SetRowIndex(int rowIndex)
		{
			this.rowIndex = rowIndex;
		}

		public void DisableButton()
		{
			ButtonComponent.enabled = false;
		}

		public void DisablePrice()
		{
			PriceTag.SetActive(false);
		}

		public void DisableName()
		{
			NameTag.SetActive(false);
		}

		public void OnCLick()
		{
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.ShopItemClickedEvent(scrollIndex, rowIndex, isMemberUnlocked, isAlreadyOwned, itemData));
		}

		public void AddListeners()
		{
			if (!isAlreadyOwned)
			{
				if (catalogChannel == null)
				{
					catalogChannel = new EventChannel(CatalogContext.EventBus);
				}
				catalogChannel.AddListener<CatalogUIEvents.ShopItemClickedEvent>(onOtherShopItemClickedEvent);
				catalogChannel.AddListener<CatalogUIEvents.BuyPanelCloseButtonClickedEvent>(onBackButtonClicked);
				catalogChannel.AddListener<CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent>(onPurchaseClicked);
			}
		}

		private void removeListeners()
		{
			if (catalogChannel != null)
			{
				catalogChannel.RemoveAllListeners();
			}
		}

		private bool onBackButtonClicked(CatalogUIEvents.BuyPanelCloseButtonClickedEvent evt)
		{
			removeListeners();
			return false;
		}

		private bool onPurchaseClicked(CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent evt)
		{
			Service.Get<EventDispatcher>().AddListener<CatalogServiceEvents.ItemPurchaseCompleteEvent>(onPurchaseComplete);
			return false;
		}

		private bool onPurchaseComplete(CatalogServiceEvents.ItemPurchaseCompleteEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<CatalogServiceEvents.ItemPurchaseCompleteEvent>(onPurchaseComplete);
			if (ItemOwned != null && PriceTag != null)
			{
				isAlreadyOwned = true;
				ItemOwned.SetActive(isAlreadyOwned);
				PriceTag.SetActive(!isAlreadyOwned);
			}
			removeListeners();
			return false;
		}

		private bool onOtherShopItemClickedEvent(CatalogUIEvents.ShopItemClickedEvent evt)
		{
			removeListeners();
			return false;
		}
	}
}
