using ClubPenguin.Adventure;
using ClubPenguin.Progression;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupScreenSplashLevelUp : RewardPopupScreen
	{
		[Serializable]
		public class MascotLevelTextMap
		{
			public string MascotName;

			public Text LevelText;
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LevelUpPopupShown
		{
		}

		private const string SPLASH_TITLE_TOKEN_LEVEL = "Rewards.RewardPopup.LevelUpTitle";

		public Animator LevelUpStarsAnimator;

		public Text TitleText;

		public Text CurrentMascotLevelText;

		public Text PrevMascotLevelText;

		public Text CurrentOverallLevelText;

		public Text PrevOverallLevelText;

		public MascotLevelTextMap[] MascotLevelText;

		private MascotService mascotService;

		private List<string> mascotNames;

		private RewardPopupController popupController;

		private void Start()
		{
			mascotNames = new List<string>();
			mascotService = Service.Get<MascotService>();
			setTitleText();
			setLevelText();
		}

		private void setTitleText()
		{
			TitleText.text = Service.Get<Localizer>().GetTokenTranslation("Rewards.RewardPopup.LevelUpTitle");
		}

		private void setLevelText()
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			getMascotNames();
			for (int i = 0; i < mascotNames.Count; i++)
			{
				if (mascotNames[i] == popupController.PopupData.MascotName)
				{
					CurrentMascotLevelText.text = progressionService.MascotLevel(mascotNames[i]).ToString();
					PrevMascotLevelText.text = (progressionService.MascotLevel(mascotNames[i]) - 1).ToString();
				}
				Text mascotLevelText = getMascotLevelText(mascotNames[i]);
				if (mascotLevelText != null)
				{
					mascotLevelText.text = progressionService.MascotLevel(mascotNames[i]).ToString();
				}
			}
			CurrentOverallLevelText.text = progressionService.Level.ToString();
			PrevOverallLevelText.text = (progressionService.Level - 1).ToString();
			LevelUpStarsAnimator.SetTrigger(popupController.PopupData.MascotName);
		}

		private void getMascotNames()
		{
			foreach (Mascot mascot in mascotService.Mascots)
			{
				mascotNames.Add(mascot.Name);
			}
		}

		private Text getMascotLevelText(string mascotName)
		{
			for (int i = 0; i < MascotLevelText.Length; i++)
			{
				if (MascotLevelText[i].MascotName == mascotName)
				{
					return MascotLevelText[i].LevelText;
				}
			}
			return null;
		}

		public override void Init(DRewardPopupScreen screenData, RewardPopupController popupController)
		{
			this.popupController = popupController;
			Service.Get<EventDispatcher>().DispatchEvent(default(LevelUpPopupShown));
		}

		public override void OnClick()
		{
			if (popupController.isChestLanded)
			{
				RewardPopupChest rewardChest = popupController.RewardChest;
				rewardChest.ChestOpenedAction = (System.Action)Delegate.Combine(rewardChest.ChestOpenedAction, new System.Action(OnChestOpened));
				popupController.RewardChest.ChestAnimator.SetTrigger("ChestOpen");
				popupController.RewardPopupAnimator.SetTrigger("ChestOpen");
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
