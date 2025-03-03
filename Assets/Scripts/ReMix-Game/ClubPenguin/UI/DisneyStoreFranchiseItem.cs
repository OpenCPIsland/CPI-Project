using ClubPenguin.Avatar;
using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.DisneyStore;
using ClubPenguin.Rewards;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DisneyStoreFranchiseItem : MonoBehaviour
	{
		private enum IconMaterialIndex
		{
			RewardItem,
			IglooItem
		}

		public const string MEMBERSHIP_FLOW_TAG = "DisneyShop";

		private const int TAG_INDEX_OTHER = 0;

		private const int TAG_INDEX_CONSUMABLE = 1;

		private const int TAG_INDEX_IGLOO = 2;

		public Text TitleText;

		public DiscountableItemPriceTag PriceTag;

		public RawImage IconImage;

		public GameObject MemberLockPanel;

		public SpriteSelector PriceTagSpriteSelector;

		public GameObject OwnedPanel;

		public MaterialSelector MatSelector;

		private DisneyStoreItemData itemData;

		private DisneyStoreFranchise storeFranchise;

		private ItemImageBuilder itemImageBuilder;

		private void Awake()
		{
			IconImage.enabled = false;
		}

		private void OnEnable()
		{
			itemImageBuilder = ItemImageBuilder.acquire();
		}

		private void OnDisable()
		{
			ItemImageBuilder.release();
			itemImageBuilder = null;
		}

		public void SetItem(DisneyStoreItemData itemData, DisneyStoreFranchise storeFranchise)
		{
			this.itemData = itemData;
			this.storeFranchise = storeFranchise;
			TitleText.text = Service.Get<Localizer>().GetTokenTranslation(itemData.Definition.TitleToken);
			PriceTag.setItem(itemData.Definition, itemData.Definition.Cost, (CellPhoneSaleActivityDefinition sale, DisneyStoreItemDefinition item) => new List<DisneyStoreItemDefinition>(sale.DisneyStoreItems).Find((DisneyStoreItemDefinition disneyStoreItem) => disneyStoreItem.Id == itemData.Definition.Id));
			loadItemIcon(itemData);
			ShowItemStatus();
		}

		public void OnItemClicked()
		{
			if (itemData.Definition.IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership("DisneyShop");
			}
			else
			{
				storeFranchise.ShowConfirmation(itemData, convertTextureToSprite(IconImage.texture), this);
			}
		}

		public void ShowItemStatus()
		{
			if (!DisneyStoreUtils.IsItemMultiPurchase(itemData) && DisneyStoreUtils.IsItemOwned(itemData))
			{
				PriceTagSpriteSelector.gameObject.SetActive(false);
				if (OwnedPanel != null)
				{
					OwnedPanel.SetActive(true);
				}
				return;
			}
			if (OwnedPanel != null)
			{
				OwnedPanel.SetActive(false);
			}
			showPriceTag(itemData);
			showMemberStatus();
		}

		private void loadItemIcon(DisneyStoreItemData item)
		{
			if (item.Definition.Icon != null && !string.IsNullOrEmpty(item.Definition.Icon.Key))
			{
				Content.LoadAsync(onCustomIconLoadComplete, item.Definition.Icon);
				return;
			}
			List<DReward> rewards = item.GetRewards();
			switch (rewards[0].Category)
			{
			case RewardCategory.equipmentTemplates:
			{
				Dictionary<int, TemplateDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
				TemplateDefinition value2;
				if (dictionary2.TryGetValue((int)rewards[0].UnlockID, out value2))
				{
					Texture2DContentKey equipmentIconPath = EquipmentPathUtil.GetEquipmentIconPath(value2.AssetName);
					Content.LoadAsync(delegate(string key, Texture2D texture)
					{
						onIconReady(texture, 0);
					}, equipmentIconPath);
				}
				break;
			}
			case RewardCategory.equipmentInstances:
				renderRewardItem(rewards[0].EquipmentRequest.definitionId, CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(rewards[0].EquipmentRequest));
				break;
			case RewardCategory.decorationInstances:
			{
				Dictionary<int, DecorationDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DecorationDefinition>>();
				DecorationDefinition value;
				if (dictionary.TryGetValue((int)rewards[0].UnlockID, out value))
				{
					Content.LoadAsync(delegate(string path, Texture2D asset)
					{
						onIconReady(asset, 1);
					}, value.Icon);
				}
				break;
			}
			}
		}

		private void renderRewardItem(long requestId, DCustomEquipment customEquipment)
		{
			try
			{
				AbstractImageBuilder.CallbackToken callbackToken = default(AbstractImageBuilder.CallbackToken);
				callbackToken.Id = requestId;
				callbackToken.DefinitionId = customEquipment.DefinitionId;
				itemImageBuilder.RequestImage(customEquipment, delegate(bool success, Texture2D texture, AbstractImageBuilder.CallbackToken token)
				{
					if (success)
					{
						onIconReady(texture, 0);
					}
				}, callbackToken, Color.clear, Color.clear);
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
			}
		}

		private void onIconReady(Texture2D texture, int materialIndex)
		{
			if (IconImage != null)
			{
				Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
				MatSelector.SelectMaterial(materialIndex);
				IconImage.texture = sprite.texture;
				IconImage.enabled = true;
			}
		}

		private void showPriceTag(DisneyStoreItemData item)
		{
			PriceTagSpriteSelector.gameObject.SetActive(true);
			RewardCategory itemRewardCategory = DisneyStoreUtils.GetItemRewardCategory(item);
			if (DisneyStoreUtils.IsIglooReward(itemRewardCategory))
			{
				PriceTagSpriteSelector.SelectSprite(2);
			}
			else if (itemRewardCategory == RewardCategory.consumables)
			{
				PriceTagSpriteSelector.SelectSprite(1);
			}
			else
			{
				PriceTagSpriteSelector.SelectSprite(0);
			}
		}

		private void onCustomIconLoadComplete(string Path, Sprite icon)
		{
			if (IconImage != null)
			{
				IconImage.texture = icon.texture;
				IconImage.enabled = true;
			}
		}

		private void showMemberStatus()
		{
			if (itemData.Definition.IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				MemberLockPanel.SetActive(true);
			}
			else
			{
				MemberLockPanel.SetActive(false);
			}
		}

		private Sprite convertTextureToSprite(Texture texture)
		{
			return Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}
	}
}
