using System;

namespace Mix.Native
{
	public class NativeKeyboardReturnKeyPressedEventArgs : EventArgs
	{
		public NativeKeyboardReturnKey ReturnKeyType
		{
			get;
			set;
		}

		public NativeKeyboardReturnKeyPressedEventArgs(NativeKeyboardReturnKey aReturnKeyType)
		{
			ReturnKeyType = aReturnKeyType;
		}
	}
}
