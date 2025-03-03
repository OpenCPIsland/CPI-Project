using ClubPenguin.Breadcrumbs;
using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.Progression;
using ClubPenguin.Props;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceItem : MonoBehaviour, IComparable
	{
		public Button Button;

		public RawImage ItemIcon;

		public DiscountableItemPriceTag PriceTag;

		public Text ItemNameText;

		public GameObject OutOfStockImage;

		public GameObject LevelIcon;

		public Text LevelIconText;

		public GameObject MemberTag;

		public Sprite DisabledButtonAsset;

		public GameObject SpecialItemEffect;

		public FeatureLabelBreadcrumb FeatureLabel;

		private MarketplaceItemLockStates lockStates;

		private PropDefinition itemDefinition;

		private Sprite theOnSprite;

		private Sprite theOffSprite;

		private Animator itemAnimator;

		private bool animatorDisabled;

		public int RequiredLevel
		{
			get;
			private set;
		}

		public bool IsSpecialItem
		{
			get;
			private set;
		}

		public bool IsOutOfStock
		{
			get
			{
				return lockStates.IsOutOfStock;
			}
		}

		public bool IsLevelLocked
		{
			get
			{
				return lockStates.IsLevelLocked;
			}
		}

		public bool IsMemberLocked
		{
			get
			{
				return lockStates.IsMemberLocked;
			}
		}

		public MarketplaceItemLockStates LockStates
		{
			get
			{
				return lockStates;
			}
		}

		public PropDefinition ItemDefinition
		{
			get
			{
				return itemDefinition;
			}
		}

		public void Init(Sprite sprite, PropDefinition propDef, bool isOutOfStock, bool isSpecialItem, int unlockLevel, Sprite offSprite, Sprite onSprite, Color textColor)
		{
			itemDefinition = propDef;
			ItemNameText.text = Service.Get<Localizer>().GetTokenTranslation(propDef.Name);
			setItemCost(itemDefinition);
			FeatureLabel.TypeId = propDef.GetNameOnServer();
			FeatureLabel.SetBreadcrumbVisibility();
			ItemIcon.texture = sprite.texture;
			lockStates.IsOutOfStock = isOutOfStock;
			SpecialItemEffect.SetActive(isSpecialItem);
			RequiredLevel = unlockLevel;
			IsSpecialItem = isSpecialItem;
			theOnSprite = onSprite;
			theOffSprite = offSprite;
			checkLockedState();
			setSprites(offSprite, onSprite);
			UpdateVisualStates();
			ItemNameText.color = textColor;
			itemAnimator = GetComponent<Animator>();
		}

		public void UpdateVisualStates()
		{
			MemberTag.SetActive(lockStates.IsMemberLocked);
			OutOfStockImage.SetActive(lockStates.IsOutOfStock);
			if (lockStates.IsLevelLocked && !lockStates.IsMemberLocked)
			{
				LevelIcon.SetActive(true);
				LevelIconText.text = Service.Get<ProgressionService>().Level.ToString();
			}
			else
			{
				LevelIcon.SetActive(false);
			}
			if (lockStates.IsOutOfStock || lockStates.IsLevelLocked || lockStates.IsMemberLocked)
			{
				setSprites(DisabledButtonAsset, DisabledButtonAsset);
				PriceTag.Hide();
				if (lockStates.IsLevelLocked)
				{
					LevelIconText.text = RequiredLevel.ToString();
				}
				ItemIcon.GetComponent<MaterialSelector>().SelectMaterial((!IsSpecialItem) ? 1 : 3);
			}
			else
			{
				setSprites(theOffSprite, theOnSprite);
				PriceTag.Show();
				ItemIcon.GetComponent<MaterialSelector>().SelectMaterial(IsSpecialItem ? 2 : 0);
			}
		}

		public void Update()
		{
			if (!animatorDisabled && itemAnimator != null)
			{
				AnimatorStateInfo currentAnimatorStateInfo = itemAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName("MarketplaceItemIntro") && currentAnimatorStateInfo.normalizedTime > 1f)
				{
					itemAnimator.enabled = false;
					animatorDisabled = true;
				}
			}
		}

		private void setSprites(Sprite offSprite, Sprite onSprite)
		{
		}

		public void checkLockedState()
		{
			if (Service.Get<ProgressionService>().Level < RequiredLevel)
			{
				lockStates.IsLevelLocked = true;
			}
			if (!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				lockStates.IsMemberLocked = itemDefinition.IsMemberOnly;
			}
			else
			{
				lockStates.IsMemberLocked = false;
			}
		}

		public int CompareTo(object obj)
		{
			MarketplaceItem marketplaceItem = (MarketplaceItem)obj;
			int num = RequiredLevel.CompareTo(marketplaceItem.RequiredLevel);
			if (num == 0 && marketplaceItem != this && IsSpecialItem)
			{
				num = -1;
			}
			return num;
		}

		private void setItemCost(PropDefinition definition)
		{
			PriceTag.setItem(definition, definition.Cost, (CellPhoneSaleActivityDefinition sale, PropDefinition item) => new List<PropDefinition>(sale.Consumables).Find((PropDefinition prop) => prop.Id == definition.Id));
		}
	}
}
