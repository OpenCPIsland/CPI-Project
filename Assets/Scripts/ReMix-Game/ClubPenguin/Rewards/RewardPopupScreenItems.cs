using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupScreenItems : RewardPopupScreen
	{
		private const float introCompleteDelay = 1f;

		private static string MEMBER_NOTIFICATION_RH_ITEMS_TOKEN = "MemberNotification.RewardScreen.RHItemsText";

		private static string MEMBER_NOTIFICATION_AA_ITEMS_TOKEN = "MemberNotification.RewardScreen.AAItemsText";

		private static string MEMBER_NOTIFICATION_LEVEL_TOKEN = "MemberNotification.RewardScreen.LevelItemsText";

		public Transform ItemParentTransform;

		public Text RewardCategoryText;

		public GridLayoutScaler ItemGridScaler;

		public GameObject MembersOnlyIndicator;

		public GameObject ItemsBG;

		public GameObject UnlockedText;

		private DRewardPopupScreenItems screenData;

		private List<RewardPopupRewardItem> items;

		private bool isIntroComplete;

		private RewardPopupController controller;

		public override void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
			controller = popupController;
			this.screenData = (DRewardPopupScreenItems)screenData;
			RewardCategoryText.text = RewardUtils.GetUnlockText(this.screenData.ItemCategory);
			popupController.RewardChest.ChestAnimator.SetTrigger("LaunchItems");
			isIntroComplete = false;
			loadItems(this.screenData.ItemCategory, this.screenData.Rewards);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component.MembershipDataUpdated += onMembershipDataUpdated;
			}
			checkMembershipDisclaimer();
			showMemberStatusIndicator();
			setBackground();
			UnlockedText.SetActive(Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && !this.screenData.IsRewardsAllNonMember);
		}

		public override void OnClick()
		{
			if (isIntroComplete)
			{
				screenComplete();
			}
		}

		protected override void checkMembershipDisclaimer()
		{
			bool flag = false;
			if (!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && !screenData.IsRewardsAllNonMember)
			{
				flag = true;
				switch (controller.PopupData.PopupType)
				{
				case DRewardPopup.RewardPopupType.levelUp:
					membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation(MEMBER_NOTIFICATION_LEVEL_TOKEN);
					break;
				case DRewardPopup.RewardPopupType.generic:
					if (controller.PopupData.MascotName == "Rockhopper")
					{
						membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation(MEMBER_NOTIFICATION_RH_ITEMS_TOKEN);
					}
					else if (!string.IsNullOrEmpty(controller.PopupData.MascotName))
					{
						membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation(MEMBER_NOTIFICATION_AA_ITEMS_TOKEN);
					}
					else
					{
						membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation(getMembershipDisclaimerTokenForRewardCategory(screenData.ItemCategory));
					}
					break;
				default:
					membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation(getMembershipDisclaimerTokenForRewardCategory(screenData.ItemCategory));
					break;
				}
				if (flag)
				{
					showMembershipDisclaimer(membershipNotificationText);
				}
				else
				{
					Service.Get<TrayNotificationManager>().DismissAllNotifications();
				}
			}
			else
			{
				Service.Get<TrayNotificationManager>().DismissAllNotifications();
			}
		}

		private void showMemberStatusIndicator()
		{
			if (!base.gameObject.IsDestroyed() && !MembersOnlyIndicator.IsDestroyed())
			{
				MembersOnlyIndicator.SetActive(!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && !screenData.IsRewardsAllNonMember);
			}
		}

		private void onMembershipDataUpdated(MembershipData membershipData)
		{
			if (!base.gameObject.IsDestroyed() && membershipData.IsMember)
			{
				Service.Get<TrayNotificationManager>().DismissAllNotifications();
				showMemberStatusIndicator();
			}
		}

		private void loadItems(RewardCategory rewardCategory, DReward[] rewards)
		{
			ItemGridScaler.Init(rewards.Length);
			items = new List<RewardPopupRewardItem>();
			for (int i = 0; i < rewards.Length; i++)
			{
				CoroutineRunner.Start(loadItem(rewardCategory, rewards[i]), this, "RewardPopupScreenItems.loadItem");
			}
			CoroutineRunner.Start(doIntroCompleteDelay(), this, "RewardPopupScreenItems.doIntroCompleteDelay");
		}

		private IEnumerator loadItem(RewardCategory rewardCategory, DReward reward)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(RewardPopupConstants.RewardPopupItemContentKey);
			yield return assetRequest;
			GameObject itemGO = Object.Instantiate(assetRequest.Asset);
			itemGO.transform.SetParent(ItemParentTransform, false);
			itemGO.transform.SetSiblingIndex(0);
			RewardPopupRewardItem item = itemGO.GetComponent<RewardPopupRewardItem>();
			items.Add(item);
			item.LoadItem(rewardCategory, reward);
		}

		private IEnumerator doIntroCompleteDelay()
		{
			yield return new WaitForSeconds(1f);
			isIntroComplete = true;
		}

		private void setBackground()
		{
			if (ItemsBG != null)
			{
				ItemsBG.GetComponent<SpriteSelector>().SelectSprite((int)screenData.ItemCategory);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			CoroutineRunner.StopAllForOwner(this);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MembershipData component;
			if (cPDataEntityCollection != null && cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component.MembershipDataUpdated -= onMembershipDataUpdated;
			}
		}
	}
}
