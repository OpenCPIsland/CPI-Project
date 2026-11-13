using System;

namespace Mix.Native
{
	public class NativeKeyboardKeyPressedEventArgs : EventArgs
	{
		public string InputText
		{
			get;
			set;
		}

		public NativeKeyboardKeyPressedEventArgs(string aText)
		{
			InputText = aText;
		}
	}
}
