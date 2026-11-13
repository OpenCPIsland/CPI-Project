using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Mix.Native;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public abstract class InputBarField : MonoBehaviour
	{
		public System.Action ESendButtonClicked;

		public System.Action EEmojiButtonClicked;

		public System.Action EKeyboardButtonClicked;

		public Action<string> OnTextChanged;

		[SerializeField]
		private Button sendButton = null;

		[SerializeField]
		private Button emojiButton = null;

		[SerializeField]
		private Button keyboardButton = null;

		[SerializeField]
		private GameObject fadeOverlay = null;

		protected EventChannel eventChannel;

		private string text = "";

		public bool ShowSuggestions
		{
			get;
			set;
		}

		public bool CatchEmotes
		{
			get;
			set;
		}

		public bool OpenKeyboardOnSelect
		{
			get;
			set;
		}

		public string Text
		{
			get
			{
				return text;
			}
			private set
			{
				text = value;
			}
		}

		protected virtual void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			SetEmojiButtonVisibility(false);
			SetKeyboardButtonVisibility(false);
			OpenKeyboardOnSelect = true;
		}

		protected virtual void OnEnable()
		{
			eventChannel.AddListener<ChatEvents.EmoteSelected>(onEmoteSelectedListener);
			eventChannel.AddListener<ChatEvents.ChatBackSpace>(onBackSpacePressed);
		}

		protected virtual void OnDisable()
		{
			eventChannel.RemoveListener<ChatEvents.EmoteSelected>(onEmoteSelectedListener);
			eventChannel.RemoveListener<ChatEvents.ChatBackSpace>(onBackSpacePressed);
		}

		public abstract void setViewText(string value);

		public virtual void SetInputFieldSelected()
		{
			if (OpenKeyboardOnSelect)
			{
				ShowKeyboard();
			}
		}

		public abstract void SetInputFieldDeselected();

		public abstract void SetCharacterLimit(int limit);

		public abstract void SetPlaceholderText(string text);

		public virtual void SetKeyboardReturnKey(NativeKeyboardReturnKey returnKey)
		{
		}

		public abstract void ShowKeyboard();

		public virtual void HideKeyboard()
		{
		}

		public abstract void Clear();

		public abstract void Reset();

		protected abstract Font GetFont();

		private bool onEmoteSelectedListener(ChatEvents.EmoteSelected evt)
		{
			if (CatchEmotes)
			{
				onEmoteSelected(evt);
			}
			return false;
		}

		protected abstract void onEmoteSelected(ChatEvents.EmoteSelected evt);

		protected void updateText(string value)
		{
			string a = filterText(value);
			if (a != Text)
			{
				Text = a;
				if (OnTextChanged != null)
				{
					OnTextChanged(Text);
				}
			}
			if (a != value)
			{
				setViewText(Text);
			}
		}

		private string filterText(string text)
		{
			Font font = GetFont();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in text)
			{
				if (font.HasCharacter(c))
				{
					stringBuilder.Append(c);
				}
				else
				{
					flag = true;
				}
			}
			return flag ? stringBuilder.ToString() : text;
		}

		private bool onBackSpacePressed(ChatEvents.ChatBackSpace evt)
		{
			int length = Text.Length;
			if (length > 0)
			{
				Text = Text.Substring(0, length - 1);
				setViewText(Text);
				updateText(Text);
			}
			return false;
		}

		public void OnSendButtonClicked()
		{
			if (ESendButtonClicked != null)
			{
				ESendButtonClicked();
			}
		}

		public void SetSendButtonVisibility(bool visible)
		{
			if (sendButton != null)
			{
				sendButton.gameObject.SetActive(visible);
			}
		}

		public void OnEmojiButtonClicked()
		{
			if (EEmojiButtonClicked != null)
			{
				EEmojiButtonClicked();
			}
		}

		public void SetEmojiButtonVisibility(bool visible)
		{
			if (emojiButton != null)
			{
				emojiButton.gameObject.SetActive(visible);
			}
		}

		public void OnKeyboardButtonClicked()
		{
			if (EKeyboardButtonClicked != null)
			{
				EKeyboardButtonClicked();
			}
		}

		public void SetKeyboardButtonVisibility(bool visible)
		{
			if (keyboardButton != null)
			{
				keyboardButton.gameObject.SetActive(visible);
			}
		}

		public void setFadeOverlayVisibility(bool visible)
		{
			if (fadeOverlay != null)
			{
				fadeOverlay.SetActive(visible);
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			OnTextChanged = null;
			ESendButtonClicked = null;
			EEmojiButtonClicked = null;
			EKeyboardButtonClicked = null;
		}
	}
}
