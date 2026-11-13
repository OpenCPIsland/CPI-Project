using System;

namespace Disney.Native
{
	public class ToggleAccessibilitiesEventArgs : EventArgs
	{
		public bool IsOn;

		public ToggleAccessibilitiesEventArgs(bool aIsOn)
		{
			IsOn = aIsOn;
		}
	}
}
