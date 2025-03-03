using System.Collections.Generic;

namespace ClubPenguin.PartyGames
{
	public interface IPartyGameSession
	{
		void StartGame(int id, List<PartyGamePlayer> players, int partyGameId);

		void EndGame(Dictionary<long, int> playerSessionIdToPlacement);

		void HandleSessionMessage(PartyGameSessionMessageTypes type, string data);
	}
}
