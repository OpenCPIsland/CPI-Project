using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class WorldServiceErrors
	{
		public struct WorldDisconnectedEvent
		{
			public readonly string ErrorMessage;

			public WorldDisconnectedEvent(string errorMessage)
			{
				ErrorMessage = errorMessage;
			}
		}

		public struct WorldNetworkErrorEvent
		{
			public readonly string ErrorMessage;

			public WorldNetworkErrorEvent(string errorMessage)
			{
				ErrorMessage = errorMessage;
			}
		}

		public struct ServerRestartingEvent
		{
			public readonly string Message;

			public ServerRestartingEvent(string message)
			{
				Message = message;
			}
		}

		public struct ServerAlertEvent
		{
			public readonly string Message;

			public ServerAlertEvent(string message)
			{
				Message = message;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RoomNotFoundEvent
		{
		}
	}
}
