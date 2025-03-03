using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Commerce;
using ClubPenguin.Core;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Participation;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.Environment;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class NetworkController : IBaseNetworkErrorHandler
	{
		private GameSceneConfig sceneConfig;

		private FTUEConfig ftueConfig;

		private Environment developmentEnvironment = Environment.PRODUCTION;

		protected EventDispatcher eventDispatcher;

		private INetworkServicesManager networkServicesManager;

		private SessionManager sessionManager;

		private LoginController loginController;

		private readonly CPDataEntityCollection dataEntityCollection;

		private bool isFirstCoinsReceived = true;

		private int mixCriticalAlertLevel;

		private static string[] remotePlayerComponentsScope = new string[3]
		{
			CPDataScopes.Quest.ToString(),
			CPDataScopes.Zone.ToString(),
			CPDataScopes.Scene.ToString()
		};

		public NetworkController(MonoBehaviour ctx, bool offlineMode)
		{
			loginController = Service.Get<LoginController>();
			NetworkServicesConfig networkServicesConfig = GenerateNetworkServiceConfig(developmentEnvironment);
			Service.Set((INetworkServicesManager)new NetworkServicesManager(ctx, networkServicesConfig, offlineMode));
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			mixCriticalAlertLevel = networkServicesConfig.MixCritialAlertLevel;
			eventDispatcher = Service.Get<EventDispatcher>();
			networkServicesManager = Service.Get<INetworkServicesManager>();
			sessionManager = Service.Get<SessionManager>();
			loginController.SetNetworkConfig(networkServicesConfig);
			Service.Get<MixLoginCreateService>().SetNetworkConfig(networkServicesConfig);
			dataEntityCollection.ResetLocalPlayerHandle();
			addListeners();
		}

		private void addListeners()
		{
			eventDispatcher.AddListener<SessionEvents.SessionStartedEvent>(onSessionStarted, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			eventDispatcher.AddListener<PlayerStateServiceEvents.PlayerOutfitChanged>(OnPlayerOutfit);
			eventDispatcher.AddListener<PlayerStateServiceEvents.PlayerProfileChanged>(OnPlayerProfile);
			eventDispatcher.AddListener<PlayerStateServiceEvents.TubeSelected>(OnPlayerTubeSelected);
			eventDispatcher.AddListener<WorldServiceEvents.PlayerJoinRoomEvent>(OnPlayerJoins, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<WorldServiceEvents.PlayerLeaveRoomEvent>(OnPlayerLeaves, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerActionServiceEvents.LocomotionActionReceived>(OnPlayerMoved, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.PlayerLocoStateChanged>(OnPlayerLocoStateChanged, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.HeldObjectDequipped>(onHeldObjectDequipped, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.DispensableEquipped>(onDispenableEquipped, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.DurableEquipped>(onDurableEquipped, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.ConsumableEquipped>(onConsumableEquipped, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.PartyGameEquipped>(onPartyGameEquipped, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.AirBubbleChanged>(OnPlayerAirBubbleChanged, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.AwayFromKeyboardStateChanged>(OnAwayFromKeyboardStateChanged, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.TemporaryHeadStatusChanged>(OnTemporaryHeadStatusChanged, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<PlayerStateServiceEvents.PlayerMembershipStatusChanged>(onPlayerMembershipStatusChanged, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<WorldServiceEvents.ItemSpawned>(OnItemCreated, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<WorldServiceEvents.ItemChanged>(OnItemUpdated, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<WorldServiceEvents.ItemDestroyed>(OnItemDestroyed, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<WorldServiceEvents.ItemMoved>(OnItemMoved, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<WorldServiceEvents.SelfLeaveRoomEvent>(onSelfLeaveRoom);
			eventDispatcher.AddListener<WorldServiceEvents.SelfRoomJoinedEvent>(onRoomJoined, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<SessionEvents.AccessTokenUpdatedEvent>(onAccessTokenUpdate);
			eventDispatcher.AddListener<RewardServiceEvents.RoomRewardsReceived>(onRoomRewardsReceived);
			eventDispatcher.AddListener<RewardServiceEvents.MyAssetsReceived>(onMyRewardAssetsReceived);
			eventDispatcher.AddListener<ConsumableServiceEvents.InventoryRecieved>(onConsumableInventoryReceived);
			eventDispatcher.AddListener<PlayerStateServiceEvents.LocalPlayerDataReceived>(onLocalPlayerDataReceived, EventDispatcher.Priority.FIRST);
		}

		private bool OnTemporaryHeadStatusChanged(PlayerStateServiceEvents.TemporaryHeadStatusChanged evt)
		{
			if (isLocalPlayer(evt.SessionId))
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull)
			{
				PresenceData component = dataEntityCollection.GetComponent<PresenceData>(dataEntityHandle);
				if (component != null)
				{
					component.TemporaryHeadStatusType = (TemporaryHeadStatusType)evt.Type;
				}
			}
			return false;
		}

		private bool OnAwayFromKeyboardStateChanged(PlayerStateServiceEvents.AwayFromKeyboardStateChanged evt)
		{
			if (isLocalPlayer(evt.SessionId))
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull)
			{
				PresenceData component = dataEntityCollection.GetComponent<PresenceData>(dataEntityHandle);
				if (component != null)
				{
					component.AFKState = new AwayFromKeyboardState((AwayFromKeyboardStateType)evt.Value, evt.EquippedObject);
				}
			}
			return false;
		}

		private bool onPlayerMembershipStatusChanged(PlayerStateServiceEvents.PlayerMembershipStatusChanged evt)
		{
			if (!DataEntityHandle.IsNullValue(dataEntityCollection.LocalPlayerHandle))
			{
				MembershipData component;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
				{
					component.IsMember = evt.IsMember;
					component.MembershipType = (evt.IsMember ? MembershipType.Member : MembershipType.None);
					if (!component.IsMember && Service.Get<AllAccessService>().IsAllAccessActive())
					{
						component.IsMember = true;
						component.MembershipType = MembershipType.AllAccessEventMember;
					}
				}
				else
				{
					Log.LogError(this, "No membership data found on local player");
				}
			}
			return false;
		}

		public static NetworkServicesConfig GenerateNetworkServiceConfig(Environment developmentEnvironment)
		{
			GameSettings gameSettings = Service.Get<GameSettings>();
			NetworkServicesConfig result = default(NetworkServicesConfig);
			IDictionary<string, object> environmentConfig = ConfigHelper.GetEnvironmentConfig(developmentEnvironment);
			IDictionary<string, object> buildPhaseConfig = ConfigHelper.GetBuildPhaseConfig();
			if (environmentConfig == null)
			{
				Log.LogError(typeof(NetworkController), "Falling back to QA environment config. ApplicationConfig did not contain an environment dictionary for " + developmentEnvironment);
				developmentEnvironment = Environment.QA;
				environmentConfig = ConfigHelper.GetEnvironmentConfig(developmentEnvironment);
			}
			IDictionary<string, object> dictionary = (IDictionary<string, object>)environmentConfig["DisneyIdClientId"];
			string text = "";
			text = "windows";
			if (string.IsNullOrEmpty(text) || !text.Contains(text))
			{
				Log.LogErrorFormatted(typeof(NetworkController), "ClientIdKey={0} not valid", text);
			}
			else
			{
				result.DisneyIdClientId = (string)dictionary[text];
			}
			result.GuestControllerHostUrl = (string)environmentConfig["GuestControllerHostUrl"];
			result.GuestControllerCDNUrl = (string)environmentConfig["GuestControllerCDNUrl"];
			result.MixAPIHostUrl = (string)environmentConfig["MixAPIHostUrl"];
			result.MixClientToken = (string)environmentConfig["MixClientToken"];
			result.iOSProvisioningId = (string)buildPhaseConfig["iOSProvisioningId"];
			result.GcmSenderId = (string)buildPhaseConfig["GcmSenderId"];
			result.MixCritialAlertLevel = (int)environmentConfig["MixCritialAlertLevel"];
			result.CPAPIServicehost = (string)environmentConfig["CPAPIServicehost"];
			result.CPAPIClientToken = (string)environmentConfig["CPAPIClientToken"];
			result.CPGameServerZone = (string)environmentConfig["CPGameServerZone"];
			result.CPGameServerEncrypted = (environmentConfig.ContainsKey("CPGameServerEncrypted") && (bool)environmentConfig["CPGameServerEncrypted"]);
			result.CPGameServerDebug = (environmentConfig.ContainsKey("CPGameServerDebug") && (bool)environmentConfig["CPGameServerDebug"]);
			result.CPLagMonitoring = (environmentConfig.ContainsKey("CPLagMonitoring") && (bool)environmentConfig["CPLagMonitoring"]);
			result.CPGameServerLatencyWindowSize = (environmentConfig.ContainsKey("CPGameServerLatencyWindowSize") ? ((int)environmentConfig["CPGameServerLatencyWindowSize"]) : 100);
			result.CPWebServiceLatencyWindowSize = (environmentConfig.ContainsKey("CPWebServiceLatencyWindowSize") ? ((int)environmentConfig["CPWebServiceLatencyWindowSize"]) : 100);
			result.CPMonitoringServicehost = (string)environmentConfig["CPMonitoringServicehost"];
			result.CPWebsiteAPIServicehost = (string)environmentConfig["CPWebsiteAPIServicehost"];
			result.CommerceResourceURLsDefinition = (string)environmentConfig["CommerceResourceURLsDefinition"];
			if (gameSettings.OfflineMode)
			{
				if (!string.IsNullOrEmpty(gameSettings.CPAPIServicehost))
				{
					result.CPAPIServicehost = gameSettings.CPAPIServicehost;
				}
				if (!string.IsNullOrEmpty(gameSettings.GuestControllerHostUrl))
				{
					result.GuestControllerHostUrl = gameSettings.GuestControllerHostUrl;
				}
				if (!string.IsNullOrEmpty(gameSettings.GuestControllerCDNUrl))
				{
					result.GuestControllerCDNUrl = gameSettings.GuestControllerCDNUrl;
				}
				if (!string.IsNullOrEmpty(gameSettings.MixAPIHostUrl))
				{
					result.MixAPIHostUrl = gameSettings.MixAPIHostUrl;
				}
				if (!string.IsNullOrEmpty(gameSettings.CPWebsiteAPIServicehost))
				{
					result.CPWebsiteAPIServicehost = gameSettings.CPWebsiteAPIServicehost;
				}
			}
			result.ClientVersion = null;

#if UNITY_ANDROID
    result.ClientApiVersion = "1.13.1";
#else
			result.ClientApiVersion = EnvironmentManager.ClientVersion.ToString();
#endif

			if (environmentConfig.ContainsKey("ClientApiVersion"))
			{
				result.ClientApiVersion = (string)environmentConfig["ClientApiVersion"];
			}

			return result;
		}

            [Invokable("Mix.CleanPersistentData", Description = "Clear all persistent MixSDK Data from device.")]
		public void LogoutAndClearData()
		{
			sessionManager.Logout();
			eventDispatcher.AddListener<SessionEvents.SessionEndedEvent>(onLogoutBeforeClear);
		}

		private bool onLogoutBeforeClear(SessionEvents.SessionEndedEvent evt)
		{
			eventDispatcher.RemoveListener<SessionEvents.SessionEndedEvent>(onLogoutBeforeClear);
			Service.Get<SceneTransitionService>().LoadScene(sceneConfig.HomeSceneName, sceneConfig.TransitionSceneName);
			try
			{
				Directory.Delete(Application.persistentDataPath + "/MixSDK", true);
			}
			catch (IOException ex)
			{
				Log.LogError(this, "Failed to delete persistent data: " + ex.Message);
			}
			return false;
		}

		public void SetGameConfig(FTUEConfig ftueConfig, GameSceneConfig sceneConfig)
		{
			this.ftueConfig = ftueConfig;
			this.sceneConfig = sceneConfig;
		}

		private bool isLocalPlayer(long sessionId)
		{
			return dataEntityCollection.IsLocalPlayer(sessionId);
		}

		private bool onSessionStarted(SessionEvents.SessionStartedEvent evt)
		{
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("swid", localUser.HashedId);
			Service.Get<ICPSwrveService>().UserUpdate(dictionary);
			Service.Get<CommerceService>().setBiUserId(localUser.HashedId);
			networkServicesManager.PlayerStateService.SetAccessToken(evt.AccessToken);
			networkServicesManager.PlayerStateService.SetSWID(evt.LocalPlayerSwid);
			networkServicesManager.PlayerStateService.GetLocalPlayerData(this);
			return false;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			LeaveRoom();
			networkServicesManager.FriendsService.ClearLocalUser(Service.Get<SessionManager>().LocalUser);
			clearLocalUser();
			isFirstCoinsReceived = true;
			return false;
		}

		private bool onAccessTokenUpdate(SessionEvents.AccessTokenUpdatedEvent evt)
		{
			networkServicesManager.PlayerStateService.SetAccessToken(evt.AccessToken);
			return false;
		}

		private bool onLocalPlayerDataReceived(PlayerStateServiceEvents.LocalPlayerDataReceived evt)
		{
			if (dataEntityCollection == null)
			{
				return false;
			}
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			bool flag = !dataEntityCollection.HasComponent<DisplayNameData>(localPlayerHandle);
			PlayerDataEntityFactory.AddLocalPlayerProfileDataComponents(dataEntityCollection, evt.Data, true);
			if (flag)
			{
				PlayerDataEntityFactory.AddCommonDataComponents(dataEntityCollection, localPlayerHandle);
				PlayerDataEntityFactory.AddLocalPlayerSessionScopeDataComponents(dataEntityCollection, localPlayerHandle);
				doFTUECheck(localPlayerHandle);
				ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
				networkServicesManager.FriendsService.SetLocalUser(localUser);
				setLocalUser(localUser);
				string text = "free";
				if (dataEntityCollection.IsLocalPlayerMember())
				{
					text = "member";
				}
				string tier = EnvironmentManager.AreHeadphonesConnected ? "headphone_on" : "headphone_off";
				Service.Get<ICPSwrveService>().Action("login", text, localUser.HashedId, tier);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("status", text);
				Service.Get<ICPSwrveService>().UserUpdate(dictionary);
				eventDispatcher.DispatchEvent(new NetworkControllerEvents.LocalPlayerDataReadyEvent(localPlayerHandle));
			}
			if (evt.Data.minigameProgress != null)
			{
				foreach (MinigameProgress item in evt.Data.minigameProgress)
				{
					MiniGamePlayCountData component = dataEntityCollection.GetComponent<MiniGamePlayCountData>(localPlayerHandle);
					if (component != null)
					{
						component.SetMinigamePlayCount(item.gameId, item.playCount);
					}
				}
			}
			return false;
		}

		private void doFTUECheck(DataEntityHandle handle)
		{
			ProfileData component = dataEntityCollection.GetComponent<ProfileData>(handle);
			component.IsFirstTimePlayer = true;
			component.IsFTUEComplete = false;
			QuestStateData component2;
			if (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2) || component2.Data == null)
			{
				return;
			}
			int num = 0;
			QuestState questState;
			while (true)
			{
				if (num < component2.Data.Count)
				{
					questState = component2.Data[num];
					if (questState.questId == ftueConfig.FtueQuestId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (questState.timesCompleted > 0 || (questState.completedObjectives != null && questState.completedObjectives.Count > 0))
			{
				component.IsFirstTimePlayer = false;
			}
			component.IsFTUEComplete = (questState.timesCompleted > 0);
			if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
			{
				component.IsFirstTimePlayer = false;
				component.IsFTUEComplete = true;
			}
		}

		public void LeaveRoom(bool immediately = false)
		{
			if (networkServicesManager.IsGameServerConnected())
			{
				networkServicesManager.WorldService.LeaveRoom(immediately);
			}
		}

		public void PauseGameServer()
		{
			LeaveRoom(true);
		}

		public void ResumeGameServer()
		{
			ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
			zoneTransitionService.LoadZone(zoneTransitionService.CurrentZone);
		}

		protected virtual bool onRoomJoined(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			if (localPlayerHandle.IsNull)
			{
				throw new MissingReferenceException("The local player handle does not exist!");
			}
			PlayerDataEntityFactory.AddCommonDataComponents(dataEntityCollection, localPlayerHandle);
			PlayerDataEntityFactory.AddCommonZoneScopeDataComponents(dataEntityCollection, localPlayerHandle);
			dataEntityCollection.AddComponent<LocalPlayerInZoneData>(localPlayerHandle);
			PresenceData component2 = dataEntityCollection.GetComponent<PresenceData>(localPlayerHandle);
			if (string.IsNullOrEmpty(evt.Room.zoneId.instanceId))
			{
				component2.World = evt.Room.world;
				component2.Room = evt.Room.zoneId.name;
				component2.ContentIdentifier = evt.Room.contentIdentifier;
				component2.InstanceRoom = null;
			}
			else
			{
				component2.InstanceRoom = evt.Room.zoneId;
			}
			DataEntityHandle[] remotePlayerHandles = dataEntityCollection.GetRemotePlayerHandles();
			for (int i = 0; i < remotePlayerHandles.Length; i++)
			{
				PresenceData component3 = dataEntityCollection.GetComponent<PresenceData>(remotePlayerHandles[i]);
				if (string.IsNullOrEmpty(evt.Room.zoneId.instanceId))
				{
					component3.World = evt.Room.world;
					component3.Room = evt.Room.zoneId.name;
					component3.ContentIdentifier = evt.Room.contentIdentifier;
					component3.InstanceRoom = null;
				}
				else
				{
					component3.InstanceRoom = evt.Room.zoneId;
				}
			}
			SessionIdData component4;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component4))
			{
				component4.SessionId = evt.SessionId;
			}
			else
			{
				component4 = dataEntityCollection.AddComponent(localPlayerHandle, delegate(SessionIdData component)
				{
					component.SessionId = evt.SessionId;
				});
			}
			eventDispatcher.DispatchEvent(new NetworkControllerEvents.LocalPlayerJoinedRoomEvent(localPlayerHandle, evt.Room.world, evt.Room.zoneId.name));
			return false;
		}

		private bool onSelfLeaveRoom(WorldServiceEvents.SelfLeaveRoomEvent evt)
		{
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				dataEntityCollection.RemoveComponent<LocalPlayerInZoneData>(localPlayerHandle);
			}
			return false;
		}

		private bool OnPlayerJoins(WorldServiceEvents.PlayerJoinRoomEvent evt)
		{
			if (isLocalPlayer(evt.SessionId))
			{
				return false;
			}
			DataEntityHandle handle = PlayerDataEntityFactory.CreateRemotePlayerEntity(dataEntityCollection, evt.Name, evt.SessionId);
			PlayerDataEntityFactory.AddCommonDataComponents(dataEntityCollection, handle);
			PlayerDataEntityFactory.AddCommonZoneScopeDataComponents(dataEntityCollection, handle);
			if (!dataEntityCollection.HasComponent<RemotePlayerData>(handle))
			{
				dataEntityCollection.AddComponent<RemotePlayerData>(handle);
			}
			if (!dataEntityCollection.HasComponent<ParticipationData>(handle))
			{
				dataEntityCollection.AddComponent<ParticipationData>(handle);
			}
			PresenceData component = dataEntityCollection.GetComponent<PresenceData>(handle);
			PresenceData component2;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2))
			{
				component.World = component2.World;
				component.Room = component2.Room;
				component.ContentIdentifier = component2.ContentIdentifier;
				component.InstanceRoom = component2.InstanceRoom;
			}
			component.IsDisconnecting = false;
			eventDispatcher.DispatchEvent(new NetworkControllerEvents.RemotePlayerJoinedRoomEvent(handle));
			return false;
		}

		private bool OnPlayerLeaves(WorldServiceEvents.PlayerLeaveRoomEvent evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull)
			{
				if (dataEntityCollection.GetComponent<LocomotionData>(dataEntityHandle).LocoState == LocomotionState.Zipline)
				{
					CoroutineRunner.Start(delayClearRemotePlayerComponents(dataEntityHandle), this, "Give users on a zipline time to clear the view");
				}
				else
				{
					clearRemotePlayerComponents(dataEntityHandle);
				}
			}
			eventDispatcher.DispatchEvent(new NetworkControllerEvents.RemotePlayerLeftRoomEvent(dataEntityHandle));
			return false;
		}

		private IEnumerator delayClearRemotePlayerComponents(DataEntityHandle handle)
		{
			PresenceData presenceData;
			if (dataEntityCollection.TryGetComponent(handle, out presenceData))
			{
				presenceData.IsDisconnecting = true;
			}
			yield return new WaitForSeconds(5f);
			if (presenceData == null || presenceData.IsDisconnecting)
			{
				clearRemotePlayerComponents(handle);
			}
		}

		private void clearRemotePlayerComponents(DataEntityHandle handle)
		{
			dataEntityCollection.RemoveComponent<ParticipationData>(handle);
			dataEntityCollection.RemoveEntityScopedComponents(handle, remotePlayerComponentsScope);
		}

		protected bool OnPlayerOutfit(PlayerStateServiceEvents.PlayerOutfitChanged evt)
		{
			DataEntityHandle dataEntityHandle = (!isLocalPlayer(evt.Outfit.sessionId.Value)) ? dataEntityCollection.FindEntity<SessionIdData, long>(evt.Outfit.sessionId.Value) : dataEntityCollection.LocalPlayerHandle;
			if (!dataEntityHandle.IsNull)
			{
				DCustomEquipment[] outfit = CustomEquipmentResponseAdaptor.ConvertResponseToOutfit(evt.Outfit.parts);
				AvatarDetailsData component;
				if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
				{
					component = dataEntityCollection.AddComponent<AvatarDetailsData>(dataEntityHandle);
				}
				component.Outfit = outfit;
			}
			return false;
		}

		protected bool OnPlayerProfile(PlayerStateServiceEvents.PlayerProfileChanged evt)
		{
			if (evt.Profile == null)
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = (!isLocalPlayer(evt.SessionId)) ? dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId) : dataEntityCollection.LocalPlayerHandle;
			if (!dataEntityHandle.IsNull)
			{
				Dictionary<int, AvatarColorDefinition> avatarColors = Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>();
				AvatarDetailsData component;
				if (!dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
				{
					component = dataEntityCollection.AddComponent<AvatarDetailsData>(dataEntityHandle);
				}
				component.BodyColor = AvatarBodyColorAdaptor.GetColorFromDefinitions(avatarColors, evt.Profile.colour);
			}
			return false;
		}

		protected bool OnPlayerTubeSelected(PlayerStateServiceEvents.TubeSelected evt)
		{
			DataEntityHandle dataEntityHandle = (!isLocalPlayer(evt.SessionId)) ? dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId) : dataEntityCollection.LocalPlayerHandle;
			TubeData component;
			if (!dataEntityHandle.IsNull && dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				component.SelectedTubeId = evt.TubeId;
			}
			return false;
		}

		private bool OnItemCreated(WorldServiceEvents.ItemSpawned evt)
		{
			addServerObjectWithItemData(evt.Item);
			return false;
		}

		private bool OnItemDestroyed(WorldServiceEvents.ItemDestroyed evt)
		{
			dataEntityCollection.RemoveEntityByName(ServerObjectItemData.GetEntityName(evt.ItemId));
			return false;
		}

		private bool OnItemUpdated(WorldServiceEvents.ItemChanged evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<ServerObjectItemData, CPMMOItemId>(evt.Item.Id);
			if (!dataEntityHandle.IsNull)
			{
				ServerObjectItemData component = dataEntityCollection.GetComponent<ServerObjectItemData>(dataEntityHandle);
				component.Item = evt.Item;
			}
			else
			{
				addServerObjectWithItemData(evt.Item);
			}
			return false;
		}

		private bool OnItemMoved(WorldServiceEvents.ItemMoved evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<ServerObjectItemData, CPMMOItemId>(evt.ItemId);
			if (!dataEntityHandle.IsNull)
			{
				ServerObjectPositionData component2;
				if (dataEntityCollection.TryGetComponent(dataEntityHandle, out component2))
				{
					component2.Position = evt.Position;
				}
				else
				{
					dataEntityCollection.AddComponent(dataEntityHandle, delegate(ServerObjectPositionData component)
					{
						component.Position = evt.Position;
					});
				}
			}
			return false;
		}

		private DataEntityHandle addServerObjectWithItemData(CPMMOItem item)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<ServerObjectItemData, CPMMOItemId>(item.Id);
			if (dataEntityHandle.IsNull)
			{
				dataEntityHandle = dataEntityCollection.AddEntity(ServerObjectItemData.GetEntityName(item.Id));
			}
			if (!dataEntityHandle.IsNull)
			{
				dataEntityCollection.AddComponent(dataEntityHandle, delegate(ServerObjectItemData component)
				{
					component.Item = item;
				});
			}
			return dataEntityHandle;
		}

		private bool OnPlayerMoved(PlayerActionServiceEvents.LocomotionActionReceived evt)
		{
			if (isLocalPlayer(evt.SessionId))
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull && dataEntityCollection.HasComponent<PositionData>(dataEntityHandle))
			{
				dataEntityCollection.GetComponent<PositionData>(dataEntityHandle).Position = evt.Action.Position;
			}
			return false;
		}

		private bool OnPlayerLocoStateChanged(PlayerStateServiceEvents.PlayerLocoStateChanged evt)
		{
			if (isLocalPlayer(evt.SessionId))
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull && dataEntityCollection.HasComponent<LocomotionData>(dataEntityHandle))
			{
				if (SceneRefs.ZiplineConnector != null)
				{
					SceneRefs.ZiplineConnector.ConnectIfNeeded(dataEntityHandle, evt.State);
				}
				dataEntityCollection.GetComponent<LocomotionData>(dataEntityHandle).LocoState = evt.State;
			}
			return false;
		}

		private bool onDispenableEquipped(PlayerStateServiceEvents.DispensableEquipped evt)
		{
			return onHeldObjectEquipped(evt.SessionId, evt.Type, HeldObjectType.DISPENSABLE);
		}

		private bool onDurableEquipped(PlayerStateServiceEvents.DurableEquipped evt)
		{
			return onHeldObjectEquipped(evt.SessionId, evt.Type, HeldObjectType.DURABLE);
		}

		private bool onConsumableEquipped(PlayerStateServiceEvents.ConsumableEquipped evt)
		{
			return onHeldObjectEquipped(evt.SessionId, evt.Type, HeldObjectType.CONSUMABLE);
		}

		private bool onPartyGameEquipped(PlayerStateServiceEvents.PartyGameEquipped evt)
		{
			return onHeldObjectEquipped(evt.SessionId, evt.Type, HeldObjectType.PARTYGAME);
		}

		private bool onHeldObjectEquipped(long sessionId, string objectId, HeldObjectType type)
		{
			if (isLocalPlayer(sessionId) && type != HeldObjectType.PARTYGAME)
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(sessionId);
			if (!dataEntityHandle.IsNull && dataEntityCollection.HasComponent<HeldObjectsData>(dataEntityHandle))
			{
				DHeldObject dHeldObject = new DHeldObject();
				dHeldObject.ObjectId = objectId;
				dHeldObject.ObjectType = type;
				dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityHandle).HeldObject = dHeldObject;
			}
			return false;
		}

		private bool OnPlayerAirBubbleChanged(PlayerStateServiceEvents.AirBubbleChanged evt)
		{
			if (isLocalPlayer(evt.SessionId))
			{
				return false;
			}
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull)
			{
				AirBubbleData component = dataEntityCollection.GetComponent<AirBubbleData>(dataEntityHandle);
				if (component != null)
				{
					component.AirBubble = evt.AirBubble;
				}
			}
			return false;
		}

		private bool onHeldObjectDequipped(PlayerStateServiceEvents.HeldObjectDequipped evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			if (!dataEntityHandle.IsNull && dataEntityCollection.HasComponent<HeldObjectsData>(dataEntityHandle))
			{
				HeldObjectsData component = dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityHandle);
				if (!isLocalPlayer(evt.SessionId) || (component.HeldObject != null && component.HeldObject.ObjectType == HeldObjectType.PARTYGAME))
				{
					dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityHandle).HeldObject = null;
				}
			}
			return false;
		}

		private bool onRoomRewardsReceived(RewardServiceEvents.RoomRewardsReceived evt)
		{
			PresenceData component2;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2) && component2.Room == evt.Room)
			{
				InZoneCollectiblesData component3;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component3))
				{
					component3.CollectedRewards = evt.EarnedRewards;
				}
				else
				{
					dataEntityCollection.AddComponent(dataEntityCollection.LocalPlayerHandle, delegate(InZoneCollectiblesData component)
					{
						component.CollectedRewards = evt.EarnedRewards;
					});
				}
			}
			return false;
		}

		private bool onMyRewardAssetsReceived(RewardServiceEvents.MyAssetsReceived evt)
		{
			if (isFirstCoinsReceived)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("monetization.coins_balance", evt.Assets.coins.ToString());
				Service.Get<ICPSwrveService>().UserUpdate(dictionary);
				dataEntityCollection.GetComponent<CoinsData>(dataEntityCollection.LocalPlayerHandle).SetInitialCoins(evt.Assets.coins);
			}
			else
			{
				dataEntityCollection.GetComponent<CoinsData>(dataEntityCollection.LocalPlayerHandle).Coins = evt.Assets.coins;
			}
			dataEntityCollection.GetComponent<CollectiblesData>(dataEntityCollection.LocalPlayerHandle).CollectibleTotals = evt.Assets.collectibleCurrencies;
			isFirstCoinsReceived = false;
			return false;
		}

		private bool onConsumableInventoryReceived(ConsumableServiceEvents.InventoryRecieved evt)
		{
			dataEntityCollection.GetComponent<ConsumableInventoryData>(dataEntityCollection.LocalPlayerHandle).ConsumableInventory = evt.Inventory;
			return false;
		}

		private void setLocalUser(ILocalUser localUser)
		{
			clearLocalUser();
			localUser.OnAlertsAdded += onAlertsAdded;
			showAlerts(localUser.Alerts);
		}

		private void clearLocalUser()
		{
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			if (localUser != null)
			{
				localUser.OnAlertsAdded -= onAlertsAdded;
			}
		}

		public void ClearAlert(IModerationAlert alert)
		{
			List<IAlert> list = new List<IAlert>();
			list.Add(alert.MixAlert);
			List<IAlert> alerts = list;
			Service.Get<SessionManager>().LocalUser.ClearAlerts(alerts, onClearAlertsSent);
		}

		private void onClearAlertsSent(IClearAlertsResult result)
		{
		}

		private void onAlertsAdded(object sender, AbstractAlertsAddedEventArgs args)
		{
			showAlerts(args.Alerts);
		}

		private void showAlerts(IEnumerable<IAlert> mixAlerts)
		{
			List<IModerationAlert> list = new List<IModerationAlert>();
			foreach (IAlert mixAlert in mixAlerts)
			{
				bool isCritical = mixAlert.Level >= mixCriticalAlertLevel;
				list.Add(new Alert(mixAlert, isCritical));
			}
			if (list.Count > 0)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ModerationServiceEvents.ShowAlerts(list));
			}
		}

		public void onRequestTimeOut()
		{
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinRequestTimeOut);
		}

		public void onGeneralNetworkError()
		{
			Service.Get<ErrorService>().OnRoomJoinError(ErrorService.RoomJoinErrorType.RoomJoinGeneralNetworkError);
		}
	}
}
