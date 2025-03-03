using ClubPenguin.Chat;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using Mix.Native;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InputBarFieldMobile : InputBarField
	{
		public Text PlaceholderText;

		public Text MessageText;

		public InputFieldCaret Caret;

		private KeyboardController keyboardController;

		private int characterLimit;

		private NativeKeyboardReturnKey returnKey;

		private string oldText;

		public string AccessibilityDeletedSpeakToken = "GlobalUI.Accessibility.Deleted";

		protected override void Awake()
		{
			keyboardController = base.gameObject.AddComponent<KeyboardController>();
			base.Awake();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			eventChannel.AddListener<KeyboardEvents.KeyPressed>(onKeyPressed);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			eventChannel.RemoveListener<KeyboardEvents.KeyPressed>(onKeyPressed);
		}

		private bool onKeyPressed(KeyboardEvents.KeyPressed evt)
		{
			oldText = base.Text;
			setText(evt.Text);
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE && oldText.Length > base.Text.Length)
			{
				string text = oldText.Substring(oldText.Length - 1, 1);
				if (EmoteManager.IsEmoteCharacter(char.Parse(text)))
				{
					string token = EmoteManager.GetToken(text);
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(token);
					MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
				}
			}
			return false;
		}

		public override void SetInputFieldSelected()
		{
			setCaretActive(true);
			base.SetInputFieldSelected();
		}

		public override void SetInputFieldDeselected()
		{
			setCaretActive(false);
		}

		public override void SetCharacterLimit(int characterLimit)
		{
			this.characterLimit = characterLimit;
		}

		public override void setViewText(string value)
		{
			MessageText.text = value;
			keyboardController.SetText(value);
			checkPlaceholderVisibility();
		}

		public override void SetPlaceholderText(string text)
		{
			PlaceholderText.text = text;
		}

		public override void SetKeyboardReturnKey(NativeKeyboardReturnKey returnKey)
		{
			this.returnKey = returnKey;
		}

		public override void ShowKeyboard()
		{
			keyboardController.ShowKeyboard(characterLimit, returnKey, base.ShowSuggestions);
			keyboardController.ShowInput(base.Text);
		}

		public override void HideKeyboard()
		{
			keyboardController.HideKeyboard();
		}

		public override void Clear()
		{
			MessageText.text = "";
			keyboardController.SetText("");
			setMessageTextActive(false);
			updateText("");
		}

		public override void Reset()
		{
			Clear();
			setCaretActive(false);
		}

		protected override Font GetFont()
		{
			return MessageText.font;
		}

		protected override void onEmoteSelected(ChatEvents.EmoteSelected evt)
		{
			string text = MessageText.text + evt.EmoteString;
			setText(text, true);
		}

		private void setText(string text, bool isEmote = false)
		{
			if (text.Length <= characterLimit)
			{
				MessageText.text = text;
				if (isEmote)
				{
					keyboardController.SetText(text);
				}
				checkPlaceholderVisibility();
				updateText(text);
			}
		}

		private void checkPlaceholderVisibility()
		{
			if (MessageText.text.Length > 0)
			{
				setMessageTextActive(true);
			}
			else
			{
				setMessageTextActive(false);
			}
		}

		private void setMessageTextActive(bool isActive)
		{
			PlaceholderText.gameObject.SetActive(!isActive);
			MessageText.gameObject.SetActive(isActive);
		}

		private void setCaretActive(bool isActive)
		{
			Caret.gameObject.SetActive(isActive);
		}
	}
}
