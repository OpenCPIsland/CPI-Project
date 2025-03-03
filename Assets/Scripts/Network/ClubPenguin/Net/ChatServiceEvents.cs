using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class ChatServiceEvents
	{
		public struct ChatActivityReceived
		{
			public readonly long SessionId;

			public ChatActivityReceived(long sessionId)
			{
				SessionId = sessionId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SendChatActivity
		{
		}

		public struct ChatActivityCancelReceived
		{
			public readonly long SessionId;

			public ChatActivityCancelReceived(long sessionId)
			{
				SessionId = sessionId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SendChatActivityCancel
		{
		}

		public struct ChatMessageReceived
		{
			public readonly long SessionId;

			public readonly string Message;

			public readonly int SizzleClipID;

			public ChatMessageReceived(long sessionId, string message, int sizzleClipID)
			{
				SessionId = sessionId;
				Message = message;
				SizzleClipID = sizzleClipID;
			}
		}

		public struct ChatMessageBlockedReceived
		{
			public readonly long SessionId;

			public ChatMessageBlockedReceived(long sessionId)
			{
				SessionId = sessionId;
			}
		}

		public class ChatMessageQuestObjective
		{
			public string QuestId;

			public string Objective;
		}
	}
}
