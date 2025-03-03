using System;

namespace Mix.Native
{
	public class NativeKeyboardInputHeightChangedEventArgs : EventArgs
	{
		public int Height
		{
			get;
			set;
		}

		public NativeKeyboardInputHeightChangedEventArgs(int aHeight)
		{
			Height = aHeight;
		}
	}
}
