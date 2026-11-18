using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public static class LanGameServerHost
	{
		public static string Address
		{
			get;
			set;
		}
	}

#if !UNITY_WEBGL
	public class LanGameServerClient : IGameServerClient
	{
		private readonly ClubPenguinClient clubPenguinClient;

		private readonly string gameZone;

		private readonly bool gameEncryption;

		private readonly bool gameDebugging;

		private readonly bool lagMonitoring;

		private readonly OfflineGameServerClient offline;

		private SmartFoxGameServerClient lan;

		private IGameServerClient active;

		private bool pendingSwitch;

		private readonly List<KeyValuePair<GameServerEvent, GameServerEventListener>> listeners = new List<KeyValuePair<GameServerEvent, GameServerEventListener>>();

		public static LanGameServerClient Instance
		{
			get;
			private set;
		}

		public bool IsLanActive
		{
			get
			{
				return active == lan && lan != null;
			}
		}

		public LanGameServerClient(ClubPenguinClient clubPenguinClient, string gameZone, bool gameEncryption, bool gameDebugging, bool lagMonitoring)
		{
			this.clubPenguinClient = clubPenguinClient;
			this.gameZone = gameZone;
			this.gameEncryption = gameEncryption;
			this.gameDebugging = gameDebugging;
			this.lagMonitoring = lagMonitoring;
			offline = new OfflineGameServerClient(clubPenguinClient);
			active = offline;
			pendingSwitch = LanGameServerHost.Address != null;
			Instance = this;
		}

		public void SetHost(string address)
		{
			LanGameServerHost.Address = string.IsNullOrEmpty(address) ? null : address;
			pendingSwitch = true;
		}

		private IGameServerClient ensureLan()
		{
			if (lan == null)
			{
				lan = new SmartFoxGameServerClient(clubPenguinClient, gameZone, gameEncryption, gameDebugging, lagMonitoring);
				foreach (KeyValuePair<GameServerEvent, GameServerEventListener> entry in listeners)
				{
					lan.AddEventListener(entry.Key, entry.Value);
				}
			}
			return lan;
		}

		private void applyPendingSwitch()
		{
			if (!pendingSwitch)
			{
				return;
			}
			pendingSwitch = false;
			active = (LanGameServerHost.Address == null) ? offline : ensureLan();
		}

		public int UserCount
		{
			get
			{
				return active.UserCount;
			}
		}

		public long ServerTime
		{
			get
			{
				return active.ServerTime;
			}
		}

		public void AddEventListener(GameServerEvent gameServerEvent, GameServerEventListener listener)
		{
			listeners.Add(new KeyValuePair<GameServerEvent, GameServerEventListener>(gameServerEvent, listener));
			offline.AddEventListener(gameServerEvent, listener);
			if (lan != null)
			{
				lan.AddEventListener(gameServerEvent, listener);
			}
		}

		public void RemoveEventListener(GameServerEvent gameServerEvent, GameServerEventListener listener)
		{
			int index = listeners.FindIndex((KeyValuePair<GameServerEvent, GameServerEventListener> entry) => entry.Key == gameServerEvent && entry.Value == listener);
			if (index >= 0)
			{
				listeners.RemoveAt(index);
			}
			offline.RemoveEventListener(gameServerEvent, listener);
			if (lan != null)
			{
				lan.RemoveEventListener(gameServerEvent, listener);
			}
		}

		public bool IsConnected()
		{
			return active.IsConnected();
		}

		public string CurrentRoom()
		{
			return active.CurrentRoom();
		}

		public void JoinRoom(SignedResponse<JoinRoomData> signedJoinRoomData)
		{
			applyPendingSwitch();
			active.JoinRoom(signedJoinRoomData);
		}

		public void LeaveRoom()
		{
			active.LeaveRoom();
		}

		public void TriggerLocomotionAction(LocomotionActionEvent action, bool droppable)
		{
			active.TriggerLocomotionAction(action, droppable);
		}

		public void SetLocomotionState(LocomotionState state)
		{
			active.SetLocomotionState(state);
		}

		public void SendChatActivity()
		{
			active.SendChatActivity();
		}

		public void SendChatActivityCancel()
		{
			active.SendChatActivityCancel();
		}

		public void SendChatMessage(SignedResponse<ChatMessage> signedChatMessage)
		{
			active.SendChatMessage(signedChatMessage);
		}

		public void SetOutfit(SignedResponse<PlayerOutfitDetails> outfit)
		{
			active.SetOutfit(outfit);
		}

		public void SetProfile(SignedResponse<Profile> profile)
		{
			active.SetProfile(profile);
		}

		public void QuestCompleteObjective(string objective)
		{
			active.QuestCompleteObjective(objective);
		}

		public void QuestSetQuestState(SignedResponse<QuestStateCollection> quests)
		{
			active.QuestSetQuestState(quests);
		}

		public void SetConsumableInventory(SignedResponse<ConsumableInventory> inventory)
		{
			active.SetConsumableInventory(inventory);
		}

		public void EquipConsumable(string type)
		{
			active.EquipConsumable(type);
		}

		public void EquipDispensable(int id)
		{
			active.EquipDispensable(id);
		}

		public void SetAirBubble(float value, int diveState)
		{
			active.SetAirBubble(value, diveState);
		}

		public void RemoveAirBubble()
		{
			active.RemoveAirBubble();
		}

		public void EquipDurable(SignedResponse<EquipDurableResponse> durable)
		{
			active.EquipDurable(durable);
		}

		public void DequipeHeldObject()
		{
			active.DequipeHeldObject();
		}

		public void UseConsumable(SignedResponse<UsedConsumable> consumable, object properties)
		{
			active.UseConsumable(consumable, properties);
		}

		public void ReuseConsumable(string type, object properties)
		{
			active.ReuseConsumable(type, properties);
		}

		public void Pickup(string id, string tag, Vector3 position)
		{
			active.Pickup(id, tag, position);
		}

		public void PrototypeSetState(object data)
		{
			active.PrototypeSetState(data);
		}

		public void PrototypeSendAction(object data)
		{
			active.PrototypeSendAction(data);
		}

		public void GetSignedStateOfLocalPlayer()
		{
			active.GetSignedStateOfLocalPlayer();
		}

		public void SendRewardNotification(SignedResponse<RewardedUserCollectionJsonHelper> response)
		{
			active.SendRewardNotification(response);
		}

		public void SendWebServiceEvent(SignedResponse<WebServiceEvent> wsEvent)
		{
			active.SendWebServiceEvent(wsEvent);
		}

		public void GetPlayerLocation(string swid)
		{
			active.GetPlayerLocation(swid);
		}

		public void RefreshMembership(SignedResponse<MembershipRightsRefresh> rightsRefresh)
		{
			active.RefreshMembership(rightsRefresh);
		}

		public void SetAwayFromKeyboard(int value)
		{
			active.SetAwayFromKeyboard(value);
		}

		public void SetTemporaryHeadStatus(int value)
		{
			active.SetTemporaryHeadStatus(value);
		}

		public void SetSelectedTube(SignedResponse<EquipTubeResponse> tube)
		{
			active.SetSelectedTube(tube);
		}

		public void SendPartyGameSessionMessage(int sessionId, int type, object properties)
		{
			active.SendPartyGameSessionMessage(sessionId, type, properties);
		}

		public void SendIglooUpdated(SignedResponse<IglooData> signedIglooData)
		{
			active.SendIglooUpdated(signedIglooData);
		}
	}
#endif
}
