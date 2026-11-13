using ClubPenguin.Net.Domain;

namespace ClubPenguin.Net.Offline
{
	public struct SessionData : IOfflineData
	{
		public ClubPenguin.Net.Domain.SessionData Data;

		public RoomIdentifier CurrentRoom;

		public void Init()
		{
			CurrentRoom = new RoomIdentifier();
		}
	}
}
