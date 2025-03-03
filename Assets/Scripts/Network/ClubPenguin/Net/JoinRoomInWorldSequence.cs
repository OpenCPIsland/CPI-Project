using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System;

namespace ClubPenguin.Net
{
	public class JoinRoomInWorldSequence
	{
		private ClubPenguinClient clubPenguinClient;

		private RoomIdentifier roomToJoin;

		private System.Action successHandler;

		private IJoinRoomErrorHandler errorHandler;

		private SignedResponse<JoinRoomData> signedJoinRoomData;

		public JoinRoomInWorldSequence(ClubPenguinClient client, RoomIdentifier room, IJoinRoomErrorHandler errorHandler, System.Action successHandler = null)
		{
			clubPenguinClient = client;
			roomToJoin = room;
			this.successHandler = successHandler;
			this.errorHandler = errorHandler;
		}

		public void JoinRoom()
		{
			if (clubPenguinClient.GameServer.IsConnected())
			{
				if (clubPenguinClient.GameServer.CurrentRoom() == roomToJoin.ToString())
				{
					Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.SelfRoomJoinedEvent(clubPenguinClient.PlayerSessionId, clubPenguinClient.PlayerName, roomToJoin, null, null, false));
					if (successHandler != null)
					{
						successHandler();
					}
				}
				else
				{
					LeaveRoomSequence leaveRoomSequence = new LeaveRoomSequence(clubPenguinClient, JoinRoom);
					leaveRoomSequence.LeaveRoom();
				}
			}
			else
			{
				bool bypassCaptcha = false;
				APICall<PostRoomPlayersOperation> aPICall = clubPenguinClient.GameApi.PostRoomPlayers(roomToJoin.world, LocalizationLanguage.GetLanguageString(roomToJoin.language), roomToJoin.zoneId.name, bypassCaptcha);
				aPICall.OnResponse += onPostRoomPlayersResponse;
				aPICall.OnError += onPostRoomPlayersError;
				aPICall.Execute();
			}
		}

		private void onPostRoomPlayersResponse(PostRoomPlayersOperation operation, HttpResponse httpResponse)
		{
			signedJoinRoomData = operation.SignedJoinRoomData;
			string subContentVersion = new ContentIdentifier(signedJoinRoomData.Data.room).subContentVersion;
			if (subContentVersion != clubPenguinClient.ContentVersionDate.ToString("yyyy-MM-dd"))
			{
				DateTime dateTime = DateTime.Parse(subContentVersion);
				clubPenguinClient.ContentVersionDate = dateTime;
				Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.ContentDateChanged(dateTime));
			}
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_JOIN, onRoomJoin);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_JOIN_ERROR, onRoomJoinError);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.ROOM_FULL, onRoomFull);
			clubPenguinClient.GameServer.JoinRoom(signedJoinRoomData);
		}

		private void onRoomJoin(GameServerEvent evt, object data)
		{
			removeSFSListeners();
			PlayerOutfitDetails outfit = signedJoinRoomData.Data.playerRoomData.outfit;
			outfit.sessionId = clubPenguinClient.PlayerSessionId;
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.SelfRoomJoinedEvent(clubPenguinClient.PlayerSessionId, clubPenguinClient.PlayerName, roomToJoin, signedJoinRoomData.Data.extraLayoutData, signedJoinRoomData.Data.roomOwnerName, signedJoinRoomData.Data.roomOwner));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerMembershipStatusChanged(signedJoinRoomData.Data.membershipRights.member));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerOutfitChanged(outfit));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerProfileChanged(clubPenguinClient.PlayerSessionId, signedJoinRoomData.Data.playerRoomData.profile));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.TubeSelected(clubPenguinClient.PlayerSessionId, signedJoinRoomData.Data.selectedTubeId));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(signedJoinRoomData.Data.playerRoomData.assets));
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.InventoryRecieved(signedJoinRoomData.Data.playerRoomData.consumableInventory));
			Service.Get<EventDispatcher>().DispatchEvent(new TaskNetworkServiceEvents.DailyTaskProgressRecieved(signedJoinRoomData.Data.playerRoomData.dailyTaskProgress));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.RoomRewardsReceived(roomToJoin.zoneId.name, signedJoinRoomData.Data.earnedRewards));
			Service.Get<EventDispatcher>().DispatchEvent(new QuestServiceEvents.QuestStatesRecieved(signedJoinRoomData.Data.playerRoomData.quests));
			signedJoinRoomData = null;
			if (successHandler != null)
			{
				successHandler();
			}
		}

		private void onPostRoomPlayersError(PostRoomPlayersOperation operation, HttpResponse response)
		{
			Log.LogNetworkErrorFormatted(this, "onPostRoomPlayersError, status: {0}, body: {1}", response.StatusCode, response.Text);
			NetworkErrorService.OnError(response, errorHandler, onPostRoomPlayersErrorMapper);
		}

		private static bool onPostRoomPlayersErrorMapper(NetworkErrorType errorType, IJoinRoomErrorHandler handler)
		{
			switch (errorType)
			{
			case NetworkErrorType.CP_ROOM_FULL:
				handler.onRoomFull();
				return true;
			case NetworkErrorType.CP_NO_SERVER_FOUND:
				handler.onNoServerFound();
				return true;
			default:
				return false;
			}
		}

		private void onRoomJoinError(GameServerEvent evt, object data)
		{
			removeSFSListeners();
			RoomJoinError roomJoinError = (RoomJoinError)data;
			errorHandler.onRoomJoinError();
		}

		private void onRoomFull(GameServerEvent evt, object data)
		{
			removeSFSListeners();
			RoomJoinError roomJoinError = (RoomJoinError)data;
			errorHandler.onRoomFull();
		}

		private void removeSFSListeners()
		{
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.ROOM_JOIN, onRoomJoin);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.ROOM_JOIN_ERROR, onRoomJoinError);
			clubPenguinClient.GameServer.RemoveEventListener(GameServerEvent.ROOM_FULL, onRoomFull);
		}
	}
}
