using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Client.Smartfox;
using ClubPenguin.Net.Client.Smartfox.Item;
using ClubPenguin.Net.Client.Smartfox.SFSObject;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Utils;
using Disney.LaunchPadFramework;
using LitJson;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Logging;
using Sfs2X.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	internal class SmartFoxGameServerClientSFSThread
	{
		private struct TimeStampRequest
		{
			public readonly Stopwatch Timer;

			public readonly bool FetchEncryptionKey;

			public TimeStampRequest(Stopwatch timer, bool fetchEncryptionKey)
			{
				Timer = timer;
				FetchEncryptionKey = fetchEncryptionKey;
			}
		}

		private const int MAX_CONNECTION_ATTEMPTS = 3;

		private readonly SmartFoxGameServerClientShared mt;

		private long timestampRequestCount = 0L;

		private Dictionary<long, TimeStampRequest> timeStampRequests = new Dictionary<long, TimeStampRequest>();

		internal SmartFoxGameServerClientSFSThread(SmartFoxGameServerClientShared smartFoxGameServerClientShared)
		{
			mt = smartFoxGameServerClientShared;
		}

		private void onConnectionRetry(BaseEvent evt)
		{
		}

		private void onConnectionResume(BaseEvent evt)
		{
		}

		private void onSocketError(BaseEvent evt)
		{
			string data = (string)evt.Params["errorMessage"];
			mt.triggerEvent(GameServerEvent.NETWORK_ERROR, data);
		}

		private void onUserVariableUpdate(BaseEvent evt)
		{
            UnityEngine.Debug.Log("UserVariableUpdate as been fired");
			User user = (User)evt.Params["user"];
			ArrayList changedVars = (ArrayList)evt.Params["changedVars"];
            broadcastUserVariables(user, changedVars);
		}

		private void broadcastUserVariables(User user, ArrayList changedVars = null)
		{
			bool flag = getSessionId(user) == mt.ClubPenguinClient.PlayerSessionId;
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.PROTOTYPE.GetKey())) && user.ContainsVariable(SocketUserVars.PROTOTYPE.GetKey()))
			{
				PrototypeState prototypeState = default(PrototypeState);
				prototypeState.id = getSessionId(user);
				prototypeState.data = JsonMapper.ToObject(user.GetVariable(SocketUserVars.PROTOTYPE.GetKey()).GetStringValue());
				mt.triggerEvent(GameServerEvent.PROTOTYPE_STATE, prototypeState);
			}
			if (changedVars == null || changedVars.Contains(SocketUserVars.EQUIPPED_OBJECT.GetKey()))
			{
				UserVariable variable = user.GetVariable(SocketUserVars.EQUIPPED_OBJECT.GetKey());
				UserVariable variable2 = user.GetVariable(SocketUserVars.EQUIPPED_OBJECT_PROPERTIES.GetKey());
				if (variable == null)
				{
					if (changedVars != null && changedVars.Contains(SocketUserVars.EQUIPPED_OBJECT_PROPERTIES.GetKey()) && variable2 == null)
					{
						mt.triggerEvent(GameServerEvent.SERVER_ITEM_REMOVED, new CPMMOItemId(getSessionId(user), CPMMOItemId.CPMMOItemParent.PLAYER));
					}
					else
					{
						mt.triggerEvent(GameServerEvent.HELD_OBJECT_DEQUIPPED, getSessionId(user));
					}
				}
				else
				{
					EquippedObject equippedObject = EquippedObject.FromSFSData(variable.GetSFSObjectValue());
					if (variable2 != null)
					{
						PlayerHeldItem playerHeldItem = new PlayerHeldItem();
						playerHeldItem.Id = new CPMMOItemId(getSessionId(user), CPMMOItemId.CPMMOItemParent.PLAYER);
						playerHeldItem.CreatorId = playerHeldItem.Id.Id;
						playerHeldItem.Type = equippedObject.EquippedObjectId;
						playerHeldItem.Properties = variable2.GetStringValue();
						mt.triggerEvent(GameServerEvent.SERVER_ITEM_ADDED, playerHeldItem);
					}
					else if (!flag || (flag && equippedObject.isPartyGame()))
					{
						HeldObjectEvent heldObjectEvent = default(HeldObjectEvent);
						heldObjectEvent.SessionId = getSessionId(user);
						heldObjectEvent.Type = equippedObject.EquippedObjectId;
						if (equippedObject.IsConsumable())
						{
							mt.triggerEvent(GameServerEvent.CONSUMABLE_EQUIPPED, heldObjectEvent);
						}
						else if (equippedObject.IsDispensable())
						{
							mt.triggerEvent(GameServerEvent.DISPENSABLE_EQUIPPED, heldObjectEvent);
						}
						else if (equippedObject.IsDurable())
						{
							mt.triggerEvent(GameServerEvent.DURABLE_EQUIPPED, heldObjectEvent);
						}
						else if (equippedObject.isPartyGame())
						{
							mt.triggerEvent(GameServerEvent.PARTYGAME_EQUIPPED, heldObjectEvent);
						}
					}
				}
			}
			else if (changedVars.Contains(SocketUserVars.EQUIPPED_OBJECT_PROPERTIES.GetKey()))
			{
				UserVariable variable3 = user.GetVariable(SocketUserVars.EQUIPPED_OBJECT.GetKey());
				if (variable3 != null && variable3.Type == VariableType.OBJECT)
				{
					PlayerHeldItem playerHeldItem = new PlayerHeldItem();
					playerHeldItem.Id = new CPMMOItemId(getSessionId(user), CPMMOItemId.CPMMOItemParent.PLAYER);
					playerHeldItem.CreatorId = playerHeldItem.Id.Id;
					playerHeldItem.Type = EquippedObject.FromSFSData(variable3.GetSFSObjectValue()).EquippedObjectId;
					playerHeldItem.Properties = user.GetVariable(SocketUserVars.EQUIPPED_OBJECT_PROPERTIES.GetKey()).GetStringValue();
					mt.triggerEvent(GameServerEvent.SERVER_ITEM_CHANGED, playerHeldItem);
				}
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.LOCOMOTION_STATE.GetKey())))
			{
				UserVariable variable4 = user.GetVariable(SocketUserVars.LOCOMOTION_STATE.GetKey());
				PlayerLocomotionStateEvent playerLocomotionStateEvent = default(PlayerLocomotionStateEvent);
				playerLocomotionStateEvent.SessionId = getSessionId(user);
				if (variable4 == null || variable4.IsNull())
				{
					playerLocomotionStateEvent.State = LocomotionState.Default;
				}
				else
				{
					playerLocomotionStateEvent.State = (LocomotionState)variable4.GetIntValue();
				}
				mt.triggerEvent(GameServerEvent.USER_LOCO_STATE_CHANGED, playerLocomotionStateEvent);
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.OUTFIT.GetKey())) && user.ContainsVariable(SocketUserVars.OUTFIT.GetKey()))
			{
				PlayerOutfitDetails playerOutfitDetails = mt.JsonService.Deserialize<PlayerOutfitDetails>(user.GetVariable(SocketUserVars.OUTFIT.GetKey()).GetStringValue());
				playerOutfitDetails.sessionId = getSessionId(user);
				mt.triggerEvent(GameServerEvent.USER_OUTFIT_CHANGED, playerOutfitDetails);
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.PROFILE.GetKey())) && user.ContainsVariable(SocketUserVars.PROFILE.GetKey()))
			{
				ProfileEvent profileEvent = default(ProfileEvent);
				profileEvent.SessionId = getSessionId(user);
				profileEvent.Profile = new Profile();
				profileEvent.Profile.colour = user.GetVariable(SocketUserVars.PROFILE.GetKey()).GetIntValue();
				mt.triggerEvent(GameServerEvent.USER_PROFILE_CHANGED, profileEvent);
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.AIR_BUBBLE.GetKey())))
			{
				UserVariable variable5 = user.GetVariable(SocketUserVars.AIR_BUBBLE.GetKey());
				PlayerAirBubbleEvent playerAirBubbleEvent = default(PlayerAirBubbleEvent);
				playerAirBubbleEvent.SessionId = getSessionId(user);
				if (variable5 == null || variable5.IsNull())
				{
					playerAirBubbleEvent.AirBubble = new AirBubble();
				}
				else
				{
					playerAirBubbleEvent.AirBubble = AirBubble.FromSFSData(variable5.GetSFSObjectValue());
				}
				mt.triggerEvent(GameServerEvent.AIR_BUBBLE_UPDATE, playerAirBubbleEvent);
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.ON_QUEST.GetKey())))
			{
				UserVariable variable4 = user.GetVariable(SocketUserVars.ON_QUEST.GetKey());
				OnQuestState onQuestState = default(OnQuestState);
				onQuestState.SessionId = getSessionId(user);
				if (variable4 != null && !variable4.IsNull())
				{
					onQuestState.MascotName = variable4.GetStringValue();
				}
				mt.triggerEvent(GameServerEvent.ON_QUEST, onQuestState);
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.SELECTED_TUBE.GetKey())) && user.ContainsVariable(SocketUserVars.SELECTED_TUBE.GetKey()))
			{
				SelectedTubeEvent selectedTubeEvent = default(SelectedTubeEvent);
				selectedTubeEvent.SessionId = getSessionId(user);
				selectedTubeEvent.TubeId = user.GetVariable(SocketUserVars.SELECTED_TUBE.GetKey()).GetIntValue();
				mt.triggerEvent(GameServerEvent.SELECTED_TUBE_CHANGED, selectedTubeEvent);
			}
			if (!flag && (changedVars == null || changedVars.Contains(SocketUserVars.TEMPORARY_HEAD_STATUS.GetKey())) && user.ContainsVariable(SocketUserVars.TEMPORARY_HEAD_STATUS.GetKey()))
			{
				TemporaryHeadStatusEvent temporaryHeadStatusEvent = default(TemporaryHeadStatusEvent);
				temporaryHeadStatusEvent.SessionId = getSessionId(user);
				temporaryHeadStatusEvent.Type = user.GetVariable(SocketUserVars.TEMPORARY_HEAD_STATUS.GetKey()).GetIntValue();
				mt.triggerEvent(GameServerEvent.TEMPORARY_HEAD_STATUS_CHANGED, temporaryHeadStatusEvent);
			}
			if (flag || (changedVars != null && !changedVars.Contains(SocketUserVars.AWAY_FROM_KEYBOARD.GetKey())))
			{
				return;
			}
			UserVariable variable6 = user.GetVariable(SocketUserVars.AWAY_FROM_KEYBOARD.GetKey());
			if (variable6 == null || variable6.IsNull())
			{
				EquippedObject equippedObject2 = null;
				UserVariable variable7 = user.GetVariable(SocketUserVars.EQUIPPED_OBJECT.GetKey());
				if (variable7 != null)
				{
					equippedObject2 = EquippedObject.FromSFSData(variable7.GetSFSObjectValue());
				}
				mt.triggerEvent(GameServerEvent.AWAY_FROM_KEYBOARD_STATE_CHANGED, new AFKEvent(getSessionId(user), 0, equippedObject2));
			}
			else
			{
				mt.triggerEvent(GameServerEvent.AWAY_FROM_KEYBOARD_STATE_CHANGED, new AFKEvent(getSessionId(user), variable6.GetIntValue(), null));
			}
		}

		internal void TriggerBinaryStateEvent(UserVariable state, long sessionId, GameServerEvent gameServerEvent)
		{
			if (state == null || state.IsNull())
			{
				mt.triggerEvent(gameServerEvent, new BinaryState(sessionId, false));
			}
			else
			{
				mt.triggerEvent(gameServerEvent, new BinaryState(sessionId, state.GetBoolValue()));
			}
		}

		internal void AddListeners(SmartFox smartFox)
		{
			smartFox.AddEventListener(SFSEvent.SOCKET_ERROR, onSocketError);
			smartFox.AddEventListener(SFSEvent.CONNECTION, onConnection);
			smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, onConnectionLost);
			smartFox.AddEventListener(SFSEvent.CRYPTO_INIT, onCryptoInit);
			smartFox.AddEventListener(SFSEvent.CONNECTION_RETRY, onConnectionRetry);
			smartFox.AddEventListener(SFSEvent.CONNECTION_RESUME, onConnectionResume);
			smartFox.AddEventListener(SFSEvent.LOGIN, onLogin);
			smartFox.AddEventListener(SFSEvent.UDP_INIT, onUdpInit);
			smartFox.AddEventListener(SFSEvent.LOGIN_ERROR, onLoginError);
			smartFox.AddEventListener(SFSEvent.LOGOUT, onLogout);
			smartFox.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, onRoomCreationError);
			smartFox.AddEventListener(SFSEvent.ROOM_JOIN, onRoomJoin);
			smartFox.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, onRoomJoinError);
			smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, onExtensionResponse);
			smartFox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, onUserVariableUpdate);
			smartFox.AddEventListener(SFSEvent.PROXIMITY_LIST_UPDATE, onProximityListUpdate);
			smartFox.AddEventListener(SFSEvent.MMOITEM_VARIABLES_UPDATE, onServerObjectUpdate);
			smartFox.AddEventListener(SFSEvent.PING_PONG, onPingPong);
			if (smartFox.Debug)
			{
				smartFox.AddLogListener(LogLevel.DEBUG, onDebugMessage);
				smartFox.AddLogListener(LogLevel.INFO, onInfoMessage);
				smartFox.AddLogListener(LogLevel.WARN, onWarnMessage);
				smartFox.AddLogListener(LogLevel.ERROR, onErrorMessage);
			}
		}

		internal void RemoveListeners(SmartFox smartFox)
		{
			smartFox.RemoveAllEventListeners();
			if (smartFox.Debug)
			{
				smartFox.RemoveLogListener(LogLevel.DEBUG, onDebugMessage);
				smartFox.RemoveLogListener(LogLevel.INFO, onInfoMessage);
				smartFox.RemoveLogListener(LogLevel.WARN, onWarnMessage);
				smartFox.RemoveLogListener(LogLevel.ERROR, onErrorMessage);
			}
		}

		private void onProximityListUpdate(BaseEvent evt)
		{
			RoomMember roomMember;
			foreach (User item in (List<User>)evt.Params["addedUsers"])
			{
				roomMember = default(RoomMember);
				roomMember.Id = getSessionId(item);
				roomMember.Name = item.Name;
				mt.triggerEvent(GameServerEvent.ROOM_USER_ADDED, roomMember);
				LocomotionActionEvent locomotionActionEvent = default(LocomotionActionEvent);
				locomotionActionEvent.SessionId = getSessionId(item);
				locomotionActionEvent.Type = LocomotionAction.SnapToPosition;
				locomotionActionEvent.Position = new Vector3(item.AOIEntryPoint.FloatX, item.AOIEntryPoint.FloatY, item.AOIEntryPoint.FloatZ);
				locomotionActionEvent.Direction = Vector3.zero;
				locomotionActionEvent.Timestamp = 0L;
				mt.triggerEvent(GameServerEvent.USER_LOCO_ACTION, locomotionActionEvent);
				broadcastUserVariables(item);
			}
			foreach (User item2 in (List<User>)evt.Params["removedUsers"])
			{
				roomMember = default(RoomMember);
				roomMember.Id = getSessionId(item2);
				roomMember.Name = item2.Name;
				mt.triggerEvent(GameServerEvent.ROOM_USER_REMOVED, roomMember);
				item2.UserManager.RemoveUser(item2);
			}
			foreach (IMMOItem item3 in (List<IMMOItem>)evt.Params["addedItems"])
			{
				CPMMOItem cPMMOItem = ItemFactory.Create(item3, getSessionId);
				mt.triggerEvent(GameServerEvent.SERVER_ITEM_ADDED, cPMMOItem);
				CPMMOItemPosition cPMMOItemPosition = default(CPMMOItemPosition);
				cPMMOItemPosition.Id = cPMMOItem.Id;
				cPMMOItemPosition.Position = new Vector3(item3.AOIEntryPoint.FloatX, item3.AOIEntryPoint.FloatY, item3.AOIEntryPoint.FloatZ);
				mt.triggerEvent(GameServerEvent.SERVER_ITEM_MOVED, cPMMOItemPosition);
			}
			foreach (IMMOItem item4 in (List<IMMOItem>)evt.Params["removedItems"])
			{
				mt.triggerEvent(GameServerEvent.SERVER_ITEM_REMOVED, new CPMMOItemId(item4.Id, CPMMOItemId.CPMMOItemParent.WORLD));
			}
		}

		private void onServerObjectUpdate(BaseEvent evt)
		{
			IMMOItem sfsItem = (IMMOItem)evt.Params["mmoItem"];
			CPMMOItem data = ItemFactory.Create(sfsItem, getSessionId);
			mt.triggerEvent(GameServerEvent.SERVER_ITEM_CHANGED, data);
		}

		private void onPingPong(BaseEvent evt)
		{
			int milliseconds = (int)evt.Params["lagValue"];
			mt.ClubPenguinClient.logGameServerPing(milliseconds);
		}

		private void onExtensionResponse(BaseEvent evt)
		{
			try
			{
				ISFSObject iSFSObject = (ISFSObject)evt.Params["params"];
				if (mt.SmartFoxEncryptor != null)
				{
					mt.SmartFoxEncryptor.DecryptParameters(iSFSObject);
				}
				int? userId = null;
				if (iSFSObject.ContainsKey("senderId"))
				{
					userId = iSFSObject.GetInt("senderId");
				}
				SmartfoxCommand? smartfoxCommand = SmartFoxCommand.FromString((string)evt.Params["cmd"]);
				if (smartfoxCommand.HasValue)
				{
					object data = null;
					GameServerEvent? gameServerEvent = null;
					switch (smartfoxCommand.Value)
					{
					case SmartfoxCommand.CHAT_ACTIVITY:
						gameServerEvent = GameServerEvent.CHAT_ACTIVITY_RECEIVED;
						data = getSessionId(userId);
						break;
					case SmartfoxCommand.CHAT_ACTIVITY_CANCEL:
						gameServerEvent = GameServerEvent.CHAT_ACTIVITY_CANCEL_RECEIVED;
						data = getSessionId(userId);
						break;
					case SmartfoxCommand.CHAT:
                      //  UnityEngine.Debug.Log("chat.msg   :::   " + iSFSObject.ToJson());
                        gameServerEvent = GameServerEvent.CHAT_MESSAGE_RECEIVED;
						data = mt.JsonService.Deserialize<ReceivedChatMessage>(iSFSObject.GetUtfString("msg"));
						break;
					case SmartfoxCommand.GET_SERVER_TIME:
						timeStampRecieved(iSFSObject.GetLong("ct"), iSFSObject.GetLong("st"));
						break;
					case SmartfoxCommand.SERVER_TIME_ERROR:
						fetchServerTimestamp(false);
						break;
					case SmartfoxCommand.GET_ROOM_ENCRYPTION_KEY:
						encryptionKeyReceived(iSFSObject.GetUtfString("ek"));
						break;
					case SmartfoxCommand.ENCRYPTION_KEY_ERROR:
						Log.LogError(this, "Failed to get room encryption key.");
						mt.teardown();
						gameServerEvent = GameServerEvent.ROOM_JOIN_ERROR;
						data = null;
						break;
					case SmartfoxCommand.LOCOMOTION_ACTION:
						gameServerEvent = GameServerEvent.USER_LOCO_ACTION;
						data = locomotionActionEventFromProps(userId.Value, iSFSObject);
						break;
					case SmartfoxCommand.PROTOTYPE:
					{
						gameServerEvent = GameServerEvent.PROTOTYPE_ACTION;
						PrototypeAction prototypeAction = default(PrototypeAction);
						prototypeAction.userid = getSessionId(userId);
						prototypeAction.data = JsonMapper.ToObject(iSFSObject.GetUtfString("data"));
						data = prototypeAction;
						break;
					}
					case SmartfoxCommand.SERVER_OBJECTIVE_COMPLETED:
						gameServerEvent = GameServerEvent.QUEST_OBJECTIVES_UPDATED;
						data = mt.JsonService.Deserialize<SignedResponse<QuestObjectives>>(iSFSObject.GetUtfString("data"));
						break;
					case SmartfoxCommand.SERVER_QUEST_ERROR:
						gameServerEvent = GameServerEvent.QUEST_ERROR;
						data = null;
						break;
					case SmartfoxCommand.SET_QUEST_STATES:
						gameServerEvent = GameServerEvent.QUEST_DATA_SYNCED;
						data = null;
						break;
					case SmartfoxCommand.USE_CONSUMABLE:
					{
						gameServerEvent = GameServerEvent.CONSUMABLE_USED;
						ConsumableEvent consumableEvent = default(ConsumableEvent);
						consumableEvent.SessionId = getSessionId(userId);
						consumableEvent.Type = iSFSObject.GetUtfString("type");
						data = consumableEvent;
						break;
					}
					case SmartfoxCommand.REUSE_FAILED:
					{
						gameServerEvent = GameServerEvent.CONSUMABLE_REUSE_FAILED;
						ConsumableUseFailureEvent consumableUseFailureEvent = default(ConsumableUseFailureEvent);
						consumableUseFailureEvent.Type = iSFSObject.GetUtfString("type");
						consumableUseFailureEvent.Properties = JsonMapper.ToObject(iSFSObject.GetUtfString("prop"));
						data = consumableUseFailureEvent;
						break;
					}
					case SmartfoxCommand.CONSUMABLE_MMO_DEPLOYED:
						if (iSFSObject.ContainsKey("ownerId") && iSFSObject.ContainsKey("senderId"))
						{
							gameServerEvent = GameServerEvent.CONSUMABLE_MMO_DEPLOYED;
							ConsumableMMODeployedEvent consumableMMODeployedEvent = default(ConsumableMMODeployedEvent);
							consumableMMODeployedEvent.SessionId = getSessionId(iSFSObject.GetInt("ownerId"));
							consumableMMODeployedEvent.ExperienceId = iSFSObject.GetInt("senderId");
							data = consumableMMODeployedEvent;
						}
						break;
					case SmartfoxCommand.SET_CONSUMABLE_PARTIAL_COUNT:
						gameServerEvent = GameServerEvent.COMSUMBLE_PARTIAL_COUNT_SET;
						data = mt.JsonService.Deserialize<SignedResponse<UsedConsumable>>(iSFSObject.GetUtfString("data"));
						break;
					case SmartfoxCommand.RECEIVED_REWARDS:
						gameServerEvent = GameServerEvent.RECEIVED_REWARDS;
						data = mt.JsonService.Deserialize<SignedResponse<RewardedUserCollectionJsonHelper>>(iSFSObject.GetUtfString("reward"));
						break;
					case SmartfoxCommand.RECEIVED_REWARDS_DELAYED:
						gameServerEvent = GameServerEvent.RECEIVED_REWARDS_DELAYED;
						data = mt.JsonService.Deserialize<SignedResponse<RewardedUserCollectionJsonHelper>>(iSFSObject.GetUtfString("reward"));
						break;
					case SmartfoxCommand.RECEIVED_ROOOM_REWARDS:
						gameServerEvent = GameServerEvent.RECEIVED_ROOOM_REWARDS;
						data = mt.JsonService.Deserialize<SignedResponse<InRoomRewards>>(iSFSObject.GetUtfString("reward"));
						break;
					case SmartfoxCommand.PROGRESSION_LEVELUP:
					{
						gameServerEvent = GameServerEvent.LEVELUP;
						UserLevelUpEvent userLevelUpEvent = default(UserLevelUpEvent);
						userLevelUpEvent.SessionId = getSessionId(userId);
						userLevelUpEvent.Level = iSFSObject.GetInt("level");
						data = userLevelUpEvent;
						break;
					}
					case SmartfoxCommand.ROOM_TRANSIENT_DATA:
						mt.LastRoomTransientData = mt.JsonService.Deserialize<SignedResponse<TransientData>>(iSFSObject.GetUtfString("d"));
						break;
					case SmartfoxCommand.TASK_COUNT:
					{
						gameServerEvent = GameServerEvent.TASK_COUNT_UPDATED;
						TaskProgress taskProgress = default(TaskProgress);
						taskProgress.taskId = iSFSObject.GetUtfString("t");
						taskProgress.counter = iSFSObject.GetInt("c");
						data = taskProgress;
						break;
					}
					case SmartfoxCommand.TASK_PROGRESS:
						gameServerEvent = GameServerEvent.TASK_PROGRESS_UPDATED;
						data = mt.JsonService.Deserialize<SignedResponse<TaskProgress>>(iSFSObject.GetUtfString("d"));
						break;
					case SmartfoxCommand.GET_SIGNED_STATE:
						gameServerEvent = GameServerEvent.STATE_SIGNED;
						data = mt.JsonService.Deserialize<SignedResponse<InWorldState>>(iSFSObject.GetUtfString("data"));
						break;
					case SmartfoxCommand.GET_PLAYER_LOCATION:
						gameServerEvent = GameServerEvent.PLAYER_LOCATION_RECEIVED;
						data = SmartFoxGameServerClientShared.deserializeVec3(iSFSObject, "pos");
						break;
					case SmartfoxCommand.PLAYER_NOT_FOUND:
						gameServerEvent = GameServerEvent.PLAYER_NOT_FOUND;
						break;
					case SmartfoxCommand.PARTY_GAME_START:
						gameServerEvent = GameServerEvent.PARTY_GAME_START;
						data = partyGameStartEventFromProps(iSFSObject);
						break;
					case SmartfoxCommand.PARTY_GAME_START_V2:
						gameServerEvent = GameServerEvent.PARTY_GAME_START_V2;
						data = partyGameStartEventV2FromProps(iSFSObject);
						break;
					case SmartfoxCommand.PARTY_GAME_END:
						gameServerEvent = GameServerEvent.PARTY_GAME_END;
						data = partyGameEndEventFromProps(iSFSObject);
						break;
					case SmartfoxCommand.PARTY_GAME_MESSAGE:
						gameServerEvent = GameServerEvent.PARTY_GAME_MESSAGE;
						data = partyGameMessageEventFromProps(iSFSObject);
						break;
					case SmartfoxCommand.IGLOO_UPDATED:
						gameServerEvent = GameServerEvent.IGLOO_UPDATED;
						data = iSFSObject.GetUtfString("igloo_id");
						break;
					case SmartfoxCommand.FORCE_LEAVE:
						gameServerEvent = GameServerEvent.FORCE_LEAVE;
						data = mt.JsonService.Deserialize<ZoneId>(iSFSObject.GetUtfString("zone_id"));
						break;
					}
					if (gameServerEvent.HasValue)
					{
						mt.triggerEvent(gameServerEvent.Value, data);
					}
				}
			}
			catch (JsonException ex)
			{
				Log.LogNetworkError(this, "Error occurred getting data from SFS: " + ex.Message);
			}
		}

		private void fetchServerTimestamp(bool fetchEncryptionKeyAfterwards)
		{
			long num = timestampRequestCount++;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			timeStampRequests.Add(num, new TimeStampRequest(stopwatch, fetchEncryptionKeyAfterwards));
			mt.send(SmartfoxCommand.GET_SERVER_TIME, new Dictionary<string, SFSDataWrapper>
			{
				{
					"ct",
					SmartFoxGameServerClientShared.serialize(num)
				}
			});
		}

		private void timeStampRecieved(long clientTime, long serverTime)
		{
			if (!timeStampRequests.ContainsKey(clientTime))
			{
				Log.LogErrorFormatted(this, "Ignoring unexpected time sync event {0}", clientTime);
				return;
			}
			TimeStampRequest timeStampRequest = timeStampRequests[clientTime];
			timeStampRequests.Remove(clientTime);
			long elapsedMilliseconds = timeStampRequest.Timer.ElapsedMilliseconds;
			timeStampRequest.Timer.Stop();
			mt.SetServerTimeUpdate(serverTime + elapsedMilliseconds / 2);
			if (timeStampRequest.FetchEncryptionKey)
			{
				fetchEncryptionKey();
			}
		}

		private void fetchEncryptionKey()
		{
			RSAParameters value = mt.ClubPenguinClient.CPKeyValueDatabase.GetRsaParameters().Value;
			string str = Convert.ToBase64String(value.Exponent);
			string str2 = Convert.ToBase64String(value.Modulus);
			mt.send(SmartfoxCommand.GET_ROOM_ENCRYPTION_KEY, new Dictionary<string, SFSDataWrapper>
			{
				{
					"pkm",
					SmartFoxGameServerClientShared.serialize(str2)
				},
				{
					"pke",
					SmartFoxGameServerClientShared.serialize(str)
				}
			});
		}

		private void encryptionKeyReceived(string encodedEncryptedEncryptionKey)
		{
			/*RSAParameters value = mt.ClubPenguinClient.CPKeyValueDatabase.GetRsaParameters().Value;
			byte[] ciphertext = Convert.FromBase64String(encodedEncryptedEncryptionKey);
			byte[] key = RsaEncryptor.Decrypt(ciphertext, value);
			mt.SmartFoxEncryptor = new SmartFoxEncryptor(key);*/
			roomJoinCompleted();
		}

		private void roomJoinCompleted()
		{
			JoinRoomData joinRoomData;
			if (mt.TryClearJoinRoomData(out joinRoomData))
			{
				mt.ClientRoomName = joinRoomData.room;
				mt.triggerEvent(GameServerEvent.ROOM_JOIN, joinRoomData.room);
			}
			mt.LastRoomTransientData = null;
		}

		private void onConnection(BaseEvent evt)
		{
			bool flag = (bool)evt.Params["success"];
			string text = (string)evt.Params["errorMessage"];
			if (flag)
			{
				if (mt.UseEncryption)
				{
					mt.TriggerInitCrypto = true;
				}
				else
				{
					mt.login();
				}
				return;
			}
			if (mt.ConnectionAttempts < 3)
			{
				mt.reconnect();
				return;
			}
			Log.LogNetworkErrorFormatted(this, "Failed to connect after {0} attempts with error: {1}. Will trigger a ROOM_JOIN_ERROR", mt.ConnectionAttempts, text);
			RoomJoinError roomJoinError = default(RoomJoinError);
			roomJoinError.roomName = mt.JoinRoomDataRoom;
			roomJoinError.errorMessage = text;
			mt.teardown();
			mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
		}

		private void onCryptoInit(BaseEvent evt)
		{
			bool flag = (bool)evt.Params["success"];
			string text = (string)evt.Params["errorMessage"];
			if (flag)
			{
				mt.login();
				return;
			}
			Log.LogNetworkErrorFormatted(this, "Failed to initialize encryption libraries. Error: {0}", text);
			RoomJoinError roomJoinError = default(RoomJoinError);
			roomJoinError.roomName = mt.JoinRoomDataRoom;
			roomJoinError.errorMessage = text;
			mt.teardown();
			mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
		}

		private void onLogin(BaseEvent evt)
		{
			User user = (User)evt.Params["user"];
			mt.ClubPenguinClient.PlayerSessionId = mt.JoinRoomDataSessionId;
			mt.ClubPenguinClient.PlayerName = user.Name;
			mt.onLogin();
		}

		private void onLoginError(BaseEvent evt)
		{
			string text = (string)evt.Params["errorMessage"];
			Log.LogNetworkErrorFormatted(this, "Login Failed. Error: {0}", text);
			RoomJoinError roomJoinError = default(RoomJoinError);
			roomJoinError.roomName = mt.JoinRoomDataRoom;
			roomJoinError.errorMessage = text;
			mt.teardown();
			mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
		}

		private void onRoomCreationError(BaseEvent evt)
		{
			Room room = (Room)evt.Params["room"];
			string text = (string)evt.Params["errorMessage"];
			Log.LogErrorFormatted(this, "Failed to create room {0}. error: {1}", room.Name, text);
			JoinRoomData joinRoomData;
			if (mt.TryClearJoinRoomDataIfRoom(room.Name, out joinRoomData))
			{
				RoomJoinError roomJoinError = default(RoomJoinError);
				roomJoinError.roomName = joinRoomData.room;
				roomJoinError.errorMessage = text;
				mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
			}
			mt.LastRoomTransientData = null;
		}

		private void onRoomJoin(BaseEvent evt)
		{
			Room room = (Room)evt.Params["room"];
			if (RoomIdentifier.EqualsIgnoreInstanceId(room.Name, mt.JoinRoomDataRoom))
			{
				mt.ClientRoomName = mt.JoinRoomDataRoom;
				if (SmartFoxGameServerClient.EnableUDP)
				{
					mt.initUDP();
				}
				else
				{
					fetchServerTimestamp(true);
				}
			}
		}

		private void onRoomJoinError(BaseEvent evt)
		{
			string text = (string)evt.Params["errorMessage"];
			short num = (short)evt.Params["errorCode"];
			Log.LogErrorFormatted(this, "Failed to join room error: {0} - {1}", num.ToString(), text);
			JoinRoomData joinRoomData;
			if (mt.TryClearJoinRoomData(out joinRoomData))
			{
				RoomJoinError roomJoinError = default(RoomJoinError);
				roomJoinError.roomName = joinRoomData.room;
				roomJoinError.errorMessage = text;
				mt.teardown();
				if (num == 20)
				{
					mt.triggerEvent(GameServerEvent.ROOM_FULL, roomJoinError);
				}
				else
				{
					mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
				}
			}
			mt.LastRoomTransientData = null;
		}

		private void onUdpInit(BaseEvent evt)
		{
			bool flag = (bool)evt.Params["success"];
			string text = (string)evt.Params["errorMessage"];
			if (flag)
			{
				fetchServerTimestamp(true);
				return;
			}
			ConfigData smartFoxConfiguration = mt.smartFoxConfiguration;
			Log.LogNetworkErrorFormatted(this, "Failed to initialize UDP connection on {0}:{1}. Error: {2}", smartFoxConfiguration.UdpHost, smartFoxConfiguration.UdpPort, text);
			if (SmartFoxGameServerClient.AbortOnUDPError)
			{
				RoomJoinError roomJoinError = default(RoomJoinError);
				roomJoinError.roomName = mt.JoinRoomDataRoom;
				roomJoinError.errorMessage = text;
				mt.teardown();
				mt.triggerEvent(GameServerEvent.ROOM_JOIN_ERROR, roomJoinError);
			}
			else
			{
				fetchServerTimestamp(true);
			}
		}

		private void onConnectionLost(BaseEvent evt)
		{
			string text = (string)evt.Params["reason"];
			if (mt.WasTornDownImmediately)
			{
				return;
			}
			string clientRoomName = mt.ClientRoomName;
			mt.teardown();
			if (text.Equals(ClientDisconnectionReason.MANUAL))
			{
				Room room;
				if (mt.TryClearSfsRoomToLeave(out room))
				{
					mt.triggerEvent(GameServerEvent.ROOM_LEAVE, clientRoomName);
					return;
				}
				string str = "Client has requested a disconnection from the game server, but this did not occur as part of the LeaveRoom process. This should not occurr.";
				mt.triggerEvent(GameServerEvent.CONNECTION_LOST, text + ": " + str);
			}
			else
			{
				mt.triggerEvent(GameServerEvent.CONNECTION_LOST, text);
				mt.SfsRoomToLeave = null;
			}
		}

		private void onLogout(BaseEvent evt)
		{
			mt.ClientRoomName = null;
			if (mt.isConnected && !mt.WasTornDownImmediately)
			{
				mt.TriggerForceTeardownAfterDelay = true;
				mt.Disconnect();
			}
			else
			{
				mt.WasTornDownImmediately = false;
			}
		}

		private void onDebugMessage(BaseEvent evt)
		{
			string text = (string)evt.Params["message"];
		}

		private void onInfoMessage(BaseEvent evt)
		{
			string text = (string)evt.Params["message"];
		}

		private void onWarnMessage(BaseEvent evt)
		{
			string text = (string)evt.Params["message"];
		}

		private void onErrorMessage(BaseEvent evt)
		{
			if (!mt.WasTornDownImmediately)
			{
				string text = (string)evt.Params["message"];
				Log.LogNetworkErrorFormatted(this, "[SFS ERROR] {0}", text);
			}
		}

		private LocomotionActionEvent locomotionActionEventFromProps(int playerId, ISFSObject props)
		{
           // UnityEngine.Debug.Log("penguin? l.a   :::   " + props.ToJson());
            LocomotionActionEvent result = default(LocomotionActionEvent);
			result.SessionId = getSessionId(playerId);
			result.Type = (LocomotionAction)props.GetByte("a");
			result.Position = SmartFoxGameServerClientShared.deserializeVec3(props, "p");
			result.Timestamp = props.GetLong("t");
			if (props.ContainsKey("d"))
			{
				result.Direction = SmartFoxGameServerClientShared.deserializeVec3(props, "d");
			}
			if (props.ContainsKey("v"))
			{
				result.Velocity = SmartFoxGameServerClientShared.deserializeVec3(props, "v");
			}
			if (props.ContainsKey("o"))
			{
				result.Object = mt.JsonService.Deserialize<ActionedObject>(props.GetUtfString("o"));
			}
			return result;
		}

		private long getSessionId(User user)
		{
			return long.Parse(user.GetVariable(SocketUserVars.SESSION_ID.GetKey()).GetStringValue());
		}

		private long getSessionId(int? userId)
		{
			if (!userId.HasValue)
			{
				return -1L;
			}
			User userById = mt.GetUserById(userId.Value);
			if (userById == null)
			{
				return -1L;
			}
			return getSessionId(userById);
		}

		private PartyGameStartEvent partyGameStartEventFromProps(ISFSObject props)
		{
			PartyGameStartEvent result = default(PartyGameStartEvent);
			result.owner = props.GetLong("owner");
			result.sessionId = props.GetInt("id");
			result.templateId = props.GetInt("template");
			result.players = props.GetLongArray("players");
			return result;
		}

		private PartyGameStartEventV2 partyGameStartEventV2FromProps(ISFSObject props)
		{
			PartyGameStartEventV2 result = default(PartyGameStartEventV2);
			result.sessionId = props.GetInt("id");
			result.templateId = props.GetInt("template");
			result.playerData = props.GetUtfString("players");
			return result;
		}

		private PartyGameEndEvent partyGameEndEventFromProps(ISFSObject props)
		{
			PartyGameEndEvent result = default(PartyGameEndEvent);
			List<PartyGameEndPlayerResult> list = mt.JsonService.Deserialize<List<PartyGameEndPlayerResult>>(props.GetUtfString("results"));
			Dictionary<long, int> dictionary = new Dictionary<long, int>();
			for (int i = 0; i < list.Count; i++)
			{
				dictionary.Add(list[i].PlayerSessionId, list[i].Placement);
			}
			result.playerSessionIdToPlacement = dictionary;
			result.sessionId = props.GetInt("id");
			return result;
		}

		private PartyGameMessageEvent partyGameMessageEventFromProps(ISFSObject props)
		{
			PartyGameMessageEvent result = default(PartyGameMessageEvent);
			result.sessionId = props.GetInt("id");
			result.type = props.GetInt("type");
			result.message = props.GetUtfString("message");
			return result;
		}
	}
}
