using System.Runtime.InteropServices;

namespace ClubPenguin.SpecialEvents
{
	public static class ScheduledCoreEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowAdjustments
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideAdjustments
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowFog
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideFog
		{
		}
	}
}
