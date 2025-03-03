using System;

namespace Tweaker.UI
{
	[Flags]
	public enum TweakableUIFlags
	{
		None = 0x0,
		HideRangeSlider = 0x1,
		RethrowExceptions = 0x2
	}
}
