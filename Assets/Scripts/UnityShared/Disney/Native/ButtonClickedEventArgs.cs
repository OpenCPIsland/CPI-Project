using System;

namespace Disney.Native
{
	public class ButtonClickedEventArgs : EventArgs
	{
		public int Id;

		public ButtonClickedEventArgs(int aId)
		{
			Id = aId;
		}
	}
}
