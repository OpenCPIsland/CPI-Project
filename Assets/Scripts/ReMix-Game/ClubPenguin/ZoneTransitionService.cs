using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class ZoneTransitionService : MonoBehaviour, IJoinRoomByNameErrorHandler, IJoinRoomErrorHandler, IJoinIglooErrorHandler, IBaseNetworkErrorHandler
	{
		private const string DEFAULT_ZONE = "Beach";

		public readonly HashSet<ZoneDefinition> Zones = new HashSet<ZoneDefinition>();

		public readonly HashSet<WorldDefinition> Worlds = new HashSet<WorldDefinition>();

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher dispatcher;

		private string pendingTransitionScene;

		public string CurrentInstanceId;

		public bool IsInIgloo = false;

		private string targetWorldToJoin;

		private string onJoinNotificationTag;

		private bool isCancelled;

		private bool isCancelledSceneTransitionRequested;

		public ZoneDefinition PreviousZone
		{
			get;
			private set;
		}

		public ZoneDefinition CurrentZone
		{
			get;
			private set;
		}

		public PrefabContentKey IglooSplashScreen
		{
			get;
			private set;
		}

		public bool IsTransitioning
		{
			get;
			private set;
		}

		public bool IsConnecting
		{
			get;
			private set;
		}

		public ZoneTransitionEvents.ZoneTransition.States TransitionState
		{
			get;
			private set;
		}

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			PreviousZone = ScriptableObject.CreateInstance<ZoneDefinition>();
			CurrentZone = ScriptableObject.CreateInstance<ZoneDefinition>();
			CurrentInstanceId = "";
			IsTransitioning = false;
			IsConnecting = false;
			isCancelled = false;
			TransitionState = ZoneTransitionEvents.ZoneTransition.States.Done;
			dispatcher.AddListener<SceneTransitionEvents.SetIsTransitioningFlag>(onSetIsTransitioningFlag);
		}

		private bool onSetIsTransitioningFlag(SceneTransitionEvents.SetIsTransitioningFlag evt)
		{
			IsTransitioning = evt.IsTransitioning;
			if (isCancelled)
			{
				checkIfZoneTransitionComplete();
			}
			if (isCancelledSceneTransitionRequested)
			{
				returnToPreviousScene();
			}
			return false;
		}

		public void LoadCurrentZone(string transitionScene, bool clearPreviousZone = false)
		{
			if (clearPreviousZone)
			{
				PreviousZone = ScriptableObject.CreateInstance<ZoneDefinition>();
			}
			if (!string.IsNullOrEmpty(CurrentZone.SceneName))
			{
				if (isSceneTransitionValid(CurrentZone.SceneName))
				{
					sceneTransition(CurrentZone.SceneName, transitionScene);
				}
			}
			else
			{
				LoadZone("Beach");
			}
		}

		public void LoadAsZoneOrScene(string scene, string transitionScene, Dictionary<string, object> sceneArgs = null)
		{
			if (GetZoneBySceneName(scene) != null)
			{
				LoadZone(scene, transitionScene);
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(scene, transitionScene, sceneArgs);
			}
		}

		public void LoadZone(ZoneDefinition zone, string transitionScene = null, string worldName = null)
		{
			if (zone.Type == ZoneDefinition.ZoneType.Igloo)
			{
				ZoneId zoneId = new ZoneId();
				zoneId.name = zone.ZoneName;
				zoneId.instanceId = CurrentInstanceId;
				LoadIgloo(zoneId, Service.Get<Localizer>().Language, SceneStateData.SceneState.Play, transitionScene);
			}
			else
			{
				LoadZone(zone.SceneName, transitionScene, worldName);
			}
		}

		public void LoadZone(string sceneName, string transitionScene = null, string worldName = null, string onJoinNotificationTag = null, Dictionary<string, object> sceneArgs = null)
		{
			if (isSceneTransitionValid(sceneName))
			{
				sceneTransition(sceneName, transitionScene, sceneArgs);
				ConnectToZone(sceneName, worldName, onJoinNotificationTag);
			}
		}

		public void LoadIgloo(ZoneId zoneId, Language language, SceneStateData.SceneState sceneState, string transitionScene = null, string onJoinNotificationTag = null)
		{
			ZoneDefinition zone = GetZone(zoneId.name);
			if (isSceneTransitionValid(zone.SceneName))
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), IglooSplashScreen.Key);
				sceneTransition(zone.SceneName, transitionScene, dictionary);
				connectToIgloo(zoneId, zone, language, onJoinNotificationTag);
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			if (dataEntityHandle.IsNull)
			{
				dataEntityHandle = dataEntityCollection.AddEntity("ActiveSceneData");
			}
			SceneStateData component;
			if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				component = dataEntityCollection.AddComponent<SceneStateData>(dataEntityHandle);
			}
			component.State = sceneState;
		}

		public void LoadZoneOffline(string sceneName, string transitionScene = null, string worldName = null, Dictionary<string, object> sceneArgs = null)
		{
			if (isSceneTransitionValid(sceneName))
			{
				sceneTransition(sceneName, transitionScene, sceneArgs, true);
				TransitionState = ZoneTransitionEvents.ZoneTransition.States.Request;
				dispatcher.DispatchEvent(new ZoneTransitionEvents.ZoneTransition(CurrentZone.SceneName, sceneName, ZoneTransitionEvents.ZoneTransition.States.Request));
				PreviousZone = CurrentZone;
				CurrentZone = GetZoneBySceneName(sceneName);
				CurrentInstanceId = "";
				IsConnecting = false;
				IsInIgloo = false;
			}
		}

		private bool isSceneTransitionValid(string sceneName)
		{
			if (IsTransitioning)
			{
				Log.LogError(this, "Zone transition called while one is already taking place.");
				return false;
			}
			if (GetZoneBySceneName(sceneName) == null)
			{
				Log.LogError(this, "Zone transition called with a sceneName that is not present in the zone definitiona manifest: " + sceneName);
				return false;
			}
			return true;
		}

		private void sceneTransition(string sceneName, string transitionScene, Dictionary<string, object> sceneArgs = null, bool allowSceneActivation = false)
		{
			Time.timeScale = 1f;
			LoadingController loadingController = Service.Get<LoadingController>();
			if (!loadingController.HasLoadingSystem(this))
			{
				loadingController.AddLoadingSystem(this);
			}
			if (string.IsNullOrEmpty(transitionScene))
			{
				transitionScene = "Loading";
			}
			TransitionState = ZoneTransitionEvents.ZoneTransition.States.Begin;
			dispatcher.DispatchEvent(new ZoneTransitionEvents.ZoneTransition(CurrentZone.SceneName, sceneName, ZoneTransitionEvents.ZoneTransition.States.Begin));
			dispatcher.AddListener<SceneTransitionEvents.TransitionComplete>(onSceneLoadComplete);
			Service.Get<SceneTransitionService>().LoadScene(sceneName, transitionScene, sceneArgs, allowSceneActivation);
		}

		public bool ConnectToZone(string sceneName, string worldName = null, string onJoinNotificationTag = null)
		{
			ZoneDefinition zoneBySceneName = GetZoneBySceneName(sceneName);
			if (zoneBySceneName == null)
			{
				Log.LogError(this, "Attempting to join a zone that doesn't exist");
				return false;
			}
			if (!startZoneTransition(sceneName, onJoinNotificationTag))
			{
				return false;
			}
			PresenceData component = dataEntityCollection.GetComponent<PresenceData>(dataEntityCollection.LocalPlayerHandle);
			ContentIdentifier contentIdentifier = generateContentIdentifier();
			if (!string.IsNullOrEmpty(worldName))
			{
				targetWorldToJoin = worldName;
				ZoneId zoneId = new ZoneId();
				zoneId.name = zoneBySceneName.ZoneName;
				Service.Get<INetworkServicesManager>().WorldService.JoinRoomInWorld(new RoomIdentifier(worldName, Service.Get<Localizer>().Language, zoneId, contentIdentifier.ToString()), this);
			}
			else if (component != null && !string.IsNullOrEmpty(component.World))
			{
				targetWorldToJoin = component.World;
				ZoneId zoneId = new ZoneId();
				zoneId.name = zoneBySceneName.ZoneName;
				Service.Get<INetworkServicesManager>().WorldService.JoinRoomInWorld(new RoomIdentifier(component.World, Service.Get<Localizer>().Language, zoneId, contentIdentifier.ToString()), this);
			}
			else
			{
				targetWorldToJoin = null;
				Service.Get<INetworkServicesManager>().WorldService.JoinRoom(zoneBySceneName.ZoneName, contentIdentifier.ToString(), Service.Get<Localizer>().LanguageString, this);
			}
			PreviousZone = CurrentZone;
			CurrentZone = zoneBySceneName;
			CurrentInstanceId = "";
			return IsConnecting;
		}

		private bool connectToIgloo(ZoneId zoneId, ZoneDefinition zoneToJoin, Language language, string onJoinNotificationTag)
		{
			if (!startZoneTransition(zoneToJoin.SceneName, onJoinNotificationTag))
			{
				return false;
			}
			ContentIdentifier contentIdentifier = generateContentIdentifier();
			targetWorldToJoin = null;
			Service.Get<INetworkServicesManager>().WorldService.JoinIgloo(zoneId, LocalizationLanguage.GetLanguageString(language), this);
			PreviousZone = CurrentZone;
			CurrentZone = zoneToJoin;
			CurrentInstanceId = zoneId.instanceId;
			return IsConnecting;
		}

		private bool startZoneTransition(string sceneName, string onJoinNotificationTag)
		{
			if (IsConnecting)
			{
				Log.LogError(this, "Zone connection called while one is already taking place.");
				return false;
			}
			this.onJoinNotificationTag = onJoinNotificationTag;
			if (!IsTransitioning)
			{
				TransitionState = ZoneTransitionEvents.ZoneTransition.States.Begin;
				dispatcher.DispatchEvent(new ZoneTransitionEvents.ZoneTransition(CurrentZone.SceneName, sceneName, ZoneTransitionEvents.ZoneTransition.States.Begin));
			}
			DateTime contentDate = Service.Get<ContentSchedulerService>().RefreshContentDate();
			Service.Get<INetworkServicesManager>().WorldService.SetContentDate(contentDate);
			dispatcher.AddListener<WorldServiceEvents.SelfRoomJoinedEvent>(onSelfJoinedRoom, EventDispatcher.Priority.FIRST);
			TransitionState = ZoneTransitionEvents.ZoneTransition.States.Request;
			dispatcher.DispatchEvent(new ZoneTransitionEvents.ZoneTransition(CurrentZone.SceneName, sceneName, ZoneTransitionEvents.ZoneTransition.States.Request));
			IsConnecting = true;
			return true;
		}

        private ContentIdentifier generateContentIdentifier()
        {
#if UNITY_ANDROID
    string clientVersion = "1"; // EnvironmentManager.BundleVersion.ToString(3);
#else
            string clientVersion = EnvironmentManager.BundleVersion.ToString(3);
#endif

            string setting = hg.ApiWebKit.Configuration.GetSetting<string>("cp-content-version");
            string subContentVersion = hg.ApiWebKit.Configuration.GetSetting<DateTime>("cp-content-version-date").ToString("yyyy-MM-dd");
            string abTestGroup = "NONE";
            // Debug.Log("test");
            return new ContentIdentifier(clientVersion, setting, subContentVersion, abTestGroup);
        }

        private bool onSelfJoinedRoom(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			IsConnecting = false;
			dispatcher.RemoveListener<WorldServiceEvents.SelfRoomJoinedEvent>(onSelfJoinedRoom);
			ZoneDefinition zone = GetZone(evt.Room.zoneId.name);
			if (zone != null && !zone.IsQuestOnly && zone.Type != ZoneDefinition.ZoneType.Igloo)
			{
				Service.Get<GameSettings>().LastZone.SetValue(evt.Room.zoneId.name);
				IsInIgloo = false;
			}
			else if (zone != null)
			{
				IsInIgloo = (zone.Type == ZoneDefinition.ZoneType.Igloo);
			}
			checkIfZoneTransitionComplete();
			checkForLocalizationChange(GetWorld(evt.Room.world));
			if (targetWorldToJoin != null && targetWorldToJoin != evt.Room.world)
			{
				onJoinNotificationTag = "GlobalUI.Notification.WorldBump";
			}
			if (!string.IsNullOrEmpty(onJoinNotificationTag))
			{
				DNotification dNotification = new DNotification();
				dNotification.Message = string.Format(Service.Get<Localizer>().GetTokenTranslation(onJoinNotificationTag), evt.Room.world);
				dNotification.PopUpDelayTime = 6f;
				dNotification.WaitForLoading = true;
				dNotification.PersistBetweenScenes = false;
				Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
				onJoinNotificationTag = string.Empty;
			}
			Service.Get<ICPSwrveService>().EndTimer("igloo");
			if (zone.Type == ZoneDefinition.ZoneType.Igloo)
			{
				startBIVisitIglooTimer(evt);
			}
			return false;
		}

		private void startBIVisitIglooTimer(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			ProfileData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				if (!string.IsNullOrEmpty(component.ZoneId.instanceId) && evt.IsRoomOwner)
				{
					Service.Get<ICPSwrveService>().StartTimer("igloo", "visit", "player");
					return;
				}
				Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FindUserSent>(onFindUserResponse);
				Service.Get<INetworkServicesManager>().FriendsService.FindUser(evt.RoomOwnerName, Service.Get<SessionManager>().LocalUser);
			}
			else
			{
				Service.Get<ICPSwrveService>().StartTimer("igloo", "visit", "unknown");
			}
		}

		private bool onFindUserResponse(FriendsServiceEvents.FindUserSent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FindUserSent>(onFindUserResponse);
			string message = "other";
			if (evt.Success)
			{
				DataEntityHandle entityByType = Service.Get<CPDataEntityCollection>().GetEntityByType<SearchedUserData>();
				FriendStatus friendStatus = FriendsDataModelService.GetFriendStatus(entityByType);
				if (friendStatus == FriendStatus.Friend)
				{
					message = "friend";
				}
			}
			Service.Get<ICPSwrveService>().StartTimer("igloo", "visit", message);
			return false;
		}

		private void checkForLocalizationChange(WorldDefinition currentWorld)
		{
			Localizer localizer = Service.Get<Localizer>();
			if (currentWorld != null && localizer.Language != currentWorld.Language)
			{
				localizer.ChangeLanguage(currentWorld.Language);
			}
		}

		public void CancelTransition(string sceneName = null)
		{
			if (IsTransitioning || IsConnecting)
			{
				dispatcher.RemoveListener<WorldServiceEvents.SelfRoomJoinedEvent>(onSelfJoinedRoom);
				dispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(onSceneLoadComplete);
				Service.Get<SceneTransitionService>().CancelTransition(sceneName);
				TransitionState = ZoneTransitionEvents.ZoneTransition.States.Cancel;
				dispatcher.DispatchEvent(new ZoneTransitionEvents.ZoneTransition(CurrentZone.SceneName, sceneName, ZoneTransitionEvents.ZoneTransition.States.Cancel));
				IsConnecting = false;
				isCancelled = true;
				checkIfZoneTransitionComplete();
			}
		}

		public void HardCancelTransition()
		{
			if (IsTransitioning || IsConnecting)
			{
				dispatcher.RemoveListener<WorldServiceEvents.SelfRoomJoinedEvent>(onSelfJoinedRoom);
				dispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(onSceneLoadComplete);
				Service.Get<SceneTransitionService>().HardCancelTransition();
				TransitionState = ZoneTransitionEvents.ZoneTransition.States.Cancel;
				IsConnecting = false;
				isCancelled = true;
			}
		}

		private bool onSceneLoadComplete(SceneTransitionEvents.TransitionComplete evt)
		{
			dispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(onSceneLoadComplete);
			checkIfZoneTransitionComplete();
			return false;
		}

		private void checkIfZoneTransitionComplete()
		{
			if (!IsTransitioning && !IsConnecting)
			{
				TransitionState = ZoneTransitionEvents.ZoneTransition.States.Done;
				Crittercism.SetValue("previousZone", PreviousZone.SceneName);
				Crittercism.SetValue("currentZone", CurrentZone.SceneName);
				Crittercism.LeaveBreadcrumb(string.Format("Transitioned zone from '{0}' to '{1}'", PreviousZone.SceneName, CurrentZone.SceneName));
				if (CurrentZone.Type == ZoneDefinition.ZoneType.Igloo)
				{
					checkIglooSceneStateDataExists();
				}
				dispatcher.DispatchEvent(new ZoneTransitionEvents.ZoneTransition(PreviousZone.SceneName, CurrentZone.SceneName, ZoneTransitionEvents.ZoneTransition.States.Done));
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				isCancelled = false;
			}
		}

		private void checkIglooSceneStateDataExists()
		{
			DataEntityHandle handle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			SceneStateData component;
			if (!dataEntityCollection.TryGetComponent(handle, out component))
			{
				component = dataEntityCollection.AddComponent<SceneStateData>(handle);
				component.State = SceneStateData.SceneState.Play;
			}
		}

		public void SetIglooSplashScreenKey(PrefabContentKey iglooSplashScreen)
		{
			IglooSplashScreen = iglooSplashScreen;
		}

		public void SetWorldsFromManifest(Manifest manifest)
		{
			Worlds.Clear();
			ScriptableObject[] assets = manifest.Assets;
			for (int i = 0; i < assets.Length; i++)
			{
				WorldDefinition item = (WorldDefinition)assets[i];
				Worlds.Add(item);
			}
		}

		public WorldDefinition GetWorld(string worldName)
		{
			foreach (WorldDefinition world in Worlds)
			{
				if (world.WorldName == worldName)
				{
					return world;
				}
			}
			return null;
		}

		public void SetZonesFromManifest(Manifest manifest)
		{
			Zones.Clear();
			ScriptableObject[] assets = manifest.Assets;
			for (int i = 0; i < assets.Length; i++)
			{
				ZoneDefinition item = (ZoneDefinition)assets[i];
				Zones.Add(item);
			}
			string name = SceneManager.GetActiveScene().name;
			foreach (ZoneDefinition zone in Zones)
			{
				if (zone.SceneName == name)
				{
					CurrentZone = zone;
					break;
				}
			}
		}

		public ZoneDefinition GetZone(string zoneName)
		{
			foreach (ZoneDefinition zone in Zones)
			{
				if (zone.ZoneName == zoneName)
				{
					return zone;
				}
			}
			return null;
		}

		public ZoneDefinition GetZoneBySceneName(string sceneName)
		{
			foreach (ZoneDefinition zone in Zones)
			{
				if (zone.SceneName == sceneName)
				{
					return zone;
				}
			}
			return null;
		}

		public void onRoomFull()
		{
			Service.Get<ICPSwrveService>().Error("network_error", "CPRoomFullError", Service.Get<GameStateController>().CurrentState(), SceneManager.GetActiveScene().name);
			Service.Get<INetworkServicesManager>().WorldService.JoinRoom(CurrentZone.ZoneName, generateContentIdentifier().ToString(), Service.Get<Localizer>().LanguageString, this);
		}

		public void onNoServerFound()
		{
			IsConnecting = false;
			Service.Get<ErrorService>().OnNoServersError();
		}

		public void onRoomJoinError()
		{
			IsConnecting = false;
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinErrorEvent);
		}

		public void onRequestTimeOut()
		{
			IsConnecting = false;
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinRequestTimeOut);
		}

		public void onGeneralNetworkError()
		{
			IsConnecting = false;
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinGeneralNetworkError);
		}

		public void onNoRoomsFound()
		{
			IsConnecting = false;
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinNoRoomsError);
		}

		void IJoinIglooErrorHandler.onRoomFull()
		{
			IsConnecting = false;
			Service.Get<ICPSwrveService>().Error("network_error", "CPRoomFullError", Service.Get<GameStateController>().CurrentState(), SceneManager.GetActiveScene().name);
			isCancelledSceneTransitionRequested = false;
			HardCancelTransition();
			if (!IsTransitioning)
			{
				returnToPreviousScene();
			}
			else
			{
				isCancelledSceneTransitionRequested = true;
			}
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooFullPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooRoomFullPromptLoaded);
			promptLoaderCMD.ExecuteImmediate();
		}

		private void onIglooRoomFullPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, null, promptLoader.Prefab);
		}

		private void returnToPreviousScene()
		{
			isCancelledSceneTransitionRequested = false;
			PresenceData component = dataEntityCollection.GetComponent<PresenceData>(dataEntityCollection.LocalPlayerHandle);
			dataEntityCollection.ClearZoneScope();
			Service.Get<GameStateController>().GoOffline();
			Service.Get<LoadingController>().ClearAllLoadingSystems();
			if (component.IsInInstancedRoom)
			{
				LoadIgloo(component.InstanceRoom, Service.Get<Localizer>().Language, SceneStateData.SceneState.Play, Service.Get<GameStateController>().SceneConfig.TransitionSceneName);
			}
			else
			{
				LoadZone(component.Room, Service.Get<GameStateController>().SceneConfig.TransitionSceneName, component.World);
			}
			Service.Get<GameStateController>().TriggerZoneConnectingEvent();
		}

		public void onRoomChanged()
		{
			IsConnecting = false;
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinGeneralNetworkError);
		}

		public void onIglooNotAvailable()
		{
			IsConnecting = false;
			Service.Get<ICPSwrveService>().Error("network_error", "CPRoomPrivateError", Service.Get<GameStateController>().CurrentState(), SceneManager.GetActiveScene().name);
			isCancelledSceneTransitionRequested = false;
			HardCancelTransition();
			if (!IsTransitioning)
			{
				returnToPreviousScene();
			}
			else
			{
				isCancelledSceneTransitionRequested = true;
			}
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooPrivatePrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooRoomFullPromptLoaded);
			promptLoaderCMD.ExecuteImmediate();
		}
	}
}
