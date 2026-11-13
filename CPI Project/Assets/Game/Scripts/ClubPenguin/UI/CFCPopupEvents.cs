using System.Runtime.InteropServices;

namespace ClubPenguin.UI
{
	public class CFCPopupEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CFCPopupOpened
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CFCPopupClosed
		{
		}
	}
}
