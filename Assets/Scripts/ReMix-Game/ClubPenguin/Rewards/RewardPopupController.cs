using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardPopupController : MonoBehaviour
	{
		private const float CanvasPlaneDistance = 150f;

		public RectTransform ScreenParentTransform;

		public RewardPopupBG PopupBG;

		public Animator RewardPopupAnimator;

		public Animator ParachuteAnimator;

		public Transform ChestParent;

		public bool isChestLanded;

		public RewardPopupChest RewardChest;

		public string PopupTitle;

		public ButtonClickListener HitBox;

		private DRewardPopupScreen[] screenData;

		private RewardPopupScreen currentScreen;

		private int currentScreenIndex;

		private int previousCameraCullingMask;

		private float previousCanvasPlaneDistance;

		private Canvas parentCanvas;

		private DRewardPopup popupData;

		private PrefabContentKey defaultChestKey = new PrefabContentKey("Rewards/RewardPopup/RewardsProgressionChest");

		public DRewardPopup PopupData
		{
			get
			{
				return popupData;
			}
		}

		private void OnValidate()
		{
		}

		public void Init(DRewardPopup popupData)
		{
			this.popupData = popupData;
			getMascotXPData();
			currentScreenIndex = 0;
			screenData = RewardPopupScreenBuilder.BuildScreens(popupData);
			isChestLanded = false;
			PopupTitle = getPopupTitle();
		}

		public void Start()
		{
			parentCanvas = GetComponentInParent<Canvas>();
			previousCanvasPlaneDistance = parentCanvas.planeDistance;
			parentCanvas.planeDistance = 150f;
			CoroutineRunner.Start(LoadPopup(), this, "RewardPopupController.StartPopup");
			Service.Get<TrayNotificationManager>().DismissAllNotifications();
		}

		private void OnEnable()
		{
			HitBox.OnClick.AddListener(onHitboxClicked);
		}

		private void OnDisable()
		{
			HitBox.OnClick.RemoveListener(onHitboxClicked);
		}

		private IEnumerator LoadPopup()
		{
			disableZoneCamera();
			if (popupData.PopupType != DRewardPopup.RewardPopupType.replay)
			{
				yield return CoroutineRunner.Start(loadChestPrefab(), this, "RewardPopupController.loadChestPrefab");
			}
			yield return CoroutineRunner.Start(showPopupScreen(screenData[currentScreenIndex]), this, "RewardPopupController.showPopupScreen");
			ScreenParentTransform.offsetMax = Vector2.zero;
			ScreenParentTransform.offsetMin = Vector2.zero;
			ScreenParentTransform.anchorMax = new Vector3(1f, 1f);
			ScreenParentTransform.anchorMin = Vector2.zero;
			ScreenParentTransform.localScale = Vector3.one;
		}

		private IEnumerator showPopupScreen(DRewardPopupScreen screenData)
		{
			PrefabContentKey screenContentKey = RewardPopupConstants.ScreenSplashContentKey;
			switch (screenData.ScreenType)
			{
			case DRewardPopupScreen.RewardScreenPopupType.splash:
				screenContentKey = RewardPopupConstants.ScreenSplashContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.splash_levelup:
				screenContentKey = RewardPopupConstants.ScreenLevelUpContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.splash_replay:
				screenContentKey = RewardPopupConstants.ScreenReplayContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.items:
				screenContentKey = RewardPopupConstants.ScreenItemContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.count:
				screenContentKey = RewardPopupConstants.ScreenCountContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.coinsxp:
				screenContentKey = RewardPopupConstants.ScreenCoinsXPContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.quests:
				screenContentKey = RewardPopupConstants.ScreenQuestsContentKey;
				break;
			case DRewardPopupScreen.RewardScreenPopupType.custom:
			{
				DRewardPopupScreenCustom dRewardPopupScreenCustom = screenData as DRewardPopupScreenCustom;
				if (dRewardPopupScreenCustom != null)
				{
					screenContentKey = dRewardPopupScreenCustom.ScreenKey;
				}
				break;
			}
			}
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(screenContentKey);
			yield return assetRequest;
			GameObject screenGO = UnityEngine.Object.Instantiate(assetRequest.Asset);
			screenGO.transform.SetParent(ScreenParentTransform, false);
			screenGO.GetComponent<RectTransform>().localPosition += new Vector3(0f, 0f, -100f);
			currentScreen = screenGO.GetComponent<RewardPopupScreen>();
			currentScreen.ScreenCompleteAction += onCurrentScreenComplete;
			currentScreen.Init(screenData, this);
		}

		private void showNextScreen()
		{
			currentScreenIndex++;
			Service.Get<TrayNotificationManager>().DismissAllNotifications();
			if (currentScreenIndex < screenData.Length)
			{
				CoroutineRunner.Start(showPopupScreen(screenData[currentScreenIndex]), this, "RewardPopupControler.showPopupScreen");
			}
			else
			{
				closeRewardPopup();
			}
		}

		private void closeRewardPopup()
		{
			if (popupData.PopupType == DRewardPopup.RewardPopupType.levelUp)
			{
				playPenguinCelebration();
			}
			parentCanvas.planeDistance = previousCanvasPlaneDistance;
			enableZoneCamera();
			Service.Get<EventDispatcher>().DispatchEvent(new RewardEvents.RewardPopupComplete(popupData));
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void playPenguinCelebration()
		{
			if (SceneRefs.ZoneLocalPlayerManager != null && SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject != null)
			{
				GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				localPlayerGameObject.AddComponent<LevelUpSequence>();
			}
		}

		private void disableZoneCamera()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Chat);
			if (gameObject != null)
			{
				Canvas[] componentsInChildren = gameObject.GetComponentsInChildren<Canvas>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
		}

		private void enableZoneCamera()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Chat);
			if (gameObject != null)
			{
				Canvas[] componentsInChildren = gameObject.GetComponentsInChildren<Canvas>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
			}
		}

		private IEnumerator loadChestPrefab()
		{
			PrefabContentKey chestContentKey = (!string.IsNullOrEmpty(popupData.MascotName) && popupData.PopupType != DRewardPopup.RewardPopupType.levelUp) ? Service.Get<MascotService>().GetMascot(popupData.MascotName).Definition.RewardPopupChestContentKey : defaultChestKey;
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(chestContentKey);
			yield return assetRequest;
			GameObject chestGO = UnityEngine.Object.Instantiate(assetRequest.Asset);
			chestGO.transform.SetParent(ChestParent, false);
			RewardChest = chestGO.GetComponent<RewardPopupChest>();
			RewardPopupChest rewardChest = RewardChest;
			rewardChest.ChestLandAction = (System.Action)Delegate.Combine(rewardChest.ChestLandAction, new System.Action(onChestLand));
			string animationTrigger = "LevelUp";
			if (popupData.PopupType == DRewardPopup.RewardPopupType.questComplete || popupData.PopupType == DRewardPopup.RewardPopupType.generic)
			{
				animationTrigger = "Quest";
			}
			ParachuteAnimator.SetTrigger(animationTrigger);
			RewardChest.ChestAnimator.SetTrigger(animationTrigger);
		}

		private void onChestLand()
		{
			RewardPopupChest rewardChest = RewardChest;
			rewardChest.ChestLandAction = (System.Action)Delegate.Remove(rewardChest.ChestLandAction, new System.Action(onChestLand));
			RewardPopupAnimator.SetTrigger("ChestLands");
			isChestLanded = true;
		}

		private void onHitboxClicked(ButtonClickListener.ClickType clickType)
		{
			if (currentScreen != null)
			{
				currentScreen.OnClick();
			}
		}

		private void onCurrentScreenComplete()
		{
			if (currentScreen != null)
			{
				currentScreen.ScreenCompleteAction -= onCurrentScreenComplete;
			}
			showNextScreen();
		}

		private void getMascotXPData()
		{
			MascotXPReward rewardable;
			if (popupData.RewardData != null && popupData.RewardData.TryGetValue(out rewardable))
			{
				foreach (KeyValuePair<string, int> item in rewardable.XP)
				{
					popupData.MascotName = item.Key;
					popupData.XP = item.Value;
				}
			}
		}

		private string getPopupTitle()
		{
			string result = "";
			if (popupData.PopupType == DRewardPopup.RewardPopupType.levelUp)
			{
				Dictionary<int, ProgressionUnlockDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, ProgressionUnlockDefinition>>();
				int level = Service.Get<ProgressionService>().Level;
				ProgressionUnlockDefinition value;
				if (dictionary.TryGetValue(level, out value) && value.ThemeDefinition != null && !string.IsNullOrEmpty(value.ThemeDefinition.LongThemeToken))
				{
					result = Service.Get<Localizer>().GetTokenTranslation(value.ThemeDefinition.LongThemeToken);
				}
			}
			return result;
		}
	}
}
