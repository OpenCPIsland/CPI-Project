using ClubPenguin.Adventure;
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
	public class RewardPopupScreenQuests : RewardPopupScreen
	{
		private const string MEMBER_NOTIFICATION_TOKEN = "MemberNotification.RewardScreen.QuestText";

		private const float introCompleteDelay = 1f;

		private static string TITLE_TEXT_KEY = "RewardScreen.UnlockedText.Quests";

		public Transform ItemParentTransform;

		public Text RewardCategoryText;

		public GameObject UnlockedText;

		public int MaxItems = 2;

		private DRewardPopupScreenQuests screenData;

		private bool isIntroComplete;

		private List<RewardPopupQuestsItem> items;

		public override void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
			this.screenData = (DRewardPopupScreenQuests)screenData;
			RewardCategoryText.text = Service.Get<Localizer>().GetTokenTranslation(TITLE_TEXT_KEY);
			isIntroComplete = false;
			loadItems(this.screenData.quests);
			UnlockedText.SetActive(Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && !this.screenData.IsRewardsAllNonMember);
			Service.Get<TrayNotificationManager>().DismissAllNotifications();
			membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation("MemberNotification.RewardScreen.QuestText");
			checkMembershipDisclaimer();
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
			if (!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() && !screenData.IsRewardsAllNonMember)
			{
				showMembershipDisclaimer(membershipNotificationText);
			}
		}

		private void loadItems(QuestDefinition[] quests)
		{
			items = new List<RewardPopupQuestsItem>();
			for (int i = 0; i < Mathf.Min(quests.Length, MaxItems); i++)
			{
				CoroutineRunner.Start(loadQuest(quests[i]), this, "RewardPopupScreenQuests.loadQuest");
			}
			CoroutineRunner.Start(doIntroCompleteDelay(), this, "RewardPopupScreenQuests.doIntroCompleteDelay");
		}

		private IEnumerator loadQuest(QuestDefinition quest)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(RewardPopupConstants.RewardPopupQuestsContentKey, quest.Mascot.AbbreviatedName);
			yield return assetRequest;
			GameObject itemGO = Object.Instantiate(assetRequest.Asset);
			itemGO.transform.SetParent(ItemParentTransform, false);
			RewardPopupQuestsItem item = itemGO.GetComponent<RewardPopupQuestsItem>();
			item.Init(quest);
			items.Add(item);
		}

		private IEnumerator doIntroCompleteDelay()
		{
			yield return new WaitForSeconds(1f);
			isIntroComplete = true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
