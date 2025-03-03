using ClubPenguin.Adventure;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Net.Locomotion;
using ClubPenguin.Participation;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(AvatarView))]
	public class ZoneLocalPlayerManager : MonoBehaviour
	{
		public GameObject LocalPlayerGameObject;

		private AvatarView avatarView;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher eventDispatcher;

		private bool localPlayerCreated;

		private void Awake()
		{
			localPlayerCreated = false;
			SceneRefs.SetZoneLocalPlayerManager(this);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventDispatcher = Service.Get<EventDispatcher>();
			if (LocalPlayerGameObject == null)
			{
				throw new MissingReferenceException("LocalPlayerGameObject is null");
			}
			setupLocalPlayer();
			eventDispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerReadyToSpawn>(onLocalPlayerReadyToSpawn);
		}

		private void OnDestroy()
		{
			eventDispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerReadyToSpawn>(onLocalPlayerReadyToSpawn);
		}

		private void setupLocalPlayer()
		{
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			AvatarDataHandle avatarDataHandle = LocalPlayerGameObject.AddComponent<AvatarDataHandle>();
			avatarDataHandle.SetHandle(localPlayerHandle, true);
			GameObjectReferenceData gameObjectReferenceData = dataEntityCollection.AddComponent<GameObjectReferenceData>(localPlayerHandle);
			gameObjectReferenceData.GameObject = LocalPlayerGameObject;
		}

		public void AssignAvatarView(AvatarView _avatarView)
		{
			avatarView = _avatarView;
			readyForAvatar();
		}

		private bool onLocalPlayerReadyToSpawn(PlayerSpawnedEvents.LocalPlayerReadyToSpawn evt)
		{
			eventDispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerReadyToSpawn>(onLocalPlayerReadyToSpawn);
			ParticipationData component = dataEntityCollection.GetComponent<ParticipationData>(dataEntityCollection.LocalPlayerHandle);
			if (component != null)
			{
				component.CurrentParticipationState = ParticipationState.Ready;
			}
			createLocalPlayer(LocalPlayerGameObject, evt.Handle);
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest == null || activeQuest.Definition.name != Service.Get<GameStateController>().FTUEConfig.FtueQuestId)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(UIDisablerEvents.EnableAllUIElements));
			}
			readyForAvatar();
			return false;
		}

		private void readyForAvatar()
		{
			if (localPlayerCreated && avatarView != null)
			{
				if (avatarView.IsReady)
				{
					onAvatarIsReady(avatarView);
				}
				else
				{
					avatarView.OnReady += onAvatarIsReady;
				}
			}
		}

		private void createLocalPlayer(GameObject playerGameObject, DataEntityHandle playerdataHandle)
		{
			bool flag = true;
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			SceneStateData component;
			if (!dataEntityHandle.IsNull && dataEntityCollection.TryGetComponent(dataEntityHandle, out component) && component.State != 0)
			{
				flag = false;
			}
			if (flag)
			{
				playerGameObject.AddComponent<LocomotionBroadcaster>();
			}
			setInitialPosition(playerGameObject, playerdataHandle);
			localPlayerCreated = true;
		}

		private void onAvatarIsReady(AvatarBaseAsync view)
		{
			avatarView.OnReady -= onAvatarIsReady;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerSpawnedEvents.LocalPlayerSpawned(view.gameObject, dataEntityCollection.LocalPlayerHandle));
		}

		private void setInitialPosition(GameObject playerGameObject, DataEntityHandle playerdataHandle)
		{
			if (!playerdataHandle.IsNull)
			{
				PositionData component = dataEntityCollection.GetComponent<PositionData>(playerdataHandle);
				if (component != null && component.Position != Vector3.zero)
				{
					playerGameObject.transform.position = dataEntityCollection.GetComponent<PositionData>(playerdataHandle).Position;
				}
			}
		}
	}
}
