using System.Runtime.InteropServices;

namespace ClubPenguin
{
	public static class ChatActivityServiceEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SendChatActivity
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SendChatActivityCancel
		{
		}
	}
}
