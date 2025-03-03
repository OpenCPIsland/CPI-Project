using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class SceneTransitionService : MonoBehaviour
	{
		public enum SceneArgs
		{
			ReturnTargetScene,
			ShowCellPhoneOnEnterScene,
			LoadingScreenOverride,
			ShowCatalogOnEntry,
			ShowAvailableMarketingLoadingScreen
		}

		private struct SceneAudio
		{
			public string SceneName;

			public string AudioPrefabContentKey;

			public GameObject AudioPrefab;
		}

		private struct AsyncSceneLoadRequest
		{
			public string Name;

			public AsyncOperation Operation;

			public AsyncSceneLoadRequest(string name, AsyncOperation operation)
			{
				Name = name;
				Operation = operation;
			}
		}

		private struct SceneLoadingRequest
		{
			public readonly string SceneName;

			public readonly AssetRequest<Object> Request;

			public SceneLoadingRequest(string sceneName, AssetRequest<Object> request)
			{
				SceneName = sceneName;
				Request = request;
			}
		}

		public const string DEFAULT_TRANSITION_SCENE = "Loading";

		private readonly Dictionary<string, SceneDefinition> sceneDefinitions = new Dictionary<string, SceneDefinition>();

		private EventDispatcher dispatcher;

		private Dictionary<string, object> currentSceneArgs = new Dictionary<string, object>();

		private SceneSplashScreenController splashScreenController;

		private SceneAudio sceneAudio;

		private List<string> sceneAssetKeys;

		public LoadSceneMode LoadingMode = LoadSceneMode.Single;

		private bool isCancelRequested = false;

		private string cancellationScene;

		private ApplicationService appService;

		private string currentScene;

		private LinkedList<AsyncSceneLoadRequest> asyncLoadingScenes = new LinkedList<AsyncSceneLoadRequest>();

		private bool overrideSceneActivation = false;

		[Tweakable("SceneLoader.SceneTransitions.EnableAsyncSceneLoad", Description = "Disable loading scenes asynchronously. This will cause the loading screen to freeze while loading a scene.")]
		private static bool enableAsyncSceneLoad = true;

		[Tweakable("SceneLoader.SceneTransitions.EnableAsyncTranstionLoad", Description = "Disable loading the transition scene asynchronously.")]
		private static bool enableAsyncTranstionLoad = true;

		[Tweakable("SceneLoader.SceneTransitions.transitionSceneDelay", Description = "Number of frames to wait in transition scene before loading target scene.")]
		private static int transitionSceneDelay = 0;

		private bool isHardCancelRequested;

		public bool IsTransitioning
		{
			get;
			private set;
		}

		public string CurrentScene
		{
			get
			{
				return currentScene;
			}
		}

		public bool AllowSceneActivation()
		{
			overrideSceneActivation = true;
			for (LinkedListNode<AsyncSceneLoadRequest> linkedListNode = asyncLoadingScenes.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value.Operation.allowSceneActivation = true;
			}
			return asyncLoadingScenes.Count > 0;
		}

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			appService = Service.Get<ApplicationService>();
			splashScreenController = Service.Get<Canvas>().GetComponentInChildren<SceneSplashScreenController>();
		}

		public void SetScenesFromManifest(Manifest manifest)
		{
			sceneDefinitions.Clear();
			ScriptableObject[] assets = manifest.Assets;
			for (int i = 0; i < assets.Length; i++)
			{
				SceneDefinition sceneDefinition = (SceneDefinition)assets[i];
				sceneDefinitions.Add(sceneDefinition.SceneName, sceneDefinition);
			}
		}

		public object GetSceneArg(string key)
		{
			object value = null;
			currentSceneArgs.TryGetValue(key, out value);
			return value;
		}

		public bool HasSceneArg(string key)
		{
			return currentSceneArgs.ContainsKey(key);
		}

		public void CancelTransition(string scene)
		{
			if (scene == null)
			{
				scene = ((currentScene != null) ? currentScene : Service.Get<GameSceneConfig>().HomeSceneName);
			}
			if (asyncLoadingScenes.Count > 0)
			{
				AllowSceneActivation();
			}
			isCancelRequested = true;
			cancellationScene = scene;
		}

		public void HardCancelTransition()
		{
			AllowSceneActivation();
			isHardCancelRequested = true;
		}

		public void LoadScene(string scene, string transitionScene, Dictionary<string, object> sceneArgs = null, bool allowActivation = true)
		{
			if (IsTransitioning)
			{
				Log.LogError(this, "LoadScene called when a scene transition is currently taking place.");
			}
			else if (!appService.IsAppResuming)
			{
				if (string.IsNullOrEmpty(transitionScene))
				{
					transitionScene = "Loading";
				}
				IsTransitioning = true;
				dispatcher.DispatchEvent(new SceneTransitionEvents.SetIsTransitioningFlag(true));
				CoroutineRunner.StartPersistent(loadScene(scene, transitionScene, sceneArgs, allowActivation), this, "loadScene-" + scene);
			}
		}

		public IEnumerator SwapScene(string scene, Dictionary<string, object> sceneArgs = null)
		{
			HashSet<string> currentSceneParts = new HashSet<string>
			{
				currentScene
			};
			SceneDefinition sceneDefinition;
			if (sceneDefinitions.TryGetValue(currentScene, out sceneDefinition))
			{
				currentSceneParts.UnionWith(getCurrentAdditiveScenes(sceneDefinition));
			}
			HashSet<string> targetSceneParts = new HashSet<string>
			{
				scene
			};
			if (sceneDefinitions.TryGetValue(scene, out sceneDefinition))
			{
				targetSceneParts.UnionWith(getCurrentAdditiveScenes(sceneDefinition));
			}
			HashSet<string> intersection = new HashSet<string>(currentSceneParts);
			intersection.IntersectWith(targetSceneParts);
			if (intersection.Count == 0)
			{
				Log.LogErrorFormatted(this, "No common scene parts between {0} and {1}, scene swapping not supported", currentScene, scene);
				yield return loadScene(scene, "Loading", sceneArgs, true);
				yield break;
			}
			currentSceneParts.ExceptWith(intersection);
			targetSceneParts.ExceptWith(intersection);
			if (currentSceneParts.Count > 0)
			{
				foreach (string unloadScene in currentSceneParts)
				{
					yield return SceneManager.UnloadSceneAsync(unloadScene);
					if (sceneAudio.SceneName == unloadScene)
					{
						unpinCurrentAudioPrefab();
					}
				}
				yield return Resources.UnloadUnusedAssets();
			}
			if (targetSceneParts.Count > 0)
			{
				IsTransitioning = true;
				targetSceneParts.Remove(scene);
				yield return loadTargetScene(scene, targetSceneParts, LoadSceneMode.Additive, true);
				IsTransitioning = false;
			}
			if (sceneAudio.AudioPrefab == null)
			{
				yield return loadSceneAudioPrefab(scene);
			}
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
			currentScene = scene;
		}

		private IEnumerator loadScene(string scene, string transitionScene, Dictionary<string, object> sceneArgs, bool allowActivation)
		{
			stopCurrentRoomTimer();
			startJoinRoomBI(scene);
			yield return splashScreenController.LoadSplashScreen(scene, sceneArgs);
			prepareForSceneTransition(scene, transitionScene, sceneArgs);
			yield return loadTransitionScene(transitionScene);
			yield return cleanupPreviousScene();
			scene = checkTransitionCancelled(scene);
			yield return loadSceneAudioPrefab(scene);
			if (!isCancelRequested)
			{
				yield return loadTargetScene(scene, allowActivation);
			}
			if (isCancelRequested)
			{
				IsTransitioning = false;
				dispatcher.DispatchEvent(new SceneTransitionEvents.SetIsTransitioningFlag(false));
				stopJoinRoomBI(scene, "failure");
				LoadScene(cancellationScene, "Loading");
			}
			else if (isHardCancelRequested)
			{
				IsTransitioning = false;
				dispatcher.DispatchEvent(new SceneTransitionEvents.SetIsTransitioningFlag(false));
				stopJoinRoomBI(scene, "failure");
			}
			else
			{
				Crittercism.SetValue("previousScene", currentScene);
				Crittercism.SetValue("currentScene", scene);
				Crittercism.LeaveBreadcrumb(string.Format("Transitioned scene from '{0}' to '{1}'", currentScene, scene));
				currentScene = scene;
				startLoggingTargetScene();
				stopJoinRoomBI(scene, "success");
			}
		}

		private void stopCurrentRoomTimer()
		{
			Service.Get<ICPSwrveService>().EndTimer("room");
		}

		private void startJoinRoomBI(string scene)
		{
			if (scene != "Home" && scene != "Boot")
			{
				Service.Get<ICPSwrveService>().StartTimer("join_room", "join_room", null, scene);
			}
		}

		private void stopJoinRoomBI(string scene, string result)
		{
			if (scene != "Home" && scene != "Boot")
			{
				Service.Get<ICPSwrveService>().EndTimer("join_room", null, result);
			}
		}

		private void prepareForSceneTransition(string scene, string transitionScene, Dictionary<string, object> sceneArgs)
		{
			dispatcher.DispatchEvent(new SceneTransitionEvents.TransitionStart(scene));
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Accessibility.Loading");
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
			}
			LoadingController loadingController = Service.Get<LoadingController>();
			loadingController.AddLoadingSystem(this);
			if (string.IsNullOrEmpty(transitionScene))
			{
				transitionScene = "Loading";
			}
			CoroutineRunner.StopTransientCoroutines();
			if (sceneArgs != null)
			{
				currentSceneArgs = sceneArgs;
			}
			else
			{
				currentSceneArgs.Clear();
			}
		}

		private IEnumerator loadTransitionScene(string transitionScene)
		{
			if (SceneManager.GetActiveScene().name.Equals(transitionScene))
			{
				yield break;
			}
			if (enableAsyncTranstionLoad)
			{
				yield return SceneManager.LoadSceneAsync(transitionScene, LoadingMode);
			}
			else
			{
				SceneManager.LoadScene(transitionScene, LoadingMode);
			}
			if (transitionSceneDelay > 0)
			{
				int counter = transitionSceneDelay;
				while (counter > 0)
				{
					counter--;
					yield return null;
				}
			}
		}

		private IEnumerator cleanupPreviousScene()
		{
			unpinCurrentAudioPrefab();
			SceneRefs.ClearAll();
			ClubPenguin.Core.SceneRefs.ClearAll();
			if (sceneAssetKeys != null)
			{
				for (int i = 0; i < sceneAssetKeys.Count; i++)
				{
					Content.TryUnpinBundle(sceneAssetKeys[i]);
				}
			}
			Content.BundleManager.UnmountUnusedBundles();
			yield return null;
		}

		private string checkTransitionCancelled(string scene)
		{
			if (isCancelRequested)
			{
				isCancelRequested = false;
				AllowSceneActivation();
				if (cancellationScene != null)
				{
					scene = cancellationScene;
					cancellationScene = null;
				}
			}
			return scene;
		}

		private IEnumerator loadSceneAudioPrefab(string scene)
		{
			SceneDefinition sceneDefinition;
			if (!sceneDefinitions.TryGetValue(scene, out sceneDefinition))
			{
				yield break;
			}
			PrefabContentKey audioContentKey = sceneDefinition.SceneAudioContentKey;
			if (audioContentKey == null || string.IsNullOrEmpty(audioContentKey.Key) || !Content.ContainsKey(audioContentKey))
			{
				yield break;
			}
			AssetRequest<GameObject> audioRequest = Content.LoadAsync(audioContentKey);
			yield return audioRequest;
			if (audioRequest.Asset != null)
			{
				if (sceneAudio.AudioPrefab != null)
				{
					Log.LogErrorFormatted(this, "SceneAudio {0} for scene {1} was not properly cleaned up before loading a new one.", sceneAudio.AudioPrefab.name, sceneAudio.SceneName);
					unpinCurrentAudioPrefab();
				}
				Content.TryPinBundle(audioContentKey.Key);
				sceneAudio = default(SceneAudio);
				sceneAudio.SceneName = scene;
				sceneAudio.AudioPrefabContentKey = audioContentKey.Key;
				sceneAudio.AudioPrefab = Object.Instantiate(audioRequest.Asset);
				Object.DontDestroyOnLoad(sceneAudio.AudioPrefab);
			}
			else
			{
				Log.LogError(this, "Failed to load audio prefab for scene: " + scene);
			}
		}

		private void unpinCurrentAudioPrefab()
		{
			if (sceneAudio.AudioPrefab != null)
			{
				Object.Destroy(sceneAudio.AudioPrefab);
				sceneAudio.AudioPrefab = null;
				Content.TryUnpinBundle(sceneAudio.AudioPrefabContentKey);
				sceneAudio.SceneName = string.Empty;
			}
		}

		private IEnumerator loadTargetScene(string sceneName, bool allowActivation)
		{
			string[] additiveScenes = null;
			SceneDefinition sceneDefinition;
			if (sceneDefinitions.TryGetValue(sceneName, out sceneDefinition))
			{
				additiveScenes = getCurrentAdditiveScenes(sceneDefinition);
			}
			if (additiveScenes == null)
			{
				additiveScenes = new string[0];
			}
			if (!(sceneDefinition == null))
			{
				yield return loadPrerequisiteContent(sceneDefinition);
			}
			yield return loadTargetScene(sceneName, additiveScenes, LoadingMode, allowActivation);
			IsTransitioning = false;
			dispatcher.DispatchEvent(new SceneTransitionEvents.SetIsTransitioningFlag(false));
			dispatcher.DispatchEvent(new SceneTransitionEvents.TransitionComplete(sceneName));
			Service.Get<LoadingController>().RemoveLoadingSystem(this);
		}

		private static string[] getCurrentAdditiveScenes(SceneDefinition sceneDefinition)
		{
			for (int i = 0; i < sceneDefinition.AdditiveSceneOverrides.Length; i++)
			{
				AdditiveSceneOverride additiveSceneOverride = sceneDefinition.AdditiveSceneOverrides[i];
				if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(additiveSceneOverride.DateDefinitionKey))
				{
					return additiveSceneOverride.AdditiveScenes;
				}
				if (!string.IsNullOrEmpty(additiveSceneOverride.PlayerPrefsKey) && DisplayNamePlayerPrefs.HasKey(additiveSceneOverride.PlayerPrefsKey))
				{
					return additiveSceneOverride.AdditiveScenes;
				}
			}
			return sceneDefinition.AdditiveScenes;
		}

		private IEnumerator loadPrerequisiteContent(SceneDefinition sceneDefinition)
		{
			yield return Service.Get<ScenePrereqContentManager>().LoadPrereqBundlesForScene(sceneDefinition.SceneName);
		}

		private IEnumerator loadTargetScene(string sceneName, IEnumerable<string> additiveScenes, LoadSceneMode mainSceneLoadingMode, bool allowActivation)
		{
			int targetFrameRate = -1;
			float fixedDeltaTime = 0f;
			SceneDefinition mainSceneDefinition;
			if (sceneDefinitions.TryGetValue(sceneName, out mainSceneDefinition))
			{
				targetFrameRate = mainSceneDefinition.TargetFrameRate;
				fixedDeltaTime = mainSceneDefinition.FixedDeltaTime;
			}
			if (targetFrameRate == -1)
			{
				targetFrameRate = 30;
			}
			if (Mathf.Approximately(0f, fixedDeltaTime))
			{
				fixedDeltaTime = 0.0333333f;
			}
			Application.targetFrameRate = targetFrameRate;
			Time.fixedDeltaTime = fixedDeltaTime;
			Queue<string> loadableScenes = new Queue<string>();
			List<SceneLoadingRequest> assetRequests = new List<SceneLoadingRequest>();
			AssetRequest<Object> mainSceneAssetRequest = null;
			sceneAssetKeys = new List<string>();
			if (false)
			{
				foreach (string additiveScene2 in additiveScenes)
				{
					loadableScenes.Enqueue(additiveScene2);
				}
			}
			else
			{
				if (Content.ContainsKey(sceneName))
				{
					mainSceneAssetRequest = Content.LoadAsync<Object>(sceneName);
				}
				foreach (string additiveScene3 in additiveScenes)
				{
					if (Content.ContainsKey(additiveScene3))
					{
						assetRequests.Add(new SceneLoadingRequest(additiveScene3, Content.LoadAsync<Object>(additiveScene3)));
					}
					else
					{
						loadableScenes.Enqueue(additiveScene3);
					}
				}
			}
			if (mainSceneAssetRequest != null)
			{
				while (!mainSceneAssetRequest.Finished)
				{
					yield return null;
				}
			}
			loadAvailableScene(sceneName, allowActivation, mainSceneLoadingMode);
			while (assetRequests.Count > 0 || loadableScenes.Count > 0)
			{
				while (loadableScenes.Count > 0)
				{
					string additiveScene = loadableScenes.Dequeue();
					SceneDefinition sceneDefinition;
					if (sceneDefinitions.TryGetValue(additiveScene, out sceneDefinition) && !(sceneDefinition == null))
					{
						yield return loadPrerequisiteContent(sceneDefinition);
					}
					loadAvailableScene(additiveScene, allowActivation, LoadSceneMode.Additive);
				}
				int i;
				for (i = 0; i < assetRequests.Count && !assetRequests[i].Request.Finished; i++)
				{
				}
				if (i < assetRequests.Count)
				{
					SceneLoadingRequest sceneLoadingRequest = assetRequests[i];
					loadableScenes.Enqueue(sceneLoadingRequest.SceneName);
					assetRequests.RemoveAt(i);
				}
				else
				{
					yield return null;
				}
			}
			while (asyncLoadingScenes.Count > 0)
			{
				yield return asyncLoadingScenes.First.Value.Operation;
				Crittercism.LeaveBreadcrumb(string.Format("Finished loading scene '{0}'", asyncLoadingScenes.First.Value.Name));
				asyncLoadingScenes.RemoveFirst();
			}
			overrideSceneActivation = false;
		}

		private void loadAvailableScene(string sceneName, bool allowActivation, LoadSceneMode loadingMode)
		{
			if (Content.ContainsKey(sceneName))
			{
				Content.TryPinBundle(sceneName);
				sceneAssetKeys.Add(sceneName);
			}
			Crittercism.LeaveBreadcrumb(string.Format("Begin load scene '{0}' loadingMode= '{1}'", sceneName, loadingMode));
			if (enableAsyncSceneLoad)
			{
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadingMode);
				asyncOperation.allowSceneActivation = (allowActivation || overrideSceneActivation);
				asyncLoadingScenes.AddLast(new AsyncSceneLoadRequest(sceneName, asyncOperation));
			}
			else
			{
				SceneManager.LoadScene(sceneName, loadingMode);
				Crittercism.LeaveBreadcrumb(string.Format("Finished loading scene '{0}' loadingMode= '{1}'", sceneName, loadingMode));
			}
		}

		private void startLoggingTargetScene()
		{
			Service.Get<ICPSwrveService>().Action("game.room_visit", SceneManager.GetActiveScene().name);
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "00", SceneManager.GetActiveScene().name);
			Service.Get<ICPSwrveService>().StartTimer("room", "room." + SceneManager.GetActiveScene().name);
		}

		private void OnDestroy()
		{
			unpinCurrentAudioPrefab();
			if (sceneAssetKeys != null)
			{
				for (int i = 0; i < sceneAssetKeys.Count; i++)
				{
					Content.TryUnpinBundle(sceneAssetKeys[i]);
				}
			}
			Content.BundleManager.UnmountUnusedBundles();
		}
	}
}
