using System;

namespace ClubPenguin.Net.Client
{
	public interface IOfflineRoomFactory
	{
		IOfflineRoomRunner Create(string room, Action<GameServerEvent, object> processEvent, Func<long> generateMMOItemId, OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager);
	}
}
