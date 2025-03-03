using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.Input;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.UI
{
	public class CellPhoneHud : MonoBehaviour
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CellPhoneAutoOpen
		{
		}

		public const string LAST_OPEN_PREFS_KEY = "DailyChallengesLastOpen";

		private const string DEFAULT_BI_PROMPT = "none";

		private const string RING_ANIM_TRIGGER = "Notification";

		private readonly PrefabContentKey animPrefabConentKey = new PrefabContentKey("Prefabs/CellPhoneIntroOutroScreen");

		private readonly PrefabContentKey prefabContentKey = new PrefabContentKey("Prefabs/CellPhoneScreen");

		public GameObject CellPhoneButton;

		public float AutoOpenDelay = 1f;

		private bool isCellPhoneOpenOrOpening = false;

		private bool isShowing = true;

		private bool autoOpened = false;

		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		private GameObject cellPhonePrefab;

		private GameObject cellPhoneAnim;

		private GameObject cellPhoneAnimPrefab;

		private CellPhoneNotificationHandler notificationHandler;

		private void Start()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			eventChannel.AddListener<HudEvents.HideCellPhoneHud>(onHideCellPhoneHud);
			eventChannel.AddListener<HudEvents.ShowCellPhoneHud>(onShowCellPhoneHud);
			eventChannel.AddListener<CellPhoneEvents.CellPhoneClosed>(onCellPhoneClosed);
			notificationHandler = GetComponent<CellPhoneNotificationHandler>();
			if (Service.Get<SceneTransitionService>().HasSceneArg(SceneTransitionService.SceneArgs.ShowCellPhoneOnEnterScene.ToString()))
			{
				CoroutineRunner.Start(showPhone(false), this, "showPhone(false)");
			}
			else
			{
				if (!Service.Get<GameStateController>().IsFTUEComplete || Service.Get<ZoneTransitionService>().IsInIgloo)
				{
					return;
				}
				bool flag = true;
				DateTime dateTime = Service.Get<ContentSchedulerService>().PresentTime();
				if (shouldPreventPhoneFromOpening())
				{
					flag = false;
					PlayerPrefs.SetString("DailyChallengesLastOpen", dateTime.GetTimeInMilliseconds().ToString());
				}
				else if (PlayerPrefs.HasKey("DailyChallengesLastOpen"))
				{
					string @string = PlayerPrefs.GetString("DailyChallengesLastOpen");
					if (!string.IsNullOrEmpty(@string))
					{
						DateTime dateTime2 = Convert.ToInt64(@string).MsToDateTime();
						if (dateTime.Day == dateTime2.Day)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					autoOpened = true;
					if (!Service.Get<LoadingController>().IsLoading)
					{
						CoroutineRunner.Start(playRingAnimationAndShowPhone(), this, "CellPhoneRing");
					}
					else
					{
						eventDispatcher.AddListener<LoadingController.LoadingScreenHiddenEvent>(onLoadingScreenHidden);
					}
					logOpenPhoneBi();
					PlayerPrefs.SetString("DailyChallengesLastOpen", dateTime.GetTimeInMilliseconds().ToString());
				}
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			if (cellPhoneAnim != null)
			{
				cellPhoneAnim.GetComponent<CellPhoneIntroAnimation>().EIntroAnimCompleted -= onIntroAnimComplete;
				cellPhoneAnim.GetComponent<CellPhoneIntroAnimation>().EOutroAnimCompleted -= onOutroAnimComplete;
			}
		}

		private bool onLoadingScreenHidden(LoadingController.LoadingScreenHiddenEvent evt)
		{
			eventDispatcher.RemoveListener<LoadingController.LoadingScreenHiddenEvent>(onLoadingScreenHidden);
			CoroutineRunner.Start(playRingAnimationAndShowPhone(), this, "CellPhoneRing");
			return false;
		}

		private bool onHideCellPhoneHud(HudEvents.HideCellPhoneHud evt)
		{
			hideHud();
			return false;
		}

		private bool onShowCellPhoneHud(HudEvents.ShowCellPhoneHud evt)
		{
			showHud();
			return false;
		}

		private void showHud()
		{
			if (!isShowing)
			{
				base.gameObject.SetActive(true);
				isShowing = true;
			}
		}

		private void hideHud()
		{
			if (isShowing)
			{
				base.gameObject.SetActive(false);
				isShowing = false;
			}
		}

		private IEnumerator playRingAnimationAndShowPhone()
		{
			isCellPhoneOpenOrOpening = true;
			yield return new WaitForSeconds(0.5f);
			CellPhoneButton.GetComponent<Animator>().SetTrigger("Notification");
			yield return new WaitForSeconds(AutoOpenDelay);
			startOpenCellPhone();
		}

		public void OnCellPhoneButtonPressed(ButtonClickListener.ClickType clickType)
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if ((activeQuest == null || activeQuest.Definition.name != "AAC001Q001LeakyShip" || activeQuest.IsObjectiveComplete("Obj0FSM")) && !isCellPhoneOpenOrOpening && SceneManager.GetActiveScene().name != "ClothingDesigner")
			{
				startOpenCellPhone();
			}
		}

		private void startOpenCellPhone()
		{
			if (!Service.Get<UIElementDisablerManager>().IsUIElementDisabled("CellphoneButton"))
			{
				CoroutineRunner.Start(showPhone(true), this, "showPhone(true)");
				isCellPhoneOpenOrOpening = true;
			}
			else
			{
				isCellPhoneOpenOrOpening = false;
			}
		}

		private void onIntroAnimComplete()
		{
			setupCellPhonePrefab();
		}

		private void onOutroAnimComplete()
		{
			hideAnim(true);
			isCellPhoneOpenOrOpening = false;
		}

		private IEnumerator showPhone(bool playTransitionAnimation)
		{
			eventChannel.AddListener<CellPhoneEvents.HideLoadingScreen>(onHideLoadingScreen);
			AssetRequest<GameObject> cellPhoneRequest = null;
			if (cellPhonePrefab == null)
			{
				cellPhoneRequest = Content.LoadAsync(prefabContentKey);
			}
			AssetRequest<GameObject> animRequest = null;
			if (cellPhoneAnimPrefab == null)
			{
				animRequest = Content.LoadAsync(animPrefabConentKey);
			}
			if (cellPhoneRequest != null)
			{
				yield return cellPhoneRequest;
				cellPhonePrefab = cellPhoneRequest.Asset;
			}
			if (animRequest != null)
			{
				yield return animRequest;
				cellPhoneAnimPrefab = animRequest.Asset;
			}
			if (playTransitionAnimation)
			{
				setupAnimPrefab();
			}
			else
			{
				setupCellPhonePrefab();
			}
			logOpenPhoneBi();
		}

		private void setupAnimPrefab()
		{
			if (!Service.Get<UIElementDisablerManager>().IsUIElementDisabled("CellphoneButton"))
			{
				if (autoOpened)
				{
					eventDispatcher.DispatchEvent(default(CellPhoneAutoOpen));
					autoOpened = false;
				}
				cellPhoneAnim = UnityEngine.Object.Instantiate(cellPhoneAnimPrefab);
				cellPhoneAnim.GetComponent<CellPhoneIntroAnimation>().EIntroAnimCompleted += onIntroAnimComplete;
				cellPhoneAnim.GetComponent<CellPhoneIntroAnimation>().EOutroAnimCompleted += onOutroAnimComplete;
				SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(cellPhoneAnim, false, true, ""));
			}
			else
			{
				isCellPhoneOpenOrOpening = false;
			}
		}

		private void setupCellPhonePrefab()
		{
			if (!Service.Get<UIElementDisablerManager>().IsUIElementDisabled("CellphoneButton"))
			{
				GameObject popup = UnityEngine.Object.Instantiate(cellPhonePrefab);
				PopupEvents.ShowTopPopup evt = new PopupEvents.ShowTopPopup(popup, false, true, "Accessibility.Popup.Title.CellPhone");
				Service.Get<EventDispatcher>().DispatchEvent(evt);
			}
			else
			{
				hideAnim(true);
				isCellPhoneOpenOrOpening = false;
			}
		}

		private bool onCellPhoneClosed(CellPhoneEvents.CellPhoneClosed evt)
		{
			if (isCellPhoneOpenOrOpening)
			{
				if (cellPhoneAnim != null)
				{
					isCellPhoneOpenOrOpening = true;
					cellPhoneAnim.SetActive(true);
					cellPhoneAnim.GetComponent<Animator>().SetTrigger("Outro");
					cellPhoneAnim.GetComponent<CellPhoneIntroAnimation>().EOutroAnimCompleted += onOutroAnimComplete;
				}
				else
				{
					isCellPhoneOpenOrOpening = true;
					cellPhoneAnim = UnityEngine.Object.Instantiate(cellPhoneAnimPrefab);
					Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(cellPhoneAnim, false, true, ""));
					cellPhoneAnim.GetComponent<CellPhoneIntroAnimation>().EOutroAnimCompleted += onOutroAnimComplete;
					cellPhoneAnim.GetComponent<Animator>().SetTrigger("Outro");
				}
			}
			return false;
		}

		private void hideAnim(bool destroy)
		{
			if (destroy)
			{
				UnityEngine.Object.Destroy(cellPhoneAnim);
			}
			else
			{
				cellPhoneAnim.gameObject.SetActive(false);
			}
		}

		private bool onHideLoadingScreen(CellPhoneEvents.HideLoadingScreen evt)
		{
			eventChannel.RemoveListener<CellPhoneEvents.HideLoadingScreen>(onHideLoadingScreen);
			if (cellPhoneAnim != null && cellPhoneAnim.activeSelf)
			{
				hideAnim(false);
			}
			isCellPhoneOpenOrOpening = false;
			return false;
		}

		private void logOpenPhoneBi()
		{
			string tier = "none";
			if (notificationHandler.NotifcationData != null)
			{
				tier = "DailyChallengeUpdate";
			}
			Service.Get<ICPSwrveService>().Action("flipper_phone", "open", tier);
		}

		private bool shouldPreventPhoneFromOpening()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle entityByType = cPDataEntityCollection.GetEntityByType<AllAccessCelebrationData>();
			if (!entityByType.IsNull)
			{
				AllAccessCelebrationData component = cPDataEntityCollection.GetComponent<AllAccessCelebrationData>(entityByType);
				if (component.ShowAllAccessCelebration)
				{
					return true;
				}
			}
			return false;
		}
	}
}
