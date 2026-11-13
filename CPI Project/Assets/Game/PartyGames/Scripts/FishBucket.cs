using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucket : AbstractPartyGameSession
	{
		public class FishBucketPlayerData
		{
			public long PlayerId;

			public int PlayerNum;

			public bool IsLocalPlayer;

			public int Score;

			public string DisplayName;

			public int Placement;

			public FishBucketBucket BucketReference;

			public FishBucketPlayerData(long playerId, int playerNum, bool isLocalPlayer, string displayName)
			{
				PlayerId = playerId;
				PlayerNum = playerNum;
				IsLocalPlayer = isLocalPlayer;
				DisplayName = displayName;
				Score = 0;
				Placement = -1;
				BucketReference = null;
			}
		}

		private enum FishBucketState
		{
			AwaitingPositions,
			Intro,
			Game,
			Finished
		}

		private enum ControlsState
		{
			Disabled,
			Action,
			ActionAndPass
		}

		private const int DEFINITION_ID = 1;

		private readonly ParticipationController localPlayerParticipationController;

		private PartyGameDefinition partyGameDefinition;

		private FishBucketDefinition fishBucketDefinition;

		private FishBucketState currentState;

		private ControlsState currentControlsState;

		private JsonService jsonService;

		private long localPlayerSessionId;

		private long currentTurnPlayerSessionId;

		private CPDataEntityCollection dataEntityCollection;

		private GameObject localPlayerGameObject;

		private GameObject cannonGameObject;

		private FishBucketAnimationController animationController;

		private FishBucketHud hudUI;

		private Dictionary<long, FishBucketPlayerData> playerData;

		private EventDispatcher dispatcher;

		private int totalCards;

		private List<long> playersAbandonded;

		private readonly PrefabContentKey PLAYER_POSITION_PREFAB_KEY = new PrefabContentKey("Prefabs/PartyGameSpawnChecker");

		private readonly PrefabContentKey CANNON_PREFAB_KEY = new PrefabContentKey("Prefabs/FishBucket/FishBucketCannon");

		private readonly PrefabContentKey HUD_UI_PREFAB_KEY = new PrefabContentKey("Prefabs/FishBucket/FishBucketHud");

		private readonly PrefabContentKey END_POPUP_PREFAB_KEY = new PrefabContentKey("Prefabs/FishBucket/FishBucketEndGame");

		public FishBucket()
		{
			dispatcher = Service.Get<EventDispatcher>();
			playersAbandonded = new List<long>();
			localPlayerParticipationController = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<ParticipationController>();
			jsonService = Service.Get<JsonService>();
			localPlayerSessionId = Service.Get<CPDataEntityCollection>().LocalPlayerSessionId;
			localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			createFishBucketAnimationController();
			addListeners();
			getDefinitions();
		}

		protected override void handleSessionMessage(PartyGameSessionMessageTypes type, string data)
		{
			switch (type)
			{
			case PartyGameSessionMessageTypes.AddItem:
			case PartyGameSessionMessageTypes.RemoveItem:
				break;
			case PartyGameSessionMessageTypes.SetGameState:
			{
				PartyGameSessionMessages.SetGameState setGameState = jsonService.Deserialize<PartyGameSessionMessages.SetGameState>(data);
				FishBucketState gameStateId = (FishBucketState)setGameState.GameStateId;
				changeState(gameStateId);
				break;
			}
			case PartyGameSessionMessageTypes.SetGameStartPositions:
				handleSetGameStartPositions(jsonService.Deserialize<PartyGameSessionMessages.SetGameStartData>(data));
				break;
			case PartyGameSessionMessageTypes.ShowTurnOutput:
				handleShowTurnOutput(jsonService.Deserialize<PartyGameSessionMessages.ShowTurnOutput>(data));
				break;
			case PartyGameSessionMessageTypes.PlayerTurnStart:
				handlePlayerTurnStart(jsonService.Deserialize<PartyGameSessionMessages.PlayerTurnStart>(data));
				break;
			case PartyGameSessionMessageTypes.PlayerLeftGame:
				handlePlayerLeftGame(jsonService.Deserialize<PartyGameSessionMessages.PlayerLeftGame>(data));
				break;
			}
		}

		protected override void startGame()
		{
			loadAudioPrefab(partyGameDefinition);
			changeState(FishBucketState.AwaitingPositions);
			PartyGameUtils.AddParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.AddActionConfirmationFilter(partyGameDefinition);
			PartyGameUtils.DisableMainNavigation();
			PartyGameUtils.DisableLocomotionControls();
			initPlayerData();
			Content.LoadAsync(onHudPrefabLoaded, HUD_UI_PREFAB_KEY);
			setAnimationControllerPlayers();
			if (base.players[0].UserSessionId == localPlayerSessionId)
			{
				PartyGameUtils.LogGameStartBi(partyGameDefinition.name, base.players.Count);
				PartyGameUtils.StartBiTimer(partyGameDefinition.name, base.sessionId);
			}
			if (!Service.Get<UIElementDisablerManager>().IsUIElementDisabled("CellphoneButton"))
			{
				PartyGameUtils.DisableCellPhoneButton();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameEvents.PartyGameStarted(partyGameDefinition));
		}

		private void setAnimationControllerPlayers()
		{
			List<long> list = new List<long>();
			for (int i = 0; i < base.players.Count; i++)
			{
				list.Add(base.players[i].UserSessionId);
			}
			animationController.SetPlayers(list);
		}

		private void initPlayerData()
		{
			string displayName = "";
			playerData = new Dictionary<long, FishBucketPlayerData>();
			for (int i = 0; i < base.players.Count; i++)
			{
				long userSessionId = base.players[i].UserSessionId;
				DataEntityHandle handle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(userSessionId);
				DisplayNameData component;
				if (dataEntityCollection.TryGetComponent(handle, out component))
				{
					displayName = component.DisplayName;
				}
				FishBucketPlayerData value = new FishBucketPlayerData(userSessionId, i + 1, localPlayerSessionId == userSessionId, displayName);
				GameObjectReferenceData component2;
				if (dataEntityCollection.TryGetComponent(handle, out component2))
				{
					component2.GameObject.GetComponent<PropUser>().EPropSpawned += onPropSpawned;
				}
				playerData[userSessionId] = value;
			}
		}

		private void onPropSpawned(Prop prop)
		{
			SessionIdData component;
			if (dataEntityCollection.TryGetComponent(prop.PropUserRef.PlayerHandle, out component))
			{
				FishBucketPlayerData fishBucketPlayerData = playerData[component.SessionId];
				FishBucketBucket component2 = prop.GetComponent<FishBucketBucket>();
				component2.SetBucketColor(fishBucketPlayerData.PlayerNum);
				playerData[component.SessionId].BucketReference = component2;
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick", true));
			}
			prop.PropUserRef.EPropSpawned -= onPropSpawned;
		}

		private void onHudPrefabLoaded(string path, GameObject prefab)
		{
			if (currentState != FishBucketState.Finished)
			{
				GameObject gameObject = GameObject.Find("HudLeftPanel");
				GameObject gameObject2 = UnityEngine.Object.Instantiate(prefab, gameObject.transform, false);
				hudUI = gameObject2.GetComponent<FishBucketHud>();
				hudUI.Init(playerData, fishBucketDefinition);
				hudUI.SetState(FishBucketHud.FishBucketHudState.Instructions);
				animationController.SetFishBucketHud(hudUI);
				if (totalCards != 0)
				{
					hudUI.SetTotalCards(totalCards);
				}
			}
		}

		protected override void endGame(Dictionary<long, int> playerSessionIdToPlacement)
		{
			if (playerSessionIdToPlacement.Count == 0)
			{
				PartyGameUtils.LogBalkBi(partyGameDefinition.name);
				return;
			}
			foreach (FishBucketPlayerData value in playerData.Values)
			{
				if (playerSessionIdToPlacement.ContainsKey(value.PlayerId))
				{
					value.Placement = playerSessionIdToPlacement[value.PlayerId];
				}
			}
			List<PartyGameEndGamePlayerData> list = new List<PartyGameEndGamePlayerData>();
			bool flag = false;
			foreach (FishBucketPlayerData value2 in playerData.Values)
			{
				PartyGameEndGamePlayerData item = new PartyGameEndGamePlayerData(value2.PlayerId, value2.Placement, value2.PlayerNum, value2.IsLocalPlayer, value2.Score, true);
				if (list.Count == 0 || value2.Placement == -1)
				{
					list.Add(item);
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (value2.Placement <= list[i].Placement || list[i].Placement == -1)
						{
							list.Insert(i, item);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(item);
					}
					flag = false;
				}
			}
			loadEndGameScreen(list.ToArray());
			if (base.players[0].UserSessionId == localPlayerSessionId)
			{
				PartyGameUtils.StopBiTimer(base.sessionId);
				PartyGameUtils.LogGameEndBi(partyGameDefinition.name, playerSessionIdToPlacement.Count);
			}
		}

		private void loadEndGameScreen(PartyGameEndGamePlayerData[] orderedPlayerData)
		{
			Content.LoadAsync(delegate(string path, GameObject popupPrefab)
			{
				onEndGameScreenLoaded(popupPrefab, orderedPlayerData);
			}, END_POPUP_PREFAB_KEY);
		}

		private void onEndGameScreenLoaded(GameObject endGamePopup, PartyGameEndGamePlayerData[] orderedPlayerData)
		{
			PartyGameEndGamePopup component = UnityEngine.Object.Instantiate(endGamePopup).GetComponent<PartyGameEndGamePopup>();
			component.SetPlayerResults(orderedPlayerData, partyGameDefinition, base.sessionId);
			dispatcher.DispatchEvent(new PopupEvents.ShowCameraSpacePopup(component.gameObject));
		}

		protected override void destroy()
		{
			PartyGameUtils.RemoveParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.RemoveActionConfirmationFilter();
			PartyGameUtils.EnableMainNavigation();
			PartyGameUtils.EnableLocomotionControls();
			animationController.Destroy();
			removeListeners();
			ResetCamera();
			UnityEngine.Object.Destroy(hudUI.gameObject);
			UnityEngine.Object.Destroy(cannonGameObject);
			if (animationController != null)
			{
				FishBucketAnimationController fishBucketAnimationController = animationController;
				fishBucketAnimationController.CannonRotationCompleteAction = (System.Action)Delegate.Remove(fishBucketAnimationController.CannonRotationCompleteAction, new System.Action(onCannonRotationComplete));
			}
			CoroutineRunner.StopAllForOwner(this);
			currentState = FishBucketState.Finished;
			PartyGameUtils.EnableCellPhoneButton();
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameEvents.PartyGameEnded(partyGameDefinition));
		}

		private void createFishBucketAnimationController()
		{
			animationController = new FishBucketAnimationController();
			FishBucketAnimationController fishBucketAnimationController = animationController;
			fishBucketAnimationController.CannonRotationCompleteAction = (System.Action)Delegate.Combine(fishBucketAnimationController.CannonRotationCompleteAction, new System.Action(onCannonRotationComplete));
		}

		private void onCannonRotationComplete()
		{
			if (currentTurnPlayerSessionId == localPlayerSessionId)
			{
				changeControlState(ControlsState.Action);
			}
			else
			{
				changeControlState(ControlsState.Disabled);
			}
		}

		private void handlePlayerLeftGame(PartyGameSessionMessages.PlayerLeftGame data)
		{
			playersAbandonded.Add(data.PlayerId);
			animationController.RemovePlayer(data.PlayerId);
			hudUI.OnPlayerRemoved(data.PlayerId);
		}

		private void handlePlayerTurnStart(PartyGameSessionMessages.PlayerTurnStart data)
		{
			if (currentState == FishBucketState.Game)
			{
				bool isLocalPlayer = data.PlayerId == localPlayerSessionId;
				currentTurnPlayerSessionId = data.PlayerId;
				animationController.StartPlayerTurn(currentTurnPlayerSessionId);
				hudUI.SetCurrentPlayersTurn(playerData[data.PlayerId], isLocalPlayer);
			}
		}

		private void handleShowTurnOutput(PartyGameSessionMessages.ShowTurnOutput data)
		{
			changeControlState(ControlsState.Disabled);
			if (currentState != FishBucketState.Game)
			{
				return;
			}
			if (!playersAbandonded.Contains(data.PlayerId))
			{
				FishBucketPlayerData fishBucketPlayerData = playerData[data.PlayerId];
				fishBucketPlayerData.Score = Math.Max(fishBucketPlayerData.Score + data.ScoreDelta, 0);
				hudUI.ShowTurn(fishBucketPlayerData, data.ScoreDelta);
				animationController.ShowTurnOutput(data.PlayerId, data.ScoreDelta);
				if (fishBucketPlayerData.BucketReference != null)
				{
					fishBucketPlayerData.BucketReference.SetFillAmount(fishBucketPlayerData.Score);
				}
			}
			CoroutineRunner.Start(waitForTurnSequenceComplete(data.ScoreDelta), this, "WaitForTurnSequenceComplete");
		}

		private IEnumerator waitForTurnSequenceComplete(int fishDelta)
		{
			float waitTime = (fishDelta > 0) ? fishBucketDefinition.NextTurnDelayFishInSeconds : fishBucketDefinition.NextTurnDelaySquidInSeconds;
			yield return new WaitForSeconds(waitTime);
			onShowTurnOutputComplete(fishDelta);
		}

		private void onShowTurnOutputComplete(int fishDelta)
		{
			if (fishDelta > 0)
			{
				hudUI.OnTurnSequenceComplete(fishDelta);
				if (currentTurnPlayerSessionId == localPlayerSessionId)
				{
					changeControlState(ControlsState.ActionAndPass);
				}
			}
		}

		private void handleSetGameStartPositions(PartyGameSessionMessages.SetGameStartData data)
		{
			if (currentState == FishBucketState.AwaitingPositions)
			{
				CoroutineRunner.Start(createCannon(data.GamePositions[0]), this, "CreateFishBucketCannon");
				if (localPlayerSessionId == base.players[0].UserSessionId)
				{
					CoroutineRunner.Start(MoveLocalPlayerToPosition(data.OwnerPosition, data.GamePositions[0]), this, "");
				}
				else
				{
					int num = base.players.FindIndex((PartyGamePlayer p) => p.UserSessionId == localPlayerSessionId) - 1;
					CoroutineRunner.Start(MoveLocalPlayerToPosition(data.PlayerPositions[num], data.GamePositions[0]), this, "");
				}
				totalCards = data.IntData[0];
				if (hudUI != null)
				{
					hudUI.SetTotalCards(totalCards);
				}
			}
		}

		private void changeState(FishBucketState newState)
		{
			currentState = newState;
			switch (newState)
			{
			case FishBucketState.AwaitingPositions:
				startAwaitingPositions();
				break;
			case FishBucketState.Intro:
				startIntro();
				break;
			case FishBucketState.Game:
				hudUI.SetState(FishBucketHud.FishBucketHudState.InGame);
				break;
			}
		}

		private void startIntro()
		{
		}

		private void startAwaitingPositions()
		{
			if (base.players[0].UserSessionId == localPlayerSessionId)
			{
				CoroutineRunner.Start(setUpStartPositions(), this, "SetUpStartPositions");
			}
		}

		private IEnumerator setUpStartPositions()
		{
			Vector3[] playerPositions = new Vector3[base.players.Count - 1];
			Vector3[] gamePositions = new Vector3[1];
			Vector3 ownerPosition = localPlayerGameObject.transform.position;
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(PLAYER_POSITION_PREFAB_KEY);
			yield return assetRequest;
			Vector3 cameraDirection = Camera.main.transform.position - localPlayerGameObject.transform.position;
			cameraDirection.y = 0f;
			PartyGameSpawnLocationValidator spawnLocations = UnityEngine.Object.Instantiate(assetRequest.Asset, ownerPosition, Quaternion.LookRotation(cameraDirection)).GetComponent<PartyGameSpawnLocationValidator>();
			yield return null;
			PartyGameSessionMessages.SetGameStartData startPositions = new PartyGameSessionMessages.SetGameStartData
			{
				OwnerPosition = spawnLocations.GetPlayerPosition(0)
			};
			for (int i = 1; i < base.players.Count && i < spawnLocations.PlayerPositions.Length; i++)
			{
				playerPositions[i - 1] = spawnLocations.GetPlayerPosition(i);
			}
			gamePositions[0] = localPlayerGameObject.transform.position;
			startPositions.PlayerPositions = playerPositions;
			startPositions.GamePositions = gamePositions;
			sendSessionMessage(PartyGameSessionMessageTypes.SetGameStartPositions, startPositions);
			UnityEngine.Object.Destroy(spawnLocations.gameObject);
		}

		private IEnumerator MoveLocalPlayerToPosition(Vector3 positionFinal, Vector3 cannonPosition)
		{
			yield return new WaitForSeconds(1f);
			DirectionalPlayerMover mover = new DirectionalPlayerMover();
			mover.StartPlayerMovement(positionFinal, cannonPosition - positionFinal);
		}

		private IEnumerator createCannon(Vector3 position)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(CANNON_PREFAB_KEY);
			yield return assetRequest;
			Vector3 cameraDirection = Camera.main.transform.position - localPlayerGameObject.transform.position;
			cameraDirection.y = 0f;
			cannonGameObject = UnityEngine.Object.Instantiate(assetRequest.Asset, position, Quaternion.LookRotation(cameraDirection));
			animationController.SetCannon(cannonGameObject);
			ChangeCamera();
		}

		private void ChangeCamera()
		{
			FishBucketCannon componentInChildren = cannonGameObject.GetComponentInChildren<FishBucketCannon>();
			GameObject gameObject = (PlatformUtils.GetPlatformType() == PlatformType.Mobile) ? componentInChildren.CameraPosition : componentInChildren.StandaloneCameraPosition;
			CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
			evt.Controller = gameObject.GetComponent<CameraController>();
			dispatcher.DispatchEvent(evt);
			GameObject gameObject2 = (PlatformUtils.GetPlatformType() == PlatformType.Mobile) ? componentInChildren.CameraTarget : componentInChildren.StandaloneCameraTarget;
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(gameObject2.transform));
		}

		private void ResetCamera()
		{
			FishBucketCannon componentInChildren = cannonGameObject.GetComponentInChildren<FishBucketCannon>();
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			GameObject gameObject = (PlatformUtils.GetPlatformType() == PlatformType.Mobile) ? componentInChildren.CameraPosition : componentInChildren.StandaloneCameraPosition;
			evt.Controller = gameObject.GetComponent<CameraController>();
			dispatcher.DispatchEvent(evt);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(localPlayerGameObject.transform));
		}

		private void onDoActionEvent(LocomotionController.LocomotionAction actionType, object userData = null)
		{
			if (currentTurnPlayerSessionId == localPlayerSessionId)
			{
				switch (actionType)
				{
				case LocomotionController.LocomotionAction.Action1:
				case LocomotionController.LocomotionAction.Action2:
					changeControlState(ControlsState.Disabled);
					break;
				}
			}
		}

		private void enableFishButton()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
		}

		private void enablePassButton()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
		}

		private void disableActionButtons()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
		}

		private void addListeners()
		{
			localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>().OnDoActionEvent += onDoActionEvent;
			dispatcher.AddListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			dispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
		}

		private bool onControlLayoutLoadComplete(ControlsScreenEvents.ControlLayoutLoadComplete evt)
		{
			CoroutineRunner.Start(showCurrentButtonStateDelayed(), this, "");
			return false;
		}

		private IEnumerator showCurrentButtonStateDelayed()
		{
			yield return null;
			showControlsForState(currentControlsState);
		}

		private void removeListeners()
		{
			if (localPlayerGameObject != null)
			{
				localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>().OnDoActionEvent -= onDoActionEvent;
			}
			dispatcher.RemoveListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			dispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
		}

		private void getDefinitions()
		{
			partyGameDefinition = getPartyGameDefinition(1);
			fishBucketDefinition = (FishBucketDefinition)partyGameDefinition.GameData;
		}

		private void changeControlState(ControlsState newState)
		{
			currentControlsState = newState;
			showControlsForState(currentControlsState);
		}

		private void showControlsForState(ControlsState state)
		{
			switch (state)
			{
			case ControlsState.Disabled:
				disableActionButtons();
				break;
			case ControlsState.Action:
				enableFishButton();
				break;
			case ControlsState.ActionAndPass:
				enableFishButton();
				enablePassButton();
				break;
			}
		}

		private bool onSceneTransition(SceneTransitionEvents.TransitionStart evt)
		{
			dispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
			PartyGameUtils.EnableMainNavigation();
			PartyGameUtils.EnableLocomotionControls();
			PartyGameUtils.EnableCellPhoneButton();
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			dispatcher.RemoveListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			return false;
		}
	}
}
