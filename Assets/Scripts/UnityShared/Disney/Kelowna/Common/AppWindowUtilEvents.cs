using System.Runtime.InteropServices;

namespace Disney.Kelowna.Common
{
	public static class AppWindowUtilEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct WindowCloseClickedEvent
		{
		}
	}
}
