using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using hg.ApiWebKit.core.http;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public class JoinRoomSequence
	{
		private ClubPenguinClient clubPenguinClient;

		private string roomName;

		private string language;

		private Action successHandler;

		private IJoinRoomByNameErrorHandler errorHandler;

		public JoinRoomSequence(ClubPenguinClient client, string room, string language, IJoinRoomByNameErrorHandler errorHandler, Action successHandler = null)
		{
			clubPenguinClient = client;
			roomName = room;
			this.language = language;
			this.successHandler = successHandler;
			this.errorHandler = errorHandler;
		}

		public void JoinRoom()
		{
			string[] worlds = null;
			string[] rooms = new string[1]
			{
				roomName
			};
			APICall<GetRoomsOperation> rooms2 = clubPenguinClient.GameApi.GetRooms(language, worlds, rooms, 1);
			rooms2.OnResponse += onRoomsFoundToJoin;
			rooms2.OnError += onRoomsFoundToJoinError;
			rooms2.Execute();
		}

		private void onRoomsFoundToJoin(GetRoomsOperation operation, HttpResponse httpResponse)
		{
			List<RoomIdentifier> roomsFound = operation.RoomsFound;
			if (roomsFound.Count > 0)
			{
				JoinRoomInWorldSequence joinRoomInWorldSequence = new JoinRoomInWorldSequence(clubPenguinClient, roomsFound[0], errorHandler, successHandler);
				joinRoomInWorldSequence.JoinRoom();
			}
			else
			{
				errorHandler.onNoRoomsFound();
			}
		}

		private void onRoomsFoundToJoinError(GetRoomsOperation operation, HttpResponse response)
		{
			NetworkErrorService.OnError(response, errorHandler);
		}
	}
}
