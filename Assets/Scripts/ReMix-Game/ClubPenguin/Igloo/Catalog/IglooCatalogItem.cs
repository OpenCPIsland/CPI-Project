using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.Catalog
{
	public class IglooCatalogItem : MonoBehaviour
	{
		[SerializeField]
		private Text titleText;

		[SerializeField]
		private Text priceText;

		[SerializeField]
		private RawImage iconImage;

		[SerializeField]
		private GameObject lockPanel;

		[SerializeField]
		private GameObject memberLockBadge;

		[SerializeField]
		private GameObject levelLockBadge;

		[SerializeField]
		private Text levelLockText;

		[SerializeField]
		private GameObject mascotBadges;

		[SerializeField]
		private PersistentBreadcrumbTypeDefinitionKey structureBreadcrumbType;

		[SerializeField]
		private PersistentBreadcrumbTypeDefinitionKey decorationBreadcrumbType;

		[SerializeField]
		private GameObject structureSizeIcon;

		[SerializeField]
		private SpriteSelector structureIconSpriteSelector;

		public NotificationBreadcrumb breadcrumb;

		private IglooCatalogController catalog;

		private IglooCatalogItemData itemData;

		private void Awake()
		{
			iconImage.enabled = false;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void SetItem(IglooCatalogItemData itemData, IglooCatalogController catalog)
		{
			this.itemData = itemData;
			this.catalog = catalog;
			if (itemData.IsDecoration())
			{
				breadcrumb.SetBreadcrumbId(decorationBreadcrumbType, itemData.ID.ToString());
				structureSizeIcon.SetActive(false);
			}
			else
			{
				breadcrumb.SetBreadcrumbId(structureBreadcrumbType, itemData.ID.ToString());
				structureIconSpriteSelector.SelectSprite(itemData.StructureSize - 1);
				structureSizeIcon.SetActive(true);
			}
			titleText.text = Service.Get<Localizer>().GetTokenTranslation(itemData.TitleToken);
			priceText.text = itemData.Cost.ToString();
			ShowItemStatus();
		}

		public IglooCatalogItemData GetItemData()
		{
			return itemData;
		}

		public void SetImageFromTexture2D(string path, Texture2D icon)
		{
			itemData.SetImageFromTexture2D(path, icon);
			if (iconImage != null)
			{
				iconImage.texture = itemData.IconSprite.texture;
				iconImage.enabled = true;
			}
		}

		public void OnItemClicked()
		{
			if (memberLockBadge.activeSelf)
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership("igloo_catalog_item");
			}
			else if (!levelLockBadge.activeSelf && !mascotBadges.activeSelf)
			{
				if (itemData.IsDecoration())
				{
					Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(decorationBreadcrumbType, itemData.ID.ToString());
					Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb("Decoration");
				}
				else
				{
					Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(structureBreadcrumbType, itemData.ID.ToString());
					Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb("Structure");
				}
				catalog.ShowConfirmation(itemData, convertTextureToSprite(iconImage.texture), this);
			}
		}

		private Sprite convertTextureToSprite(Texture texture)
		{
			return Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}

		public void ShowItemStatus()
		{
			if (itemData.IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				setMemberLocked();
			}
			else if (itemData.LevelLocked)
			{
				setLevelLocked(itemData.Level);
			}
			else if (itemData.ProgressionLocked)
			{
				setMascotLocked(itemData.MascotName);
			}
			else
			{
				setUnlocked();
			}
		}

		private void setMemberLocked()
		{
			lockPanel.SetActive(true);
			memberLockBadge.SetActive(true);
			levelLockBadge.SetActive(false);
			mascotBadges.SetActive(false);
		}

		private void setLevelLocked(int level)
		{
			lockPanel.SetActive(true);
			memberLockBadge.SetActive(false);
			levelLockBadge.SetActive(true);
			mascotBadges.SetActive(false);
			levelLockText.text = level.ToString();
		}

		private void setMascotLocked(string mascotName)
		{
			lockPanel.SetActive(true);
			memberLockBadge.SetActive(false);
			levelLockBadge.SetActive(false);
			mascotBadges.SetActive(true);
			IList<Transform> children = mascotBadges.GetChildren();
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].name.Equals(mascotName))
				{
					children[i].gameObject.SetActive(true);
				}
				else
				{
					children[i].gameObject.SetActive(false);
				}
			}
		}

		private void setUnlocked()
		{
			lockPanel.SetActive(false);
			memberLockBadge.SetActive(false);
			levelLockBadge.SetActive(false);
			mascotBadges.SetActive(false);
		}
	}
}
