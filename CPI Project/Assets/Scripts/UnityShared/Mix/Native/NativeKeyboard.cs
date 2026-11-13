using System;
using UnityEngine;

namespace Mix.Native
{
	public class NativeKeyboard : MonoBehaviour
	{
		protected INativeKeyboardEvents caller;

		public event EventHandler OnNativeKeyboardHiding = delegate
		{
		};

		public event EventHandler OnNativeKeyboardHidden = delegate
		{
		};

		public event EventHandler OnNativeKeyboardShowing = delegate
		{
		};

		public event EventHandler<NativeKeyboardShownEventArgs> OnNativeKeyboardShown = delegate
		{
		};

		public event EventHandler<NativeKeyboardKeyPressedEventArgs> OnNativeKeyboardKeyPressed = delegate
		{
		};

		public event EventHandler<NativeKeyboardHeightChangedEventArgs> OnNativeKeyboardHeightChanged = delegate
		{
		};

		public event EventHandler<NativeKeyboardReturnKeyPressedEventArgs> OnNativeKeyboardReturnKeyPressed = delegate
		{
		};

		public event EventHandler<NativeKeyboardTypeChangedEventArgs> OnNativeKeyboardTypeChanged = delegate
		{
		};

		public event EventHandler<NativeKeyboardInputHeightChangedEventArgs> OnNativeKeyboardInputHeightChanged = delegate
		{
		};

		public virtual void NativeLog()
		{
		}

		public virtual void Initialize()
		{
		}

		public virtual void ShowKeyboard(INativeKeyboardEvents aCaller, NativeKeyboardAlignment aAlignment = NativeKeyboardAlignment.Left, NativeKeyboardEntryType aKeyboardType = NativeKeyboardEntryType.Default, NativeKeyboardReturnKey aReturnKeyType = NativeKeyboardReturnKey.Done, int aMaxCharacters = 0, bool aShowSuggestions = false, bool aMultipleLines = false, bool aPassword = false)
		{
		}

		public virtual void ShowInput(string aPrePopulatedText, string aDefaultText, int aXPos, int aYPos, int aWidth, int aHeight, int canvasHeight, int aFontSize = 20, string aFontColor = "#000000", string aFontFace = "")
		{
		}

		public virtual void SetText(string aText)
		{
		}

		public virtual void Reposition(int aXPos, int aYPos, int aWidth, int aHeight)
		{
		}

		public virtual void Hide()
		{
		}

		public virtual void SetClipboardText(string aText)
		{
		}

		public virtual void KeyboardHidden(string aHeight)
		{
			this.OnNativeKeyboardHidden(this, EventArgs.Empty);
			if (caller != null)
			{
				caller.OnNativeKeyboardHidden();
			}
		}

		public virtual void KeyboardWillHide(string aHeight)
		{
			this.OnNativeKeyboardHiding(this, EventArgs.Empty);
			if (caller != null)
			{
				caller.OnNativeKeyboardHiding();
			}
		}

		public virtual void KeyboardShown(string aHeight)
		{
			this.OnNativeKeyboardShown(this, new NativeKeyboardShownEventArgs(int.Parse(aHeight)));
			if (caller != null)
			{
				caller.OnNativeKeyboardShown(int.Parse(aHeight));
			}
		}

		public virtual void KeyboardWillShow(string aHeight)
		{
			this.OnNativeKeyboardShowing(this, EventArgs.Empty);
			if (caller != null)
			{
				caller.OnNativeKeyboardShowing();
			}
		}

		public virtual void KeyboardHeightChanged(string aHeight)
		{
			this.OnNativeKeyboardHeightChanged(this, new NativeKeyboardHeightChangedEventArgs(int.Parse(aHeight)));
			if (caller != null)
			{
				caller.OnNativeKeyboardHeightChanged(int.Parse(aHeight));
			}
		}

		public virtual void KeyboardTypeChanged(string aType)
		{
			this.OnNativeKeyboardTypeChanged(this, new NativeKeyboardTypeChangedEventArgs((NativeKeyboardType)int.Parse(aType)));
		}

		public virtual void TextHeightChanged(string aHeight)
		{
			this.OnNativeKeyboardInputHeightChanged(this, new NativeKeyboardInputHeightChangedEventArgs(int.Parse(aHeight)));
		}

		public virtual void TextChanged(string aText)
		{
			this.OnNativeKeyboardKeyPressed(this, new NativeKeyboardKeyPressedEventArgs(aText));
			if (caller != null)
			{
				caller.OnNativeKeyboardKeyPressed(aText);
			}
		}

		public virtual void ReturnKeyPressed(string aText)
		{
			this.OnNativeKeyboardReturnKeyPressed(this, new NativeKeyboardReturnKeyPressedEventArgs((NativeKeyboardReturnKey)int.Parse(aText)));
			if (caller != null)
			{
				caller.OnNativeKeyboardReturnKeyPressed((NativeKeyboardReturnKey)int.Parse(aText));
			}
		}
	}
}
