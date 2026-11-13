using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct RoomJoinError
	{
		public string roomName;

		public string errorMessage;
	}
}
