using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PlayerScoreController : AvatarPositionTranslator
	{
		[SerializeField]
		private float maximumRange = 5f;

		public PrefabContentKey PlayerScorePrefabContentKey;

		private EventChannel eventChannel;

		private Dictionary<long, List<GameObject>> playerScoresDictionary;

		protected override void awakeInit()
		{
			playerScoresDictionary = new Dictionary<long, List<GameObject>>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerSpawnedEvents.RemotePlayerSpawned>(onRemotePlayerSpawned);
			eventChannel.AddListener<PlayerScoreEvents.ShowPlayerScore>(onShowPlayerScore);
			eventChannel.AddListener<PlayerScoreEvents.RemovePlayerScore>(onRemovePlayerScore);
		}

		public void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		public void Update()
		{
			foreach (KeyValuePair<long, List<GameObject>> item in playerScoresDictionary)
			{
				List<GameObject> value;
				if (playerScoresDictionary.TryGetValue(item.Key, out value) && value.Count > 0)
				{
					GameObject gameObject = value[0];
					if (gameObject != null)
					{
						Transform avatar = getAvatar(item.Key);
						if (avatar != null && avatar.gameObject.activeInHierarchy && isWithinRange(avatar, maximumRange) && isWithinViewport(avatar.position))
						{
							if (!gameObject.activeSelf)
							{
								gameObject.SetActive(true);
							}
							RectTransform component = gameObject.GetComponent<RectTransform>();
							if (isWithinViewport(avatar.position + Vector3.up * 0.8f))
							{
								component.anchoredPosition = getScreenPoint(avatar.position + Vector3.up * 0.8f);
								gameObject.GetComponentsInChildren<PlayerScoreComponent>()[0].ShowScore();
							}
							else
							{
								gameObject.SetActive(false);
							}
						}
						else if (gameObject.activeSelf)
						{
							gameObject.SetActive(false);
						}
					}
				}
			}
		}

		private bool onRemotePlayerSpawned(PlayerSpawnedEvents.RemotePlayerSpawned evt)
		{
			if (!evt.Handle.IsNull)
			{
				dataEntityCollection.GetComponent<RemotePlayerData>(evt.Handle).PlayerRemoved += onPlayerRemoved;
			}
			return false;
		}

		private void onPlayerRemoved(RemotePlayerData remotePlayerData)
		{
			remotePlayerData.PlayerRemoved -= onPlayerRemoved;
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(remotePlayerData);
			long sessionId = dataEntityCollection.GetComponent<SessionIdData>(entityByComponent).SessionId;
			if (!playerScoresDictionary.ContainsKey(sessionId))
			{
				return;
			}
			List<GameObject> value;
			if (playerScoresDictionary.TryGetValue(sessionId, out value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					removePlayerScore(sessionId, value[i]);
				}
			}
			playerScoresDictionary.Remove(sessionId);
		}

		private bool onShowPlayerScore(PlayerScoreEvents.ShowPlayerScore evt)
		{
			Content.LoadAsync(delegate(string path, GameObject prefab)
			{
				onPlayerScorePrefabLoaded(prefab, evt.PlayerId, evt.Score, evt.ParticleEffectType, evt.XPTintColor);
			}, PlayerScorePrefabContentKey);
			return false;
		}

		private void onPlayerScorePrefabLoaded(GameObject prefab, long playerId, string score, PlayerScoreEvents.ParticleType particleType, Color xpTintColor)
		{
			GameObject playerScoreObject = Object.Instantiate(prefab);
			addToPlayerScoresDictionary(playerId, playerScoreObject, score, particleType, xpTintColor);
		}

		private void addToPlayerScoresDictionary(long playerId, GameObject playerScoreObject, string score, PlayerScoreEvents.ParticleType particleType, Color xpTintColor)
		{
			List<GameObject> value;
			if (!playerScoresDictionary.ContainsKey(playerId))
			{
				value = new List<GameObject>();
				playerScoresDictionary.Add(playerId, value);
			}
			if (playerScoresDictionary.TryGetValue(playerId, out value))
			{
				value.Add(playerScoreObject);
				playerScoreObject.transform.SetParent(base.transform, false);
				playerScoreObject.GetComponentsInChildren<PlayerScoreComponent>()[0].Init(playerId, playerScoreObject, score, particleType, xpTintColor);
			}
		}

		private bool onRemovePlayerScore(PlayerScoreEvents.RemovePlayerScore evt)
		{
			removePlayerScore(evt.PlayerId, evt.PlayerScoreObject);
			return false;
		}

		private void removePlayerScore(long playerId, GameObject playerScoreObject)
		{
			if (!(playerScoreObject != null) || !playerScoresDictionary.ContainsKey(playerId))
			{
				return;
			}
			List<GameObject> value;
			if (playerScoresDictionary.TryGetValue(playerId, out value))
			{
				value.Remove(playerScoreObject);
				if (value.Count == 0)
				{
					playerScoresDictionary.Remove(playerId);
				}
			}
			Object.Destroy(playerScoreObject);
		}
	}
}
