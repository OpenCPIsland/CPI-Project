using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System;

namespace ClubPenguin.Net
{
	public class JoinIglooSequence
	{
		private ClubPenguinClient clubPenguinClient;

		private ZoneId iglooToJoin;

		private string language;

		private IJoinIglooErrorHandler errorHandler;

		private SignedResponse<JoinRoomData> signedJoinRoomData;

		public JoinIglooSequence(ClubPenguinClient client, ZoneId igloo, string language, IJoinIglooErrorHandler errorHandler)
		{
			clubPenguinClient = client;
			iglooToJoin = igloo;
			this.language = language;
			this.errorHandler = errorHandler;
		}

		public void JoinIgloo()
		{
			if (clubPenguinClient.GameServer.IsConnected())
			{
				LeaveRoomSequence leaveRoomSequence = new LeaveRoomSequence(clubPenguinClient, JoinIgloo);
				leaveRoomSequence.LeaveRoom();
				return;
			}
			bool bypassCaptcha = false;
			APICall<PostIglooPlayersOperation> aPICall = clubPenguinClient.GameApi.PostIglooPlayers(iglooToJoin, language, bypassCaptcha);
			aPICall.OnResponse += onPostRoomPlayersResponse;
			aPICall.OnError += onPostRoomPlayersError;
			aPICall.Execute();
		}

		private void onPostRoomPlayersResponse(PostIglooPlayersOperation operation, HttpResponse httpResponse)
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
			Service.Get<EventDispatcher>().DispatchEvent(new WorldServiceEvents.SelfRoomJoinedEvent(clubPenguinClient.PlayerSessionId, clubPenguinClient.PlayerName, new RoomIdentifier((string)data), signedJoinRoomData.Data.extraLayoutData, signedJoinRoomData.Data.roomOwnerName, signedJoinRoomData.Data.roomOwner));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerMembershipStatusChanged(signedJoinRoomData.Data.membershipRights.member));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerOutfitChanged(outfit));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerProfileChanged(clubPenguinClient.PlayerSessionId, signedJoinRoomData.Data.playerRoomData.profile));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.TubeSelected(clubPenguinClient.PlayerSessionId, signedJoinRoomData.Data.selectedTubeId));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(signedJoinRoomData.Data.playerRoomData.assets));
			Service.Get<EventDispatcher>().DispatchEvent(new ConsumableServiceEvents.InventoryRecieved(signedJoinRoomData.Data.playerRoomData.consumableInventory));
			Service.Get<EventDispatcher>().DispatchEvent(new TaskNetworkServiceEvents.DailyTaskProgressRecieved(signedJoinRoomData.Data.playerRoomData.dailyTaskProgress));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.RoomRewardsReceived(iglooToJoin.name, signedJoinRoomData.Data.earnedRewards));
			Service.Get<EventDispatcher>().DispatchEvent(new QuestServiceEvents.QuestStatesRecieved(signedJoinRoomData.Data.playerRoomData.quests));
			signedJoinRoomData = null;
		}

		private void onPostRoomPlayersError(PostIglooPlayersOperation operation, HttpResponse response)
		{
			Log.LogNetworkErrorFormatted(this, "onPostRoomPlayersError, status: {0}, body: {1}", response.StatusCode, response.Text);
			NetworkErrorService.OnError(response, errorHandler, onPostRoomPlayersErrorMapper);
		}

		private static bool onPostRoomPlayersErrorMapper(NetworkErrorType errorType, IJoinIglooErrorHandler handler)
		{
			switch (errorType)
			{
			case NetworkErrorType.CP_ROOM_FULL:
				handler.onRoomFull();
				return true;
			case NetworkErrorType.CP_NO_SERVER_FOUND:
				handler.onNoServerFound();
				return true;
			case NetworkErrorType.CP_IGLOO_WRONG_ROOM:
				handler.onRoomChanged();
				return true;
			case NetworkErrorType.CP_IGLOO_UNAVAILABLE:
				handler.onIglooNotAvailable();
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
