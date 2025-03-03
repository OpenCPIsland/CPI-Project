using ClubPenguin.Avatar;
using ClubPenguin.Cinematography;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class WorldChatController : AvatarPositionTranslator
	{
		[SerializeField]
		private float ScaleFactor;

		[SerializeField]
		private float DefaultDistance;

		[SerializeField]
		private float Offset;

		private readonly Vector3 DEFAULT_BUBBLE_SCALE = new Vector3(1f, 1f, 1f);

		private EventChannel eventChannel;

		private GameObjectPool worldSpeechBubblePool;

		private Transform cameraTransform;

		private bool ignoreRemoteChat;

		private Dictionary<long, WorldSpeechBubble> activeSpeechBubbles = new Dictionary<long, WorldSpeechBubble>();

		public bool IgnoreRemoteChat
		{
			set
			{
				ignoreRemoteChat = value;
			}
		}

		protected override void startInit()
		{
			worldSpeechBubblePool = GameObject.Find("Pool[WorldSpeechBubble]").GetComponent<GameObjectPool>();
			cameraTransform = Camera.main.transform;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<ChatMessageSender.SendChatMessage>(onSendChatMessage);
			eventChannel.AddListener<ChatServiceEvents.SendChatActivity>(onSendChatActivity);
			eventChannel.AddListener<ChatServiceEvents.SendChatActivityCancel>(onSendChatActivityCancel);
			eventChannel.AddListener<ChatServiceEvents.ChatMessageReceived>(onChatMessageReceived);
			eventChannel.AddListener<ChatServiceEvents.ChatActivityReceived>(onChatActivityReceived);
			eventChannel.AddListener<ChatServiceEvents.ChatMessageBlockedReceived>(onChatMessageBlockedReceived);
			eventChannel.AddListener<ChatServiceEvents.ChatActivityCancelReceived>(onChatActivityCancelReceived);
			eventChannel.AddListener<WorldServiceEvents.PlayerLeaveRoomEvent>(onPlayerLeaveRoom);
			eventChannel.AddListener<CinematographyEvents.RenderingStateChanged>(onRenderingStateChanged);
			eventChannel.AddListener<UIDisablerEvents.CameraCullingStateChanged>(onCameraCullingStateChanged);
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			ClearSpeechBubbles();
		}

		private void Update()
		{
			foreach (KeyValuePair<long, WorldSpeechBubble> activeSpeechBubble in activeSpeechBubbles)
			{
				RectTransform component = activeSpeechBubble.Value.GetComponent<RectTransform>();
				Transform avatar = getAvatar(activeSpeechBubble.Key);
				if (avatar != null)
				{
					AvatarView component2 = avatar.GetComponent<AvatarView>();
					Vector3 position = avatar.position;
					position.y += component2.GetBounds().extents.y * Offset;
					if (isWithinViewport(position))
					{
						if (!activeSpeechBubble.Value.IsActive)
						{
							activeSpeechBubble.Value.SetActive(true);
						}
						component.anchoredPosition = getScreenPoint(position);
						setSpeechBubbleScale(activeSpeechBubble.Value, avatar);
					}
					else if (activeSpeechBubble.Value.IsActive)
					{
						activeSpeechBubble.Value.SetActive(false);
						component.anchoredPosition = Vector3.zero;
					}
				}
			}
		}

		private void setSpeechBubbleScale(WorldSpeechBubble bubble, Transform avatarTransform)
		{
			float magnitude = (avatarTransform.position - cameraTransform.position).magnitude;
			bubble.transform.GetChild(0).transform.localScale = DEFAULT_BUBBLE_SCALE * (1f - (magnitude - DefaultDistance) * ScaleFactor);
		}

		public void ClearSpeechBubbles()
		{
			foreach (KeyValuePair<long, WorldSpeechBubble> activeSpeechBubble in activeSpeechBubbles)
			{
				if (worldSpeechBubblePool != null && activeSpeechBubble.Value.gameObject != null)
				{
					worldSpeechBubblePool.Unspawn(activeSpeechBubble.Value.gameObject);
				}
				activeSpeechBubble.Value.OnCompleteEvent -= onSpeechBubbleComplete;
			}
			activeSpeechBubbles.Clear();
		}

		public WorldSpeechBubble GetActiveSpeechBubble(long sessionID)
		{
			WorldSpeechBubble value = null;
			activeSpeechBubbles.TryGetValue(sessionID, out value);
			return value;
		}

		private bool onSendChatMessage(ChatMessageSender.SendChatMessage evt)
		{
			showChatMessage(base.localSessionId, evt.Message, (!(evt.SizzleClip == null)) ? evt.SizzleClip.Id : 0, true, evt.IsChatPhrase);
			return false;
		}

		private bool onSendChatActivity(ChatServiceEvents.SendChatActivity evt)
		{
			showActiveTyping(base.localSessionId);
			return false;
		}

		private bool onSendChatActivityCancel(ChatServiceEvents.SendChatActivityCancel evt)
		{
			if (isSpeechBubbleActive(base.localSessionId))
			{
				WorldSpeechBubble speechBubble = getSpeechBubble(base.localSessionId);
				if (speechBubble != null)
				{
					speechBubble.SetChatInactive();
				}
			}
			return false;
		}

		private bool onChatMessageBlockedReceived(ChatServiceEvents.ChatMessageBlockedReceived evt)
		{
			if (isSpeechBubbleActive(base.localSessionId))
			{
				WorldSpeechBubble speechBubble = getSpeechBubble(base.localSessionId);
				if (speechBubble != null)
				{
					speechBubble.SetChatBlocked(base.localSessionId);
				}
			}
			return false;
		}

		private bool onChatMessageReceived(ChatServiceEvents.ChatMessageReceived evt)
		{
			if (evt.SessionId == base.localSessionId || !ignoreRemoteChat)
			{
				showChatMessage(evt.SessionId, evt.Message, evt.SizzleClipID);
			}
			return false;
		}

		private bool onChatActivityReceived(ChatServiceEvents.ChatActivityReceived evt)
		{
			if (evt.SessionId != base.localSessionId && !ignoreRemoteChat)
			{
				showActiveTyping(evt.SessionId);
			}
			return false;
		}

		private bool onChatActivityCancelReceived(ChatServiceEvents.ChatActivityCancelReceived evt)
		{
			if (isSpeechBubbleActive(evt.SessionId))
			{
				WorldSpeechBubble speechBubble = getSpeechBubble(evt.SessionId);
				if (speechBubble != null)
				{
					speechBubble.SetChatInactive();
				}
			}
			return false;
		}

		private bool onPlayerLeaveRoom(WorldServiceEvents.PlayerLeaveRoomEvent evt)
		{
			if (isSpeechBubbleActive(evt.SessionId))
			{
				WorldSpeechBubble worldSpeechBubble = activeSpeechBubbles[evt.SessionId];
				worldSpeechBubblePool.Unspawn(worldSpeechBubble.gameObject);
				worldSpeechBubble.OnCompleteEvent -= onSpeechBubbleComplete;
				activeSpeechBubbles.Remove(evt.SessionId);
			}
			return false;
		}

		private bool onRenderingStateChanged(CinematographyEvents.RenderingStateChanged evt)
		{
			rebuildLayouts(evt.IsRenderingEnabled);
			return false;
		}

		private bool onCameraCullingStateChanged(UIDisablerEvents.CameraCullingStateChanged evt)
		{
			rebuildLayouts(evt.IsRenderingEnabled);
			return false;
		}

		private void rebuildLayouts(bool isEnabled)
		{
			if (isEnabled)
			{
				foreach (KeyValuePair<long, WorldSpeechBubble> activeSpeechBubble in activeSpeechBubbles)
				{
					activeSpeechBubble.Value.RebuildLayout();
				}
			}
		}

		private void onSpeechBubbleComplete(WorldSpeechBubble speechBubble)
		{
			worldSpeechBubblePool.Unspawn(speechBubble.gameObject);
			speechBubble.OnCompleteEvent -= onSpeechBubbleComplete;
			activeSpeechBubbles.Remove(speechBubble.SessionId);
		}

		private void showChatMessage(long sessionId, string message, int sizzleclipID, bool isAwaitingModeration = false, bool isLocalChatPhrase = false)
		{
			bool flag = dataEntityCollection.IsLocalPlayer(sessionId);
			if (!string.IsNullOrEmpty(message))
			{
				WorldSpeechBubble speechBubble = getSpeechBubble(sessionId);
				if (isAwaitingModeration)
				{
					if (isLocalChatPhrase)
					{
						speechBubble.ShowChatPhraseMessage(sessionId, message);
					}
					else
					{
						speechBubble.ShowAwaitingModerationMessage(sessionId, message);
					}
				}
				else if (!isLocalChatPhrase || !flag)
				{
					speechBubble.ShowChatMessage(sessionId, message);
				}
			}
			if (sizzleclipID > 0 && (isAwaitingModeration || !flag))
			{
				Transform avatar = getAvatar(sessionId);
				if (avatar != null && LocomotionUtils.CanPlaySizzle(avatar.gameObject))
				{
					Animator component = avatar.GetComponent<Animator>();
					component.SetInteger(AnimationHashes.Params.Emote, sizzleclipID);
					component.SetTrigger(AnimationHashes.Params.PlayEmote);
				}
			}
		}

		private void showActiveTyping(long sessionId)
		{
			WorldSpeechBubble speechBubble = getSpeechBubble(sessionId);
			if (speechBubble != null)
			{
				speechBubble.SetChatActive(sessionId);
			}
		}

		private bool isSpeechBubbleActive(long sessionId)
		{
			return activeSpeechBubbles.ContainsKey(sessionId);
		}

		private WorldSpeechBubble getSpeechBubble(long sessionId)
		{
			WorldSpeechBubble worldSpeechBubble = null;
			if (activeSpeechBubbles.ContainsKey(sessionId))
			{
				worldSpeechBubble = activeSpeechBubbles[sessionId];
			}
			else
			{
				GameObject gameObject = worldSpeechBubblePool.Spawn();
				if (gameObject != null)
				{
					worldSpeechBubble = gameObject.GetComponent<WorldSpeechBubble>();
					if (worldSpeechBubble == null)
					{
						throw new InvalidOperationException("WorldChatController.getSpeechBubble: Did not get speech bubble from pool");
					}
					gameObject.transform.SetParent(base.transform, false);
					gameObject.transform.localRotation = Quaternion.identity;
					worldSpeechBubble.OnCompleteEvent += onSpeechBubbleComplete;
					activeSpeechBubbles.Add(sessionId, worldSpeechBubble);
				}
			}
			return worldSpeechBubble;
		}
	}
}
