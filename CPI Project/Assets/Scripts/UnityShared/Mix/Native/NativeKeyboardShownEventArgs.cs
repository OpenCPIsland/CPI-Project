using System;

namespace Mix.Native
{
	public class NativeKeyboardShownEventArgs : EventArgs
	{
		public int Height
		{
			get;
			set;
		}

		public NativeKeyboardShownEventArgs(int aHeight)
		{
			Height = aHeight;
		}
	}
}
