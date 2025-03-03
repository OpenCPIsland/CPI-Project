using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public class WorldService : BaseNetworkService, IWorldService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.CONNECTION_LOST, onConnectionLost);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.NETWORK_ERROR, onNetworkError);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_USER_ADDED, onUserAdded);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_USER_REMOVED, onUserRemoved);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.SERVER_ITEM_ADDED, onServerItemAdded);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.SERVER_ITEM_REMOVED, onServerItemRemoved);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.SERVER_ITEM_CHANGED, onServerItemChanged);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.SERVER_ITEM_MOVED, onServerItemMoved);
		}

		private void removeListeners()
		{
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.CONNECTION_LOST, onConnectionLost);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.NETWORK_ERROR, onNetworkError);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.ROOM_USER_ADDED, onUserAdded);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.ROOM_USER_REMOVED, onUserRemoved);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.SERVER_ITEM_ADDED, onServerItemAdded);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.SERVER_ITEM_REMOVED, onServerItemRemoved);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.SERVER_ITEM_CHANGED, onServerItemChanged);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.SERVER_ITEM_MOVED, onServerItemMoved);
		}

		private void onConnectionLost(GameServerEvent evt, object data)
		{
			string errorMessage = (string)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceErrors.WorldDisconnectedEvent(errorMessage));
		}

		private void onNetworkError(GameServerEvent evt, object data)
		{
			string errorMessage = (string)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceErrors.WorldNetworkErrorEvent(errorMessage));
		}

		private void onUserAdded(GameServerEvent evt, object data)
		{
			RoomMember roomMember = (RoomMember)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.PlayerJoinRoomEvent(roomMember.Id, roomMember.Name));
		}

		private void onUserRemoved(GameServerEvent evt, object data)
		{
			RoomMember roomMember = (RoomMember)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.PlayerLeaveRoomEvent(roomMember.Id));
		}

		public void GetWorldsWithRoomPopulation(string room, string language)
		{
			APICall<GetRoomPopulationForWorldsOperation> roomPopulationForWorlds = clubPenguinClient.GameApi.GetRoomPopulationForWorlds(room, language);
			roomPopulationForWorlds.OnResponse += worldsWithRoomPopulationFound;
			roomPopulationForWorlds.OnError += handleCPResponseError;
			roomPopulationForWorlds.Execute();
		}

		private void worldsWithRoomPopulationFound(GetRoomPopulationForWorldsOperation operation, HttpResponse response)
		{
			handleCPResponse(operation.Response);
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.WorldsWithRoomPopulationReceivedEvent(operation.Response.worldRoomPopulations));
		}

		public void JoinRoom(string room, string contentIdentifier, string language, IJoinRoomByNameErrorHandler errorHandler)
		{
			JoinRoomSequence joinRoomSequence = new JoinRoomSequence(clubPenguinClient, room, language, errorHandler);
			joinRoomSequence.JoinRoom();
		}

		public void JoinRoomInWorld(RoomIdentifier room, IJoinRoomErrorHandler errorHandler)
		{
			JoinRoomInWorldSequence joinRoomInWorldSequence = new JoinRoomInWorldSequence(clubPenguinClient, room, errorHandler);
			joinRoomInWorldSequence.JoinRoom();
		}

		public void JoinIgloo(ZoneId igloo, string language, IJoinIglooErrorHandler errorHandler)
		{
			JoinIglooSequence joinIglooSequence = new JoinIglooSequence(clubPenguinClient, igloo, language, errorHandler);
			joinIglooSequence.JoinIgloo();
		}

		public void SetContentDate(DateTime currentDate)
		{
			clubPenguinClient.ContentVersionDate = currentDate;
		}

		public void LeaveRoom(bool immediately = false)
		{
			LeaveRoomSequence leaveRoomSequence = new LeaveRoomSequence(clubPenguinClient);
			leaveRoomSequence.LeaveRoom(immediately);
		}

		private void onServerItemAdded(GameServerEvent gameServerEvent, object data)
		{
			CPMMOItem item = (CPMMOItem)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.ItemSpawned(item));
		}

		private void onServerItemRemoved(GameServerEvent gameServerEvent, object data)
		{
			CPMMOItemId itemId = (CPMMOItemId)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.ItemDestroyed(itemId));
		}

		private void onServerItemChanged(GameServerEvent gameServerEvent, object data)
		{
			CPMMOItem item = (CPMMOItem)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.ItemChanged(item));
		}

		private void onServerItemMoved(GameServerEvent gameServerEvent, object data)
		{
			CPMMOItemPosition cPMMOItemPosition = (CPMMOItemPosition)data;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.ItemMoved(cPMMOItemPosition.Id, cPMMOItemPosition.Position));
		}

		public void GetRoomPopulation(string world, string language)
		{
			APICall<GetRoomPopulationOperation> roomPopulation = clubPenguinClient.GameApi.GetRoomPopulation(world, language);
			roomPopulation.OnResponse += delegate(GetRoomPopulationOperation op, HttpResponse HttpResponse)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.RoomPopulationReceivedEvent(op.Response.roomPopulations));
			};
			roomPopulation.OnError += delegate
			{
				Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.RoomPopulationReceivedEvent(new List<RoomPopulation>()));
			};
			roomPopulation.OnError += handleCPResponseError;
			roomPopulation.Execute();
		}
	}
}
