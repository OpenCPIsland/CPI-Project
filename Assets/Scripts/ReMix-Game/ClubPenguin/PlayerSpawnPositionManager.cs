using ClubPenguin.Adventure;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Locomotion;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using ClubPenguin.World;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class PlayerSpawnPositionManager : MonoBehaviour
	{
		private CPDataEntityCollection dataEntityCollection;

		private AvatarDataHandle playerDataHandle;

		private SpawnedAction actionOnSpawned;

		private Vector3? localPlayerSpawnPostion;

		private EventDispatcher eventDispatcher;

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			playerDataHandle = GetComponent<AvatarDataHandle>();
			eventDispatcher = Service.Get<EventDispatcher>();
			if (!playerDataHandle.Handle.IsNull && dataEntityCollection.HasComponent<LocalPlayerInZoneData>(playerDataHandle.Handle))
			{
				spawnLocalPlayerInZone();
			}
			else
			{
				eventDispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerReadyToSpawn>(onLocalPlayerReadyToSpawn, EventDispatcher.Priority.FIRST);
			}
			eventDispatcher.AddListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
		}

		public void SpawnPlayer(SpawnPlayerParams spawnPlayerParams)
		{
			if (spawnPlayerParams.Zone != null || !string.IsNullOrEmpty(spawnPlayerParams.SceneName))
			{
				bool flag = false;
				PresenceData component = dataEntityCollection.GetComponent<PresenceData>(dataEntityCollection.LocalPlayerHandle);
				ZoneTransitionService zoneTransitionService = Service.Get<ZoneTransitionService>();
				if (spawnPlayerParams.Zone != null)
				{
					ZoneDefinition zoneBySceneName = zoneTransitionService.GetZoneBySceneName(component.Room);
					flag = (zoneBySceneName != null && string.Equals(spawnPlayerParams.Zone.ZoneName, zoneBySceneName.ZoneName));
				}
				else if (!string.IsNullOrEmpty(spawnPlayerParams.SceneName))
				{
					flag = ((!zoneTransitionService.IsInIgloo) ? string.Equals(component.Room, spawnPlayerParams.SceneName) : string.Equals(zoneTransitionService.CurrentZone.SceneName, spawnPlayerParams.SceneName));
				}
				if (!flag)
				{
					SpawnData component2;
					if (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2))
					{
						component2 = dataEntityCollection.AddComponent<SpawnData>(dataEntityCollection.LocalPlayerHandle);
					}
					if (spawnPlayerParams.SpawnedAction != null)
					{
						component2.SpawnedAction = spawnPlayerParams.SpawnedAction;
					}
					component2.Position = spawnPlayerParams.Position;
					component2.Rotation = spawnPlayerParams.Rotation;
					component2.PendingReward = spawnPlayerParams.PendingReward;
					if (spawnPlayerParams.Zone != null)
					{
						zoneTransitionService.LoadZone(spawnPlayerParams.Zone, "Loading");
					}
					else
					{
						zoneTransitionService.LoadZone(spawnPlayerParams.SceneName, "Loading");
					}
					return;
				}
			}
			if (spawnPlayerParams.PendingReward != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.QUICK_NOTIFICATION, "", spawnPlayerParams.PendingReward));
			}
			if (Vector3.Distance(base.transform.position, spawnPlayerParams.Position) <= 3f)
			{
				if (spawnPlayerParams.SpawnedAction != null)
				{
					executeSpawedAction(spawnPlayerParams.SpawnedAction);
				}
				return;
			}
			LocomotionTracker component3 = base.gameObject.GetComponent<LocomotionTracker>();
			if (component3.IsCurrentControllerOfType<SlideController>())
			{
				component3.SetCurrentController<RunController>();
			}
			else if (spawnPlayerParams.GetOutOfSwimming && component3.IsCurrentControllerOfType<SwimController>())
			{
				Animator component4 = base.gameObject.GetComponent<Animator>();
				component4.SetTrigger(AnimationHashes.Params.SwimToWalk);
				component3.SetCurrentController<RunController>();
			}
			Vector3 zero = Vector3.zero;
			if (spawnPlayerParams.NudgePlayer && component3.IsCurrentControllerOfType<RunController>())
			{
				zero.y = 0.5f;
			}
			base.transform.position = spawnPlayerParams.Position + zero;
			ClubPenguin.Core.SceneRefs.Get<BaseCamera>().Snap();
			if (spawnPlayerParams.NudgePlayer)
			{
				actionOnSpawned = spawnPlayerParams.SpawnedAction;
				CoroutineRunner.Start(LocomotionUtils.nudgePlayer(component3, onNudgePlayerDone), component3.gameObject, "MoveAfterJump");
			}
			else if (spawnPlayerParams.SpawnedAction != null)
			{
				executeSpawedAction(spawnPlayerParams.SpawnedAction);
			}
		}

		private void onNudgePlayerDone()
		{
			if (actionOnSpawned != null)
			{
				executeSpawedAction(actionOnSpawned);
			}
		}

		private bool onHudInitCompleted(HudEvents.HudInitComplete evt)
		{
			eventDispatcher.RemoveListener<HudEvents.HudInitComplete>(onHudInitCompleted);
			if (actionOnSpawned != null)
			{
				executeSpawedAction(actionOnSpawned);
			}
			return false;
		}

		private bool onSceneTransitionComplete(SceneTransitionEvents.TransitionComplete evt)
		{
			eventDispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(onSceneTransitionComplete);
			if (localPlayerSpawnPostion.HasValue)
			{
				base.transform.position = localPlayerSpawnPostion.Value;
			}
			return false;
		}

		private bool onLocalPlayerReadyToSpawn(PlayerSpawnedEvents.LocalPlayerReadyToSpawn evt)
		{
			eventDispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerReadyToSpawn>(onLocalPlayerReadyToSpawn);
			spawnLocalPlayerInZone();
			return false;
		}

		private void executeSpawedAction(SpawnedAction spawnedAction)
		{
			if (spawnedAction.Reward != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.QUICK_NOTIFICATION, "", spawnedAction.Reward));
			}
			switch (spawnedAction.Action)
			{
			case SpawnedAction.SPAWNED_ACTION.StartQuest:
				if (spawnedAction.Quest != null)
				{
					Service.Get<TutorialManager>().EndTutorial();
					Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.StartQuest(spawnedAction.Quest));
				}
				break;
			case SpawnedAction.SPAWNED_ACTION.ReplayQuest:
				if (spawnedAction.Quest != null)
				{
					Service.Get<TutorialManager>().EndTutorial();
					Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.ReplayQuest(spawnedAction.Quest));
				}
				break;
			}
		}

		private void spawnLocalPlayerInZone()
		{
			if (Service.Get<ZoneTransitionService>().CurrentZone.AlwaysSpawnPlayerAtDefaultLocation)
			{
				spawnNearDefaultLocation();
				return;
			}
			actionOnSpawned = null;
			Quaternion rotation = Quaternion.identity;
			PositionData component3;
			if (playerDataHandle.Handle == dataEntityCollection.LocalPlayerHandle)
			{
				Vector3 vector = Vector3.zero;
				bool flag = false;
				PausedStateData component;
				SpawnData component2;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
				{
					if (component.ShouldSkipResume)
					{
						component.ShouldSkipResume = false;
					}
					else
					{
						vector = component.Position;
						flag = true;
					}
				}
				else if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2))
				{
					vector = component2.Position;
					rotation = component2.Rotation;
					actionOnSpawned = component2.SpawnedAction;
					actionOnSpawned.Reward = component2.PendingReward;
					flag = true;
					dataEntityCollection.RemoveComponent<SpawnData>(dataEntityCollection.LocalPlayerHandle);
				}
				if (flag && dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component3))
				{
					component3.Position = vector;
					localPlayerSpawnPostion = vector;
					base.transform.SetPositionAndRotation(vector, rotation);
					if (actionOnSpawned != null && (actionOnSpawned.Action != 0 || actionOnSpawned.Reward != null))
					{
						eventDispatcher.AddListener<HudEvents.HudInitComplete>(onHudInitCompleted);
					}
					return;
				}
			}
			if (isSpawnNearPlayer())
			{
				Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendLocationInRoomReceived>(onFriendLocationReceived);
				Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendNotInRoom>(onFriendNotInRoom);
				Service.Get<INetworkServicesManager>().FriendsService.GetFriendLocationInRoom(dataEntityCollection.GetComponent<SpawnAtPlayerData>(playerDataHandle.Handle).PlayerSWID);
				return;
			}
			bool flag2 = false;
			component3 = dataEntityCollection.GetComponent<PositionData>(playerDataHandle.Handle);
			if (!flag2 && component3 != null && component3.Position != Vector3.zero)
			{
				LocomotionActionEvent action = default(LocomotionActionEvent);
				action.Type = LocomotionAction.Move;
				action.Position = component3.Position;
				action.Direction = Vector3.zero;
				Service.Get<INetworkServicesManager>().PlayerActionService.LocomotionAction(action);
				flag2 = true;
			}
			if (!flag2)
			{
				spawnAtSceneLocation();
			}
		}

		public bool spawnAtSceneLocation()
		{
			bool flag = false;
			ZoneDefinition previousZone = Service.Get<ZoneTransitionService>().PreviousZone;
			if (previousZone != null)
			{
				flag = spawnNearTransition(previousZone);
			}
			if (!flag)
			{
				flag = spawnNearDefaultLocation();
			}
			if (ClubPenguin.Core.SceneRefs.IsSet<BaseCamera>())
			{
				ClubPenguin.Core.SceneRefs.Get<BaseCamera>().Snap();
			}
			return flag;
		}

		private bool isSpawnNearPlayer()
		{
			if (playerDataHandle == null || playerDataHandle.Handle.IsNull)
			{
				return false;
			}
			return null != dataEntityCollection.GetComponent<SpawnAtPlayerData>(playerDataHandle.Handle);
		}

		private bool onFriendNotInRoom(FriendsServiceEvents.FriendNotInRoom evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FriendLocationInRoomReceived>(onFriendLocationReceived);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FriendNotInRoom>(onFriendNotInRoom);
			if (!playerDataHandle.Handle.IsNull)
			{
				dataEntityCollection.RemoveComponent<SpawnAtPlayerData>(playerDataHandle.Handle);
			}
			spawnAtSceneLocation();
			return false;
		}

		private bool onFriendLocationReceived(FriendsServiceEvents.FriendLocationInRoomReceived evt)
		{
			dataEntityCollection.RemoveComponent<SpawnAtPlayerData>(dataEntityCollection.LocalPlayerHandle);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FriendLocationInRoomReceived>(onFriendLocationReceived);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FriendNotInRoom>(onFriendNotInRoom);
			if (!playerDataHandle.Handle.IsNull)
			{
				base.transform.position = evt.Location;
				LocomotionActionEvent action = default(LocomotionActionEvent);
				action.Type = LocomotionAction.Move;
				action.Position = base.transform.position;
				action.Direction = Vector3.zero;
				Service.Get<INetworkServicesManager>().PlayerActionService.LocomotionAction(action);
				BaseCamera baseCamera = ClubPenguin.Core.SceneRefs.Get<BaseCamera>();
				if (baseCamera != null)
				{
					baseCamera.Snap();
				}
				dataEntityCollection.RemoveComponent<SpawnAtPlayerData>(playerDataHandle.Handle);
			}
			return false;
		}

		private bool spawnNearTransition(ZoneDefinition previousZone)
		{
			bool result = false;
			GameObject gameObject = null;
			ZoneTransition[] array = Object.FindObjectsOfType<ZoneTransition>();
			foreach (ZoneTransition zoneTransition in array)
			{
				if (zoneTransition.Zone == null)
				{
					Log.LogErrorFormatted(this, "Invalid zone transition found while attempting to spawn player. No zone definition assigned to transition '{0}'", zoneTransition.GetPath());
				}
				else if (zoneTransition.Zone.ZoneName == previousZone.ZoneName)
				{
					gameObject = zoneTransition.gameObject;
					break;
				}
			}
			if (gameObject != null)
			{
				SpawnPointSelector component = gameObject.GetComponent<SpawnPointSelector>();
				if (component != null)
				{
					result = true;
					setPositionToSpawnPoint(component);
				}
				else
				{
					result = false;
				}
			}
			else
			{
				MapTransition mapTransition = Object.FindObjectOfType<MapTransition>();
				if (mapTransition != null)
				{
					SpawnPointSelector component = mapTransition.GetComponent<SpawnPointSelector>();
					if (component != null)
					{
						result = true;
						setPositionToSpawnPoint(component);
					}
				}
			}
			return result;
		}

		private bool spawnNearDefaultLocation()
		{
			bool result = false;
			SpawnPointSelector spawnPointSelector = Object.FindObjectOfType<DefaultSpawnPointSelector>();
			if (spawnPointSelector != null)
			{
				result = true;
				setPositionToSpawnPoint(spawnPointSelector);
			}
			return result;
		}

		private void setPositionToSpawnPoint(SpawnPointSelector spawner)
		{
			StartCoroutine(setPositionToSpawnPointWaitAFrame(spawner));
		}

		private IEnumerator setPositionToSpawnPointWaitAFrame(SpawnPointSelector spawner)
		{
			base.transform.position = spawner.SelectSpawnPosition(CoordinateSpace.World);
			base.transform.rotation = spawner.SelectSpawnRotation(CoordinateSpace.World);
			yield return null;
			LocomotionTracker tracker = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionTracker>();
			if (tracker != null)
			{
				tracker.SetCurrentController<RunController>();
			}
			base.transform.position = spawner.SelectSpawnPosition(CoordinateSpace.World);
			base.transform.rotation = spawner.SelectSpawnRotation(CoordinateSpace.World);
			yield return null;
			LocomotionActionEvent movement = default(LocomotionActionEvent);
			movement.Type = LocomotionAction.Move;
			movement.Position = base.transform.position;
			movement.Direction = Vector3.zero;
			sendNetworkMessage(movement);
		}

		private void sendNetworkMessage(LocomotionActionEvent action)
		{
			if (GetComponent<LocomotionBroadcaster>() != null)
			{
				Service.Get<INetworkServicesManager>().PlayerActionService.LocomotionAction(action);
			}
		}
	}
}
