using System.Runtime.InteropServices;

namespace ClubPenguin
{
	public class PlayerNameEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowPlayerNames
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HidePlayerNames
		{
		}
	}
}
