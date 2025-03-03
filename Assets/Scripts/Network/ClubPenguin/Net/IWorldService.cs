using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Net
{
	public interface IWorldService : INetworkService
	{
		void GetWorldsWithRoomPopulation(string room, string language);

		void JoinRoom(string room, string contentIdentifier, string language, IJoinRoomByNameErrorHandler errorHandler);

		void JoinRoomInWorld(RoomIdentifier room, IJoinRoomErrorHandler errorHandler);

		void JoinIgloo(ZoneId igloo, string language, IJoinIglooErrorHandler errorHandler);

		void LeaveRoom(bool immediately = false);

		void SetContentDate(DateTime currentDate);

		void GetRoomPopulation(string world, string language);
	}
}
