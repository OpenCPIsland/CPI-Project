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
        public const char SystemMessagePrefix = '\u200B';
        public const char SystemErrorPrefix = '\u200C';

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

        private Text[] blockedTextComponents;
        private string[] blockedTextOriginals;

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
            cacheBlockedTextDefaults();
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

        private static bool isSystemMessage(string msg)
        {
            return !string.IsNullOrEmpty(msg) && msg.Length >= 1 && msg[0] == SystemMessagePrefix;
        }

        private static bool isSystemErrorMessage(string msg)
        {
            return !string.IsNullOrEmpty(msg) && msg.Length >= 2 && msg[0] == SystemMessagePrefix && msg[1] == SystemErrorPrefix;
        }

        private static string stripSystemPrefixes(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return msg;
            }

            if (msg.Length >= 2 && msg[0] == SystemMessagePrefix && msg[1] == SystemErrorPrefix)
            {
                return msg.Substring(2);
            }

            if (msg.Length >= 1 && msg[0] == SystemMessagePrefix)
            {
                return msg.Substring(1);
            }

            return msg;
        }

        private void showChatMessage(bool isAwaitingModeration)
        {
            bool system = isSystemMessage(message);
            bool systemError = isSystemErrorMessage(message);
            string renderMessage = stripSystemPrefixes(message);

            if (systemError)
            {
                showCustomBlockedChat(renderMessage);
                return;
            }

            MessageText.gameObject.SetActive(true);
            ActiveTypingPanel.SetActive(false);
            BlockedTextPanel.SetActive(false);
            isMessageShowing = true;
            CoroutineRunner.StopAllForOwner(this);

            bool flag = renderMessage.Length <= maxEmoteString;
            int num = renderMessage.Length + previousEmoteMessage.Length;
            bool flag2 = true;
            string text = "";

            MessageText.material = FontMaterialDefault;

            if (isAwaitingModeration && !system)
            {
                MessageText.material = FontMaterialWaiting;
            }

            if (flag)
            {
                string text2 = renderMessage;
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
                text = ((num > maxEmoteString) ? renderMessage : (previousEmoteMessage + renderMessage));
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
                MessageText.text = renderMessage;
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
            restoreBlockedTextDefaults();
            MessageText.gameObject.SetActive(false);
            ActiveTypingPanel.SetActive(false);
            BlockedTextPanel.SetActive(true);
            isMessageShowing = true;
            adjustBubbleSize();
            openBubble();
            CoroutineRunner.Start(waitForDisplayTime(), this, "waitForDisplayTime");
        }

        private void showCustomBlockedChat(string blockedText)
        {
            setBlockedText(blockedText);
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

        private void cacheBlockedTextDefaults()
        {
            if (BlockedTextPanel == null)
            {
                return;
            }

            blockedTextComponents = BlockedTextPanel.GetComponentsInChildren<Text>(true);
            if (blockedTextComponents == null || blockedTextComponents.Length == 0)
            {
                return;
            }

            blockedTextOriginals = new string[blockedTextComponents.Length];
            for (int i = 0; i < blockedTextComponents.Length; i++)
            {
                blockedTextOriginals[i] = blockedTextComponents[i] != null ? blockedTextComponents[i].text : null;
            }
        }

        private void restoreBlockedTextDefaults()
        {
            if (blockedTextComponents == null || blockedTextOriginals == null)
            {
                return;
            }

            for (int i = 0; i < blockedTextComponents.Length && i < blockedTextOriginals.Length; i++)
            {
                if (blockedTextComponents[i] != null && blockedTextOriginals[i] != null)
                {
                    blockedTextComponents[i].text = blockedTextOriginals[i];
                }
            }
        }

        private void setBlockedText(string text)
        {
            if (BlockedTextPanel == null)
            {
                return;
            }

            if (blockedTextComponents == null || blockedTextComponents.Length == 0)
            {
                cacheBlockedTextDefaults();
            }

            if (blockedTextComponents == null)
            {
                return;
            }

            for (int i = 0; i < blockedTextComponents.Length; i++)
            {
                if (blockedTextComponents[i] != null)
                {
                    blockedTextComponents[i].text = text;
                }
            }
        }
    }
}