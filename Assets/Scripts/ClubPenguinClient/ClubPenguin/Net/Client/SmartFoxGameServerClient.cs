using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Client.Smartfox;
using ClubPenguin.Net.Client.Smartfox.SFSObject;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Utils;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public class SmartFoxGameServerClient : IGameServerClient
	{
		private const int DISCONNECT_TIMEOUT_SEC = 5;

		private const int TARGET_EVENT_PROCESSING_MS = 10;

		[Tweakable("Session.GameServer.EnableUDP", Description = "Enable UDP events in the SFS game server")]
		public static bool EnableUDP = false;

		[Tweakable("Session.GameServer.AbortOnUDPError")]
		public static bool AbortOnUDPError = false;

		private IDictionary<GameServerEvent, GameServerEventListener> eventListeners = new Dictionary<GameServerEvent, GameServerEventListener>();

		private List<KeyValuePair<GameServerEvent, object>> queuedEvents = new List<KeyValuePair<GameServerEvent, object>>();

		private bool waitForRoomJoinEvent = false;

		private Stopwatch clientTimer = new Stopwatch();

		private Stopwatch eventProcessingTimer = new Stopwatch();

		private long lastServerTime = 0L;

		private readonly SmartFoxGameServerClientShared mt;

		public long ServerTime
		{
			get
			{
				return lastServerTime + clientTimer.ElapsedMilliseconds;
			}
		}

		public int UserCount
		{
			get
			{
				if (mt != null)
				{
					return mt.UserCount;
				}
				return 0;
			}
		}

		public SmartFoxGameServerClient(ClubPenguinClient clubPenguinClient, string gameZone, bool gameEncryption, bool gameDebugging, bool lagMonitoring)
		{
			mt = new SmartFoxGameServerClientShared(clubPenguinClient, gameZone, gameEncryption, gameDebugging, lagMonitoring);
			CoroutineRunner.StartPersistent(processMessagesCoroutine(), this, "Smartfox message procession");
		}

		public bool IsConnected()
		{
			if (mt != null)
			{
				return mt.isConnected;
			}
			return false;
		}

		public string CurrentRoom()
		{
			if (mt != null)
			{
				return mt.ClientRoomName;
			}
			return null;
		}

		private IEnumerator processMessagesCoroutine()
		{
			while (true)
			{
				if (mt.TriggerForceTeardownAfterDelay)
				{
					CoroutineRunner.Start(forceTeardownAfterDelay(mt.smartFoxRef, 5), this, "forceTeardownAfterDelay");
					mt.TriggerForceTeardownAfterDelay = false;
				}
				if (mt.TriggerInitCrypto)
				{
					CoroutineRunner.Start(mt.InitCrypto(), this, "InitCrypto");
					mt.TriggerInitCrypto = false;
				}
				long serverTime;
				if (mt.TryGetServerTimeUpdate(out serverTime))
				{
					lastServerTime = serverTime;
					clientTimer.Reset();
					clientTimer.Start();
				}
				try
				{
					processEvents();
				}
				catch (Exception ex)
				{
					Log.LogError(this, "Catching error in the smartfox event processing loop, so networking doesn't stop: " + ex.Message);
					Log.LogException(this, ex);
				}
				yield return null;
			}
		}

		public void AddEventListener(GameServerEvent gameServerEvent, GameServerEventListener listener)
		{
			if (!eventListeners.ContainsKey(gameServerEvent))
			{
				eventListeners.Add(gameServerEvent, listener);
			}
			else
			{
                IDictionary<GameServerEvent, GameServerEventListener> gameServerEvents = this.eventListeners;
                IDictionary<GameServerEvent, GameServerEventListener> gameServerEvents1 = gameServerEvents;
                GameServerEvent gameServerEvent1 = gameServerEvent;
                GameServerEvent gameServerEvent2 = gameServerEvent1;
                gameServerEvents[gameServerEvent1] = (GameServerEventListener)Delegate.Combine(gameServerEvents1[gameServerEvent2], listener);

            }
        }

		public void RemoveEventListener(GameServerEvent gameServerEvent, GameServerEventListener listener)
		{
            
			if (eventListeners.ContainsKey(gameServerEvent))
			{
                IDictionary<GameServerEvent, GameServerEventListener> gameServerEvents = this.eventListeners;
                IDictionary<GameServerEvent, GameServerEventListener> gameServerEvents1 = gameServerEvents;
                GameServerEvent gameServerEvent1 = gameServerEvent;
                GameServerEvent gameServerEvent2 = gameServerEvent1;
                gameServerEvents[gameServerEvent1] = (GameServerEventListener)Delegate.Remove(gameServerEvents1[gameServerEvent2], listener);

            }
        }

		private void processEvents()
		{
			int count = mt.TriggeredEvents.Count;
			if (count != 0)
			{
				eventProcessingTimer.Reset();
				eventProcessingTimer.Start();
				int num = Mathf.Max(count, 10);
				KeyValuePair<GameServerEvent, object> item;
				while (eventProcessingTimer.ElapsedMilliseconds <= num && mt.TriggeredEvents.TryDequeue(out item))
				{
					processEvent(item.Key, item.Value);
				}
				eventProcessingTimer.Stop();
			}
		}

		private void processEvent(GameServerEvent gameServerEvent, object data)
		{
			GameServerEventListener value;
			if (waitForRoomJoinEvent)
			{
				if (gameServerEvent == GameServerEvent.ROOM_JOIN || gameServerEvent == GameServerEvent.ROOM_FULL || gameServerEvent == GameServerEvent.ROOM_JOIN_ERROR || gameServerEvent == GameServerEvent.CONNECTION_LOST)
				{
					waitForRoomJoinEvent = false;
					processEvent(gameServerEvent, data);
					foreach (KeyValuePair<GameServerEvent, object> queuedEvent in queuedEvents)
					{
						processEvent(queuedEvent.Key, queuedEvent.Value);
					}
					queuedEvents.Clear();
				}
				else
				{
					queuedEvents.Add(new KeyValuePair<GameServerEvent, object>(gameServerEvent, data));
				}
			}
			else if (eventListeners.TryGetValue(gameServerEvent, out value) && value != null)
			{
				try
				{
					value(gameServerEvent, data);
				}
				catch (Exception ex)
				{
					Log.LogError(this, string.Concat("Exception triggered by game server event ", gameServerEvent, ": ", ex.Message));
					Log.LogException(this, ex);
				}
			}
		}

		public void JoinRoom(SignedResponse<JoinRoomData> signedJoinRoomData)
		{
			mt.SignedJoinRoomData = signedJoinRoomData;
			waitForRoomJoinEvent = true;
			if (!mt.isConnected)
			{
				mt.connect(signedJoinRoomData.Data.host, signedJoinRoomData.Data.tcpPort, signedJoinRoomData.Data.httpsPort);
				return;
			}
			if (!mt.isLoggedIn)
			{
				mt.login();
				return;
			}
			string clientRoomName = mt.ClientRoomName;
			if (clientRoomName != null && clientRoomName == signedJoinRoomData.Data.room)
			{
				mt.triggerEvent(GameServerEvent.ROOM_JOIN, clientRoomName);
				return;
			}
			RoomJoinError roomJoinError = default(RoomJoinError);
			roomJoinError.roomName = signedJoinRoomData.Data.room;
			roomJoinError.errorMessage = "Already logged in";
			mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
		}

		public void LeaveRoom()
		{
			mt.SfsRoomToLeave = mt.getCurrentRoom();
			mt.send(SmartfoxCommand.LEAVE_ROOM);
		}

		private IEnumerator forceTeardownAfterDelay(object sfsToTeardown, int delaySeconds)
		{
			yield return new WaitForSeconds(delaySeconds);
			if (object.ReferenceEquals(mt.smartFoxRef, sfsToTeardown))
			{
			}
			forceTearDown(sfsToTeardown);
		}

		private void forceTearDown(object sfsToTeardown)
		{
			if (!object.ReferenceEquals(mt.smartFoxRef, sfsToTeardown))
			{
				return;
			}
			string text = mt.ClientRoomName;
			mt.teardown();
			Room room;
			if (mt.TryClearSfsRoomToLeave(out room))
			{
				if (text == null)
				{
					text = room.Name;
				}
				mt.triggerEvent(GameServerEvent.ROOM_LEAVE, text);
			}
		}

		public void TriggerLocomotionAction(LocomotionActionEvent action, bool droppable)
		{
			Dictionary<string, SFSDataWrapper> dictionary = new Dictionary<string, SFSDataWrapper>();
			dictionary.Add("a", SmartFoxGameServerClientShared.serialize((byte)action.Type));
			dictionary.Add("p", SmartFoxGameServerClientShared.serialize(action.Position));
			dictionary.Add("t", SmartFoxGameServerClientShared.serialize(ServerTime));
			Dictionary<string, SFSDataWrapper> dictionary2 = dictionary;
			if (action.Direction.HasValue)
			{
				dictionary2.Add("d", SmartFoxGameServerClientShared.serialize(action.Direction.Value));
			}
			if (action.Velocity.HasValue)
			{
				dictionary2.Add("v", SmartFoxGameServerClientShared.serialize(action.Velocity.Value));
			}
			if (action.Object != null)
			{
				dictionary2.Add("o", SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(action.Object)));
			}
			mt.send(SmartfoxCommand.LOCOMOTION_ACTION, dictionary2, null, droppable);
		}

		public void SendChatActivity()
		{
			mt.send(SmartfoxCommand.CHAT_ACTIVITY);
		}

		public void SendChatActivityCancel()
		{
			mt.send(SmartfoxCommand.CHAT_ACTIVITY_CANCEL);
		}

		public void SendChatMessage(SignedResponse<ChatMessage> signedChatMessage)
		{
			Dictionary<string, SFSDataWrapper> dictionary = new Dictionary<string, SFSDataWrapper>();
			dictionary.Add("msg", SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(signedChatMessage)));
			Dictionary<string, SFSDataWrapper> parameters = dictionary;
			if (mt.SmartFoxEncryptor != null)
			{
				try
				{
					mt.SmartFoxEncryptor.EncryptParameter("msg", parameters);
				}
				catch (Exception ex)
				{
					Log.LogErrorFormatted(this, "Failed to encrypt parameter. Exception: {0}", ex);
					Log.LogException(this, ex);
				}
			}
			mt.send(SmartfoxCommand.CHAT, parameters);
		}

		public void PrototypeSetState(object data)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.PROTOTYPE,
					mt.JsonService.Serialize(data)
				}
			});
		}

		public void PrototypeSendAction(object data)
		{
			mt.send(SmartfoxCommand.PROTOTYPE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"data",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(data))
				}
			});
		}

		public void SetAwayFromKeyboard(int value)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.AWAY_FROM_KEYBOARD,
					value
				}
			});
		}

		public void SetTemporaryHeadStatus(int value)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.TEMPORARY_HEAD_STATUS,
					value
				}
			});
		}

		public void SetOutfit(SignedResponse<PlayerOutfitDetails> outfit)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.OUTFIT,
					mt.JsonService.Serialize(outfit)
				}
			});
		}

		public void SetProfile(SignedResponse<Profile> profile)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.PROFILE,
					mt.JsonService.Serialize(profile)
				}
			});
		}

		public void SetLocomotionState(LocomotionState state)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.LOCOMOTION_STATE,
					(state == LocomotionState.Default) ? null : new byte?((byte)state)
				}
			});
		}

		public void QuestCompleteObjective(string objective)
		{
			mt.send(SmartfoxCommand.COMPLETE_OBJECTIVE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"obj",
					SmartFoxGameServerClientShared.serialize(objective)
				}
			});
		}

		public void QuestSetQuestState(SignedResponse<QuestStateCollection> quests)
		{
			mt.send(SmartfoxCommand.SET_QUEST_STATES, new Dictionary<string, SFSDataWrapper>
			{
				{
					"data",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(quests))
				}
			});
		}

		public void SetConsumableInventory(SignedResponse<ConsumableInventory> inventory)
		{
			mt.send(SmartfoxCommand.SET_CONSUMABLE_INVENTORY, new Dictionary<string, SFSDataWrapper>
			{
				{
					"data",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(inventory))
				}
			});
		}

		public void SetAirBubble(float value, int diveState)
		{
			AirBubble airBubble = new AirBubble();
			airBubble.time = ServerTime;
			airBubble.value = value;
			airBubble.diveState = diveState;
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.AIR_BUBBLE,
					airBubble.ToSFSObject()
				}
			});
		}

		public void RemoveAirBubble()
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.AIR_BUBBLE,
					null
				}
			});
		}

		public void EquipConsumable(string type)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.EQUIPPED_OBJECT,
					EquippedObject.CreateConsumableObject(type).ToSFSObject()
				}
			});
		}

		public void EquipDispensable(int id)
		{
			mt.send(SmartfoxCommand.EQUIP_DISPENSABLE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"type",
					SmartFoxGameServerClientShared.serialize(id)
				}
			});
		}

		public void EquipDurable(SignedResponse<EquipDurableResponse> durable)
		{
			mt.send(SmartfoxCommand.EQUIP_DURABLE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"type",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(durable))
				}
			});
		}

		public void RefreshMembership(SignedResponse<MembershipRightsRefresh> rights)
		{
			mt.send(SmartfoxCommand.MEMBERSHIP_REFRESH, new Dictionary<string, SFSDataWrapper>
			{
				{
					"rights",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(rights))
				}
			});
		}

		public void DequipeHeldObject()
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.EQUIPPED_OBJECT,
					null
				}
			});
		}

		public void UseConsumable(SignedResponse<UsedConsumable> consumable, object properties)
		{
			mt.send(SmartfoxCommand.USE_CONSUMABLE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"type",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(consumable))
				},
				{
					"prop",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(properties))
				}
			});
		}

		public void ReuseConsumable(string type, object properties)
		{
			mt.send(SmartfoxCommand.REUSE_CONSUMABLE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"type",
					SmartFoxGameServerClientShared.serialize(type)
				},
				{
					"prop",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(properties))
				}
			});
		}

		public void Pickup(string id, string tag, Vector3 position)
		{
			mt.send(SmartfoxCommand.PICKUP, new Dictionary<string, SFSDataWrapper>
			{
				{
					"id",
					SmartFoxGameServerClientShared.serialize(id)
				},
				{
					"t",
					SmartFoxGameServerClientShared.serialize(tag)
				},
				{
					"p",
					SmartFoxGameServerClientShared.serialize(position)
				}
			});
		}

		public void GetSignedStateOfLocalPlayer()
		{
			mt.send(SmartfoxCommand.GET_SIGNED_STATE);
		}

		public void SendRewardNotification(SignedResponse<RewardedUserCollectionJsonHelper> rewardCollection)
		{
			mt.send(SmartfoxCommand.SEND_EARNED_REWARDS, new Dictionary<string, SFSDataWrapper>
			{
				{
					"reward",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(rewardCollection))
				}
			});
		}

		public void SendWebServiceEvent(SignedResponse<WebServiceEvent> wsEvent)
		{
			mt.send(SmartfoxCommand.WEBSERVICE_EVENT, new Dictionary<string, SFSDataWrapper>
			{
				{
					"d",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(wsEvent))
				}
			}, new object[1]
			{
				wsEvent.Data.type
			});
		}

		public void GetPlayerLocation(string swid)
		{
			mt.send(SmartfoxCommand.GET_PLAYER_LOCATION, new Dictionary<string, SFSDataWrapper>
			{
				{
					"swid",
					SmartFoxGameServerClientShared.serialize(swid)
				}
			});
		}

		public void SetSelectedTube(SignedResponse<EquipTubeResponse> tube)
		{
			updateUserVars(new Dictionary<SocketUserVars, object>
			{
				{
					SocketUserVars.SELECTED_TUBE,
					mt.JsonService.Serialize(tube)
				}
			});
		}

		public void SendPartyGameSessionMessage(int sessionId, int type, object properties)
		{
			mt.send(SmartfoxCommand.PARTY_GAME_MESSAGE, new Dictionary<string, SFSDataWrapper>
			{
				{
					"id",
					SmartFoxGameServerClientShared.serialize(sessionId)
				},
				{
					"type",
					SmartFoxGameServerClientShared.serialize(type)
				},
				{
					"message",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(properties))
				}
			});
		}

		public void SendIglooUpdated(SignedResponse<IglooData> signedIglooData)
		{
			mt.send(SmartfoxCommand.IGLOO_UPDATED, new Dictionary<string, SFSDataWrapper>
			{
				{
					"igloo_data",
					SmartFoxGameServerClientShared.serialize(mt.JsonService.Serialize(signedIglooData), true)
				}
			});
		}

		private void updateUserVars(IDictionary<SocketUserVars, object> userVars)
		{
			List<UserVariable> list = new List<UserVariable>();
			foreach (SocketUserVars key in userVars.Keys)
			{
				addIfChanged(list, key, userVars[key]);
			}
			if (list.Count > 0)
			{
				mt.send(new SetUserVariablesRequest(list));
			}
		}

		private void addIfChanged(List<UserVariable> userVariables, SocketUserVars key, object newValue)
		{
			Type typeFromSFSVariableType = SmartFoxUtil.GetTypeFromSFSVariableType(key.GetVariableType());
			object obj = (newValue == null) ? null : Convert.ChangeType(newValue, typeFromSFSVariableType);
			if (mt.isLoggedIn)
			{
				UserVariable var = null;
				bool flag = mt.TryGetUserVariable(key.GetKey(), out var);
				if ((!flag && obj != null) || (flag && obj != SmartFoxUtil.GetValueFromSFSUserValue(var, typeFromSFSVariableType)))
				{
					userVariables.Add(new SFSUserVariable(key.GetKey(), obj));
				}
			}
		}
	}
}
