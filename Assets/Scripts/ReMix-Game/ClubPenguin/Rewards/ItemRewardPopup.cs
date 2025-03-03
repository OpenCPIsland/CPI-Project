using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class ItemRewardPopup : AnimatedPopup
	{
		public ButtonClickListener TakeItemButton;

		public Transform BackgroundContainer;

		public Text ItemNameText;

		public RectTransform ItemPanel;

		private readonly PrefabContentKey BGContentKey = new PrefabContentKey("Rewards/RewardPopup/ItemRewardPopupBG_*");

		private RewardCategory rewardCategory;

		private DReward rewardData;

		private RewardPopupRewardItem item;

		private DItemRewardPopup popupData;

		protected override void awake()
		{
			base.awake();
			ItemImageBuilder.acquire();
		}

		protected override void popupOpenAnimationComplete()
		{
			TakeItemButton.OnClick.AddListener(onCollectClicked);
			base.popupOpenAnimationComplete();
		}

		private void OnDisable()
		{
			TakeItemButton.OnClick.RemoveListener(onCollectClicked);
		}

		private void onCollectClicked(ButtonClickListener.ClickType clickType)
		{
			TakeItemButton.OnClick.RemoveListener(onCollectClicked);
			if (popupData.CoinReward > 0)
			{
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(popupData.CoinReward);
			}
			if (popupData.XpReward > 0)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(RewardEvents.ShowSuppressedAddXP));
			}
			ClosePopup();
		}

		public void SetData(DItemRewardPopup itemRewardPopupData)
		{
			popupData = itemRewardPopupData;
			updateBackground();
			ItemNameText.text = itemRewardPopupData.ItemName;
			rewardCategory = itemRewardPopupData.RewardCategory;
			rewardData = itemRewardPopupData.RewardData;
			CoroutineRunner.Start(loadItem(rewardCategory, rewardData), this, "RewardPopupScreenItems.loadItem");
		}

		private IEnumerator loadItem(RewardCategory rewardCategory, DReward reward)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(RewardPopupConstants.RewardPopupItemContentKey);
			yield return assetRequest;
			GameObject itemGO = UnityEngine.Object.Instantiate(assetRequest.Asset);
			itemGO.transform.SetParent(ItemPanel, false);
			item = itemGO.GetComponent<RewardPopupRewardItem>();
			RewardPopupRewardItem rewardPopupRewardItem = item;
			rewardPopupRewardItem.IconLoadCompleteAction = (Action<RewardPopupRewardItem>)Delegate.Combine(rewardPopupRewardItem.IconLoadCompleteAction, new Action<RewardPopupRewardItem>(OnItemLoadComplete));
			item.LoadItem(rewardCategory, reward);
		}

		private void OnItemLoadComplete(RewardPopupRewardItem rewardItem)
		{
			RewardPopupRewardItem rewardPopupRewardItem = item;
			rewardPopupRewardItem.IconLoadCompleteAction = (Action<RewardPopupRewardItem>)Delegate.Remove(rewardPopupRewardItem.IconLoadCompleteAction, new Action<RewardPopupRewardItem>(OnItemLoadComplete));
		}

		private void updateBackground()
		{
			PrefabContentKey key = new PrefabContentKey(BGContentKey, popupData.MascotDefinition.AbbreviatedName);
			Content.LoadAsync(onBGLoaded, key);
		}

		private void onBGLoaded(string path, GameObject bgPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(bgPrefab);
			gameObject.transform.SetParent(BackgroundContainer, false);
		}

		protected override void onDestroy()
		{
			base.onDestroy();
			ItemImageBuilder.release();
		}
	}
}
