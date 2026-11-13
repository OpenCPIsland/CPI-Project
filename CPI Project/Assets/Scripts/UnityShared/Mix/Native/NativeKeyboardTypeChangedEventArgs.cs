using System;

namespace Mix.Native
{
	public class NativeKeyboardTypeChangedEventArgs : EventArgs
	{
		public NativeKeyboardType KeyboardType
		{
			get;
			set;
		}

		public NativeKeyboardTypeChangedEventArgs(NativeKeyboardType aKeyboardType)
		{
			KeyboardType = aKeyboardType;
		}
	}
}
