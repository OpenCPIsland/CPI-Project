using ClubPenguin.Cinematography;
using ClubPenguin.Configuration;
using ClubPenguin.Core;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneController : MonoBehaviour
	{
		public static readonly object LoadingSystemObject = new object();

		private static PrefabContentKey homeScreenContentKey = new PrefabContentKey("Prefabs/CellPhoneActivityScreen/CellPhoneActivityScreen");

		public GameObject ScreenContentPanel;

		public GameObject LoadingScreen;

		public float MinLoadScreenTime = 1f;

		private EventDispatcher dispatcher;

		private EventChannel eventChannel;

		private GameObject currentScreen;

		private bool minLoadingTimeComplete;

		private bool pendingHideLoadingScreen;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(dispatcher);
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.CellPhone, true));
			eventChannel.AddListener<CellPhoneEvents.ChangeCellPhoneScreen>(onChangeScreen);
			eventChannel.AddListener<CellPhoneEvents.CellPhoneClosed>(onCellPhoneClosed);
			eventChannel.AddListener<CellPhoneEvents.ReturnToHomeScreen>(onReturnToHome);
			eventChannel.AddListener<CellPhoneEvents.HideLoadingScreen>(onHideLoadingScreen);
			if (PlatformUtils.GetPlatformType() == PlatformType.Mobile)
			{
				ClubPenguin.Core.SceneRefs.Get<CameraRenderingControl>().DisableRendering(true, false);
			}
			loadScreen(homeScreenContentKey, false);
			dispatcher.DispatchEvent(default(RewardEvents.SuppressLevelUpPopup));
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			dispatcher.DispatchEvent(default(RewardEvents.UnsuppressLevelUpPopup));
		}

		private bool onCellPhoneClosed(CellPhoneEvents.CellPhoneClosed evt)
		{
			Object.Destroy(base.gameObject);
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Here));
			if (PlatformUtils.GetPlatformType() == PlatformType.Mobile)
			{
				ClubPenguin.Core.SceneRefs.Get<CameraRenderingControl>().EnableRendering();
			}
			return false;
		}

		private bool onReturnToHome(CellPhoneEvents.ReturnToHomeScreen evt)
		{
			loadScreen(homeScreenContentKey);
			return false;
		}

		private bool onHideLoadingScreen(CellPhoneEvents.HideLoadingScreen evt)
		{
			if (LoadingScreen.activeSelf)
			{
				if (minLoadingTimeComplete)
				{
					LoadingScreen.SetActive(false);
				}
				else
				{
					pendingHideLoadingScreen = true;
				}
			}
			return false;
		}

		private bool onChangeScreen(CellPhoneEvents.ChangeCellPhoneScreen evt)
		{
			switch (evt.Behaviour)
			{
			case CellPhoneAppBehaviour.LoadScene:
				if (string.IsNullOrEmpty(evt.BehaviourParam))
				{
					Log.LogError(this, "Cell phone load scene param not set for LoadScene");
				}
				else
				{
					loadScene(evt, evt.BehaviourParam, null);
				}
				break;
			case CellPhoneAppBehaviour.ChangeScreen:
			{
				if (string.IsNullOrEmpty(evt.BehaviourParam))
				{
					Log.LogError(this, "Cell phone change screen param not set for ChangeScreen");
					break;
				}
				PrefabContentKey prefabKey = new PrefabContentKey(evt.BehaviourParam);
				loadApplet(evt, prefabKey);
				break;
			}
			case CellPhoneAppBehaviour.LoadSettings:
			{
				ConditionalConfiguration conditionalConfiguration = Service.Get<ConditionalConfiguration>();
				int num = conditionalConfiguration.Get("System.Memory.property", 0);
				if (PlatformUtils.GetAspectRatioType() == AspectRatioType.Portrait || (evt.AppletSceneSystemMemoryThreshold >= 0 && num <= evt.AppletSceneSystemMemoryThreshold))
				{
					loadScene(evt, "Settings", null);
					break;
				}
				PrefabContentKey prefabKey = new PrefabContentKey(evt.BehaviourParam);
				loadScreen(prefabKey);
				break;
			}
			}
			return false;
		}

		private void loadApplet(CellPhoneEvents.ChangeCellPhoneScreen evt, PrefabContentKey prefabKey)
		{
			ConditionalConfiguration conditionalConfiguration = Service.Get<ConditionalConfiguration>();
			int num = conditionalConfiguration.Get("System.Memory.property", 0);
			if (evt.AppletSceneSystemMemoryThreshold >= 0 && num <= evt.AppletSceneSystemMemoryThreshold)
			{
				loadAppletScene(evt, prefabKey);
				return;
			}
			loadScreen(prefabKey, false);
			CoroutineRunner.Start(runLoadingScreenTimer(), this, "CellPhoneLoadingScreenTimer");
		}

		private void loadScene(CellPhoneEvents.ChangeCellPhoneScreen evt, string scene, Dictionary<string, object> args)
		{
			if (args == null)
			{
				args = new Dictionary<string, object>();
			}
			args.Add(SceneTransitionService.SceneArgs.ShowCellPhoneOnEnterScene.ToString(), true);
			if (!string.IsNullOrEmpty(evt.LoadingScreenOverride))
			{
				args.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), evt.LoadingScreenOverride);
			}
			Service.Get<SceneTransitionService>().LoadScene(scene, "Loading", args);
		}

		private void loadAppletScene(CellPhoneEvents.ChangeCellPhoneScreen evt, PrefabContentKey prefabKey)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("appletToLaunch", prefabKey);
			Dictionary<string, object> args = dictionary;
			loadScene(evt, "CellPhoneApplet", args);
			Service.Get<LoadingController>().AddLoadingSystem(LoadingSystemObject);
		}

		private void loadScreen(PrefabContentKey prefabKey, bool showLoadingScreen = true)
		{
			if (currentScreen != null)
			{
				Object.Destroy(currentScreen);
			}
			if (showLoadingScreen)
			{
				LoadingScreen.SetActive(true);
				minLoadingTimeComplete = false;
				CoroutineRunner.Start(runLoadingScreenTimer(), this, "CellPhoneLoadingScreenTimer");
			}
			Content.LoadAsync(onScreenLoaded, prefabKey);
		}

		private void onScreenLoaded(string path, GameObject screenPrefab)
		{
			currentScreen = Object.Instantiate(screenPrefab, ScreenContentPanel.transform, false);
			if (Service.Get<LoadingController>().HasLoadingSystem(LoadingSystemObject))
			{
				Service.Get<LoadingController>().RemoveLoadingSystem(LoadingSystemObject);
			}
		}

		private IEnumerator runLoadingScreenTimer()
		{
			yield return new WaitForSeconds(MinLoadScreenTime);
			if (pendingHideLoadingScreen)
			{
				LoadingScreen.SetActive(false);
				pendingHideLoadingScreen = false;
			}
			else
			{
				minLoadingTimeComplete = true;
			}
		}
	}
}
