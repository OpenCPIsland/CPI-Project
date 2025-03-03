using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Progression;
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
	public class MarketplaceEventRowItem : MonoBehaviour
	{
		private const string MEMBERSHIP_FLOW_TAG = "party_supplies_market";

		public Text TitleText;

		public RawImage IconImage;

		public Button IconButton;

		public GameObject Collect;

		public Text CollectText;

		public Button CollectButton;

		public Text CoinsText;

		public Text XpText;

		public GameObject MaxXpText;

		public GameObject CollectWait;

		public GameObject MemberLockPanel;

		public GameObject OwnedPanel;

		private MarketplaceEventItem item;

		private Color itemIconBackgroundColor;

		private Color itemIconPenguinColor;

		private ItemImageBuilder itemImageBuilder;

		private string itemNameForBI;

		private bool isMemberOnly;

		private void Awake()
		{
			IconImage.enabled = false;
		}

		private void OnDestroy()
		{
			if (item != null)
			{
				item.CollectItemSucceeded -= onCollectItemSucceeded;
			}
		}

		public void SetItem(MarketplaceEventItem item, Material itemIconImageMaterial, Color itemIconBackgroundColor, Color itemIconPenguinColor, string eventNameForBI)
		{
			this.item = item;
			this.itemIconBackgroundColor = itemIconBackgroundColor;
			this.itemIconPenguinColor = itemIconPenguinColor;
			IconImage.material = itemIconImageMaterial;
			loadCoinsAndXP(item);
			loadItemIcon(item);
			showItemStatus();
			showMemberStatus();
		}

		public void OnCollectClicked()
		{
			setCollectWaitVisible(true);
			item.CollectItemSucceeded += onCollectItemSucceeded;
			item.CollectItemFailed += onCollectItemFailed;
			item.CollectItem();
		}

		private void onCollectItemSucceeded()
		{
			item.CollectItemSucceeded -= onCollectItemSucceeded;
			showItemStatus();
			string tier = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() ? "member" : "free";
			Service.Get<ICPSwrveService>().Action("reward_claim", tier, itemNameForBI);
		}

		private void onCollectItemFailed()
		{
			setCollectWaitVisible(false);
		}

		private void setCollectWaitVisible(bool collectWaitVisible)
		{
			CollectWait.SetActive(collectWaitVisible);
			CollectText.enabled = !collectWaitVisible;
			CollectButton.enabled = !collectWaitVisible;
			IconButton.enabled = !collectWaitVisible;
		}

		private void showItemStatus()
		{
			if (OwnedPanel != null)
			{
				OwnedPanel.SetActive(item.HasItem());
				Collect.SetActive(item.IsAvailable() && !item.HasItem());
				IconButton.enabled = (item.IsAvailable() && !item.HasItem());
			}
		}

		private void showMemberStatus()
		{
			if (isMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				MemberLockPanel.SetActive(true);
			}
			else
			{
				MemberLockPanel.SetActive(false);
			}
		}

		private void loadCoinsAndXP(MarketplaceEventItem item)
		{
			if (CoinsText != null)
			{
				int coinReward = item.GetCoinReward();
				if (coinReward != 0)
				{
					CoinsText.text = coinReward.ToString();
				}
			}
			if (XpText != null)
			{
				Dictionary<string, int> xpReward = item.GetXpReward();
				if (xpReward != null && xpReward.Count != 0)
				{
					using (Dictionary<string, int>.Enumerator enumerator = xpReward.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, int> current = enumerator.Current;
							if (Service.Get<ProgressionService>().IsMascotMaxLevel(current.Key))
							{
								XpText.gameObject.SetActive(false);
								MaxXpText.SetActive(true);
							}
							else
							{
								XpText.text = current.Value.ToString();
							}
						}
					}
				}
			}
		}

		private void loadItemIcon(MarketplaceEventItem item)
		{
			List<DReward> rewards = item.GetRewards();
			for (int i = 0; i < rewards.Count; i++)
			{
				TemplateDefinition value;
				switch (rewards[i].Category)
				{
				case RewardCategory.equipmentTemplates:
				{
					Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
					if (dictionary.TryGetValue((int)rewards[0].UnlockID, out value))
					{
						Texture2DContentKey equipmentIconPath = EquipmentPathUtil.GetEquipmentIconPath(value.AssetName);
						Content.LoadAsync(delegate(string key, Texture2D texture)
						{
							onIconReady(texture);
						}, equipmentIconPath);
						TitleText.text = Service.Get<Localizer>().GetTokenTranslation(value.Name);
						itemNameForBI = value.AssetName;
						isMemberOnly = value.IsMemberOnly;
					}
					return;
				}
				case RewardCategory.equipmentInstances:
				{
					Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
					if (dictionary.TryGetValue(rewards[0].EquipmentRequest.definitionId, out value))
					{
						TitleText.text = Service.Get<Localizer>().GetTokenTranslation(value.Name);
						itemNameForBI = value.AssetName;
						isMemberOnly = value.IsMemberOnly;
					}
					renderRewardItem(rewards[0].EquipmentRequest.definitionId, CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(rewards[0].EquipmentRequest));
					return;
				}
				}
			}
		}

		private void renderRewardItem(long requestId, DCustomEquipment customEquipment)
		{
			itemImageBuilder = ItemImageBuilder.acquire();
			try
			{
				AbstractImageBuilder.CallbackToken callbackToken = default(AbstractImageBuilder.CallbackToken);
				callbackToken.Id = requestId;
				callbackToken.DefinitionId = customEquipment.DefinitionId;
				itemImageBuilder.RequestImage(customEquipment, delegate(bool success, Texture2D texture, AbstractImageBuilder.CallbackToken token)
				{
					if (success)
					{
						onIconReady(texture);
					}
					else
					{
						cleanupItemImageBuilder();
					}
				}, callbackToken, itemIconBackgroundColor, itemIconPenguinColor);
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
				cleanupItemImageBuilder();
			}
		}

		private void onIconReady(Texture2D texture)
		{
			if (IconImage != null)
			{
				Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
				IconImage.texture = sprite.texture;
				IconImage.enabled = true;
			}
			cleanupItemImageBuilder();
		}

		private void cleanupItemImageBuilder()
		{
			if (itemImageBuilder != null)
			{
				ItemImageBuilder.release();
				itemImageBuilder = null;
			}
		}
	}
}
