using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupRewardItem : MonoBehaviour
	{
		private const string ANIMATOR_EXIT_TRIGGER = "exit";

		public Action<RewardPopupRewardItem> IconLoadCompleteAction;

		public Animator ItemAnimator;

		public GameObject LoadingSpinner;

		public GameObject MemberLockOverlay;

		public Text ItemNameText;

		private RewardCategory rewardCategory;

		private DReward rewardData;

		private readonly PrefabContentKey fabricIconKey = new PrefabContentKey("Rewards/RewardPopup/FabricIcon");

		private readonly PrefabContentKey itemIconKey = new PrefabContentKey("Rewards/RewardPopup/ItemIcon");

		private readonly PrefabContentKey prefabIconKey = new PrefabContentKey("Rewards/RewardPopup/PrefabIcon");

		private Image itemImage;

		private GameObject prefabContainer;

		private bool showItemName;

		public void LoadItem(RewardCategory rewardCategory, DReward rewardData, bool allowMemberLockOverlay = false, bool showItemName = false)
		{
			this.rewardCategory = rewardCategory;
			this.rewardData = rewardData;
			if (rewardCategory == RewardCategory.fabrics)
			{
				loadItemIconContainer(fabricIconKey);
			}
			else if (rewardCategory == RewardCategory.partySupplies || rewardCategory == RewardCategory.consumables || rewardCategory == RewardCategory.musicTracks || rewardCategory == RewardCategory.iglooSlots || rewardCategory == RewardCategory.decorationInstances || rewardCategory == RewardCategory.decorationPurchaseRights || rewardCategory == RewardCategory.structureInstances || rewardCategory == RewardCategory.structurePurchaseRights)
			{
				loadItemIconContainer(prefabIconKey);
			}
			else
			{
				loadItemIconContainer(itemIconKey);
			}
			if (allowMemberLockOverlay)
			{
				object obj = rewardData.UnlockID;
				if (obj == null)
				{
					obj = rewardData.EquipmentRequest.definitionId;
				}
				bool flag = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() || !RewardUtils.IsRewardMemberOnly(rewardCategory, obj);
				MemberLockOverlay.SetActive(!flag);
			}
			this.showItemName = showItemName;
		}

		private void loadItemIcon()
		{
			IRewardIconRenderer rewardIconRenderer = new RewardIconRendererFactory().GetRewardIconRenderer(rewardCategory);
			rewardIconRenderer.RenderReward(rewardData, onIconRenderComplete);
		}

		private void loadItemIconContainer(PrefabContentKey key)
		{
			Content.LoadAsync(onIconContainerLoaded, key);
		}

		private void onIconContainerLoaded(string path, GameObject iconPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(iconPrefab);
			gameObject.transform.SetParent(base.transform, false);
			if (gameObject.GetComponent<FabricIcon>() != null)
			{
				itemImage = gameObject.GetComponent<FabricIcon>().IconImage;
			}
			else
			{
				prefabContainer = gameObject;
			}
			loadItemIcon();
		}

		private void onIconRenderComplete(Sprite iconSprite, RectTransform iconPrefab, string itemName = null)
		{
			LoadingSpinner.SetActive(false);
			if (iconSprite != null)
			{
				itemImage.sprite = iconSprite;
				itemImage.color = new Color(1f, 1f, 1f, 1f);
			}
			else if (iconPrefab != null)
			{
				iconPrefab.SetParent(prefabContainer.transform, false);
			}
			MemberLockOverlay.transform.SetAsLastSibling();
			if (showItemName && !string.IsNullOrEmpty(itemName))
			{
				ItemNameText.text = Service.Get<Localizer>().GetTokenTranslation(itemName);
				ItemNameText.gameObject.SetActive(true);
				ItemNameText.transform.SetAsLastSibling();
			}
			else
			{
				ItemNameText.gameObject.SetActive(false);
			}
			if (IconLoadCompleteAction != null)
			{
				IconLoadCompleteAction(this);
			}
		}
	}
}
