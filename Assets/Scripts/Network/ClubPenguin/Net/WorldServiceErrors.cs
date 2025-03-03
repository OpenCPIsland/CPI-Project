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

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RoomNotFoundEvent
		{
		}
	}
}
