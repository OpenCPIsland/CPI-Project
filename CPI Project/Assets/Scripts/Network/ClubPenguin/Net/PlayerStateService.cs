using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Client.Operations;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	internal class PlayerStateService : BaseNetworkService, IPlayerStateService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.USER_OUTFIT_CHANGED, onPlayerOutfitChanged);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.USER_PROFILE_CHANGED, onPlayerProfileChanged);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.USER_LOCO_STATE_CHANGED, onPlayerLocoStateChanged);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.HELD_OBJECT_DEQUIPPED, onHeldObjectDequipped);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.DISPENSABLE_EQUIPPED, onDispensableEquipped);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.DURABLE_EQUIPPED, onDurableEquipped);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.PARTYGAME_EQUIPPED, onPartyGameEquipped);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.AIR_BUBBLE_UPDATE, onAirBubbleUpdate);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.AWAY_FROM_KEYBOARD_STATE_CHANGED, onAwayFromKeyboardStateChanged);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.SELECTED_TUBE_CHANGED, onSelectedTubeChanged);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.TEMPORARY_HEAD_STATUS_CHANGED, onTemporaryHeadStatusChanged);
		}

		private void onAirBubbleUpdate(GameServerEvent gameServerEvent, object data)
		{
			if (data != null)
			{
				PlayerAirBubbleEvent playerAirBubbleEvent = (PlayerAirBubbleEvent)data;
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.AirBubbleChanged(playerAirBubbleEvent.SessionId, playerAirBubbleEvent.AirBubble));
			}
		}

		private void onTemporaryHeadStatusChanged(GameServerEvent gameServerEvent, object data)
		{
			if (data != null)
			{
				TemporaryHeadStatusEvent temporaryHeadStatusEvent = (TemporaryHeadStatusEvent)data;
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.TemporaryHeadStatusChanged(temporaryHeadStatusEvent.SessionId, temporaryHeadStatusEvent.Type));
			}
		}

		public string GetAccessToken()
		{
			return clubPenguinClient.AccessToken;
		}

		public void SetAccessToken(string accessToken)
		{
			clubPenguinClient.AccessToken = accessToken;
		}

		public void SetSWID(string swid)
		{
			clubPenguinClient.SWID = swid;
		}

		public void SetLocomotionState(LocomotionState state)
		{
			clubPenguinClient.GameServer.SetLocomotionState(state);
		}

		public void SetOutfit(PlayerOutfit outfit)
		{
			APICall<SetOutfitOperation> aPICall = clubPenguinClient.PlayerApi.SetOutfit(outfit);
			aPICall.OnResponse += outfitChangeComplete;
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += outfitChangeError;
			aPICall.Execute();
		}

		public void SetProfile(Profile profile)
		{
			APICall<SetProfileOperation> aPICall = clubPenguinClient.PlayerApi.SetProfile(profile);
			aPICall.OnResponse += profileChangeComplete;
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += profileChangeError;
			aPICall.Execute();
		}

		public void GetLocalPlayerData(IBaseNetworkErrorHandler errorHandler)
		{
			APICall<GetLocalPlayerDataOperation> localPlayerData = clubPenguinClient.PlayerApi.GetLocalPlayerData();
			localPlayerData.OnResponse += localPlayerDataReturned;
			localPlayerData.OnError += delegate(GetLocalPlayerDataOperation o, HttpResponse response)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceErrors.PlayerProfileError));
				NetworkErrorService.OnError(response, errorHandler);
			};
			localPlayerData.Execute();
		}

		public void GetOtherPlayerDataBySwid(string swid)
		{
			APICall<GetOtherPlayerDataBySwidOperation> otherPlayerDataBySwid = clubPenguinClient.PlayerApi.GetOtherPlayerDataBySwid(swid);
			otherPlayerDataBySwid.OnResponse += otherPlayerDataReturned;
			otherPlayerDataBySwid.OnError += handleCPResponseError;
			otherPlayerDataBySwid.Execute();
		}

		public void GetOtherPlayerDataBySwids(IList<string> swids)
		{
			APICall<GetOtherPlayerDataBySwidsOperation> otherPlayerDataBySwids = clubPenguinClient.PlayerApi.GetOtherPlayerDataBySwids(swids);
			otherPlayerDataBySwids.OnResponse += otherPlayerDatasReturned;
			otherPlayerDataBySwids.OnError += handleCPResponseError;
			otherPlayerDataBySwids.Execute();
		}

		public void GetOtherPlayerDataByDisplayName(string displayName)
		{
			APICall<GetOtherPlayerDataByDisplayNameOperation> otherPlayerDataByDisplayName = clubPenguinClient.PlayerApi.GetOtherPlayerDataByDisplayName(displayName);
			otherPlayerDataByDisplayName.OnResponse += otherPlayerDataReturned;
			otherPlayerDataByDisplayName.OnError += handleCPResponseError;
			otherPlayerDataByDisplayName.Execute();
		}

		public void GetOtherPlayerDataByDisplayNames(IList<string> displayNames)
		{
			APICall<GetOtherPlayerDataByDisplayNamesOperation> otherPlayerDataByDisplayNames = clubPenguinClient.PlayerApi.GetOtherPlayerDataByDisplayNames(displayNames);
			otherPlayerDataByDisplayNames.OnResponse += otherPlayerDatasReturned;
			otherPlayerDataByDisplayNames.OnError += handleCPResponseError;
			otherPlayerDataByDisplayNames.Execute();
		}

		public void GetOtherPlayerDataBySessionId(long sessionId)
		{
			APICall<GetOtherPlayerDataBySessionIdOperation> otherPlayerDataBySessionId = clubPenguinClient.PlayerApi.GetOtherPlayerDataBySessionId(sessionId);
			otherPlayerDataBySessionId.OnResponse += otherPlayerDataReturned;
			otherPlayerDataBySessionId.OnError += handleCPResponseError;
			otherPlayerDataBySessionId.Execute();
		}

		public void GetOnlinePlayersBySwids(IList<string> swids)
		{
			APICall<GetOnlinePlayersBySwidsOperation> onlinePlayersBySwids = clubPenguinClient.PlayerApi.GetOnlinePlayersBySwids(swids);
			onlinePlayersBySwids.OnResponse += onlinePlayerSwidsReturned;
			onlinePlayersBySwids.OnError += handleCPResponseError;
			onlinePlayersBySwids.Execute();
		}

		public void MigrateLegacy(CPIDCredentials cpidCreds)
		{
			APICall<MigrateLegacyAccountOperation> aPICall = clubPenguinClient.PlayerApi.MigrateLegacyAccount(cpidCreds);
			aPICall.OnResponse += migrationSuccessful;
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += migrationError;
			aPICall.Execute();
		}

		public void GetPreregistrationData()
		{
			APICall<GetPreregistrationDataOperation> preregistrationData = clubPenguinClient.PlayerApi.GetPreregistrationData();
			preregistrationData.OnResponse += preregistrationDataReturned;
			preregistrationData.OnError += handleCPResponseError;
			preregistrationData.Execute();
		}

		public void SetReferral(string referrer)
		{
			APICall<SetReferralOperation> aPICall = clubPenguinClient.PlayerApi.SetReferral(referrer);
			aPICall.OnResponse += setReferalComplete;
			aPICall.OnError += handleCPResponseError;
			aPICall.OnError += setReferalError;
			aPICall.Execute();
		}

		public void EquipDispensable(int id)
		{
			clubPenguinClient.GameServer.EquipDispensable(id);
		}

		public void EquipDurable(int propId)
		{
			APICall<EquipDurableOperation> aPICall = clubPenguinClient.DurableApi.EquipDurable(propId);
			aPICall.OnResponse += delegate(EquipDurableOperation op, HttpResponse httpResponse)
			{
				SignedResponse<EquipDurableResponse> signedEquipDurableResponse = op.SignedEquipDurableResponse;
				clubPenguinClient.GameServer.EquipDurable(signedEquipDurableResponse);
			};
			aPICall.OnError += delegate(EquipDurableOperation op, HttpResponse httpResponse)
			{
				Log.LogNetworkError(this, httpResponse.Summary());
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void SelectTube(int tubeId)
		{
			APICall<EquipTubeOperation> aPICall = clubPenguinClient.TubeApi.EquipTube(tubeId);
			aPICall.OnResponse += delegate(EquipTubeOperation op, HttpResponse HttpResponse)
			{
				SignedResponse<EquipTubeResponse> signedEquipTubeResponse = op.SignedEquipTubeResponse;
				clubPenguinClient.GameServer.SetSelectedTube(signedEquipTubeResponse);
			};
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void DequipHeldObject()
		{
			clubPenguinClient.GameServer.DequipeHeldObject();
		}

		public void SetAwayFromKeyboard(int value)
		{
			clubPenguinClient.GameServer.SetAwayFromKeyboard(value);
		}

		public void SetTemporaryHeadStatus(int value)
		{
			clubPenguinClient.GameServer.SetTemporaryHeadStatus(value);
		}

		private void outfitChangeComplete(SetOutfitOperation operation, HttpResponse httpResponse)
		{
			SignedResponse<PlayerOutfitDetails> responseBody = operation.ResponseBody;
			clubPenguinClient.GameServer.SetOutfit(responseBody);
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceEvents.PlayerOutfitSaved));
		}

		private void outfitChangeError(SetOutfitOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceErrors.PlayerOutfitChangeError));
		}

		private void profileChangeComplete(SetProfileOperation operation, HttpResponse response)
		{
			SignedResponse<Profile> responseBody = operation.ResponseBody;
			clubPenguinClient.GameServer.SetProfile(responseBody);
		}

		private void profileChangeError(SetProfileOperation operation, HttpResponse response)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceErrors.PlayerProfileChangeError));
		}

		private void onPlayerLocoStateChanged(GameServerEvent evt, object data)
		{
			PlayerLocomotionStateEvent playerLocomotionStateEvent = (PlayerLocomotionStateEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerLocoStateChanged(playerLocomotionStateEvent.SessionId, playerLocomotionStateEvent.State));
		}

		private void onPlayerOutfitChanged(GameServerEvent evt, object data)
		{
			PlayerOutfitDetails outfit = (PlayerOutfitDetails)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerOutfitChanged(outfit));
		}

		private void onPlayerProfileChanged(GameServerEvent evt, object data)
		{
			ProfileEvent profileEvent = (ProfileEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PlayerProfileChanged(profileEvent.SessionId, profileEvent.Profile));
		}

		private void localPlayerDataReturned(GetLocalPlayerDataOperation operation, HttpResponse response)
		{
			LocalPlayerData responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.LocalPlayerDataReceived(responseBody));
		}

		private void migrationSuccessful(MigrateLegacyAccountOperation operation, HttpResponse response)
		{
			MigrationData responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.MigrationDataRecieved(responseBody));
		}

		private void migrationError(MigrateLegacyAccountOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceErrors.LegacyAccountMigrationError));
		}

		private void preregistrationDataReturned(GetPreregistrationDataOperation operation, HttpResponse response)
		{
			PreregistrationData responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PreregistrationDataReceived(responseBody));
		}

		private void otherPlayerDataReturned(GetOtherPlayerDataOperation operation, HttpResponse response)
		{
			OtherPlayerData responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.OtherPlayerDataReceived(responseBody));
		}

		private void otherPlayerDatasReturned(GetOtherPlayerDatasOperation operation, HttpResponse response)
		{
			List<OtherPlayerData> responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.OtherPlayerDataListReceived(responseBody));
		}

		private void onlinePlayerSwidsReturned(GetOnlinePlayersBySwidsOperation operation, HttpResponse response)
		{
			List<string> responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.OnlinePlayerSwidListReceived(responseBody));
		}

		private void onHeldObjectDequipped(GameServerEvent gameServerEvent, object data)
		{
			long sessionId = (long)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.HeldObjectDequipped(sessionId));
		}

		private void onDispensableEquipped(GameServerEvent gameServerEvent, object data)
		{
			HeldObjectEvent heldObjectEvent = (HeldObjectEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.DispensableEquipped(heldObjectEvent.SessionId, heldObjectEvent.Type));
		}

		private void onPartyGameEquipped(GameServerEvent GameServerEvent, object data)
		{
			HeldObjectEvent heldObjectEvent = (HeldObjectEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.PartyGameEquipped(heldObjectEvent.SessionId, heldObjectEvent.Type));
		}

		private void onDurableEquipped(GameServerEvent gameServerEvent, object data)
		{
			HeldObjectEvent heldObjectEvent = (HeldObjectEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.DurableEquipped(heldObjectEvent.SessionId, heldObjectEvent.Type));
		}

		private void onAwayFromKeyboardStateChanged(GameServerEvent gameServerEvent, object data)
		{
			AFKEvent aFKEvent = (AFKEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.AwayFromKeyboardStateChanged(aFKEvent.SessionId, aFKEvent.AFKValue, aFKEvent.EquippedObject));
		}

		private void onSelectedTubeChanged(GameServerEvent gameServerEvent, object data)
		{
			SelectedTubeEvent selectedTubeEvent = (SelectedTubeEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerStateServiceEvents.TubeSelected(selectedTubeEvent.SessionId, selectedTubeEvent.TubeId));
		}

		void IPlayerStateService.SetAirBubble(float value, int diveState)
		{
			clubPenguinClient.GameServer.SetAirBubble(value, diveState);
		}

		void IPlayerStateService.RemoveAirBubble()
		{
			clubPenguinClient.GameServer.RemoveAirBubble();
		}

		private void setReferalComplete(SetReferralOperation operation, HttpResponse response)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceEvents.PlayerReferralSet));
		}

		private void setReferalError(SetReferralOperation operation, HttpResponse response)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerStateServiceErrors.PlayerReferralError));
		}
	}
}
