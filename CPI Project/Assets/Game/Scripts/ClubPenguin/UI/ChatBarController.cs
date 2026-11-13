using ClubPenguin.Analytics;
using ClubPenguin.Chat;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using Mix.Native;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ChatBarController : PassiveStateHandler
	{
		private enum ChatBarWidth
		{
			Closed,
			Open
		}

		public const string DEFAULT = "Default";

		public const string INSTANT = "Instant";

		public const string EMOTE_INSTANT = "EmoteInstant";

		public const string EMOTE_INPUT = "EmoteInput";

		public const string EXIT = "Exit";

		private static int ANIM_OPEN_MENU = Animator.StringToHash("OpenMenu");

		private static int ANIM_CLOSE_MENU = Animator.StringToHash("CloseMenu");

		public GameObject CloseButtonContainer;

		public OnOffSpriteSelector InstantButton;

		public OnOffSpriteSelector EmoteButton;

		public Animator ChatBarAnimator;

		public Transform QuickEmoteLocation;

		public Transform QuickChatLocation;

		public Transform EmoteLocation;

		public int CharacterLimit;

		[Header("Placeholder Tags")]
		public string InputPlaceholderToken;

		public string InstantPlaceholderToken;

		private InputBarFieldLoader inputBarFieldLoader;

		private Animator instantButtonAnimator;

		private StateMachineContext smc;

		private ChatBarState currentState;

		private ChatBarWidth currentChatBarWidth;

		private KeyboardEventSource rootKeyboardEventSource;

		public InputBarField InputBarField
		{
			get;
			private set;
		}

		public ChatBarState CurrentState
		{
			get
			{
				return currentState;
			}
		}

		private event System.Action onAnimationComplete;

		private void Awake()
		{
			instantButtonAnimator = GetComponent<Animator>();
			inputBarFieldLoader = base.gameObject.GetComponent<InputBarFieldLoader>();
			InputBarFieldLoader obj = inputBarFieldLoader;
			obj.OnInputBarFieldLoaded = (Action<InputBarField>)Delegate.Combine(obj.OnInputBarFieldLoaded, new Action<InputBarField>(onInputBarFieldLoaded));
			inputBarFieldLoader.LoadInputBarField();
		}

		private IEnumerator Start()
		{
			smc = GetComponentInParent<StateMachineContext>();
			CloseButtonContainer.SetActive(true);
			findRootKeyboardEventSource();
			if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
			{
				while (!smc.ContainsStateMachine("Root"))
				{
					yield return null;
				}
				smc.SendEvent(new ExternalEvent("ChatFullScreenLoader", "enable"));
			}
		}

		private void OnEnable()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<ChatEvents.EmoteSelected>(onEmoteSelected);
			eventDispatcher.AddListener<KeyboardEvents.ReturnKeyPressed>(onReturnKeyPressed);
			eventDispatcher.AddListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
			eventDispatcher.AddListener<ChatEvents.InputBarLoaded>(onChatBarOpened);
			eventDispatcher.DispatchEvent(default(ChatEvents.ChatOpened));
		}

		private void OnDisable()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.RemoveListener<ChatEvents.EmoteSelected>(onEmoteSelected);
			eventDispatcher.RemoveListener<KeyboardEvents.ReturnKeyPressed>(onReturnKeyPressed);
			eventDispatcher.RemoveListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
			eventDispatcher.RemoveListener<ChatEvents.InputBarLoaded>(onChatBarOpened);
			eventDispatcher.DispatchEvent(default(ChatEvents.ChatClosed));
		}

		private void findRootKeyboardEventSource()
		{
			KeyboardEventSource[] components = GetComponents<KeyboardEventSource>();
			int num = 0;
			while (true)
			{
				if (num < components.Length)
				{
					if (components[num].Target == "Root")
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			rootKeyboardEventSource = components[num];
		}

		private void onInputBarFieldLoaded(InputBarField inputBarField)
		{
			if (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape)
			{
				CoroutineRunner.Start(CheckRootState(), this, "CheckRootState");
			}
			InputBarFieldLoader obj = inputBarFieldLoader;
			obj.OnInputBarFieldLoaded = (Action<InputBarField>)Delegate.Remove(obj.OnInputBarFieldLoaded, new Action<InputBarField>(onInputBarFieldLoaded));
			InputBarField = inputBarField;
			inputBarField.SetCharacterLimit(CharacterLimit);
			inputBarField.SetKeyboardReturnKey(NativeKeyboardReturnKey.Send);
			inputBarField.ShowSuggestions = true;
			updateStateUI();
			inputBarField.OnTextChanged = (Action<string>)Delegate.Combine(inputBarField.OnTextChanged, new Action<string>(onTextChanged));
			inputBarField.ESendButtonClicked = (System.Action)Delegate.Combine(inputBarField.ESendButtonClicked, new System.Action(OnSendButtonClicked));
			inputBarField.EEmojiButtonClicked = (System.Action)Delegate.Combine(inputBarField.EEmojiButtonClicked, new System.Action(onInputBarEmojiButtonClicked));
			inputBarField.EKeyboardButtonClicked = (System.Action)Delegate.Combine(inputBarField.EKeyboardButtonClicked, new System.Action(onInputBarKeyboardButtonClicked));
		}

		private bool onChatBarOpened(ChatEvents.InputBarLoaded evt)
		{
			if (PlatformUtils.GetAspectRatioType() == AspectRatioType.Landscape)
			{
				StateMachine component = GetComponent<StateMachine>();
				if (component.CurrentStateName == "EmoteInput")
				{
					component.SendEvent("keyboardOpen");
				}
			}
			return false;
		}

		private IEnumerator CheckRootState()
		{
			WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();
			StateMachineContext smc = GetComponentInParent<StateMachineContext>();
			while (base.gameObject != null)
			{
				string state = smc.GetStateMachineState("Root");
				if (smc != null && (state == "ChatInputLoader" || state == "ChatInstantEmoteLoader_Forced"))
				{
					smc.SendEvent(new ExternalEvent("Root", "chat_loaded"));
				}
				yield return waitForFrame;
			}
		}

		public override void HandleStateChange(string newState)
		{
			currentState = (ChatBarState)Enum.Parse(typeof(ChatBarState), newState);
			Service.Get<EventDispatcher>().DispatchEvent(new ChatBarEvents.ChatBarStateChanged(currentState));
			updateStateUI();
		}

		private void updateStateUI()
		{
			rootKeyboardEventSource.enabled = (currentState == ChatBarState.Default);
			switch (currentState)
			{
			case ChatBarState.Default:
			case ChatBarState.EmoteInput:
				showInputBarState();
				break;
			case ChatBarState.Instant:
			case ChatBarState.EmoteInstant:
				showInstantBarState();
				break;
			case ChatBarState.Exit:
				if (ChatBarAnimator != null && currentChatBarWidth == ChatBarWidth.Open)
				{
					setChatBarWidth(ChatBarWidth.Closed);
					onAnimationComplete += onExitAnimationComplete;
				}
				else
				{
					smc.SendEvent(new ExternalEvent("Root", "exitFinished"));
				}
				break;
			}
		}

		private void onExitAnimationComplete()
		{
			smc.SendEvent(new ExternalEvent("Root", "exitFinished"));
			onAnimationComplete -= onExitAnimationComplete;
		}

		private void showInputBarState()
		{
			instantButtonAnimator.SetBool(ANIM_OPEN_MENU, false);
			instantButtonAnimator.SetBool(ANIM_CLOSE_MENU, true);
			setChatBarWidth(ChatBarWidth.Open);
			if (InputBarField != null)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(InputPlaceholderToken);
				InputBarField.SetPlaceholderText(tokenTranslation);
				InputBarField.SetSendButtonVisibility(true);
				InputBarField.SetEmojiButtonVisibility(currentState == ChatBarState.Default);
				InputBarField.SetKeyboardButtonVisibility(currentState == ChatBarState.EmoteInput);
				InputBarField.setFadeOverlayVisibility(false);
				InputBarField.CatchEmotes = true;
				InputBarField.OpenKeyboardOnSelect = false;
				if (currentState == ChatBarState.Default)
				{
					InputBarField.SetInputFieldSelected();
					smc.SendEvent(new ExternalEvent("Root", "chat_loaded"));
					EmoteButton.IsOn = false;
				}
				else
				{
					InputBarField.SetInputFieldDeselected();
				}
			}
			InstantButton.IsOn = false;
			setKeyboardVisibility(currentState == ChatBarState.Default);
		}

		private void showInstantBarState()
		{
			instantButtonAnimator.SetBool(ANIM_CLOSE_MENU, false);
			instantButtonAnimator.SetBool(ANIM_OPEN_MENU, true);
			setChatBarWidth(ChatBarWidth.Closed);
			if (InputBarField != null)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(InstantPlaceholderToken);
				InputBarField.SetPlaceholderText(tokenTranslation);
				InputBarField.SetSendButtonVisibility(false);
				InputBarField.SetEmojiButtonVisibility(false);
				InputBarField.SetKeyboardButtonVisibility(false);
				InputBarField.setFadeOverlayVisibility(true);
				InputBarField.CatchEmotes = false;
				InputBarField.OpenKeyboardOnSelect = true;
				InputBarField.SetInputFieldDeselected();
			}
			InstantButton.IsOn = (currentState == ChatBarState.Instant);
			EmoteButton.IsOn = (currentState == ChatBarState.EmoteInstant);
			setKeyboardVisibility(false);
		}

		private void setChatBarWidth(ChatBarWidth chatBarWidth)
		{
			currentChatBarWidth = chatBarWidth;
			if (ChatBarAnimator != null)
			{
				switch (chatBarWidth)
				{
				case ChatBarWidth.Open:
					ChatBarAnimator.ResetTrigger("Exit");
					ChatBarAnimator.SetTrigger("Enter");
					break;
				case ChatBarWidth.Closed:
					ChatBarAnimator.ResetTrigger("Enter");
					ChatBarAnimator.SetTrigger("Exit");
					break;
				}
			}
		}

		private void setKeyboardVisibility(bool visible)
		{
			if (InputBarField != null)
			{
				if (visible)
				{
					InputBarField.ShowKeyboard();
				}
				else
				{
					InputBarField.HideKeyboard();
				}
			}
		}

		public void OnSendButtonClicked()
		{
			sendChatMessage();
		}

		private void onInputBarEmojiButtonClicked()
		{
			if (smc != null)
			{
				smc.SendEvent(new ExternalEvent("InputBarChat", "emote_input"));
				if (ClubPenguin.Core.SceneRefs.Get<IScreenContainerStateHandler>().IsKeyboardShown)
				{
					Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardClosedAfterEmojiButtonClicked);
				}
				else
				{
					sendEmoteEvent();
				}
			}
		}

		private bool onKeyboardClosedAfterEmojiButtonClicked(KeyboardEvents.KeyboardHidden evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardClosedAfterEmojiButtonClicked);
			sendEmoteEvent();
			return false;
		}

		public void OnAnimationComplete()
		{
			this.onAnimationComplete.InvokeSafe();
		}

		private void sendEmoteEvent()
		{
			if (smc != null)
			{
				smc.SendEvent(new ExternalEvent("ChatScreenPanel", "emote"));
			}
		}

		private void onInputBarKeyboardButtonClicked()
		{
			if (smc != null)
			{
				smc.SendEvent(new ExternalEvent("InputBarChat", "keyboardOpen"));
			}
		}

		private bool onKeyboardShown(KeyboardEvents.KeyboardShown evt)
		{
			if (smc != null)
			{
				smc.SendEvent(new ExternalEvent("ChatScreenPanel", "keyboardOpen"));
			}
			return false;
		}

		private void onTextChanged(string value)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatActivityServiceEvents.SendChatActivity));
		}

		private bool onReturnKeyPressed(KeyboardEvents.ReturnKeyPressed evt)
		{
			sendChatMessage();
			return false;
		}

		private void sendChatMessage()
		{
			string text = InputBarField.Text.Trim();
			InputBarField.Clear();
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				string messageWithLocalizedEmotes = EmoteManager.GetMessageWithLocalizedEmotes(text);
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(messageWithLocalizedEmotes);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new ChatMessageSender.SendChatMessage(text, null));
			int num = 0;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < text.Length; i++)
			{
				if (EmoteManager.IsEmoteCharacter(text[i]))
				{
					num2++;
				}
				else if (!char.IsWhiteSpace(text[i]) && !flag)
				{
					flag = true;
					num++;
				}
				else if (char.IsWhiteSpace(text[i]))
				{
					flag = false;
				}
			}
			Service.Get<ICPSwrveService>().Action("game.chat", text.Length.ToString(), num.ToString(), num2.ToString());
			smc.SendEvent(new ExternalEvent("Root", "chat_close"));
		}

		private bool onEmoteSelected(ChatEvents.EmoteSelected evt)
		{
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatActivityServiceEvents.SendChatActivityCancel));
			CoroutineRunner.StopAllForOwner(this);
		}

		public void OnQuickEmoteButtonPressed()
		{
			string stateMachineState = smc.GetStateMachineState("InputBarChat");
			if (stateMachineState != "EmoteInstant")
			{
				smc.SendEvent(new ExternalEvent("Root", "chat_instant_emote"));
			}
			else
			{
				smc.SendEvent(new ExternalEvent("InputBarChat", "keyboardOpen"));
			}
		}

		public void OnQuickChatButtonPressed()
		{
			string stateMachineState = smc.GetStateMachineState("InputBarChat");
			if (stateMachineState != "Instant")
			{
				smc.SendEvent(new ExternalEvent("Root", "chat_instant"));
			}
			else
			{
				smc.SendEvent(new ExternalEvent("InputBarChat", "keyboardOpen"));
			}
		}
	}
}
