using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using ClubPenguin.Video;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class GameStateController : MonoBehaviour
	{
		private const string CONTINUE_FTUE_LAYOUT_ID = "OneIdFullscreen";

		private PrefabContentKey accountRootKey = new PrefabContentKey("TreeNodes/AccountRootNodePrefab");

		[Header("Game State Machine Events")]
		public string EnterGameEvent;

		public string StartFtueEvent;

		public string ContinueFtueEvent;

		public string MixLoginEvent;

		public string StartGameEvent;

		public string ZoneConnectingEvent;

		public string ZoneConnectedEvent;

		public string SceneWithoutZoneEvent;

		public string PausedEvent;

		public string OfflineEvent;

		public string DefaultEvent;

		[Header("Account State Machine Events")]
		public string LoadLoginEvent;

		public string LoginSuccessEvent;

		public string LoadCreateEvent;

		public string LoadMembershipEvent;

		public string ContinueFromFTUEEvent;

		[Header("Scene / Zone Config")]
		public GameSceneConfig SceneConfig;

		[Header("FTUE Config")]
		public FTUEConfig FTUEConfig;

		private StateMachine gameStateMachine;

		private AssetRequest<GameObject> accountSystemRequest;

		private bool accountSystemFromFTUE;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher eventDispatcher;

		private NetworkController networkController;

		private string accountSystemStartEvent;

		private GameObject introVideoSplashScreen;

		private int activeAccountSystemsCount = 0;

		private bool membershipOpen = false;

		public bool DoFTUECheckOnZoneChange
		{
			get;
			set;
		}

		public string LoginZone
		{
			get;
			set;
		}

		public bool IsAccountSystenActive
		{
			get
			{
				return activeAccountSystemsCount > 0;
			}
		}

		public bool IsOnFtueIntro
		{
			get
			{
				ProfileData component;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
				{
					return component.IsFirstTimePlayer;
				}
				return false;
			}
		}

		public bool IsFTUEComplete
		{
			get
			{
				Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
				if (activeQuest != null && activeQuest.Id == Service.Get<GameStateController>().FTUEConfig.FtueQuestId && Service.Get<INetworkServicesManager>().QuestService.PendingSetStatusStatus == QuestStatus.COMPLETED)
				{
					return true;
				}
				ProfileData component;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
				{
					return component.IsFTUEComplete;
				}
				return false;
			}
		}

		public event System.Action OnAccountAccountSystemDeacitvated;

		private IEnumerator Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventDispatcher = Service.Get<EventDispatcher>();
			networkController = Service.Get<NetworkController>();
			eventDispatcher.AddListener<AccountSystemEvents.AccountSystemCreated>(onAccountSystemCreated);
			eventDispatcher.AddListener<AccountSystemEvents.AccountSystemDestroyed>(onAccountSystemDestroyed);
			while (GetComponent<StateMachine>() == null)
			{
				yield return null;
			}
			gameStateMachine = GetComponent<StateMachine>();
			if (gameStateMachine == null)
			{
				throw new MissingReferenceException("No game state machine is set");
			}
		}

		private void OnDestroy()
		{
			if (eventDispatcher != null)
			{
				eventDispatcher.RemoveListener<PlayerStateServiceEvents.OtherPlayerDataReceived>(onOwnerPlayerDataReceived);
				eventDispatcher.RemoveListener<AccountSystemEvents.AccountSystemCreated>(onAccountSystemCreated);
				eventDispatcher.RemoveListener<AccountSystemEvents.AccountSystemDestroyed>(onAccountSystemDestroyed);
			}
			this.OnAccountAccountSystemDeacitvated = null;
		}

		public string CurrentState()
		{
			return gameStateMachine.CurrentState.Name;
		}

		public void PlayIntroVideo()
		{
			Service.Get<ICPSwrveService>().Action("intro_video_replay", "start");
			ClubPenguin.Video.Video.PlayFullScreenVideo("IntroVideo.mp4");
			Service.Get<SceneTransitionService>().LoadScene("Home", "Loading");
		}

		public string GetZoneToLoad()
		{
			if (IsOnFtueIntro)
			{
				return FTUEConfig.FtueSceneName;
			}
			GameSettings gameSettings = Service.Get<GameSettings>();
			string result;
			if (string.IsNullOrEmpty(LoginZone))
			{
				result = ((string.IsNullOrEmpty(gameSettings.LastZone.Value) || !(Service.Get<ZoneTransitionService>().GetZoneBySceneName(gameSettings.LastZone.Value) != null)) ? SceneConfig.DefaultZoneName : gameSettings.LastZone.Value);
			}
			else
			{
				result = LoginZone;
				LoginZone = "";
			}
			return result;
		}

		public void ReturnToZoneScene(Dictionary<string, object> sceneArgs = null)
		{
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			if (zoneTransitionService.IsInIgloo)
			{
				DataEntityHandle activeHandle = Service.Get<SceneLayoutDataManager>().GetActiveHandle();
				SceneOwnerData component;
				if (dataEntityCollection.TryGetComponent(activeHandle, out component))
				{
					if (component.IsOwner)
					{
						ProfileData component2;
						if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2))
						{
							Service.Get<ZoneTransitionService>().LoadIgloo(component2.ZoneId, Service.Get<Localizer>().Language, SceneStateData.SceneState.Play);
						}
						else
						{
							returnToZoneSceneError("Could not find local owner ProfileData. Did not join igloo after returning from SceneWithoutZone state.");
						}
					}
					else
					{
						eventDispatcher.AddListener<PlayerStateServiceEvents.OtherPlayerDataReceived>(onOwnerPlayerDataReceived);
						Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataByDisplayName(component.Name);
					}
				}
				else
				{
					returnToZoneSceneError("Could not find igloo OwnerData. Did not join igloo after returning from SceneWithoutZone state.");
				}
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(zoneTransitionService.CurrentZone.SceneName, SceneConfig.TransitionSceneName, sceneArgs);
			}
		}

		private bool onOwnerPlayerDataReceived(PlayerStateServiceEvents.OtherPlayerDataReceived evt)
		{
			eventDispatcher.RemoveListener<PlayerStateServiceEvents.OtherPlayerDataReceived>(onOwnerPlayerDataReceived);
			Service.Get<ZoneTransitionService>().LoadIgloo(evt.Data.zoneId, Service.Get<Localizer>().Language, SceneStateData.SceneState.Play);
			return false;
		}

		private void returnToZoneSceneError(string message)
		{
			Log.LogErrorFormatted(this, message);
			Service.Get<ZoneTransitionService>().LoadZone(Service.Get<GameSettings>().LastZone.Value);
		}

		public void ChangeServer(string worldName)
		{
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			PresenceData component;
			string sceneName = (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component)) ? GetZoneToLoad() : component.Room;
			Service.Get<LoadingController>().ClearAllLoadingSystems();
			zoneTransitionService.LoadZone(sceneName, SceneConfig.TransitionSceneName, worldName);
			gameStateMachine.SendEvent(ZoneConnectingEvent);
		}

		public void TriggerZoneConnectingEvent()
		{
			gameStateMachine.SendEvent(ZoneConnectingEvent);
		}

		public void ReturnToHome()
		{
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			if (zoneTransitionService.IsTransitioning || zoneTransitionService.IsConnecting)
			{
				Service.Get<ZoneTransitionService>().CancelTransition(SceneConfig.HomeSceneName);
			}
			else
			{
				zoneTransitionService.LoadAsZoneOrScene(SceneConfig.HomeSceneName, SceneConfig.TransitionSceneName);
			}
			gameStateMachine.SendEvent(DefaultEvent);
			networkController.LeaveRoom();
		}

		public void ReconnectFromHome()
		{
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			string text = zoneTransitionService.CurrentZone.SceneName;
			if (text == null)
			{
				text = GetZoneToLoad();
			}
			zoneTransitionService.LoadZone(text);
		}

		public void ExitAfterBan()
		{
			SessionManager sessionManager = Service.Get<SessionManager>();
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			dataEntityCollection.ClearZoneScope();
			sessionManager.DisposeSession();
			GoOffline();
			if (zoneTransitionService.IsTransitioning || zoneTransitionService.IsConnecting)
			{
				Service.Get<ZoneTransitionService>().CancelTransition(SceneConfig.HomeSceneName);
				gameStateMachine.SendEvent(DefaultEvent);
			}
			else
			{
				Service.Get<LoadingController>().ClearAllLoadingSystems();
				ReturnToHome();
			}
		}

		public void ExitWorld(bool logout = false)
		{
			SessionManager sessionManager = Service.Get<SessionManager>();
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			if (sessionManager.HasSession)
			{
				if (logout)
				{
					try
					{
						sessionManager.Logout();
					}
					catch (ObjectDisposedException)
					{
					}
				}
				else
				{
					dataEntityCollection.ClearZoneScope();
					sessionManager.DisposeSession();
				}
			}
			else
			{
				dataEntityCollection.ClearZoneScope();
			}
			if (zoneTransitionService.IsTransitioning || zoneTransitionService.IsConnecting)
			{
				Service.Get<ZoneTransitionService>().CancelTransition(SceneConfig.HomeSceneName);
				GoOffline();
				gameStateMachine.SendEvent(DefaultEvent);
			}
			else
			{
				GoOffline();
				Service.Get<LoadingController>().ClearAllLoadingSystems();
				ReturnToHome();
			}
		}

		public void RetryConnection(string roomName, string worldName = null)
		{
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			zoneTransitionService.LoadZone(zoneTransitionService.CurrentZone, SceneConfig.TransitionSceneName);
		}

		public void EnterGame()
		{
			gameStateMachine.SendEvent(EnterGameEvent);
		}

		public void EnterGameStandalone()
		{
			gameStateMachine.SendEvent(MixLoginEvent);
		}

		public void StartFTUE()
		{
			gameStateMachine.SendEvent(StartFtueEvent);
		}

		public void ContinueFTUE()
		{
			gameStateMachine.SendEvent(ContinueFtueEvent);
		}

		public void ResetStateMachine()
		{
			gameStateMachine.SendEvent(DefaultEvent);
		}

		public void StartGame()
		{
			gameStateMachine.SendEvent(StartGameEvent);
			logAccessibilityActions();
		}

		public void GoOffline()
		{
			gameStateMachine.SendEvent(OfflineEvent);
		}

		public void ShowAccountSystem(string AccountSystemEvent)
		{
			initializeAccountSystem(AccountSystemEvent);
		}

		[Invokable("UI.AccountSystem.ShowAccountSystemFromFTUE")]
		public void ShowAccountSystemFromFTUE()
		{
			initializeAccountSystem(ContinueFromFTUEEvent, true);
		}

		public void ShowAccountSystemCreate()
		{
			initializeAccountSystem(LoadCreateEvent);
		}

		public void ShowAccountSystemLogin()
		{
			initializeAccountSystem(LoadLoginEvent);
		}

		public void ShowAccountSystemMembership(string trigger = null)
		{
			if (!membershipOpen)
			{
				membershipOpen = true;
				if (trigger != null)
				{
					string currentMembershipStatus = Service.Get<MembershipService>().GetCurrentMembershipStatus();
					Service.Get<ICPSwrveService>().Action("game.free_trial", trigger, currentMembershipStatus, SceneManager.GetActiveScene().name);
				}
				initializeAccountSystem(LoadMembershipEvent);
			}
		}

		public void UpdateDisplayName()
		{
			initializeAccountSystem(LoadMembershipEvent);
		}

		private void initializeAccountSystem(string startEvent, bool fromFTUE = false)
		{
			accountSystemStartEvent = startEvent;
			accountSystemFromFTUE = fromFTUE;
			if (accountSystemRequest != null && accountSystemRequest.Asset != null)
			{
				accountSystemRequest.Asset.GetComponent<SEDFSMStartEventSource>().StartEvent = startEvent;
			}
			else
			{
				accountSystemRequest = Content.LoadAsync(OnContentLoaded, accountRootKey);
			}
		}

		private void OnContentLoaded(string key, GameObject asset)
		{
			if (!base.gameObject.IsDestroyed())
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(asset);
				gameObject.GetComponent<SEDFSMStartEventSource>().StartEvent = accountSystemStartEvent;
				if (accountSystemFromFTUE)
				{
					LayoutMappings component = gameObject.GetComponent<LayoutMappings>();
					component.SetLayoutType("OneIdFullscreen", "center");
				}
				string currentScene = Service.Get<SceneTransitionService>().CurrentScene;
				if (currentScene == "Settings" && ClubPenguin.Core.SceneRefs.IsSet<CameraSpacePopupManager>())
				{
					eventDispatcher.DispatchEvent(new PopupEvents.ShowCameraSpacePopup(gameObject));
				}
				else if ((bool)GameObject.Find("TopCanvas"))
				{
					eventDispatcher.DispatchEvent(new PopupEvents.ShowTopPopup(gameObject));
				}
				else
				{
					eventDispatcher.DispatchEvent(new PopupEvents.ShowPopup(gameObject));
				}
			}
			accountSystemRequest = null;
		}

		private bool onAccountSystemCreated(AccountSystemEvents.AccountSystemCreated evt)
		{
			activeAccountSystemsCount++;
			return false;
		}

		private bool onAccountSystemDestroyed(AccountSystemEvents.AccountSystemDestroyed evt)
		{
			activeAccountSystemsCount--;
			membershipOpen = false;
			if (activeAccountSystemsCount <= 0)
			{
				activeAccountSystemsCount = 0;
				this.OnAccountAccountSystemDeacitvated.InvokeSafe();
			}
			return false;
		}

		public void SetFTUEComplete()
		{
			ProfileData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.IsFTUEComplete = true;
			}
			else
			{
				Log.LogError(this, "Unable to obtain player data - dataEntityCollection.TryGetComponent<ProfileData>(dataEntityCollection.LocalPlayerHandle, out profileData)");
			}
		}

		private void logAccessibilityActions()
		{
			string text = "";
			float @float = PlayerPrefs.GetFloat("accessibility_scale");
			if (@float >= 1.2f)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "_";
				}
				text += "LargeText";
			}
			if (MonoSingleton<NativeAccessibilityManager>.Instance.Native.IsSwitchControlEnabled())
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "_";
				}
				text += "SwitchControl";
			}
			if (MonoSingleton<NativeAccessibilityManager>.Instance.Native.IsVoiceOverEnabled())
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "_";
				}
				text += "VoiceOver";
			}
			if (MonoSingleton<NativeAccessibilityManager>.Instance.Native.IsDisplayZoomEnabled())
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += "_";
				}
				text += "DisplayZoom";
			}
			if (!string.IsNullOrEmpty(text))
			{
				Service.Get<ICPSwrveService>().Action("accessibility_login_detected", text);
			}
		}
	}
}
