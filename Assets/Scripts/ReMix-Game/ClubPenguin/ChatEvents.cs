using System.Runtime.InteropServices;

namespace ClubPenguin
{
	public static class ChatEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowFullScreen
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideFullScreen
		{
		}

		public struct EmoteSelected
		{
			public readonly string EmoteString;

			public EmoteSelected(string emoteString)
			{
				EmoteString = emoteString;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SizzleClipSelected
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SizzleClipDeselected
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct OpenChatBar
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct InputBarLoaded
		{
		}

		public struct ChatEmoteMessageShown
		{
			public readonly string Message;

			public readonly long SessionId;

			public ChatEmoteMessageShown(string message, long sessionId)
			{
				Message = message;
				SessionId = sessionId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ChatBackSpace
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ChatOpened
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ChatClosed
		{
		}
	}
}
