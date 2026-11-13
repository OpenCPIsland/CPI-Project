using Disney.Kelowna.Common.DataModel;

namespace ClubPenguin
{
	public class NetworkControllerEvents
	{
		public struct LocalPlayerDataReadyEvent
		{
			public readonly DataEntityHandle Handle;

			public LocalPlayerDataReadyEvent(DataEntityHandle handle)
			{
				Handle = handle;
			}
		}

		public struct LocalPlayerJoinedRoomEvent
		{
			public readonly DataEntityHandle Handle;

			public readonly string World;

			public readonly string Room;

			public LocalPlayerJoinedRoomEvent(DataEntityHandle handle, string world, string room)
			{
				Handle = handle;
				World = world;
				Room = room;
			}
		}

		public struct RemotePlayerJoinedRoomEvent
		{
			public readonly DataEntityHandle Handle;

			public RemotePlayerJoinedRoomEvent(DataEntityHandle handle)
			{
				Handle = handle;
			}
		}

		public struct RemotePlayerLeftRoomEvent
		{
			public readonly DataEntityHandle Handle;

			public RemotePlayerLeftRoomEvent(DataEntityHandle handle)
			{
				Handle = handle;
			}
		}
	}
}
