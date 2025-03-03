using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SceneSplashScreenController : MonoBehaviour
	{
		public GameObject MainSplashScreen;

		public GameObject SplashScreenOverlay;

		public string[] SceneNames;

		public string[] SplashScreenPaths;

		public readonly PrefabContentKey defaultPrefabKey = new PrefabContentKey("LoadingScreenPrefabs/RoomLoadScreen_Default");

		public readonly PrefabContentKey cellPhonePrefabKey = new PrefabContentKey("LoadingScreenPrefabs/CellPhoneLoadScreen");

		private GameObject currentSplashScreen = null;

		private bool finished = false;

		private void OnDisable()
		{
			if (currentSplashScreen != null)
			{
				UnityEngine.Object.Destroy(currentSplashScreen);
				Resources.UnloadUnusedAssets();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
		}

		public IEnumerator LoadSplashScreen(string sceneName, Dictionary<string, object> sceneArgs)
		{
			if (MainSplashScreen != null)
			{
				UnityEngine.Object.Destroy(MainSplashScreen);
			}
			SplashScreenOverlay.SetActive(true);
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
			PrefabContentKey splashScreenKey = defaultPrefabKey;
			bool showCellPhone = false;
			PrefabContentKey marketingLoadingScreenKey = null;
			if (sceneArgs != null)
			{
				showCellPhone = sceneArgs.ContainsKey(SceneTransitionService.SceneArgs.ShowCellPhoneOnEnterScene.ToString());
				if (sceneArgs.ContainsKey(SceneTransitionService.SceneArgs.ShowAvailableMarketingLoadingScreen.ToString()))
				{
					marketingLoadingScreenKey = getMarketingLoadingScreenKey();
				}
			}
			if (marketingLoadingScreenKey != null)
			{
				splashScreenKey = marketingLoadingScreenKey;
			}
			else if (showCellPhone)
			{
				splashScreenKey = cellPhonePrefabKey;
			}
			else if (sceneArgs != null && sceneArgs.ContainsKey(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString()))
			{
				splashScreenKey = new PrefabContentKey(sceneArgs[SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString()].ToString());
			}
			else
			{
				int num = Array.IndexOf(SceneNames, sceneName);
				if (num != -1 && !string.IsNullOrEmpty(SplashScreenPaths[num]))
				{
					splashScreenKey = new PrefabContentKey(SplashScreenPaths[num]);
				}
			}
			Content.LoadAsync(OnSplashScreenContentLoaded, splashScreenKey);
			while (!finished)
			{
				yield return null;
			}
		}

		private PrefabContentKey getMarketingLoadingScreenKey()
		{
			Dictionary<int, MarketingLoadingScreenDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, MarketingLoadingScreenDefinition>>();
			List<PrefabContentKey> list = new List<PrefabContentKey>();
			foreach (MarketingLoadingScreenDefinition value in dictionary.Values)
			{
				if (value.ScreenPrefabContentKeys != null && value.ScreenPrefabContentKeys.Length > 0)
				{
					ScheduledEventDateDefinition scheduledEventDatedDef = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[value.DateDefinitionKey.Id];
					if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(scheduledEventDatedDef))
					{
						for (int i = 0; i < value.ScreenPrefabContentKeys.Length; i++)
						{
							list.Add(value.ScreenPrefabContentKeys[i]);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				return list[index];
			}
			return null;
		}

		private void OnSplashScreenContentLoaded(string key, GameObject asset)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(asset);
			gameObject.transform.SetParent(base.transform, false);
			currentSplashScreen = gameObject;
			SplashScreenOverlay.SetActive(false);
			Service.Get<EventDispatcher>().DispatchEvent(default(SplashScreenEvents.SplashScreenOpened));
			finished = true;
		}
	}
}
