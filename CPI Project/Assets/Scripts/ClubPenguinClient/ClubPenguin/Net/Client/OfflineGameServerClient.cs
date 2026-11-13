using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public class OfflineGameServerClient : IGameServerClient
	{
		public interface IConsumable
		{
			IEnumerator Consume(int partialCount, string type, object properties);

			void SetGameServerClient(OfflineGameServerClient client);

			void HandleInteraction(LocomotionActionEvent action);

			int PartialCount();

			string Type();
		}

		public class EchoConsumable : IConsumable
		{
			protected OfflineGameServerClient client;

			protected string type;

			public virtual IEnumerator Consume(int partialCount, string type, object properties)
			{
				type = this.type;
				yield return client.dequipeHeldObjectResult();
			}

			public virtual void HandleInteraction(LocomotionActionEvent action)
			{
			}

			public virtual int PartialCount()
			{
				return 0;
			}

			public void SetGameServerClient(OfflineGameServerClient client)
			{
				this.client = client;
			}

			public string Type()
			{
				return type;
			}
		}

		public class PlayerHeldConsumable : EchoConsumable
		{
			protected int startingActionAcount;

			protected int actionCount;

			public PlayerHeldConsumable(int actionCount)
			{
				startingActionAcount = actionCount;
			}

			public override IEnumerator Consume(int partialCount, string type, object properties)
			{
				base.type = type;
				if (partialCount > 0)
				{
					actionCount = partialCount;
				}
				else
				{
					actionCount = startingActionAcount;
				}
				PlayerHeldItem item = new PlayerHeldItem();
				item.Id = new CPMMOItemId(client.clubPenguinClient.PlayerSessionId, CPMMOItemId.CPMMOItemParent.PLAYER);
				item.CreatorId = item.Id.Id;
				item.Type = type;
				item.Properties = "{\"actionCount\":" + actionCount + "}";
				yield return null;
				client.processEvent(GameServerEvent.SERVER_ITEM_ADDED, item);
			}

			public override int PartialCount()
			{
				return actionCount;
			}

			public override void HandleInteraction(LocomotionActionEvent action)
			{
				if (action.Type == LocomotionAction.Interact && action.Object.type == ObjectType.PLAYER)
				{
					actionCount--;
					if (actionCount <= 0)
					{
						client.DequipeHeldObject();
						return;
					}
					PlayerHeldItem playerHeldItem = new PlayerHeldItem();
					playerHeldItem.Id = new CPMMOItemId(client.clubPenguinClient.PlayerSessionId, CPMMOItemId.CPMMOItemParent.PLAYER);
					playerHeldItem.CreatorId = playerHeldItem.Id.Id;
					playerHeldItem.Type = type;
					playerHeldItem.Properties = "{\"actionCount\":" + actionCount + "}";
					client.processEvent(GameServerEvent.SERVER_ITEM_CHANGED, playerHeldItem);
				}
			}
		}

		public class SharedConsumable : PlayerHeldConsumable
		{
			private string spawnType;

			public SharedConsumable(int actionCount, string spawnType)
				: base(actionCount)
			{
				this.spawnType = spawnType;
			}

			public override void HandleInteraction(LocomotionActionEvent action)
			{
			}
		}

		public class TimedConsumable : EchoConsumable
		{
			protected Reward totalReward;

			protected Reward minReward;

			protected float rewardRadius;

			protected float timeToLive;

			protected TimedItem item;

			public TimedConsumable(float timeToLive, float rewardRadius, Reward totalReward, Reward minReward)
			{
				this.timeToLive = timeToLive;
				this.rewardRadius = rewardRadius;
				this.totalReward = totalReward;
				this.minReward = minReward;
			}

			public override IEnumerator Consume(int partialCount, string type, object properties)
			{
				base.type = type;
				item = new TimedItem(timeToLive);
				item.Id = new CPMMOItemId(client.generateMMOItemId(), CPMMOItemId.CPMMOItemParent.WORLD);
				item.CreatorId = client.clubPenguinClient.PlayerSessionId;
				item.Type = type;
				ConsumableMMODeployedEvent data = default(ConsumableMMODeployedEvent);
				data.SessionId = client.clubPenguinClient.PlayerSessionId;
				data.ExperienceId = item.Id.Id;
				Vector3 vecPosition = Vector3.zero;
				if (properties is Vector3)
				{
					vecPosition = (Vector3)properties;
				}
				CPMMOItemPosition position = default(CPMMOItemPosition);
				position.Id = item.Id;
				position.Position = vecPosition;
				yield return null;
				client.processEvent(GameServerEvent.CONSUMABLE_MMO_DEPLOYED, data);
				yield return base.Consume(partialCount, type, properties);
				yield return null;
				client.processEvent(GameServerEvent.SERVER_ITEM_ADDED, item);
				yield return null;
				client.processEvent(GameServerEvent.SERVER_ITEM_MOVED, position);
				client.activeConsumables.Add(this);
				yield return countDown();
			}

			private IEnumerator countDown()
			{
				while (true)
				{
					yield return new WaitForSeconds(1f);
					item.TimeToLive -= 1f;
					if (item.TimeToLive <= 0f)
					{
						break;
					}
					client.processEvent(GameServerEvent.SERVER_ITEM_CHANGED, item);
				}
				sendReward();
				destroyItem();
			}

			protected void sendReward()
			{
				JsonService jsonService = Service.Get<JsonService>();
				Dictionary<string, RewardJsonReader> dictionary = new Dictionary<string, RewardJsonReader>();
				dictionary.Add(client.clubPenguinClient.PlayerSessionId.ToString(), jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(totalReward))));
				SignedResponse<RewardedUserCollectionJsonHelper> signedResponse = new SignedResponse<RewardedUserCollectionJsonHelper>();
				signedResponse.Data = new RewardedUserCollectionJsonHelper
				{
					rewards = dictionary,
					source = RewardSource.WORLD_OBJECT,
					sourceId = item.Id.Id.ToString()
				};
				SignedResponse<RewardedUserCollectionJsonHelper> data = signedResponse;
				client.processEvent(GameServerEvent.RECEIVED_REWARDS, data);
			}

			protected void destroyItem()
			{
				client.processEvent(GameServerEvent.SERVER_ITEM_REMOVED, item.Id);
				client.activeConsumables.Remove(this);
			}

			public new void SetGameServerClient(OfflineGameServerClient client)
			{
				base.client = client;
			}
		}

		public class ActionedConsumable : TimedConsumable
		{
			private int startingActionCount;

			private int actionCount;

			public bool AutoDequipeHeldObject
			{
				get
				{
					return true;
				}
			}

			public ActionedConsumable(float timeToLive, float rewardRadius, Reward totalReward, Reward minReward, int startingActionCount)
				: base(timeToLive, rewardRadius, totalReward, minReward)
			{
				this.startingActionCount = startingActionCount;
			}

			public override IEnumerator Consume(int partialCount, string type, object properties)
			{
				base.type = type;
				if (partialCount > 0)
				{
					actionCount = partialCount;
				}
				else
				{
					actionCount = startingActionCount;
				}
				item = new ActionedItem(timeToLive, actionCount);
				item.Id = new CPMMOItemId(client.generateMMOItemId(), CPMMOItemId.CPMMOItemParent.WORLD);
				item.CreatorId = client.clubPenguinClient.PlayerSessionId;
				item.Type = type;
				ConsumableMMODeployedEvent data = default(ConsumableMMODeployedEvent);
				data.SessionId = client.clubPenguinClient.PlayerSessionId;
				data.ExperienceId = item.Id.Id;
				Vector3 vecPosition = Vector3.zero;
				if (properties is Vector3)
				{
					vecPosition = (Vector3)properties;
				}
				CPMMOItemPosition position = default(CPMMOItemPosition);
				position.Id = item.Id;
				position.Position = vecPosition;
				yield return null;
				client.processEvent(GameServerEvent.CONSUMABLE_MMO_DEPLOYED, data);
				yield return client.dequipeHeldObjectResult();
				client.processEvent(GameServerEvent.SERVER_ITEM_ADDED, item);
				client.processEvent(GameServerEvent.SERVER_ITEM_MOVED, position);
				client.activeConsumables.Add(this);
				yield return countDown();
			}

			public override int PartialCount()
			{
				return actionCount;
			}

			public override void HandleInteraction(LocomotionActionEvent action)
			{
				if (action.Type == LocomotionAction.Interact && action.Object.type == ObjectType.SERVER && action.Object.id == item.Id.Id.ToString())
				{
					(item as ActionedItem).ActionCount--;
				}
			}

			private IEnumerator countDown()
			{
				while (true)
				{
					yield return new WaitForSeconds(1f);
					if ((item as ActionedItem).ActionCount <= 0)
					{
						sendReward();
						break;
					}
					item.TimeToLive -= 1f;
					if (item.TimeToLive <= 0f)
					{
						break;
					}
					client.processEvent(GameServerEvent.SERVER_ITEM_CHANGED, item);
				}
				destroyItem();
			}
		}

		public interface IPartyGameSession
		{
			void HandleMessage(long userSessionId, int type, object properties);

			void SetSessionId(int sessionId);

			void StartSession();

			void EndSession(long userSessionId);
		}

		public class PartyGameSessionManager
		{
			private readonly Dictionary<int, IPartyGameSession> activePartyGames = new Dictionary<int, IPartyGameSession>();

			private int nextSessionId = 1;

			private OfflineGameServerClient client;

			public PartyGameSessionManager(OfflineGameServerClient offlineGameServerClient)
			{
				client = offlineGameServerClient;
			}

			public int StartSession(IPartyGameSession partyGame)
			{
				int num = nextSessionId++;
				activePartyGames.Add(num, partyGame);
				partyGame.SetSessionId(num);
				partyGame.StartSession();
				return num;
			}

			public void EndSession(int sessionId)
			{
				IPartyGameSession value;
				if (activePartyGames.TryGetValue(sessionId, out value))
				{
					value.EndSession(client.clubPenguinClient.PlayerSessionId);
					activePartyGames.Remove(sessionId);
				}
			}

			public void SendPartyGameSessionMessage(int sessionId, int type, object properties)
			{
				IPartyGameSession value;
				if (activePartyGames.TryGetValue(sessionId, out value))
				{
					value.HandleMessage(client.clubPenguinClient.PlayerSessionId, type, properties);
				}
			}
		}

		private readonly OfflineDatabase database;

		private readonly IOfflineDefinitionLoader definitions;

		private readonly IDictionary<GameServerEvent, GameServerEventListener> eventListeners = new Dictionary<GameServerEvent, GameServerEventListener>();

		private readonly ClubPenguinClient clubPenguinClient;

		private IOfflineRoomRunner currentRoom;

		private string currentQuest;

		private IConsumable currentConsumable = null;

		private readonly HashSet<IConsumable> activeConsumables = new HashSet<IConsumable>();

		private readonly PartyGameSessionManager partyGameSessionManager;

		private Dictionary<string, long> inRoomRewards;

		private long nextMMOItemId = 1L;

		public long ServerTime
		{
			get
			{
				return DateTime.UtcNow.GetTimeInMilliseconds();
			}
		}

		public int UserCount
		{
			get
			{
				return 1;
			}
		}

		public OfflineGameServerClient(ClubPenguinClient clubPenguinClient)
		{
			database = Service.Get<OfflineDatabase>();
			definitions = Service.Get<IOfflineDefinitionLoader>();
			this.clubPenguinClient = clubPenguinClient;
			partyGameSessionManager = new PartyGameSessionManager(this);
		}

		private long generateMMOItemId()
		{
			return nextMMOItemId++;
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

		public string CurrentRoom()
		{
			if (currentRoom == null)
			{
				return null;
			}
			return currentRoom.RoomName;
		}

		public void DequipeHeldObject()
		{
			CoroutineRunner.StartPersistent(dequipeHeldObjectResult(), this, "dequipeHeldObjectResult");
		}

		private IEnumerator dequipeHeldObjectResult()
		{
			yield return null;
			if (currentConsumable != null)
			{
				int num = currentConsumable.PartialCount();
				if (num > 0)
				{
					processEvent(GameServerEvent.COMSUMBLE_PARTIAL_COUNT_SET, new SignedResponse<UsedConsumable>
					{
						Data = new UsedConsumable
						{
							partialCount = num,
							type = currentConsumable.Type()
						}
					});
				}
				currentConsumable = null;
			}
			processEvent(GameServerEvent.HELD_OBJECT_DEQUIPPED, clubPenguinClient.PlayerSessionId);
		}

		public void EquipConsumable(string type)
		{
		}

		public void EquipDispensable(int id)
		{
		}

		public void EquipDurable(SignedResponse<EquipDurableResponse> durable)
		{
		}

		public void GetPlayerLocation(string swid)
		{
			CoroutineRunner.StartPersistent(getPlayerLocationResponse(), this, "getPlayerLocationResponse");
		}

		private IEnumerator getPlayerLocationResponse()
		{
			yield return null;
			processEvent(GameServerEvent.PLAYER_NOT_FOUND, null);
		}

		public void GetSignedStateOfLocalPlayer()
		{
			CoroutineRunner.StartPersistent(getSignedStateOfLocalPlayerResponse(), this, "getSignedStateOfLocalPlayerResponse");
		}

		private IEnumerator getSignedStateOfLocalPlayerResponse()
		{
			yield return null;
			processEvent(GameServerEvent.STATE_SIGNED, new SignedResponse<InWorldState>());
		}

		public bool IsConnected()
		{
			return currentRoom != null;
		}

		public void JoinRoom(SignedResponse<JoinRoomData> signedJoinRoomData)
		{
			currentRoom = Service.Get<IOfflineRoomFactory>().Create(signedJoinRoomData.Data.room, processEvent, generateMMOItemId, partyGameSessionManager);
			currentRoom.Start();
			clubPenguinClient.PlayerSessionId = signedJoinRoomData.Data.sessionId;
			clubPenguinClient.PlayerName = signedJoinRoomData.Data.userName;
			inRoomRewards = signedJoinRoomData.Data.earnedRewards;
			if (inRoomRewards == null)
			{
				inRoomRewards = new Dictionary<string, long>();
			}
			setCurrentQuest(signedJoinRoomData.Data.playerRoomData.quests);
			CoroutineRunner.StartPersistent(joinRoomResponse(), this, "joinRoomResponse");
		}

		private IEnumerator joinRoomResponse()
		{
			yield return null;
			processEvent(GameServerEvent.ROOM_JOIN, currentRoom.RoomName);
		}

		public void LeaveRoom()
		{
			CoroutineRunner.StartPersistent(leaveRoomResponse(currentRoom.RoomName), this, "leaveRoomResponse");
			currentRoom.End();
			currentRoom = null;
		}

		private IEnumerator leaveRoomResponse(string room)
		{
			yield return null;
			processEvent(GameServerEvent.ROOM_LEAVE, room);
		}

		public void Pickup(string id, string tag, Vector3 position)
		{
			inRoomRewards[id] = ServerTime;
			CoroutineRunner.StartPersistent(pickupResponse(), this, "pickupResponse");
		}

		private IEnumerator pickupResponse()
		{
			yield return null;
			SignedResponse<ClubPenguin.Net.Domain.InRoomRewards> response = new SignedResponse<ClubPenguin.Net.Domain.InRoomRewards>
			{
				Data = new ClubPenguin.Net.Domain.InRoomRewards
				{
					collected = inRoomRewards,
					room = new RoomIdentifier(currentRoom.RoomName).zoneId.name
				}
			};
			processEvent(GameServerEvent.RECEIVED_ROOOM_REWARDS, response);
		}

		public void PrototypeSendAction(object data)
		{
		}

		public void PrototypeSetState(object data)
		{
		}

		public void QuestCompleteObjective(string objective)
		{
			CoroutineRunner.StartPersistent(questCompleteObjectiveResponse(objective), this, "questCompleteObjectiveResponse");
		}

		private IEnumerator questCompleteObjectiveResponse(string objective)
		{
			yield return null;
			QuestObjectives objectives = new QuestObjectives();
			HashSet<string> objs = new HashSet<string>
			{
				objective
			};
			foreach (QuestStates.QuestState quest in database.Read<QuestStates>().Quests)
			{
				if (quest.questId == currentQuest)
				{
					if (quest.completedObjectives != null)
					{
						foreach (string completedObjective in quest.completedObjectives)
						{
							objs.Add(completedObjective);
						}
					}
					break;
				}
			}
			objectives.AddRange(objs);
			processEvent(GameServerEvent.QUEST_OBJECTIVES_UPDATED, new SignedResponse<QuestObjectives>
			{
				Data = objectives
			});
		}

		public void QuestSetQuestState(SignedResponse<QuestStateCollection> quests)
		{
			setCurrentQuest(quests.Data);
			CoroutineRunner.StartPersistent(setQuestStateResponse(), this, "setQuestStateResponse");
		}

		private void setCurrentQuest(QuestStateCollection quests)
		{
			foreach (QuestState quest in quests)
			{
				if (quest.status == QuestStatus.ACTIVE)
				{
					currentQuest = quest.questId;
				}
			}
		}

		private IEnumerator setQuestStateResponse()
		{
			yield return null;
			processEvent(GameServerEvent.QUEST_DATA_SYNCED, null);
		}

		public void RefreshMembership(SignedResponse<MembershipRightsRefresh> rightsRefresh)
		{
		}

		public void RemoveAirBubble()
		{
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

		public void ReuseConsumable(string type, object properties)
		{
			CoroutineRunner.StartPersistent(useConsumableAction(type, 0, properties), this, "useConsumableAction");
		}

		public void SendChatActivity()
		{
		}

		public void SendChatActivityCancel()
		{
		}

		public void SendChatMessage(SignedResponse<ChatMessage> signedChatMessage)
		{
		}

		public void SendIglooUpdated(SignedResponse<IglooData> signedIglooData)
		{
		}

		public void SendPartyGameSessionMessage(int sessionId, int type, object properties)
		{
			partyGameSessionManager.SendPartyGameSessionMessage(sessionId, type, properties);
		}

		public void SendRewardNotification(SignedResponse<RewardedUserCollectionJsonHelper> response)
		{
		}

		public void SendWebServiceEvent(SignedResponse<WebServiceEvent> wsEvent)
		{
		}

		public void SetAirBubble(float value, int diveState)
		{
		}

		public void SetAwayFromKeyboard(int value)
		{
		}

		public void SetConsumableInventory(SignedResponse<ClubPenguin.Net.Domain.ConsumableInventory> inventory)
		{
		}

		public void SetLocomotionState(LocomotionState state)
		{
		}

		public void SetOutfit(SignedResponse<ClubPenguin.Net.Domain.PlayerOutfitDetails> outfit)
		{
		}

		public void SetProfile(SignedResponse<ClubPenguin.Net.Domain.Profile> profile)
		{
		}

		public void SetSelectedTube(SignedResponse<EquipTubeResponse> tube)
		{
		}

		public void SetTemporaryHeadStatus(int value)
		{
		}

		public void TriggerLocomotionAction(LocomotionActionEvent action, bool droppable)
		{
			action.SessionId = clubPenguinClient.PlayerSessionId;
			if (action.Object != null && action.Object.type == ObjectType.LOCAL)
			{
				List<string> list = new List<string>();
				list.Add(action.Object.id);
				Reward inRoomReward = definitions.GetInRoomReward(list);
				if (!inRoomReward.isEmpty())
				{
					inRoomRewards[action.Object.id] = ServerTime;
					CoroutineRunner.StartPersistent(pickupResponse(), this, "pickupResponse");
				}
			}
			if (currentConsumable != null)
			{
				currentConsumable.HandleInteraction(action);
			}
			foreach (IConsumable activeConsumable in activeConsumables)
			{
				activeConsumable.HandleInteraction(action);
			}
			if (currentRoom != null)
			{
				currentRoom.HandleInteraction(action);
			}
		}

		public void UseConsumable(SignedResponse<UsedConsumable> consumable, object properties)
		{
			CoroutineRunner.StartPersistent(useConsumableAction(consumable.Data.type, consumable.Data.partialCount, properties), this, "useConsumableAction");
		}

		private IEnumerator useConsumableAction(string type, int partialCount, object properties)
		{
			yield return null;
			currentConsumable = definitions.GetConsumable(type);
			if (currentConsumable != null)
			{
				currentConsumable.SetGameServerClient(this);
				yield return currentConsumable.Consume(partialCount, type, properties);
			}
		}

		private void processEvent(GameServerEvent gameServerEvent, object data)
		{
			GameServerEventListener value;
			if (eventListeners.TryGetValue(gameServerEvent, out value) && value != null)
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
	}
}
