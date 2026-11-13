using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class ItemRewardPopupHalf : AnimatedPopup
	{
		private const string CLAIM_TOKEN = "Rewards.PopupLite.ClaimItem";

		private const string MAX_XP_LEVEL_TOKEN = "MyProgress.MaxLevelText";

		public Text ItemNameText;

		public Text ClaimText;

		public ButtonClickListener CollectButton;

		public GameObject CoinOnlyContainer;

		public Text CoinOnlyAmountText;

		public GameObject XPOnlyContainer;

		public GameObject CoinAndXPContainer;

		public Text CoinAndXP_CoinAmountText;

		public SpriteSelector CoinAndXP_XPSpriteSelector;

		public Text CoinAndXP_XPText;

		public Image ItemIcon;

		private static PrefabContentKey XP_PREFAB_CONTENT_KEY = new PrefabContentKey("Rewards/RewardPopupScreens/ImagePanel_*_Sm");

		private DItemRewardPopup popupData;

		private bool isItemRewardPopupClosing;

		private Localizer localizer;

		protected override void start()
		{
			CollectButton.OnClick.AddListener(onCollectClicked);
		}

		protected override void onDestroy()
		{
			CollectButton.OnClick.RemoveListener(onCollectClicked);
		}

		private void onCollectClicked(ButtonClickListener.ClickType clickType)
		{
			if (!isItemRewardPopupClosing)
			{
				isItemRewardPopupClosing = true;
				if (popupData.CoinReward > 0)
				{
					Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(popupData.CoinReward);
				}
				if (popupData.XpReward > 0)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(RewardEvents.ShowSuppressedAddXP));
				}
				CollectButton.OnClick.RemoveListener(onCollectClicked);
				ClosePopup();
			}
		}

		public void SetData(DItemRewardPopup itemRewardPopupData)
		{
			localizer = Service.Get<Localizer>();
			popupData = itemRewardPopupData;
			ItemNameText.text = itemRewardPopupData.ItemName;
			ClaimText.text = localizer.GetTokenTranslation("Rewards.PopupLite.ClaimItem");
			if (itemRewardPopupData.IsQuestItem)
			{
				ItemIcon.gameObject.SetActive(true);
				Content.LoadAsync(onSpriteLoaded, itemRewardPopupData.RewardIcon);
				CoinOnlyContainer.SetActive(false);
			}
			else if (itemRewardPopupData.CoinReward == 0 && itemRewardPopupData.XpReward != 0)
			{
				ItemIcon.gameObject.SetActive(false);
				XPOnlyContainer.SetActive(true);
				CoroutineRunner.Start(loadXPPrefab(itemRewardPopupData), this, "loadXPPrefab");
			}
			else if (itemRewardPopupData.CoinReward != 0 && itemRewardPopupData.XpReward != 0)
			{
				ItemIcon.gameObject.SetActive(false);
				CoinAndXPContainer.SetActive(true);
				CoinAndXP_CoinAmountText.text = itemRewardPopupData.CoinReward.ToString();
				if (Service.Get<ProgressionService>().IsMascotMaxLevel(itemRewardPopupData.MascotDefinition.name))
				{
					CoinAndXP_XPText.text = localizer.GetTokenTranslation("MyProgress.MaxLevelText");
				}
				else
				{
					CoinAndXP_XPText.text = itemRewardPopupData.XpReward.ToString();
				}
				int num = 0;
				while (true)
				{
					if (num < CoinAndXP_XPSpriteSelector.Sprites.Length)
					{
						string value = string.Format("_{0}", itemRewardPopupData.MascotAbbreviatedName);
						if (CoinAndXP_XPSpriteSelector.Sprites[num].name.Contains(value))
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				CoinAndXP_XPSpriteSelector.SelectSprite(num);
			}
			else if (itemRewardPopupData.CoinReward != 0 && itemRewardPopupData.XpReward == 0)
			{
				ItemIcon.gameObject.SetActive(false);
				CoinOnlyContainer.SetActive(true);
				CoinOnlyAmountText.text = itemRewardPopupData.CoinReward.ToString();
			}
		}

		private IEnumerator loadXPPrefab(DItemRewardPopup itemRewardPopupData)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(XP_PREFAB_CONTENT_KEY, itemRewardPopupData.MascotAbbreviatedName);
			yield return request;
			GameObject xpObject = Object.Instantiate(request.Asset, XPOnlyContainer.transform, false);
			Text xpText = xpObject.GetComponentInChildren<Text>();
			if (xpText != null)
			{
				if (Service.Get<ProgressionService>().IsMascotMaxLevel(itemRewardPopupData.MascotDefinition.name))
				{
					xpText.text = localizer.GetTokenTranslation("MyProgress.MaxLevelText");
				}
				xpText.text = itemRewardPopupData.XpReward.ToString();
			}
		}

		private void onSpriteLoaded(string path, Sprite sprite)
		{
			ItemIcon.GetComponent<Image>().sprite = sprite;
		}
	}
}
