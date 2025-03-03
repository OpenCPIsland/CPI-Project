using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.PartyGames;
using ClubPenguin.ScheduledWorldObjects;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using SwrveUnity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace ClubPenguin
{
	public class OfflineRoomRunner : IOfflineRoomRunner
	{
		private interface IWorldObject
		{
			string GetId();

			void HandleInteraction(LocomotionActionEvent action);
		}

		private class TubeRaceWorldObject : IWorldObject
		{
			private PartygameLobbyMmoItem cpItem;

			private Action<GameServerEvent, object> processEvent;

			private OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager;

			private TubeRaceDefinition raceDefinition;

			private List<long> usersInRace;

			public TubeRaceWorldObject(PartygameLobbyMmoItem cpItem, Action<GameServerEvent, object> processEvent, OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager, TubeRaceDefinition raceDefinition)
			{
				this.cpItem = cpItem;
				this.processEvent = processEvent;
				this.partyGameSessionManager = partyGameSessionManager;
				this.raceDefinition = raceDefinition;
				usersInRace = new List<long>();
			}

			public string GetId()
			{
				return cpItem.Id.Id.ToString();
			}

			public void HandleInteraction(LocomotionActionEvent action)
			{
				usersInRace.Add(action.SessionId);
				cpItem.playerData = getPlayerData(usersInRace);
				processEvent(GameServerEvent.SERVER_ITEM_CHANGED, cpItem);
				HeldObjectEvent heldObjectEvent = default(HeldObjectEvent);
				heldObjectEvent.SessionId = action.SessionId;
				heldObjectEvent.Type = "TubeRaceLobbyServerAdded";
				processEvent(GameServerEvent.PARTYGAME_EQUIPPED, heldObjectEvent);
				PlayerLocomotionStateEvent playerLocomotionStateEvent = default(PlayerLocomotionStateEvent);
				playerLocomotionStateEvent.SessionId = action.SessionId;
				playerLocomotionStateEvent.State = LocomotionState.Slide;
				processEvent(GameServerEvent.USER_LOCO_STATE_CHANGED, playerLocomotionStateEvent);
			}

			internal void StartRace()
			{
				if (usersInRace.Count > 0)
				{
					foreach (long item in usersInRace)
					{
						processEvent(GameServerEvent.HELD_OBJECT_DEQUIPPED, item);
					}
					TubeRaceGameSession partyGame = new TubeRaceGameSession(usersInRace, cpItem, processEvent, partyGameSessionManager, raceDefinition);
					partyGameSessionManager.StartSession(partyGame);
				}
			}

			internal static string getPlayerData(IEnumerable<long> usersInRace)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{\"Players\":[");
				foreach (long item in usersInRace)
				{
					stringBuilder.Append("{\"TeamId\":0,\"RoleId\":0,\"UserSessionId\":" + item + "},");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append("]}");
				return stringBuilder.ToString();
			}
		}

		private class TubeRaceGameSession : OfflineGameServerClient.IPartyGameSession
		{
			private int sessionId;

			private Dictionary<long, int> userScores;

			private PartygameLobbyMmoItem cpItem;

			private Action<GameServerEvent, object> processEvent;

			private Stopwatch timer;

			private OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager;

			private TubeRaceDefinition raceDefinition;

			public TubeRaceGameSession(List<long> usersInRace, PartygameLobbyMmoItem cpItem, Action<GameServerEvent, object> processEvent, OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager, TubeRaceDefinition raceDefinition)
			{
				userScores = new Dictionary<long, int>();
				foreach (long item in usersInRace)
				{
					userScores.Add(item, 0);
				}
				this.cpItem = cpItem;
				this.processEvent = processEvent;
				this.partyGameSessionManager = partyGameSessionManager;
				this.raceDefinition = raceDefinition;
				timer = new Stopwatch();
			}

			public void SetSessionId(int sessionId)
			{
				this.sessionId = sessionId;
			}

			public void StartSession()
			{
				PartyGameStartEventV2 partyGameStartEventV = default(PartyGameStartEventV2);
				partyGameStartEventV.sessionId = sessionId;
				partyGameStartEventV.templateId = cpItem.gameTemplateId;
				partyGameStartEventV.playerData = TubeRaceWorldObject.getPlayerData(userScores.Keys);
				processEvent(GameServerEvent.PARTY_GAME_START_V2, partyGameStartEventV);
				PartyGameMessageEvent partyGameMessageEvent = default(PartyGameMessageEvent);
				partyGameMessageEvent.sessionId = sessionId;
				partyGameMessageEvent.type = 10;
				partyGameMessageEvent.message = "{\"ModifierLayoutId\":" + UnityEngine.Random.Range(0, 3) + "}";
				processEvent(GameServerEvent.PARTY_GAME_MESSAGE, partyGameMessageEvent);
				foreach (long key in userScores.Keys)
				{
					PlayerLocomotionStateEvent playerLocomotionStateEvent = default(PlayerLocomotionStateEvent);
					playerLocomotionStateEvent.SessionId = key;
					playerLocomotionStateEvent.State = LocomotionState.Racing;
					processEvent(GameServerEvent.USER_LOCO_STATE_CHANGED, playerLocomotionStateEvent);
				}
				timer.Start();
			}

			public void HandleMessage(long userSessionId, int type, object properties)
			{
				switch (type)
				{
				case 11:
					collectScoreModifier(userSessionId, (PartyGameSessionMessages.CollectTubeRaceScoreModifier)properties);
					break;
				case 12:
					partyGameSessionManager.EndSession(sessionId);
					break;
				}
			}

			private void collectScoreModifier(long userSessionId, PartyGameSessionMessages.CollectTubeRaceScoreModifier modifierId)
			{
				TubeRaceScoreModifier[] array = UnityEngine.Object.FindObjectsOfType<TubeRaceScoreModifier>();
				int num = 0;
				TubeRaceScoreModifier tubeRaceScoreModifier;
				while (true)
				{
					if (num < array.Length)
					{
						tubeRaceScoreModifier = array[num];
						if (tubeRaceScoreModifier.ModifierId == modifierId.ModifierId)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				userScores[userSessionId] += tubeRaceScoreModifier.ScoreDelta;
			}

			public void EndSession(long userSessionId)
			{
				timer.Stop();
				JsonService jsonService = Service.Get<JsonService>();
				PartyGameSessionMessages.TubeRacePlayerResult tubeRacePlayerResult = new PartyGameSessionMessages.TubeRacePlayerResult();
				tubeRacePlayerResult.CompletionTimeInMilliseconds = timer.ElapsedMilliseconds;
				tubeRacePlayerResult.OverallScore = raceDefinition.StartingPoints - timer.ElapsedMilliseconds * raceDefinition.PointsDeductedPerSecond / 1000 + userScores[userSessionId];
				tubeRacePlayerResult.PlayerId = userSessionId;
				tubeRacePlayerResult.ScoreModifier = userScores[userSessionId];
				PartyGameSessionMessages.TubeRacePlayerResult objectToSerialize = tubeRacePlayerResult;
				PartyGameMessageEvent partyGameMessageEvent = default(PartyGameMessageEvent);
				partyGameMessageEvent.sessionId = sessionId;
				partyGameMessageEvent.type = 13;
				partyGameMessageEvent.message = jsonService.Serialize(objectToSerialize);
				PartyGameMessageEvent partyGameMessageEvent2 = partyGameMessageEvent;
				processEvent(GameServerEvent.PARTY_GAME_MESSAGE, partyGameMessageEvent2);
				PartyGameDefinition value;
				if (Service.Get<IGameData>().Get<Dictionary<int, PartyGameDefinition>>().TryGetValue(cpItem.gameTemplateId, out value))
				{
					foreach (PartyGameDefinition.PartyGameReward reward in value.Rewards)
					{
						if (reward.Placement == PartyGameEndPlacement.FIRST)
						{
							Dictionary<string, RewardJsonReader> dictionary = new Dictionary<string, RewardJsonReader>();
							dictionary.Add(userSessionId.ToString(), jsonService.Deserialize<RewardJsonReader>(jsonService.Serialize(RewardJsonWritter.FromReward(reward.Reward.ToReward()))));
							SignedResponse<RewardedUserCollectionJsonHelper> signedResponse = new SignedResponse<RewardedUserCollectionJsonHelper>();
							signedResponse.Data = new RewardedUserCollectionJsonHelper
							{
								rewards = dictionary,
								source = RewardSource.MINI_GAME,
								sourceId = sessionId.ToString()
							};
							SignedResponse<RewardedUserCollectionJsonHelper> arg = signedResponse;
							processEvent(GameServerEvent.RECEIVED_REWARDS_DELAYED, arg);
							break;
						}
					}
				}
				CoroutineRunner.StartPersistent(finishEndSession(), this, "finishEndSession");
			}

			private IEnumerator finishEndSession()
			{
				yield return null;
				int placement = 0;
				Dictionary<long, int> playerSessionIdToPlacement = new Dictionary<long, int>();
				foreach (long key in userScores.Keys)
				{
					playerSessionIdToPlacement.Add(key, placement);
					if (placement < 4)
					{
						placement++;
					}
				}
				PartyGameEndEvent endEvent = new PartyGameEndEvent
				{
					playerSessionIdToPlacement = playerSessionIdToPlacement,
					sessionId = sessionId
				};
				processEvent(GameServerEvent.PARTY_GAME_END, endEvent);
				foreach (long key2 in userScores.Keys)
				{
					processEvent(GameServerEvent.HELD_OBJECT_DEQUIPPED, key2);
					PlayerLocomotionStateEvent playerLocomotionStateEvent = default(PlayerLocomotionStateEvent);
					playerLocomotionStateEvent.SessionId = key2;
					playerLocomotionStateEvent.State = LocomotionState.Default;
					processEvent(GameServerEvent.USER_LOCO_STATE_CHANGED, playerLocomotionStateEvent);
				}
			}
		}

		private readonly string roomName;

		private readonly Action<GameServerEvent, object> processEvent;

		private readonly Func<long> generateMMOItemId;

		private readonly OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager;

		private readonly List<ICoroutine> coroutines;

		private readonly List<IWorldObject> worldObjects;

		private EventDispatcher eventDispatcher;

		public string RoomName
		{
			get
			{
				return roomName;
			}
		}

		public OfflineRoomRunner(string roomName, Action<GameServerEvent, object> processEvent, Func<long> generateMMOItemId, OfflineGameServerClient.PartyGameSessionManager partyGameSessionManager)
		{
			this.generateMMOItemId = generateMMOItemId;
			this.roomName = roomName;
			this.processEvent = processEvent;
			this.partyGameSessionManager = partyGameSessionManager;
			coroutines = new List<ICoroutine>();
			worldObjects = new List<IWorldObject>();
			eventDispatcher = Service.Get<EventDispatcher>();
		}

		public void Start()
		{
			eventDispatcher.AddListener<SceneTransitionEvents.TransitionComplete>(sceneLoaded);
		}

		public void HandleInteraction(LocomotionActionEvent action)
		{
			if (action.Type == LocomotionAction.Interact && action.Object != null && action.Object.type == ObjectType.SERVER)
			{
				foreach (IWorldObject worldObject in worldObjects)
				{
					if (action.Object.id == worldObject.GetId())
					{
						worldObject.HandleInteraction(action);
					}
				}
			}
		}

		public void End()
		{
			foreach (ICoroutine coroutine in coroutines)
			{
				if (!coroutine.Completed && !coroutine.Cancelled)
				{
					coroutine.Cancel();
				}
			}
			coroutines.Clear();
		}

		private bool sceneLoaded(SceneTransitionEvents.TransitionComplete evt)
		{
			eventDispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(sceneLoaded);
			startTubeRacingSchedule();
			startScheduledWorldObjects();
			return false;
		}

		private void startScheduledWorldObjects()
		{
			ScheduledWorldObjectConfiguration[] array = UnityEngine.Object.FindObjectsOfType<ScheduledWorldObjectConfiguration>();
			foreach (ScheduledWorldObjectConfiguration scheduledWorldObjectConfiguration in array)
			{
				List<StatefulWorldObject> list = new List<StatefulWorldObject>();
				Transform[] componentsInChildren = scheduledWorldObjectConfiguration.GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					if (!(transform == scheduledWorldObjectConfiguration.transform))
					{
						StatefulWorldObject statefulWorldObject = new StatefulWorldObject();
						statefulWorldObject.CreatorId = -1L;
						statefulWorldObject.Id = new CPMMOItemId(generateMMOItemId(), CPMMOItemId.CPMMOItemParent.WORLD);
						statefulWorldObject.State = ScheduledWorldObjectState.Inactive;
						statefulWorldObject.Path = transform.GetPath();
						statefulWorldObject.Timestamp = 0L;
						StatefulWorldObject statefulWorldObject2 = statefulWorldObject;
						processEvent(GameServerEvent.SERVER_ITEM_ADDED, statefulWorldObject2);
						CPMMOItemPosition cPMMOItemPosition = default(CPMMOItemPosition);
						cPMMOItemPosition.Id = statefulWorldObject2.Id;
						cPMMOItemPosition.Position = transform.position;
						processEvent(GameServerEvent.SERVER_ITEM_MOVED, cPMMOItemPosition);
						list.Add(statefulWorldObject2);
					}
				}
				coroutines.Add(CoroutineRunner.StartPersistent(triggerScheduledWorldObjects(scheduledWorldObjectConfiguration, list), this, "triggerScheduledWorldObjects"));
			}
		}

		private IEnumerator triggerScheduledWorldObjects(ScheduledWorldObjectConfiguration config, List<StatefulWorldObject> worldObjects)
		{
			int activeIndex = 0;
			if (config.Behaviour == SelectionBehaviour.NonRepeatingRandom)
			{
				worldObjects.Shuffle();
			}
			while (true)
			{
				setWorldObjectNewState(worldObjects[activeIndex], ScheduledWorldObjectState.Inactive);
				switch (config.Behaviour)
				{
				case SelectionBehaviour.Ordered:
					activeIndex = (activeIndex + 1) % worldObjects.Count;
					break;
				case SelectionBehaviour.Random:
					activeIndex = UnityEngine.Random.Range(0, worldObjects.Count - 1);
					break;
				case SelectionBehaviour.NonRepeatingRandom:
					activeIndex++;
					if (activeIndex > worldObjects.Count - 1)
					{
						activeIndex = 0;
						worldObjects.Shuffle();
					}
					break;
				}
				setWorldObjectNewState(worldObjects[activeIndex], ScheduledWorldObjectState.Active);
				yield return new WaitForSeconds(config.CycleTimeInSeconds);
			}
		}

		private void setWorldObjectNewState(StatefulWorldObject worldObject, ScheduledWorldObjectState state)
		{
			if (worldObject.State != state)
			{
				worldObject.State = state;
				worldObject.Timestamp = DateTime.UtcNow.GetTimeInMilliseconds();
				processEvent(GameServerEvent.SERVER_ITEM_CHANGED, worldObject);
			}
		}

		private void startTubeRacingSchedule()
		{
			Dictionary<int, PartyGameLauncherDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, PartyGameLauncherDefinition>>();
			TubeRaceLobby[] array = UnityEngine.Object.FindObjectsOfType<TubeRaceLobby>();
			TubeRaceLobby[] array2 = array;
			foreach (TubeRaceLobby tubeRaceLobby in array2)
			{
				PartyGameDefinition gameDefinition = tubeRaceLobby.GameDefinition;
				PartyGameLauncherDefinition value;
				if (!(gameDefinition != null) || !dictionary.TryGetValue(gameDefinition.Id, out value))
				{
					continue;
				}
				PartyGameLobbyMmoItemTeamDefinition partyGameLobbyMmoItemTeamDefinition = gameDefinition.LobbyData as PartyGameLobbyMmoItemTeamDefinition;
				if (partyGameLobbyMmoItemTeamDefinition != null)
				{
					TubeRaceDefinition tubeRaceDefinition = gameDefinition.GameData as TubeRaceDefinition;
					if (tubeRaceDefinition != null)
					{
						coroutines.Add(CoroutineRunner.StartPersistent(triggerTubeRace(partyGameLobbyMmoItemTeamDefinition, gameDefinition.Id, value.EveryXMinutesAfterTheHour, tubeRaceDefinition), this, "triggerTubeRace"));
					}
				}
			}
		}

		private IEnumerator triggerTubeRace(PartyGameLobbyMmoItemTeamDefinition lobbyData, int templateId, int minutesBetweenLaunches, TubeRaceDefinition raceDefinition)
		{
			while (true)
			{
				yield return new WaitForSeconds(minutesBetweenLaunches * 60 - lobbyData.LobbyLengthInSeconds);
				PartygameLobbyMmoItem cpItem = new PartygameLobbyMmoItem
				{
					CreatorId = -1L,
					gameTemplateId = templateId,
					Id = new CPMMOItemId(generateMMOItemId(), CPMMOItemId.CPMMOItemParent.WORLD),
					playerData = "{\"Players\":[]}",
					timeStartedInSeconds = DateTime.UtcNow.GetTimeInSeconds(),
					timeToLive = lobbyData.LobbyLengthInSeconds
				};
				processEvent(GameServerEvent.SERVER_ITEM_ADDED, cpItem);
				CPMMOItemPosition position = default(CPMMOItemPosition);
				position.Id = cpItem.Id;
				position.Position = lobbyData.LobbyLocation;
				processEvent(GameServerEvent.SERVER_ITEM_MOVED, position);
				TubeRaceWorldObject worldObject = new TubeRaceWorldObject(cpItem, processEvent, partyGameSessionManager, raceDefinition);
				worldObjects.Add(worldObject);
				yield return new WaitForSeconds(lobbyData.LobbyLengthInSeconds);
				worldObject.StartRace();
				worldObjects.Remove(worldObject);
				processEvent(GameServerEvent.SERVER_ITEM_REMOVED, cpItem.Id);
			}
		}
	}
}
