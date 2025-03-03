using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Participation;
using ClubPenguin.PartyGames;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FindFour : AbstractPartyGameSession
	{
		private enum FindFourState
		{
			Intro,
			Game,
			Finished
		}

		private enum ControlsState
		{
			Disabled,
			Enabled
		}

		public enum FindFourTokenColor
		{
			RED,
			BLUE
		}

		public enum FindFourMoveDirection
		{
			RIGHT,
			LEFT
		}

		public const float CLIENT_TURN_TIME_MODIFIER = -1f;

		private const int DEFINITION_ID = 2;

		private const string ACTION_BUTTON = "ActionButton";

		private const string LEFT_ARROW = "LeftArrowButton";

		private const string RIGHT_ARROW = "RightArrowButton";

		private GameObject boardGameObject;

		private FindFourBoard board;

		private readonly ParticipationController localPlayerParticipationController;

		private PartyGameDefinition partyGameDefinition;

		private FindFourDefinition findFourDefinition;

		private CPDataEntityCollection dataEntityCollection;

		private EventDispatcher dispatcher;

		private JsonService jsonService;

		private FindFourState currentState;

		private ControlsState currentControlsState;

		private long localPlayerSessionId;

		private GameObject localPlayerGameObject;

		private FindFourHud hudUI;

		private ControlsScreenData controlsData;

		private readonly UIElementDisablerManager disablerManager;

		private readonly PrefabContentKey BOARD_PREFAB_KEY = new PrefabContentKey("Prefabs/FindFour/FindFourBoard");

		private readonly PrefabContentKey ARROW_BUTTONS_PREFAB_KEY = new PrefabContentKey("Prefabs/FindFour/FindFourArrowButtonsPanel");

		private readonly PrefabContentKey HUD_UI_PREFAB_KEY = new PrefabContentKey("Prefabs/FindFour/FindFourHud");

		private readonly PrefabContentKey END_POPUP_PREFAB_KEY = new PrefabContentKey("Prefabs/FindFour/FindFourEndGame");

		public FindFour()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			disablerManager = Service.Get<UIElementDisablerManager>();
			dispatcher = Service.Get<EventDispatcher>();
			jsonService = Service.Get<JsonService>();
			localPlayerSessionId = dataEntityCollection.LocalPlayerSessionId;
			localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			localPlayerParticipationController = localPlayerGameObject.GetComponent<ParticipationController>();
			DataEntityHandle handle = dataEntityCollection.FindEntityByName("ControlsScreenData");
			controlsData = dataEntityCollection.GetComponent<ControlsScreenData>(handle);
			addListeners();
			getDefinitions();
		}

		protected override void handleSessionMessage(PartyGameSessionMessageTypes type, string data)
		{
			switch (type)
			{
			case PartyGameSessionMessageTypes.SetGameState:
			{
				PartyGameSessionMessages.SetGameState setGameState = jsonService.Deserialize<PartyGameSessionMessages.SetGameState>(data);
				FindFourState gameStateId = (FindFourState)setGameState.GameStateId;
				changeState(gameStateId);
				break;
			}
			case PartyGameSessionMessageTypes.ShowTurnOutput:
				handleShowTurnOutput(jsonService.Deserialize<PartyGameSessionMessages.ShowTurnOutput>(data));
				break;
			case PartyGameSessionMessageTypes.PlayerTurnStart:
				handlePlayerTurnStart(jsonService.Deserialize<PartyGameSessionMessages.PlayerTurnStart>(data));
				break;
			}
		}

		protected override void startGame()
		{
			hideRemotePlayers();
			loadAudioPrefab(partyGameDefinition);
			changeState(FindFourState.Intro);
			PartyGameUtils.AddParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.AddActionConfirmationFilter(partyGameDefinition);
			PartyGameUtils.DisableMainNavigation();
			PartyGameUtils.DisableLocomotionControls();
			Vector3 position = default(Vector3);
			DataEntityHandle handle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(base.players[0].UserSessionId);
			GameObjectReferenceData component;
			if (dataEntityCollection.TryGetComponent(handle, out component))
			{
				position = component.GameObject.transform.position;
			}
			CoroutineRunner.Start(createBoard(position), this, "CreateFindFourBoard");
			Content.LoadAsync(onHudPrefabLoaded, HUD_UI_PREFAB_KEY);
			dispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
			changeControlState(ControlsState.Disabled);
			if (base.players[0].UserSessionId == localPlayerSessionId)
			{
				PartyGameUtils.LogGameStartBi(partyGameDefinition.name, base.players.Count);
				PartyGameUtils.StartBiTimer(partyGameDefinition.name, base.sessionId);
			}
			if (!disablerManager.IsUIElementDisabled("CellphoneButton"))
			{
				PartyGameUtils.DisableCellPhoneButton();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameEvents.PartyGameStarted(partyGameDefinition));
			dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
		}

		private void onHudPrefabLoaded(string path, GameObject prefab)
		{
			if (currentState != FindFourState.Finished)
			{
				long[] array = new long[base.players.Count];
				for (int i = 0; i < base.players.Count; i++)
				{
					array[i] = base.players[i].UserSessionId;
				}
				GameObject gameObject = GameObject.Find("HudLeftPanel");
				GameObject gameObject2 = Object.Instantiate(prefab, gameObject.transform, false);
				hudUI = gameObject2.GetComponent<FindFourHud>();
				hudUI.Init(array, findFourDefinition);
				hudUI.SetState(FindFourHud.FindFourHudState.Instructions);
			}
		}

		protected override void endGame(Dictionary<long, int> playerSessionIdToPlacement)
		{
			if (playerSessionIdToPlacement.Count == 0)
			{
				PartyGameUtils.LogBalkBi(partyGameDefinition.name);
				return;
			}
			List<PartyGameEndGamePlayerData> list = new List<PartyGameEndGamePlayerData>();
			foreach (KeyValuePair<long, int> item in playerSessionIdToPlacement)
			{
				list.Add(new PartyGameEndGamePlayerData(item.Key, item.Value, (item.Key != base.players[0].UserSessionId) ? 1 : 0, item.Key == localPlayerSessionId));
			}
			list.Sort((PartyGameEndGamePlayerData a, PartyGameEndGamePlayerData b) => a.Placement.CompareTo(b.Placement));
			loadEndGameScreen(list.ToArray());
			if (base.players[0].UserSessionId == localPlayerSessionId)
			{
				bool won = playerSessionIdToPlacement[localPlayerSessionId] == 0;
				PartyGameUtils.StopBiTimer(base.sessionId);
				PartyGameUtils.LogGameEndBi(partyGameDefinition.name, playerSessionIdToPlacement.Count, won);
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
			PartyGameEndGamePopup component = Object.Instantiate(endGamePopup).GetComponent<PartyGameEndGamePopup>();
			component.SetPlayerResults(orderedPlayerData, partyGameDefinition, base.sessionId);
			dispatcher.DispatchEvent(new PopupEvents.ShowCameraSpacePopup(component.gameObject));
		}

		protected override void destroy()
		{
			dispatcher.RemoveListener<InputEvents.ActionEvent>(onActionEvent);
			ResetCamera();
			showRemotePlayers();
			removeListeners();
			PartyGameUtils.RemoveParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.RemoveActionConfirmationFilter();
			PartyGameUtils.EnableMainNavigation();
			PartyGameUtils.EnableLocomotionControls();
			dispatcher.DispatchEvent(default(ControlsScreenEvents.ReturnToDefaultLeftOption));
			Object.Destroy(boardGameObject);
			Object.Destroy(hudUI.gameObject);
			PartyGameUtils.EnableCellPhoneButton();
			dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			CoroutineRunner.StopAllForOwner(this);
		}

		private void changeState(FindFourState newState)
		{
			currentState = newState;
			switch (newState)
			{
			case FindFourState.Intro:
				break;
			case FindFourState.Game:
				hudUI.SetState(FindFourHud.FindFourHudState.InGame);
				break;
			}
		}

		private IEnumerator createBoard(Vector3 position)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(BOARD_PREFAB_KEY);
			yield return assetRequest;
			Vector3 cameraDirection = Camera.main.transform.position - position;
			cameraDirection.y = 0f;
			boardGameObject = Object.Instantiate(assetRequest.Asset, position, Quaternion.LookRotation(cameraDirection));
			board = boardGameObject.GetComponent<FindFourBoard>();
			board.Init(findFourDefinition);
			board.OnColumnClicked += onColumnClicked;
			ChangeCamera();
		}

		private void ChangeCamera()
		{
			CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
			GameObject gameObject = (PlatformUtils.GetPlatformType() == PlatformType.Mobile) ? board.CameraPosition : board.StandaloneCameraPosition;
			evt.Controller = gameObject.GetComponent<CameraController>();
			dispatcher.DispatchEvent(evt);
			GameObject gameObject2 = (PlatformUtils.GetPlatformType() == PlatformType.Mobile) ? board.CameraTarget : board.StandaloneCameraTarget;
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(gameObject2.transform));
			Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("LocalPlayer"));
			Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("PhysicsObjects"));
			Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("AllPlayerInteractibles"));
		}

		private void onColumnClicked(int columnIndex)
		{
			sendTurnOutput(columnIndex);
		}

		private void ResetCamera()
		{
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			GameObject gameObject = (PlatformUtils.GetPlatformType() == PlatformType.Mobile) ? board.CameraPosition : board.StandaloneCameraPosition;
			evt.Controller = gameObject.GetComponent<CameraController>();
			dispatcher.DispatchEvent(evt);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(localPlayerGameObject.transform));
			Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("AllPlayerInteractibles");
			Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("PhysicsObjects");
			Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("LocalPlayer");
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			switch (evt.Action)
			{
			case InputEvents.Actions.Action1:
				sendTurnOutput(board.CurrentTokenColumn);
				break;
			case InputEvents.Actions.Action2:
				board.MoveToken(FindFourMoveDirection.LEFT);
				break;
			case InputEvents.Actions.Action3:
				board.MoveToken(FindFourMoveDirection.RIGHT);
				break;
			}
			return false;
		}

		private void sendTurnOutput(int column)
		{
			sendSessionMessage(PartyGameSessionMessageTypes.ShowTurnOutput, new PartyGameSessionMessages.ShowTurnOutput(localPlayerSessionId, column));
			changeControlState(ControlsState.Disabled);
			CoroutineRunner.StopAllForOwner(this);
		}

		private void handleShowTurnOutput(PartyGameSessionMessages.ShowTurnOutput data)
		{
			if (data.PlayerId != localPlayerSessionId)
			{
				board.CreateNewToken(getTokenColor(data.PlayerId));
			}
			board.PlaceToken(data.ScoreDelta);
			changeControlState(ControlsState.Disabled);
			hudUI.EndTurn();
			CoroutineRunner.StopAllForOwner(this);
		}

		private void handlePlayerTurnStart(PartyGameSessionMessages.PlayerTurnStart data)
		{
			if (currentState == FindFourState.Game)
			{
				if (data.PlayerId == localPlayerSessionId)
				{
					changeControlState(ControlsState.Enabled);
					board.CreateNewToken(getTokenColor(data.PlayerId), true);
					CoroutineRunner.Start(dropClientTokenOnTurnTimeout(), this, "dropClientTokenOnTurnTimeout");
				}
				hudUI.SetCurrentPlayersTurn(data.PlayerId);
			}
		}

		private IEnumerator dropClientTokenOnTurnTimeout()
		{
			yield return new WaitForSeconds((float)findFourDefinition.TurnTimeInSeconds + -1f);
			sendTurnOutput(board.CurrentTokenColumn);
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
				disableInputButtons();
				break;
			case ControlsState.Enabled:
				enableInputButtons();
				break;
			}
		}

		private void disableInputButtons()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("LeftArrowButton"));
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("RightArrowButton"));
			if (board != null)
			{
				board.IsMouseActive = false;
			}
		}

		private void enableInputButtons()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("LeftArrowButton"));
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("RightArrowButton"));
			if (board != null)
			{
				board.IsMouseActive = true;
			}
		}

		private void hideRemotePlayers()
		{
			RemotePlayerVisibilityState.HideRemotePlayers();
		}

		private void showRemotePlayers()
		{
			RemotePlayerVisibilityState.ShowRemotePlayers();
		}

		private FindFourTokenColor getTokenColor(long playerId)
		{
			if (playerId == base.players[0].UserSessionId)
			{
				return FindFourTokenColor.RED;
			}
			return FindFourTokenColor.BLUE;
		}

		private void getDefinitions()
		{
			partyGameDefinition = getPartyGameDefinition(2);
			findFourDefinition = (FindFourDefinition)partyGameDefinition.GameData;
		}

		private bool onControlLayoutLoadComplete(ControlsScreenEvents.ControlLayoutLoadComplete evt)
		{
			showControlsForState(currentControlsState);
			return false;
		}

		private bool onSceneTransition(SceneTransitionEvents.TransitionStart evt)
		{
			removeListeners();
			return false;
		}

		private void addListeners()
		{
			dispatcher.AddListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			dispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
		}

		private void removeListeners()
		{
			dispatcher.RemoveListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			dispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
		}
	}
}
