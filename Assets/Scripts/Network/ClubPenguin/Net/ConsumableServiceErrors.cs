using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class ConsumableServiceErrors
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct NotEnoughCoins
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct Unknown
		{
		}
	}
}
