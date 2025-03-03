using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.LOD;
using ClubPenguin.Net.Locomotion;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class ZoneRemotePlayerManager : MonoBehaviour
	{
		public GameObject RemotePlayerContainer;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher eventDispatcher;

		private HashSet<RemotePlayerData> playerRemovedListeners;

		private Shader opaqueCPUShader;

		private Shader transparentCPUShader;

		private Shader opaqueGPUShader;

		private Shader transparentGPUShader;

		private HashSet<DataEntityHandle> avatarViewReady = new HashSet<DataEntityHandle>();

		private HashSet<DataEntityHandle> hasBeenPlaced = new HashSet<DataEntityHandle>();

		public void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventDispatcher = Service.Get<EventDispatcher>();
			playerRemovedListeners = new HashSet<RemotePlayerData>();
			SceneRefs.SetZoneRemotePlayerManager(this);
			opaqueCPUShader = Shader.Find("CpRemix/Combined Avatar");
			transparentCPUShader = Shader.Find("CpRemix/Combined Avatar Alpha");
			opaqueGPUShader = Shader.Find("CpRemix/GPU Combined Avatar");
			transparentGPUShader = Shader.Find("CpRemix/GPU Combined Avatar Alpha");
			if (RemotePlayerContainer == null)
			{
				throw new MissingReferenceException("RemotePlayerContainer is null");
			}
		}

		public void Start()
		{
			setupRemotePlayers();
			eventDispatcher.AddListener<NetworkControllerEvents.RemotePlayerJoinedRoomEvent>(onRemotePlayerJoinedRoom);
		}

		public void OnDestroy()
		{
			eventDispatcher.RemoveListener<NetworkControllerEvents.RemotePlayerJoinedRoomEvent>(onRemotePlayerJoinedRoom);
			foreach (RemotePlayerData playerRemovedListener in playerRemovedListeners)
			{
				playerRemovedListener.PlayerRemoved -= onPlayerRemoved;
			}
			playerRemovedListeners.Clear();
		}

		private void setupRemotePlayers()
		{
			DataEntityHandle[] remotePlayerHandles = dataEntityCollection.GetRemotePlayerHandles();
			for (int i = 0; i < remotePlayerHandles.Length; i++)
			{
				createRemotePlayer(remotePlayerHandles[i], false);
			}
			Service.Get<LODService>().SetupComplete(LODSystem.REMOTE_PLAYER);
		}

		private bool onRemotePlayerJoinedRoom(NetworkControllerEvents.RemotePlayerJoinedRoomEvent evt)
		{
			createRemotePlayer(evt.Handle);
			return false;
		}

		private bool createRemotePlayer(DataEntityHandle remotePlayerHandle, bool attemptSpawn = true)
		{
			if (!dataEntityCollection.HasComponent<GameObjectReferenceData>(remotePlayerHandle))
			{
				dataEntityCollection.AddComponent<GameObjectReferenceData>(remotePlayerHandle);
			}
			if (!dataEntityCollection.HasComponent<AirBubbleData>(remotePlayerHandle))
			{
				dataEntityCollection.AddComponent<AirBubbleData>(remotePlayerHandle);
			}
			PositionData component = dataEntityCollection.GetComponent<PositionData>(remotePlayerHandle);
			RemotePlayerData component2 = dataEntityCollection.GetComponent<RemotePlayerData>(remotePlayerHandle);
			component2.PlayerRemoved += onPlayerRemoved;
			playerRemovedListeners.Add(component2);
			PresenceData component3 = dataEntityCollection.GetComponent<PresenceData>(remotePlayerHandle);
			if (component3 != null)
			{
				component3.PresenceDataUpdated += onPresenceDataUpdated;
			}
			if (!dataEntityCollection.HasComponent<LODRequestReference>(remotePlayerHandle))
			{
				LODRequestData requestData = new LODRequestData(LODSystem.REMOTE_PLAYER, remotePlayerHandle, component);
				requestData.OnGameObjectGeneratedEvent += onLODGameObjectGenerated;
				requestData.OnGameObjectRevokedEvent += onLODGameObjectRevoked;
				LODRequest request = Service.Get<LODService>().Request(requestData, attemptSpawn);
				dataEntityCollection.AddComponent<LODRequestReference>(remotePlayerHandle).Request = request;
			}
			return false;
		}

		private void onPlayerMoved(PositionData positionData, Vector3 newPosition)
		{
			if (positionData == null)
			{
				return;
			}
			positionData.PlayerMoved -= onPlayerMoved;
			if (base.gameObject.IsDestroyed())
			{
				return;
			}
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(positionData);
			if (DataEntityHandle.IsNullValue(entityByComponent))
			{
				return;
			}
			GameObjectReferenceData component = dataEntityCollection.GetComponent<GameObjectReferenceData>(entityByComponent);
			if (component != null)
			{
				GameObject gameObject = component.GameObject;
				if (!gameObject.IsDestroyed())
				{
					placeRemotePlayer(gameObject, entityByComponent, newPosition);
				}
			}
		}

		private void placeRemotePlayer(GameObject remotePlayer, DataEntityHandle remotePlayerHandle, Vector3 newPosition)
		{
			remotePlayer.transform.position = newPosition;
			if (avatarViewReady.Contains(remotePlayerHandle))
			{
				fadeInRemotePlayer(remotePlayer);
				avatarViewReady.Remove(remotePlayerHandle);
			}
			else
			{
				hasBeenPlaced.Add(remotePlayerHandle);
			}
		}

		private void onAvatarIsReady(AvatarBaseAsync avatarView)
		{
			avatarView.OnReady -= onAvatarIsReady;
			GameObject gameObject = avatarView.gameObject;
			AvatarDataHandle component = gameObject.GetComponent<AvatarDataHandle>();
			RemotePlayerData component2;
			if (!(component != null) || component.Handle.IsNull || !dataEntityCollection.TryGetComponent(component.Handle, out component2))
			{
				return;
			}
			DataEntityHandle handle = null;
			if (AvatarDataHandle.TryGetPlayerHandle(gameObject, out handle))
			{
				avatarViewReady.Add(handle);
				PositionData component3 = dataEntityCollection.GetComponent<PositionData>(handle);
				if (component3.Position != Vector3.zero)
				{
					placeRemotePlayer(gameObject, handle, component3.Position);
				}
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerSpawnedEvents.RemotePlayerSpawned(gameObject, component.Handle));
		}

		private void onPresenceDataUpdated(PresenceData presenceData)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(presenceData);
			LODRequestReference component;
			if (!entityByComponent.IsNull && dataEntityCollection.TryGetComponent(entityByComponent, out component))
			{
				if (presenceData.AFKState.Type == AwayFromKeyboardStateType.AwayFromWorld)
				{
					Service.Get<LODService>().PauseRequest(component.Request);
				}
				else if (component.Request.IsPaused)
				{
					Service.Get<LODService>().UnpauseRequest(component.Request);
				}
			}
		}

		private void onPlayerRemoved(RemotePlayerData component)
		{
			component.PlayerRemoved -= onPlayerRemoved;
			playerRemovedListeners.Remove(component);
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(component);
			if (entityByComponent.IsNull)
			{
				Log.LogError(this, "RemotePlayerData removed, but the handle could not be found for the component");
			}
			LODRequestReference component2;
			if (dataEntityCollection.TryGetComponent(entityByComponent, out component2))
			{
				Service.Get<LODService>().RemoveRequest(component2.Request);
			}
			avatarViewReady.Remove(entityByComponent);
			hasBeenPlaced.Remove(entityByComponent);
			dataEntityCollection.RemoveComponent<GameObjectReferenceData>(entityByComponent);
		}

		public IList<Transform> GetRemotePlayers()
		{
			IList<Transform> list = new List<Transform>();
			foreach (Transform item in RemotePlayerContainer.transform)
			{
				list.Add(item);
			}
			return list;
		}

		public void AssignAvatarView(AvatarView avatarView, DataEntityHandle remotePlayerHandle, GameObject remotePlayerGO)
		{
			GameObjectReferenceData component = dataEntityCollection.GetComponent<GameObjectReferenceData>(remotePlayerHandle);
			component.GameObject = remotePlayerGO;
			if (avatarView.IsReady)
			{
				onAvatarIsReady(avatarView);
			}
			else
			{
				avatarView.OnReady += onAvatarIsReady;
			}
			PositionData component2 = dataEntityCollection.GetComponent<PositionData>(remotePlayerHandle);
			if (component2.Position != Vector3.zero)
			{
				placeRemotePlayer(remotePlayerGO, remotePlayerHandle, component2.Position);
			}
			else
			{
				component2.PlayerMoved += onPlayerMoved;
			}
		}

		private void onLODGameObjectGenerated(GameObject remotePlayer, DataEntityHandle remotePlayerHandle, LODRequestData requestData)
		{
			requestData.OnGameObjectGeneratedEvent -= onLODGameObjectGenerated;
			remotePlayer.name = "rp_" + dataEntityCollection.GetComponent<DisplayNameData>(remotePlayerHandle).DisplayName;
			enableRenderers(remotePlayer, false);
			remotePlayer.transform.SetParent(RemotePlayerContainer.transform);
			AvatarDataHandle component = remotePlayer.GetComponent<AvatarDataHandle>();
			component.SetHandle(remotePlayerHandle);
			remotePlayer.GetComponent<LocomotionReceiver>().Init();
		}

		private void onLODGameObjectRevoked(GameObject remotePlayer, DataEntityHandle remotePlayerHandle, LODRequestData requestData)
		{
			requestData.OnGameObjectGeneratedEvent -= onLODGameObjectGenerated;
			makeRenderersTransparent(remotePlayer, true);
			GameObjectReferenceData component;
			if (!remotePlayerHandle.IsNull && dataEntityCollection.TryGetComponent(remotePlayerHandle, out component))
			{
				component.GameObject = null;
			}
		}

		private void makeRenderersTransparent(GameObject remotePlayer, bool makeTransparent)
		{
			Shader shader = null;
			Shader shader2 = null;
			Renderer[] array = null;
			if ((bool)remotePlayer.GetComponent<GpuSkinnedRenderer>())
			{
				shader = (makeTransparent ? transparentGPUShader : opaqueGPUShader);
				shader2 = (makeTransparent ? opaqueGPUShader : transparentGPUShader);
				array = remotePlayer.GetComponentsInChildren<MeshRenderer>();
			}
			else
			{
				shader = (makeTransparent ? transparentCPUShader : opaqueCPUShader);
				shader2 = (makeTransparent ? opaqueCPUShader : transparentCPUShader);
				array = remotePlayer.GetComponentsInChildren<SkinnedMeshRenderer>();
			}
			if (array == null)
			{
				return;
			}
			foreach (Renderer renderer in array)
			{
				for (int j = 0; j < renderer.materials.Length; j++)
				{
					if (renderer.materials[j].shader == shader2)
					{
						renderer.materials[j].shader = shader;
					}
				}
			}
		}

		private void enableRenderers(GameObject remotePlayer, bool enable = true)
		{
			Renderer[] componentsInChildren = remotePlayer.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.enabled != enable)
				{
					renderer.enabled = enable;
				}
			}
		}

		private void disableRenderers(GameObject remotePlayer)
		{
			enableRenderers(remotePlayer, false);
		}

		private void fadeInRemotePlayer(GameObject remotePlayer)
		{
			enableRenderers(remotePlayer);
			makeRenderersTransparent(remotePlayer, true);
			remotePlayer.AddComponent<FadeIn>().ECompleted += delegate
			{
				makeRenderersTransparent(remotePlayer, false);
			};
		}
	}
}
