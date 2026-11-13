using System;

namespace Tweaker
{
	[Flags]
	public enum TweakerOptionFlags
	{
		None = 0x0,
		Default = 0x1,
		ScanForInvokables = 0x2,
		ScanForTweakables = 0x4,
		ScanForWatchables = 0x8,
		ScanInEverything = 0x10,
		ScanInEntryAssembly = 0x20,
		ScanInExecutingAssembly = 0x40,
		ScanInNonSystemAssemblies = 0x80,
		DoNotAutoScan = 0x100,
		IncludeTests = 0x200
	}
}
