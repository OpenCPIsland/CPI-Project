using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public interface IGameServerClient
	{
		int UserCount
		{
			get;
		}

		long ServerTime
		{
			get;
		}

		void AddEventListener(GameServerEvent gameServerEvent, GameServerEventListener listener);

		void RemoveEventListener(GameServerEvent gameServerEvent, GameServerEventListener listener);

		bool IsConnected();

		string CurrentRoom();

		void JoinRoom(SignedResponse<JoinRoomData> signedJoinRoomData);

		void LeaveRoom();

		void TriggerLocomotionAction(LocomotionActionEvent action, bool droppable);

		void SetLocomotionState(LocomotionState state);

		void SendChatActivity();

		void SendChatActivityCancel();

		void SendChatMessage(SignedResponse<ChatMessage> signedChatMessage);

		void SetOutfit(SignedResponse<PlayerOutfitDetails> outfit);

		void SetProfile(SignedResponse<Profile> profile);

		void QuestCompleteObjective(string objective);

		void QuestSetQuestState(SignedResponse<QuestStateCollection> quests);

		void SetConsumableInventory(SignedResponse<ConsumableInventory> inventory);

		void EquipConsumable(string type);

		void EquipDispensable(int id);

		void SetAirBubble(float value, int diveState);

		void RemoveAirBubble();

		void EquipDurable(SignedResponse<EquipDurableResponse> durable);

		void DequipeHeldObject();

		void UseConsumable(SignedResponse<UsedConsumable> consumable, object properties);

		void ReuseConsumable(string type, object properties);

		void Pickup(string id, string tag, Vector3 position);

		void PrototypeSetState(object data);

		void PrototypeSendAction(object data);

		void GetSignedStateOfLocalPlayer();

		void SendRewardNotification(SignedResponse<RewardedUserCollectionJsonHelper> response);

		void SendWebServiceEvent(SignedResponse<WebServiceEvent> wsEvent);

		void GetPlayerLocation(string swid);

		void RefreshMembership(SignedResponse<MembershipRightsRefresh> rightsRefresh);

		void SetAwayFromKeyboard(int value);

		void SetTemporaryHeadStatus(int value);

		void SetSelectedTube(SignedResponse<EquipTubeResponse> tube);

		void SendPartyGameSessionMessage(int sessionId, int type, object properties);

		void SendIglooUpdated(SignedResponse<IglooData> signedIglooData);
	}
}
