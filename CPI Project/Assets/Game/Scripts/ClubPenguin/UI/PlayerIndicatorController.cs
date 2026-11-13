using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PlayerIndicatorController : AvatarPositionTranslator
	{
		[SerializeField]
		private float maximumRange = 5f;

		private EventChannel eventChannel;

		private Dictionary<long, GameObject> activePlayerIndicator;

		protected override void awakeInit()
		{
			activePlayerIndicator = new Dictionary<long, GameObject>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PlayerSpawnedEvents.RemotePlayerSpawned>(onRemotePlayerSpawned);
			eventChannel.AddListener<PlayerIndicatorEvents.ShowPlayerIndicator>(onShowPlayerIndicator);
			eventChannel.AddListener<PlayerIndicatorEvents.RemovePlayerIndicator>(onRemovePlayerIndicator);
			eventChannel.AddListener<PlayerIndicatorEvents.HidePlayerIndicators>(onHidePlayerIndicators);
			eventChannel.AddListener<PlayerIndicatorEvents.ShowPlayerIndicators>(onShowPlayerIndicators);
		}

		public void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		public void Update()
		{
			if (localPlayer == null && activePlayerIndicator.Count > 0)
			{
				localPlayer = getAvatar(base.localSessionId);
			}
			foreach (KeyValuePair<long, GameObject> item in activePlayerIndicator)
			{
				Transform avatar = getAvatar(item.Key);
				bool flag = isLocalPlayer(item.Key);
				if (avatar != null && avatar.gameObject.activeInHierarchy && (flag || isWithinRange(avatar, maximumRange)))
				{
					if (!item.Value.activeSelf)
					{
						item.Value.SetActive(true);
					}
					RectTransform component = item.Value.GetComponent<RectTransform>();
					if (isWithinViewport(avatar.position + Vector3.up * 0.8f))
					{
						component.anchoredPosition = getScreenPoint(avatar.position + Vector3.up * 0.8f);
					}
					else
					{
						item.Value.SetActive(false);
					}
				}
				else if (item.Value.activeSelf)
				{
					item.Value.SetActive(false);
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
			if (activePlayerIndicator.ContainsKey(sessionId))
			{
				removePlayerIndicator(sessionId, false);
			}
		}

		private bool onShowPlayerIndicator(PlayerIndicatorEvents.ShowPlayerIndicator evt)
		{
			if (activePlayerIndicator.ContainsKey(evt.PlayerId))
			{
				throw new InvalidOperationException("Indicator already exists for this Player Id");
			}
			evt.IndicatorObject.transform.SetParent(base.transform, false);
			activePlayerIndicator.Add(evt.PlayerId, evt.IndicatorObject);
			return false;
		}

		private bool onRemovePlayerIndicator(PlayerIndicatorEvents.RemovePlayerIndicator evt)
		{
			removePlayerIndicator(evt.PlayerId, evt.IsStored, evt.Destroy);
			return false;
		}

		private void removePlayerIndicator(long playerId, bool isStored, bool destroy = true)
		{
			if (activePlayerIndicator == null || !activePlayerIndicator.ContainsKey(playerId))
			{
				return;
			}
			GameObject gameObject = activePlayerIndicator[playerId];
			activePlayerIndicator.Remove(playerId);
			SessionIdData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				long sessionId = component.SessionId;
				if (playerId == sessionId && isStored)
				{
					tweenLocalPlayerIndicator(gameObject);
				}
				else if (destroy)
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			else if (destroy)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		private bool onHidePlayerIndicators(PlayerIndicatorEvents.HidePlayerIndicators evt)
		{
			GetComponentInChildren<Canvas>().enabled = false;
			return false;
		}

		private bool onShowPlayerIndicators(PlayerIndicatorEvents.ShowPlayerIndicators evt)
		{
			GetComponentInChildren<Canvas>().enabled = true;
			return false;
		}

		private void tweenLocalPlayerIndicator(GameObject go)
		{
			if (!go.IsDestroyed())
			{
				InvitationIndicatorController componentInChildren = go.GetComponentInChildren<InvitationIndicatorController>();
				if (componentInChildren != null)
				{
					componentInChildren.TweenToMainNav();
				}
			}
		}
	}
}
