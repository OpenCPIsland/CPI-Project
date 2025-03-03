using System.Runtime.InteropServices;

namespace ClubPenguin.World
{
	public class InWorldUIEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableInWorldText
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableInWorldText
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableActionIndicators
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableActionIndicators
		{
		}
	}
}
