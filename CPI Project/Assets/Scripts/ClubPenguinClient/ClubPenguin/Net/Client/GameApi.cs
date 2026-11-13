using ClubPenguin.Net.Domain;
using System;

namespace ClubPenguin.Net.Client
{
	public class GameApi
	{
		private ClubPenguinClient clubPenguinClient;

		public GameApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<GetRoomsOperation> GetRooms(string language, string[] worlds, string[] rooms, int limit = 0)
		{
			GetRoomsOperation getRoomsOperation = new GetRoomsOperation(language);
			if (worlds != null && worlds.Length > 0)
			{
				string stringToEscape = string.Join(",", Array.ConvertAll(worlds, (string x) => x.ToString()));
				getRoomsOperation.Worlds = Uri.EscapeUriString(stringToEscape);
			}
			if (rooms != null && rooms.Length > 0)
			{
				string stringToEscape2 = string.Join(",", Array.ConvertAll(rooms, (string x) => x.ToString()));
				getRoomsOperation.Rooms = Uri.EscapeUriString(stringToEscape2);
			}
			getRoomsOperation.Limit = limit;
			return new APICall<GetRoomsOperation>(clubPenguinClient, getRoomsOperation);
		}

		public APICall<DeleteSessionOperation> DeleteSession(long sessionId)
		{
			DeleteSessionOperation operation = new DeleteSessionOperation(sessionId);
			return new APICall<DeleteSessionOperation>(clubPenguinClient, operation);
		}

		public APICall<PostIglooPlayersOperation> PostIglooPlayers(ZoneId igloo, string language, bool bypassCaptcha = false)
		{
			PostIglooPlayersOperation postIglooPlayersOperation = new PostIglooPlayersOperation(igloo, language);
			postIglooPlayersOperation.BypassCaptcha = bypassCaptcha;
			return new APICall<PostIglooPlayersOperation>(clubPenguinClient, postIglooPlayersOperation);
		}

		public APICall<PostRoomPlayersOperation> PostRoomPlayers(string world, string language, string room, bool bypassCaptcha = false)
		{
			PostRoomPlayersOperation postRoomPlayersOperation = new PostRoomPlayersOperation(world, language, room);
			postRoomPlayersOperation.BypassCaptcha = bypassCaptcha;
			return new APICall<PostRoomPlayersOperation>(clubPenguinClient, postRoomPlayersOperation);
		}

		public APICall<GetRoomPopulationOperation> GetRoomPopulation(string world, string language)
		{
			GetRoomPopulationOperation operation = new GetRoomPopulationOperation(world, language);
			return new APICall<GetRoomPopulationOperation>(clubPenguinClient, operation);
		}

		public APICall<GetRoomPopulationForWorldsOperation> GetRoomPopulationForWorlds(string room, string language)
		{
			GetRoomPopulationForWorldsOperation operation = new GetRoomPopulationForWorldsOperation(room, language);
			return new APICall<GetRoomPopulationForWorldsOperation>(clubPenguinClient, operation);
		}
	}
}
