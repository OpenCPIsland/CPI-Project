using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Participation;
using ClubPenguin.PartyGames;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattle : AbstractPartyGameSession
	{
		private enum ControlsState
		{
			Disabled,
			Enabled
		}

		private const string RED_TEAM_TOKEN = "Activity.DanceBattle.Team2";

		private const string BLUE_TEAM_TOKEN = "Activity.DanceBattle.Team1";

		private const int DEFINITION_ID = 3;

		private const float CADENCE_CAMERA_WAIT_TIME = 7f;

		private const int PENGUIN_WOOHOO_ANIMATION_TRIGGER = 16;

		private ControlsState currentControlsState;

		private EventDispatcher dispatcher;

		private CPDataEntityCollection dataEntityCollection;

		private CameraController currentCameraController;

		private long localPlayerSessionId;

		private readonly ParticipationController localPlayerParticipationController;

		private DanceBattleTurnData currentTurnData;

		private List<int> currentTurnInput;

		private PartyGamePlayer localPlayer;

		private EventChannel eventChannel;

		private Dictionary<long, int> playerSessionIdToPlacement;

		private PartyGameDefinition partyGameDefinition;

		private DanceBattleScoreData scoreData;

		private Queue<int> danceMoveQueue;

		private DanceBattleVisualsController danceBattleVisualsController;

		private readonly PrefabContentKey endGamePopupContentKey = new PrefabContentKey("Prefabs/DanceBattle/EndGamePopup/DanceBattleEndGamePopup");

		private StateMachineContext trayFSMContext;

		public DanceBattle()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(dispatcher);
			localPlayerSessionId = dataEntityCollection.LocalPlayerSessionId;
			partyGameDefinition = getPartyGameDefinition(3);
			localPlayerParticipationController = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<ParticipationController>();
			PartyGameUtils.DisableMainNavigation();
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				trayFSMContext = gameObject.GetComponent<StateMachineContext>();
			}
		}

		protected override void handleSessionMessage(PartyGameSessionMessageTypes type, string data)
		{
		}

		protected override void startGame()
		{
			addEventListeners();
			localPlayer = getPartyGamePlayerById(Service.Get<CPDataEntityCollection>().LocalPlayerSessionId);
			PartyGameUtils.AddParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.AddActionConfirmationFilter(partyGameDefinition, "DanceBattleExitPrompt");
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ChatButtons", true));
			PartyGameUtils.DisableCellPhoneButton(true);
			changeControlState(ControlsState.Disabled);
		}

		protected override void endGame(Dictionary<long, int> playerSessionIdToPlacement)
		{
			if (playerSessionIdToPlacement.ContainsKey(localPlayerSessionId))
			{
				this.playerSessionIdToPlacement = playerSessionIdToPlacement;
				Content.LoadAsync(onEndGamePopupLoadComplete, endGamePopupContentKey);
				if (scoreData == null)
				{
					scoreData = new DanceBattleScoreData();
					scoreData.Team1Score = 0f;
					scoreData.Team2Score = 0f;
				}
				DanceBattleUtils.LogGameEndBI((!(scoreData.Team1Score > scoreData.Team2Score)) ? 1 : 0);
			}
		}

		private void onEndGamePopupLoadComplete(string path, GameObject endGamePopupPrefab)
		{
			GameObject gameObject = Object.Instantiate(endGamePopupPrefab);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(gameObject));
			gameObject.GetComponent<PartyGameTeamEndGamePopup>().SetData(getEndGameData());
			EventManager.Instance.PostEvent("SFX/UI/DanceGame/MenuOpen", EventAction.PlaySound, null, null);
		}

		private PartyGameTeamEndGamePopupData getEndGameData()
		{
			List<PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData> list = new List<PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData>();
			PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData item = new PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData(scoreData.Team1Score, "Activity.DanceBattle.Team1", scoreData.Team1Score > scoreData.Team2Score, localPlayer.TeamId == 0, 0, false);
			PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData item2 = new PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData(scoreData.Team2Score, "Activity.DanceBattle.Team2", scoreData.Team1Score < scoreData.Team2Score, localPlayer.TeamId == 1, 1, false);
			if (scoreData.Team1Score > scoreData.Team2Score)
			{
				list.Add(item);
				list.Add(item2);
			}
			else
			{
				list.Add(item2);
				list.Add(item);
			}
			return new PartyGameTeamEndGamePopupData(list, localPlayer.TeamId, (PartyGameEndPlacement)playerSessionIdToPlacement[localPlayerSessionId], getRewardForEndGamePlacement((PartyGameEndPlacement)playerSessionIdToPlacement[localPlayerSessionId], partyGameDefinition.Rewards), base.sessionId);
		}

		protected override void destroy()
		{
			PartyGameUtils.EnableMainNavigation();
			PartyGameUtils.RemoveParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.RemoveActionConfirmationFilter();
			PartyGameUtils.EnableCellPhoneButton();
			removeEventListeners();
			changeToDefaultCamera();
			changeControlState(ControlsState.Enabled);
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ChatButtons"));
			if (danceBattleVisualsController != null)
			{
				danceBattleVisualsController.RemoveTopHud();
			}
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			switch (evt.Action)
			{
			case InputEvents.Actions.Action1:
				sendTurnOutput(1);
				break;
			case InputEvents.Actions.Action2:
				sendTurnOutput(2);
				break;
			case InputEvents.Actions.Action3:
				sendTurnOutput(3);
				break;
			}
			return false;
		}

		private void sendTurnOutput(int action)
		{
			if (currentTurnInput.Count < currentTurnData.Moves.Count)
			{
				dispatcher.DispatchEvent(new DanceBattleEvents.TurnInputSent(action, currentTurnInput.Count, localPlayerSessionId));
				sendSessionMessage(PartyGameSessionMessageTypes.ShowTurnOutput, new PartyGameSessionMessages.ShowTurnOutput(localPlayerSessionId, action));
				currentTurnInput.Add(action);
				if (currentTurnInput.Count >= currentTurnData.Moves.Count)
				{
					changeControlState(ControlsState.Disabled);
					EventManager.Instance.PostEvent("SFX/UI/DanceGame/Note2", EventAction.PlaySound, null, null);
				}
				else
				{
					EventManager.Instance.PostEvent("SFX/UI/DanceGame/Note1", EventAction.PlaySound, null, null);
				}
			}
		}

		private SizzleClipDefinition getSizzleClipDefinitionOfId(int sizzleClipId)
		{
			Dictionary<int, SizzleClipDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, SizzleClipDefinition>>();
			SizzleClipDefinition value = dictionary.GetEnumerator().Current.Value;
			if (!dictionary.TryGetValue(sizzleClipId, out value))
			{
			}
			return value;
		}

		private bool onSequenceDisplayStartEvent(DanceBattleEvents.SequenceDisplayStart evt)
		{
			changeCameraController(evt.DanceBattleVisualsController.ScreenCamera, evt.DanceBattleVisualsController.ScreenCameraTarget, true);
			return false;
		}

		private bool onSequenceDisplayEndEvent(DanceBattleEvents.SequenceDisplayEnd evt)
		{
			changeControlState(ControlsState.Enabled);
			changeCameraController(evt.DanceBattleVisualsController.DanceCamera, evt.DanceBattleVisualsController.DanceCameraTarget, false);
			return false;
		}

		private bool onDanceBattleStartEvent(DanceBattleEvents.DanceBattleStart evt)
		{
			danceBattleVisualsController = evt.DanceBattleVisualsController;
			danceBattleVisualsController.setPlayerIsInGame(true);
			changeCameraController(evt.DanceBattleVisualsController.ScreenCamera, evt.DanceBattleVisualsController.ScreenCameraTarget, true);
			CoroutineRunner.Start(changeCameraControllerWithDelay(7f, evt.DanceBattleVisualsController.DanceCamera, evt.DanceBattleVisualsController.DanceCameraTarget, false), this, "ChangeDanceBattleCamera");
			return false;
		}

		private bool onResultsDisplayStartEvent(DanceBattleEvents.ResultsDisplayStart evt)
		{
			changeCameraController(evt.DanceBattleVisualsController.ScreenCamera, evt.DanceBattleVisualsController.ScreenCameraTarget, true);
			return false;
		}

		private bool onResultsDisplayEndEvent(DanceBattleEvents.ResultsDisplayEnd evt)
		{
			changeCameraController(evt.DanceBattleVisualsController.DanceCamera, evt.DanceBattleVisualsController.DanceCameraTarget, false);
			return false;
		}

		private bool onTurnTimerComplete(DanceBattleEvents.TurnTimerComplete evt)
		{
			changeControlState(ControlsState.Disabled);
			return false;
		}

		private bool onLocalPlayerDanceSequenceComplete(DanceBattleEvents.LocalPlayerDanceSequenceComplete evt)
		{
			sendSessionMessage(PartyGameSessionMessageTypes.DanceMoveSequenceComplete, new PartyGameSessionMessages.DanceMoveSequenceComplete());
			return false;
		}

		private bool onTurnStartEvent(DanceBattleEvents.TurnStart evt)
		{
			currentTurnData = evt.TurnData;
			currentTurnInput = new List<int>();
			changeControlState(ControlsState.Disabled);
			return false;
		}

		private bool onTurnEndEvent(DanceBattleEvents.TurnEnd evt)
		{
			changeControlState(ControlsState.Disabled);
			scoreData = evt.ScoreData;
			return false;
		}

		private void changeCameraController(CameraController controller, Transform cameraTarget, bool hideControls)
		{
			if (currentCameraController != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = currentCameraController;
				dispatcher.DispatchEvent(evt);
			}
			CinematographyEvents.CameraLogicChangeEvent evt2 = default(CinematographyEvents.CameraLogicChangeEvent);
			evt2.Controller = controller;
			currentCameraController = controller;
			dispatcher.DispatchEvent(evt2);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(cameraTarget));
			if (hideControls)
			{
				trayFSMContext.SendEvent(new ExternalEvent("Root", "minnpc"));
			}
			else
			{
				trayFSMContext.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
			}
		}

		private IEnumerator changeCameraControllerWithDelay(float delay, CameraController controller, Transform cameraTarget, bool hideControls)
		{
			yield return new WaitForSeconds(delay);
			changeCameraController(controller, cameraTarget, hideControls);
		}

		private void changeToDefaultCamera()
		{
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			evt.Controller = currentCameraController;
			dispatcher.DispatchEvent(evt);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform));
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
				disableDanceMoveControls();
				break;
			case ControlsState.Enabled:
				enableDanceMoveControls();
				break;
			}
		}

		private void disableDanceMoveControls()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton1"));
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			dispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
		}

		private void enableDanceMoveControls()
		{
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton1"));
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
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

		private bool onSceneTransition(SceneTransitionEvents.TransitionStart evt)
		{
			dispatcher.RemoveListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
			PartyGameUtils.EnableMainNavigation();
			PartyGameUtils.EnableLocomotionControls();
			PartyGameUtils.EnableCellPhoneButton();
			enableDanceMoveControls();
			dispatcher.RemoveListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			return false;
		}

		private bool onFinalResultsShown(DanceBattleEvents.FinalResultsShown evt)
		{
			if (!evt.IsTie)
			{
				List<long> list = new List<long>();
				for (int i = 0; i < base.players.Count; i++)
				{
					if (base.players[i].TeamId == evt.WinningTeamId)
					{
						list.Add(base.players[i].UserSessionId);
					}
				}
				SceneRefs.CelebrationRunner.PlayCelebrationAnimation(list);
			}
			return false;
		}

		private void addEventListeners()
		{
			eventChannel.AddListener<DanceBattleEvents.SequenceDisplayStart>(onSequenceDisplayStartEvent);
			eventChannel.AddListener<DanceBattleEvents.SequenceDisplayEnd>(onSequenceDisplayEndEvent);
			eventChannel.AddListener<DanceBattleEvents.DanceBattleStart>(onDanceBattleStartEvent);
			eventChannel.AddListener<DanceBattleEvents.TurnStart>(onTurnStartEvent);
			eventChannel.AddListener<DanceBattleEvents.TurnEnd>(onTurnEndEvent);
			eventChannel.AddListener<DanceBattleEvents.ResultsDisplayStart>(onResultsDisplayStartEvent);
			eventChannel.AddListener<DanceBattleEvents.ResultsDisplayEnd>(onResultsDisplayEndEvent);
			eventChannel.AddListener<DanceBattleEvents.TurnTimerComplete>(onTurnTimerComplete);
			eventChannel.AddListener<InputEvents.ActionEvent>(onActionEvent);
			eventChannel.AddListener<ControlsScreenEvents.ControlLayoutLoadComplete>(onControlLayoutLoadComplete);
			eventChannel.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
			eventChannel.AddListener<DanceBattleEvents.FinalResultsShown>(onFinalResultsShown);
			eventChannel.AddListener<DanceBattleEvents.LocalPlayerDanceSequenceComplete>(onLocalPlayerDanceSequenceComplete);
		}

		private void removeEventListeners()
		{
			eventChannel.RemoveAllListeners();
		}

		private PartyGamePlayer getPartyGamePlayerById(long playerSessionId)
		{
			PartyGamePlayer result = null;
			for (int i = 0; i < base.players.Count; i++)
			{
				if (base.players[i].UserSessionId == playerSessionId)
				{
					result = base.players[i];
					break;
				}
			}
			return result;
		}

		private Reward getRewardForEndGamePlacement(PartyGameEndPlacement endGamePlacement, List<PartyGameDefinition.PartyGameReward> rewards)
		{
			Reward result = new Reward();
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].Placement == endGamePlacement)
				{
					result = rewards[i].Reward.ToReward();
					break;
				}
			}
			return result;
		}
	}
}
