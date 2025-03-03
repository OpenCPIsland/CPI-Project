using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupScreenSplash : RewardPopupScreen
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RewardPopupSplashShown
		{
		}

		private const string TITLE_TEXT_TOKEN = "Rewards.RewardPopup.Title";

		public Text TitleText;

		public GameObject DisclaimerGO;

		public GameObject OpenButton;

		private RewardPopupController popupController;

		private DRewardPopupScreenSplash screenData;

		private void Start()
		{
			if (!string.IsNullOrEmpty(screenData.SplashScreenTextToken))
			{
				TitleText.text = Service.Get<Localizer>().GetTokenTranslation(screenData.SplashScreenTextToken);
			}
			else
			{
				TitleText.text = Service.Get<Localizer>().GetTokenTranslation("Rewards.RewardPopup.Title");
			}
		}

		public override void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
			this.screenData = (DRewardPopupScreenSplash)screenData;
			this.popupController = popupController;
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardPopupSplashShown));
		}

		public override void OnClick()
		{
			if (popupController.PopupData.PopupType != DRewardPopup.RewardPopupType.replay && popupController.isChestLanded)
			{
				RewardPopupChest rewardChest = popupController.RewardChest;
				rewardChest.ChestOpenedAction = (System.Action)Delegate.Combine(rewardChest.ChestOpenedAction, new System.Action(OnChestOpened));
				popupController.RewardChest.ChestAnimator.SetTrigger("ChestOpen");
				popupController.RewardPopupAnimator.SetTrigger("ChestOpen");
				if (OpenButton != null)
				{
					OpenButton.SetActive(false);
				}
			}
			else if (popupController.PopupData.PopupType == DRewardPopup.RewardPopupType.replay)
			{
				screenComplete();
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			}
		}

		private void OnChestOpened()
		{
			RewardPopupChest rewardChest = popupController.RewardChest;
			rewardChest.ChestOpenedAction = (System.Action)Delegate.Remove(rewardChest.ChestOpenedAction, new System.Action(OnChestOpened));
			screenComplete();
		}
	}
}
