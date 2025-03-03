using ClubPenguin.Adventure;
using ClubPenguin.Core;
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
	public class RewardPopupScreenCoinsXP : RewardPopupScreen
	{
		private const string XP_TOKEN = "GlobalUI.Notification.XP";

		private const string COIN_TOKEN = "GlobalUI.Notification.Coins";

		private const string MEMBER_NOTIFICATION_TOKEN = "MemberNotification.RewardScreen.LevelText";

		public Text CoinsText;

		public GameObject CoinsGO;

		public GameObject XPParent;

		public GameObject XPGO;

		private DRewardPopupScreenCoinsXP screenData;

		public override void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
			this.screenData = (DRewardPopupScreenCoinsXP)screenData;
			popupController.RewardPopupAnimator.SetTrigger("ChestDismiss");
			CoinsText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Notification.Coins"), this.screenData.CoinCount);
			if (this.screenData.CoinCount == 0)
			{
				CoinsGO.SetActive(false);
			}
			if (this.screenData.XPCount == 0)
			{
				XPGO.SetActive(false);
			}
			else
			{
				CoroutineRunner.Start(loadMascotXPPrefab(this.screenData.mascotName), this, "RewardPopupScreenCoinsXP.loadMascotXPPrefab");
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MembershipData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				component.MembershipDataUpdated += onMembershipDataUpdated;
			}
			Service.Get<TrayNotificationManager>().DismissAllNotifications();
			membershipNotificationText = Service.Get<Localizer>().GetTokenTranslation("MemberNotification.RewardScreen.LevelText");
			checkMembershipDisclaimer();
		}

		private void onMembershipDataUpdated(MembershipData membershipData)
		{
			if (membershipData.IsMember)
			{
				Service.Get<TrayNotificationManager>().DismissAllNotifications();
			}
		}

		private IEnumerator loadMascotXPPrefab(string mascotName)
		{
			PrefabContentKey mascotXPContentKey = Service.Get<MascotService>().GetMascot(mascotName).Definition.RewardPopupXPContentKey;
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(mascotXPContentKey);
			yield return assetRequest;
			GameObject mascotXPGO = Object.Instantiate(assetRequest.Asset);
			mascotXPGO.transform.SetParent(XPParent.transform, false);
			mascotXPGO.GetComponentInChildren<Text>().text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Notification.XP"), screenData.XPCount);
		}

		public override void OnClick()
		{
			if (screenData.ShowXpAndCoinsUI)
			{
				if (screenData.CoinCount > 0)
				{
					Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(screenData.CoinCount);
				}
				if (screenData.XPCount > 0)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(RewardEvents.ShowSuppressedAddXP));
				}
			}
			screenComplete();
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
