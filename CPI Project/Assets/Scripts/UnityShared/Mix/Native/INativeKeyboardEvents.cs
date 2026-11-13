namespace Mix.Native
{
	public interface INativeKeyboardEvents
	{
		void OnNativeKeyboardHiding();

		void OnNativeKeyboardHidden();

		void OnNativeKeyboardShowing();

		void OnNativeKeyboardShown(int aHeight);

		void OnNativeKeyboardKeyPressed(string aString);

		void OnNativeKeyboardHeightChanged(int aHeight);

		void OnNativeKeyboardReturnKeyPressed(NativeKeyboardReturnKey aReturnKey);

		void OnNativeKeyboardTypeChanged(NativeKeyboardType aNativeKeyboardType);
	}
}
