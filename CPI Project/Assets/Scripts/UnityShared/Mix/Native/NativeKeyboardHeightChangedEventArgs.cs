using System;

namespace Mix.Native
{
	public class NativeKeyboardHeightChangedEventArgs : EventArgs
	{
		public int Height
		{
			get;
			set;
		}

		public NativeKeyboardHeightChangedEventArgs(int aHeight)
		{
			Height = aHeight;
		}
	}
}
