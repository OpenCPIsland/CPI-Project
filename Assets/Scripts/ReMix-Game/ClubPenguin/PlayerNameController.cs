using ClubPenguin.Cinematography;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class PlayerNameController : AvatarPositionTranslator
	{
		private struct PlayerNameTagAndRectTransform
		{
			public readonly PlayerNameTag PlayerNameTag;

			public readonly RectTransform RectTransform;

			public PlayerNameTagAndRectTransform(PlayerNameTag playerNameTag, RectTransform rectTransform)
			{
				PlayerNameTag = playerNameTag;
				RectTransform = rectTransform;
			}
		}

		public Transform LocalPlayerContainer;

		public Transform RemotePlayersContainer;

		public Transform FriendPlayersContainer;

		private static float maximumRange = 2f;

		private bool nameTagsActive;

		private GameObjectPool playerNameTagPool;

		private EventChannel eventChannel;

		private Dictionary<long, PlayerNameTagAndRectTransform> activePlayerNameTags;

		public event Action<long> OnPlayerNameAdded;

		public PlayerNameTag GetPlayerNameTag(long sessionId)
		{
			if (activePlayerNameTags.ContainsKey(sessionId))
			{
				return activePlayerNameTags[sessionId].PlayerNameTag;
			}
			return null;
		}

		protected override void awakeInit()
		{
			playerNameTagPool = GameObject.Find("Pool[PlayerName]").GetComponent<GameObjectPool>();
			nameTagsActive = true;
			activePlayerNameTags = new Dictionary<long, PlayerNameTagAndRectTransform>();
		}

		private void OnEnable()
		{
			if (eventChannel == null)
			{
				eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			}
			eventChannel.AddListener<PlayerSpawnedEvents.RemotePlayerSpawned>(onRemotePlayerSpawned);
			eventChannel.AddListener<PlayerNameEvents.ShowPlayerNames>(onShowPlayerNames);
			eventChannel.AddListener<PlayerNameEvents.HidePlayerNames>(onHidePlayerNames);
			eventChannel.AddListener<FriendsServiceEvents.FriendsListUpdated>(onFriendsListUpdated);
			if (!dataEntityCollection.LocalPlayerHandle.IsNull && dataEntityCollection.HasComponent<LocalPlayerInZoneData>(dataEntityCollection.LocalPlayerHandle))
			{
				localPlayer = getTransform(base.localSessionId);
			}
			if (localPlayer == null)
			{
				eventChannel.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			}
			else
			{
				setUpNameTag(dataEntityCollection.LocalPlayerHandle, true);
			}
		}

		private void Update()
		{
			if (nameTagsActive)
			{
				foreach (KeyValuePair<long, PlayerNameTagAndRectTransform> activePlayerNameTag in activePlayerNameTags)
				{
					Transform avatar = getAvatar(activePlayerNameTag.Key);
					if (avatar != null && avatar.gameObject.activeInHierarchy && isWithinRange(avatar, maximumRange) && isWithinViewport(avatar.position))
					{
						if (!activePlayerNameTag.Value.PlayerNameTag.IsActive)
						{
							activePlayerNameTag.Value.PlayerNameTag.SetActive(true);
						}
						activePlayerNameTag.Value.RectTransform.anchoredPosition = getScreenPoint(avatar.position);
					}
					else if (activePlayerNameTag.Value.PlayerNameTag.IsActive)
					{
						activePlayerNameTag.Value.PlayerNameTag.SetActive(false);
					}
				}
			}
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			setUpNameTag(evt.Handle, true);
			localPlayer = evt.LocalPlayerGameObject.transform;
			return false;
		}

		private bool onRemotePlayerSpawned(PlayerSpawnedEvents.RemotePlayerSpawned evt)
		{
			if (!evt.Handle.IsNull)
			{
				setUpNameTag(evt.Handle, false);
				dataEntityCollection.GetComponent<RemotePlayerData>(evt.Handle).PlayerRemoved += onPlayerRemoved;
			}
			return false;
		}

		private void setUpNameTag(DataEntityHandle handle, bool isLocalPlayer)
		{
			SessionIdData component;
			if (handle.IsNull || !dataEntityCollection.TryGetComponent(handle, out component))
			{
				return;
			}
			long sessionId = component.SessionId;
			string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName;
			bool flag = dataEntityCollection.HasComponent<FriendData>(handle);
			if (!activePlayerNameTags.ContainsKey(sessionId))
			{
				PlayerNameTag playerNameTagFromPool = getPlayerNameTagFromPool();
				if (isLocalPlayer)
				{
					playerNameTagFromPool.transform.SetParent(LocalPlayerContainer, false);
					playerNameTagFromPool.SetNameTagType(PlayerNameTag.Type.LocalPlayer);
				}
				else if (flag)
				{
					playerNameTagFromPool.transform.SetParent(FriendPlayersContainer, false);
					playerNameTagFromPool.SetNameTagType(PlayerNameTag.Type.Friend);
				}
				else
				{
					playerNameTagFromPool.transform.SetParent(RemotePlayersContainer, false);
					playerNameTagFromPool.SetNameTagType(PlayerNameTag.Type.RemotePlayer);
				}
				playerNameTagFromPool.Handle = handle;
				playerNameTagFromPool.SetNameText(displayName);
				playerNameTagFromPool.transform.localScale = Vector3.one;
				playerNameTagFromPool.transform.localRotation = Quaternion.identity;
				Transform transform = getTransform(sessionId);
				if (transform != null)
				{
					CameraCullingMaskHelper.SetLayerIncludingChildren(playerNameTagFromPool.transform, LayerMask.LayerToName(transform.gameObject.layer));
				}
				activePlayerNameTags.Add(sessionId, new PlayerNameTagAndRectTransform(playerNameTagFromPool, playerNameTagFromPool.GetComponent<RectTransform>()));
				if (this.OnPlayerNameAdded != null)
				{
					this.OnPlayerNameAdded(sessionId);
				}
				if (!nameTagsActive)
				{
					playerNameTagFromPool.SetActive(false);
				}
			}
		}

		private bool onShowPlayerNames(PlayerNameEvents.ShowPlayerNames evt)
		{
			nameTagsActive = true;
			return false;
		}

		private bool onHidePlayerNames(PlayerNameEvents.HidePlayerNames evt)
		{
			setPlayerNamesActive(false);
			return false;
		}

		private void setPlayerNamesActive(bool isActive)
		{
			nameTagsActive = isActive;
			foreach (KeyValuePair<long, PlayerNameTagAndRectTransform> activePlayerNameTag in activePlayerNameTags)
			{
				activePlayerNameTag.Value.PlayerNameTag.SetActive(isActive);
			}
		}

		private void onPlayerRemoved(RemotePlayerData remotePlayerData)
		{
			remotePlayerData.PlayerRemoved -= onPlayerRemoved;
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(remotePlayerData);
			long sessionId = dataEntityCollection.GetComponent<SessionIdData>(entityByComponent).SessionId;
			if (activePlayerNameTags.ContainsKey(sessionId))
			{
				PlayerNameTag playerNameTag = activePlayerNameTags[sessionId].PlayerNameTag;
				if (playerNameTag != null)
				{
					playerNameTag.SetActive(false);
					playerNameTagPool.Unspawn(playerNameTag.gameObject);
				}
				activePlayerNameTags.Remove(sessionId);
			}
		}

		private bool onFriendsListUpdated(FriendsServiceEvents.FriendsListUpdated evt)
		{
			foreach (KeyValuePair<long, PlayerNameTagAndRectTransform> activePlayerNameTag in activePlayerNameTags)
			{
				PlayerNameTag playerNameTag = activePlayerNameTag.Value.PlayerNameTag;
				if (playerNameTag.CurrentType != 0)
				{
					if (dataEntityCollection.HasComponent<FriendData>(playerNameTag.Handle))
					{
						playerNameTag.transform.SetParent(FriendPlayersContainer, false);
						playerNameTag.SetNameTagType(PlayerNameTag.Type.Friend);
					}
					else
					{
						playerNameTag.transform.SetParent(RemotePlayersContainer, false);
						playerNameTag.SetNameTagType(PlayerNameTag.Type.RemotePlayer);
					}
				}
			}
			return false;
		}

		private PlayerNameTag getPlayerNameTagFromPool()
		{
			PlayerNameTag playerNameTag = null;
			if (playerNameTagPool != null)
			{
				GameObject gameObject = playerNameTagPool.Spawn();
				if (!(gameObject != null))
				{
					throw new InvalidOperationException("PlayerNameController.getPlayerNameTag: Did not get player name tag from pool");
				}
				playerNameTag = gameObject.GetComponent<PlayerNameTag>();
				if (playerNameTag == null)
				{
					throw new NullReferenceException("Pooled object did not have a PlayerNameTag");
				}
			}
			return playerNameTag;
		}

		[Invokable("UI.WorldUI.PlayerNames.SetPlayerNameDistance")]
		public static void SetPlayerNameDistance(float distance)
		{
			maximumRange = distance;
		}

		[PublicTweak]
		[Invokable("Settings.SetPlayerNamesActive", Description = "Shows or hides names under the penguins.")]
		public static void SetPlayerNamesActive(bool value)
		{
			UnityEngine.Object.FindObjectOfType<PlayerNameController>().setPlayerNamesActive(value);
		}

		private void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		private void OnDestroy()
		{
			activePlayerNameTags.Clear();
		}
	}
}
