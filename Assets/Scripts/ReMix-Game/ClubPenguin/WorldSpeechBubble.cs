using ClubPenguin.Chat;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Animator))]
	public class WorldSpeechBubble : MonoBehaviour
	{
		private enum SpeechBubbleState
		{
			Inactive,
			Message,
			ChatPhraseMessage,
			AwaitingModeration,
			Blocked,
			Typing,
			TypingPending
		}

		public Text MessageText;

		public GameObject ActiveTypingPanel;

		public GameObject BlockedTextPanel;

		public RectTransform BubbleRect;

		public LayoutGroup PaddingLayoutGroup;

		public float DisplayTime;

		private bool isActive = true;

		public Material FontMaterialDefault;

		public Material FontMaterialWaiting;

		private Animator animator;

		private long sessionId;

		private string message;

		private bool isMessageShowing;

		private SpeechBubbleState currentState;

		private bool isVisible;

		private string previousEmoteMessage = "";

		private int emoteReduction = 6;

		[SerializeField]
		private int maxEmoteString = 5;

		[SerializeField]
		private int FontSizeDefault = 23;

		[SerializeField]
		private int FontSizeSingleEmote = 68;

		[SerializeField]
		private RectOffset PaddingDefault;

		[SerializeField]
		private RectOffset PaddingSingleEmote;

		public long SessionId
		{
			get
			{
				return sessionId;
			}
		}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
		}

		public event Action<WorldSpeechBubble> OnCompleteEvent;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			removeRaycastsFromText();
		}

		public void OnDestroy()
		{
			this.OnCompleteEvent = null;
			CoroutineRunner.StopAllForOwner(this);
		}

		public void ShowChatMessage(long sessionId, string message)
		{
			this.sessionId = sessionId;
			this.message = message;
			enterState(SpeechBubbleState.Message);
		}

		public void ShowChatPhraseMessage(long sessionId, string message)
		{
			this.sessionId = sessionId;
			this.message = message;
			enterState(SpeechBubbleState.ChatPhraseMessage);
		}

		public void ShowAwaitingModerationMessage(long sessionId, string message)
		{
			this.sessionId = sessionId;
			this.message = message;
			enterState(SpeechBubbleState.AwaitingModeration);
		}

		public void SetChatBlocked(long sessionId)
		{
			this.sessionId = sessionId;
			enterState(SpeechBubbleState.Blocked);
		}

		public void SetChatActive(long sessionId)
		{
			this.sessionId = sessionId;
			enterState(SpeechBubbleState.Typing);
		}

		public void SetChatInactive()
		{
			enterState(SpeechBubbleState.Inactive);
		}

		public void RebuildLayout()
		{
			LayoutRebuilder.MarkLayoutForRebuild(MessageText.transform as RectTransform);
		}

		private void enterState(SpeechBubbleState state)
		{
			switch (state)
			{
			case SpeechBubbleState.Message:
				if (currentState == SpeechBubbleState.AwaitingModeration)
				{
					setMessageToApproved();
				}
				else if (!isLocalPlayerChat())
				{
					showChatMessage(false);
				}
				currentState = state;
				break;
			case SpeechBubbleState.ChatPhraseMessage:
				showChatMessage(false);
				currentState = state;
				break;
			case SpeechBubbleState.AwaitingModeration:
				currentState = state;
				showChatMessage(true);
				break;
			case SpeechBubbleState.Blocked:
				currentState = state;
				showBlockedChat();
				break;
			case SpeechBubbleState.Typing:
				if (currentState == SpeechBubbleState.Inactive)
				{
					showActiveChat();
					currentState = state;
				}
				else if (currentState == SpeechBubbleState.Message)
				{
					currentState = SpeechBubbleState.TypingPending;
				}
				break;
			case SpeechBubbleState.Inactive:
				if (!isMessageShowing)
				{
					if (currentState == SpeechBubbleState.TypingPending)
					{
						currentState = state;
						enterState(SpeechBubbleState.Typing);
					}
					else
					{
						currentState = state;
						hideMessage();
					}
				}
				break;
			}
		}

		private void setMessageToApproved()
		{
			MessageText.material = FontMaterialDefault;
		}

		private void showChatMessage(bool isAwaitingModeration)
		{
			MessageText.gameObject.SetActive(true);
			ActiveTypingPanel.SetActive(false);
			BlockedTextPanel.SetActive(false);
			isMessageShowing = true;
			CoroutineRunner.StopAllForOwner(this);
			bool flag = message.Length <= maxEmoteString;
			int num = message.Length + previousEmoteMessage.Length;
			bool flag2 = true;
			string text = "";
			MessageText.material = FontMaterialDefault;
			if (flag)
			{
				string text2 = message;
				foreach (char c in text2)
				{
					if (!EmoteManager.IsEmoteCharacter(c))
					{
						flag2 = false;
						break;
					}
					playSoundForEmote(EmoteManager.GetEmoteFromCharacter(c));
				}
			}
			if (flag2 && flag)
			{
				text = ((num > maxEmoteString) ? message : (previousEmoteMessage + message));
				PaddingLayoutGroup.padding = PaddingSingleEmote;
				MessageText.fontSize = FontSizeSingleEmote - (text.Length - 1) * emoteReduction;
				MessageText.text = text;
				previousEmoteMessage = text;
				Service.Get<EventDispatcher>().DispatchEvent(new ChatEvents.ChatEmoteMessageShown(text, SessionId));
			}
			else
			{
				PaddingLayoutGroup.padding = PaddingDefault;
				MessageText.fontSize = FontSizeDefault;
				previousEmoteMessage = "";
				if (isAwaitingModeration)
				{
					MessageText.material = FontMaterialWaiting;
				}
				MessageText.text = message;
			}
			AccessibilitySettings component = MessageText.GetComponent<AccessibilitySettings>();
			if (component != null)
			{
				component.DynamicText = EmoteManager.GetMessageWithLocalizedEmotes(MessageText.text);
			}
			adjustBubbleSize();
			openBubble();
			CoroutineRunner.Start(waitForDisplayTime(), this, "waitForDisplayTime");
		}

		private void showActiveChat()
		{
			MessageText.gameObject.SetActive(false);
			ActiveTypingPanel.SetActive(true);
			BlockedTextPanel.SetActive(false);
			previousEmoteMessage = "";
			openBubble();
		}

		private void showBlockedChat()
		{
			MessageText.gameObject.SetActive(false);
			ActiveTypingPanel.SetActive(false);
			BlockedTextPanel.SetActive(true);
			isMessageShowing = true;
			adjustBubbleSize();
			openBubble();
			CoroutineRunner.Start(waitForDisplayTime(), this, "waitForDisplayTime");
		}

		private void openBubble()
		{
			if (isVisible)
			{
				animator.Play("ChatInWorldBubblePulse", -1, 0f);
			}
			else
			{
				animator.Play("ChatInWorldBubbleIntro", -1, 0f);
			}
			isVisible = true;
		}

		private IEnumerator waitForDisplayTime()
		{
			yield return new WaitForSeconds(DisplayTime);
			if (!base.gameObject.IsDestroyed())
			{
				isMessageShowing = false;
				enterState(SpeechBubbleState.Inactive);
			}
		}

		private void adjustBubbleSize()
		{
			BubbleRect.anchorMin = new Vector2(0.5f, 0f);
			BubbleRect.anchorMax = new Vector2(0.5f, 0f);
		}

		private void hideMessage()
		{
			animator.Play("ChatInWorldBubbleIntro_hidden", -1, 0f);
			isVisible = false;
		}

		public void MessageComplete()
		{
			previousEmoteMessage = "";
			if (this.OnCompleteEvent != null)
			{
				this.OnCompleteEvent(this);
			}
		}

		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
			base.transform.GetChild(0).gameObject.SetActive(isActive);
		}

		private void removeRaycastsFromText()
		{
			CanvasGroup canvasGroup = MessageText.gameObject.AddComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = false;
		}

		private bool isLocalPlayerChat()
		{
			return sessionId == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId;
		}

		private void OnDisable()
		{
			currentState = SpeechBubbleState.Inactive;
		}

		private void playSoundForEmote(EmoteDefinition definition)
		{
			if (!string.IsNullOrEmpty(definition.Sound))
			{
				DataEntityHandle handle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(sessionId);
				GameObjectReferenceData component;
				if (Service.Get<CPDataEntityCollection>().TryGetComponent(handle, out component))
				{
					SoundUtils.PlayAudioEvent(definition.Sound, component.GameObject);
				}
			}
		}
	}
}
