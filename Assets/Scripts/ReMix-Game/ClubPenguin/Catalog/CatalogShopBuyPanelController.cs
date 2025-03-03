using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClubPenguin.Catalog
{
	public class CatalogShopBuyPanelController : MonoBehaviour
	{
		public GameObject[] Arrows;

		public List<CatalogBuyPanel> CatalogBuyPanels;

		public AvatarRenderTextureComponent AvatarRenderTextureComponent;

		public int ElementIndex
		{
			get;
			private set;
		}

		public int ArrowIndex
		{
			get;
			private set;
		}

		public CatalogItemData ItemData
		{
			get;
			private set;
		}

		public CatalogShopBuyPanelState State
		{
			get;
			private set;
		}

		public void AllowPurchase(bool state)
		{
			CatalogBuyPanel catalogBuyPanel = CatalogBuyPanels.Find((CatalogBuyPanel x) => x.State == CatalogShopBuyPanelState.Purchase);
			(catalogBuyPanel as CatalogBuyPanelPurchase).SetPurchaseButtonInteractableState(state);
		}

		public void SetPanel(CatalogItemData itemData, int elementIndex, int arrowIndex, bool isMemberUnlocked, bool isRecentlyPurchased)
		{
			ElementIndex = elementIndex;
			ItemData = itemData;
			CatalogShopBuyPanelState catalogShopBuyPanelState = CatalogShopBuyPanelState.Purchase;
			catalogShopBuyPanelState = ((!isMemberUnlocked) ? CatalogShopBuyPanelState.MemberLocked : ((((ItemData.equipment.equipmentId != 0) ? true : false) || isRecentlyPurchased) ? CatalogShopBuyPanelState.AlreadyOwned : CatalogShopBuyPanelState.Purchase));
			SetState(catalogShopBuyPanelState);
			SetArrowIndex(arrowIndex);
			SetEquipment();
		}

		public void SetArrowIndex(int index)
		{
			ArrowIndex = index;
			for (int i = 0; i < Arrows.Length; i++)
			{
				if (i == index)
				{
					Arrows[i].SetActive(true);
				}
				else
				{
					Arrows[i].SetActive(false);
				}
			}
		}

		protected void SetEquipment()
		{
			DCustomEquipment dCustomEquipment = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(ItemData.equipment);
			AvatarDetailsData avatarDetailsData = new AvatarDetailsData();
			DCustomEquipment[] outfit = (dCustomEquipment.Parts.Length != 0) ? new DCustomEquipment[1]
			{
				dCustomEquipment
			} : new DCustomEquipment[0];
			avatarDetailsData.Init(outfit);
			AvatarDetailsData component;
			if (!Service.Get<CPDataEntityCollection>().LocalPlayerHandle.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component))
			{
				avatarDetailsData.BodyColor = component.BodyColor;
			}
			AvatarRenderTextureComponent.RenderAvatar(avatarDetailsData);
		}

		public void SetState(CatalogShopBuyPanelState state)
		{
			State = state;
			CatalogBuyPanel catalogBuyPanel = CatalogBuyPanels.Find((CatalogBuyPanel x) => x.State == state);
			if (!(catalogBuyPanel != null))
			{
				return;
			}
			for (int i = 0; i < CatalogBuyPanels.Count; i++)
			{
				CatalogBuyPanels[i].Hide();
			}
			catalogBuyPanel.SetText(GetItemName(), ItemData.creatorName, ItemData.numberSold.ToString());
			if (state == CatalogShopBuyPanelState.Purchase)
			{
				CatalogBuyPanelPurchase catalogBuyPanelPurchase = catalogBuyPanel as CatalogBuyPanelPurchase;
				catalogBuyPanelPurchase.ItemCostText.text = ItemData.cost.ToString();
				int coins = Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).Coins;
				if (coins >= ItemData.cost)
				{
					catalogBuyPanelPurchase.SetVisibleButton(true);
					AllowPurchase(true);
				}
				else
				{
					catalogBuyPanelPurchase.SetVisibleButton(false);
				}
			}
			catalogBuyPanel.Show();
		}

		protected string GetItemName()
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			TemplateDefinition templateDefinition = dictionary.Values.ToList().First((TemplateDefinition x) => x.Id == ItemData.equipment.definitionId);
			return (templateDefinition == null) ? "NO_NAME_FOUND" : Service.Get<Localizer>().GetTokenTranslation(templateDefinition.Name);
		}

		public void OnCloseClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.BuyPanelCloseButtonClickedEvent));
		}

		public void OnPurchaseClicked()
		{
			AllowPurchase(false);
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.BuyPanelPurchaseButtonClickedEvent(ItemData));
		}

		public void OnNeedCoinsClicked()
		{
			CatalogBuyPanel catalogBuyPanel = CatalogBuyPanels.Find((CatalogBuyPanel x) => x.State == CatalogShopBuyPanelState.Purchase);
			(catalogBuyPanel as CatalogBuyPanelPurchase).ShowNotEnoughCoinsToolTip();
		}

		public void OnKeepShoppingClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.BuyPanelCloseButtonClickedEvent));
		}

		public void OnWearItClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.InvokeBackButtonClick));
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.BuyPanelWearItButtonClickedEvent(ItemData));
		}

		public void OnLearnMoreClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.BuyPanelLearnMoreButtonClickedEvent));
		}

		public void OnRequiredLevelClicked()
		{
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.BuyPanelRequiredLevelButtonClickedEvent));
		}

		public void OnCreatorButtonClicked()
		{
			CPDataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, ItemData.creatorName);
			OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
			openPlayerCardCommand.Execute();
		}

		public void ClosePanel()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
