using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Mix.Native;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class KeyboardController : MonoBehaviour, INativeKeyboardEvents
	{
		private NativeKeyboardManager nativeKeyboardManager;

		private void Awake()
		{
			nativeKeyboardManager = Service.Get<NativeKeyboardManager>();
		}

		private void OnDestroy()
		{
			HideKeyboard();
		}

		public void ShowInput(string text)
		{
			Color32 color = Color.black;
			string aFontColor = string.Format("#{0}{1}{2}", color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
			nativeKeyboardManager.Keyboard.ShowInput(text, "", 0, 0, 1, 1, 100, 1, aFontColor);
		}

		public void ShowKeyboard(int characterLimit, NativeKeyboardReturnKey returnKeyType, bool showSuggestions)
		{
			nativeKeyboardManager.Keyboard.ShowKeyboard(this, NativeKeyboardAlignment.Left, NativeKeyboardEntryType.Default, returnKeyType, characterLimit, showSuggestions);
		}

		public void SetText(string text)
		{
			nativeKeyboardManager.Keyboard.SetText(text);
		}

		public void HideKeyboard()
		{
			nativeKeyboardManager.Keyboard.Hide();
		}

		public void OnNativeKeyboardHiding()
		{
		}

		public void OnNativeKeyboardHidden()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(KeyboardEvents.KeyboardHidden));
		}

		public void OnNativeKeyboardShowing()
		{
		}

		public void OnNativeKeyboardShown(int aHeight)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new KeyboardEvents.KeyboardShown(aHeight));
		}

		public void OnNativeKeyboardKeyPressed(string aString)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new KeyboardEvents.KeyPressed(aString));
		}

		public void OnNativeKeyboardHeightChanged(int aHeight)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new KeyboardEvents.KeyboardResized(aHeight));
		}

		public void OnNativeKeyboardReturnKeyPressed(NativeKeyboardReturnKey aReturnKey)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new KeyboardEvents.ReturnKeyPressed(aReturnKey));
		}

		public void OnNativeKeyboardTypeChanged(NativeKeyboardType aNativeKeyboardType)
		{
		}
	}
}
