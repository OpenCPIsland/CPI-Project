using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.PartyGames;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleVisualsController : MonoBehaviour
	{
		private enum DanceBattleVisualsState
		{
			Lobby,
			Intro,
			Game,
			TurnTimer,
			PerformingMoves,
			RoundResults,
			FinalResults,
			Off
		}

		public const int BLUE_TEAM_ID = 0;

		public const int RED_TEAM_ID = 1;

		public const float TOPHUD_TEAM_DELAY_IN_SECONDS = 3f;

		public const float TOPHUD_DESTROY_DELAY_IN_SECONDS = 3f;

		public const float TOPHUD_CHOOSE_DELAY_IN_SECONDS = 6f;

		private const int TIE_SELECTOR_INDEX = 2;

		private const int DEFINITION_ID = 3;

		private const string WARM_UP_TRIGGER = "WarmUp";

		private const string CADENCE_RISE_STAGE_TRIGGER = "CadenceRises";

		private const string STAGE_OFF_TRIGGER = "GameOver";

		private const string SPEAKER_BOUNCE_TRIGGER = "SpeakerBounce";

		private const string SPEAKERS_OFF_TRIGGER = "SpeakersOff";

		private const string STROBES_HIGH_TRIGGER = "StrobesHigh";

		private const string STROBES_LOW_TRIGGER = "StrobesLow";

		private const string CADENCE_LOWER_STAGE_TRIGGER = "CadenceLower";

		private const string RESULT_LOW_TRIGGER = "ResultLow";

		private const string RESULT_MED_TRIGGER = "ResultMed";

		private const string RESULT_PERFECT_RED_TRIGGER = "ResultPerfectRed";

		private const string RESULT_PERFECT_BLUE_TRIGGER = "ResultPerfectBlue";

		private const string WIN_RED_TRIGGER = "WinRed";

		private const string WIN_BLUE_TRIGGER = "WinBlue";

		private const string WIN_TIE_TRIGGER = "WinTie";

		private const string CADENCE_RISE_TRIGGER = "CadenceRise";

		private const string CADENCE_GOODBYE_TRIGGER = "CadenceGoodbye";

		private const string RED_TEAM_READY_TOKEN = "Activity.DanceBattle.ReadyTeam2";

		private const string BLUE_TEAM_READY_TOKEN = "Activity.DanceBattle.ReadyTeam1";

		private const string ROUND_TOKEN = "Activity.DanceBattle.Round";

		private const string TOPHUD_YOUR_TEAM_RED_TOKEN = "Activity.DanceBattle.PickTeam2";

		private const string TOPHUD_YOUR_TEAM_BLUE_TOKEN = "Activity.DanceBattle.PickTeam1";

		private const string TOPHUD_WAITING_TOKEN = "Activity.DanceBattle.Waiting";

		private const string TOPHUD_CHOOSING_TEAM_TOKEN = "Activity.DanceBattle.ChooseStart";

		private const string TOPHUD_RED_STARTS_TOKEN = "Activity.DanceBattle.TeamStart1";

		private const string TOPHUD_BLUE_STARTS_TOKEN = "Activity.DanceBattle.TeamStart2";

		private const string BLUE_TEAM_PLAYING_TOKEN = "Activity.DanceBattle.TeamPlaying1";

		private const string RED_TEAM_PLAYING_TOKEN = "Activity.DanceBattle.TeamPlaying2";

		private const string RED_TEAM_WON_TOKEN = "Activity.DanceBattle.TeamRedWin";

		private const string BLUE_TEAM_WON_TOKEN = "Activity.DanceBattle.TeamBlueWin";

		private const string TIE_WIN_TOKEN = "PartyGames.FindFour.Draw";

		private const string MOVE_ICON_INTRO_TRIGGER = "Intro";

		public DanceBattleLobbyMmoItemObserver LobbyObserver;

		public DanceBattleMmoItemObserver GameObserver;

		public DanceBattleAnimationEventHandler AnimEventHandler;

		public CameraController LobbyCamera;

		public DanceBattleFloorController FloorController;

		public Animator StageAnimator;

		public Animator CadenceAnimator;

		public GameObject[] Screens;

		public GameObject MoveIconContainer;

		public GameObject MoveIconInputContainer;

		public UITimer TurnTimer;

		public Text RoundText;

		public Text WinnerText;

		public TintSelector WinnerBG;

		public GameObjectSelector EndGameParticleSelector;

		public GameObject RemotePenguinScreen;

		public PrefabContentKey MoveIconKey;

		public PrefabContentKey LobbyPrefabKey;

		public PrefabContentKey TopHudPrefabKey;

		public CameraController DanceCamera;

		public CameraController ScreenCamera;

		public Transform ScreenCameraTarget;

		public Transform DanceCameraTarget;

		public CountdownTimer PreLobbyTimer;

		public CountdownTimer LobbyTimer;

		public DanceBattleScoreBar RedScoreBar;

		public DanceBattleScoreBar BlueScoreBar;

		public string MusicTargetObjectPath = "Audio.TriggersTown/MusicDanceGame";

		public float DanceMusicStartDelay = 1.5f;

		private GameObject musicTargetObject;

		private PartyGameTopHud topHud;

		private EventDispatcher dispatcher;

		private PartyGameDefinition partyGameDefinition;

		private DanceBattleDefinition danceBattleDefinition;

		private PartyGameLobbyMmoItemTeamDefinition lobbyDefinition;

		private PartyGameLauncherDefinition launcherDefinition;

		private DanceBattleDefinition.DanceSequenceSet currentSequenceSet;

		private CPDataEntityCollection dataEntityCollection;

		private long localPlayerSessionId;

		private bool localPlayerIsInGame;

		private Localizer localizer;

		private GameObject moveIconPrefab;

		private List<DanceBattleMoveIcon> moveIconList;

		private DanceBattleVisualsState currentState;

		private DanceBattleTurnData currentTurnData;

		private DanceBattleScoreData currentScoreData;

		private float lastScoreRed;

		private float lastScoreRedDelta;

		private float lastScoreBlue;

		private float lastScoreBlueDelta;

		private int numPlayersInLobby;

		private int redTeamLobbyCount;

		private int blueTeamLobbyCount;

		private DanceBattleLobby spawnedLobby;

		private int localPlayerTeamId;

		private bool isFirstTurn;

		private void Start()
		{
			PreLobbyTimer.Format = formatTimer;
			LobbyTimer.Format = formatTimer;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dispatcher = Service.Get<EventDispatcher>();
			localPlayerSessionId = dataEntityCollection.LocalPlayerSessionId;
			localizer = Service.Get<Localizer>();
			moveIconList = new List<DanceBattleMoveIcon>();
			getDefinitions();
			DanceBattleLobbyMmoItemObserver lobbyObserver = LobbyObserver;
			lobbyObserver.LobbyStartedAction = (Action<long>)Delegate.Combine(lobbyObserver.LobbyStartedAction, new Action<long>(onLobbyStarted));
			DanceBattleLobbyMmoItemObserver lobbyObserver2 = LobbyObserver;
			lobbyObserver2.LobbyEndedAction = (System.Action)Delegate.Combine(lobbyObserver2.LobbyEndedAction, new System.Action(onLobbyEnded));
			DanceBattleLobbyMmoItemObserver lobbyObserver3 = LobbyObserver;
			lobbyObserver3.LobbyPlayersUpdatedAction = (Action<PartyGamePlayerCollection>)Delegate.Combine(lobbyObserver3.LobbyPlayersUpdatedAction, new Action<PartyGamePlayerCollection>(onLobbyPlayersUpdated));
			DanceBattleMmoItemObserver gameObserver = GameObserver;
			gameObserver.GameStartedAction = (System.Action)Delegate.Combine(gameObserver.GameStartedAction, new System.Action(onGameStarted));
			DanceBattleMmoItemObserver gameObserver2 = GameObserver;
			gameObserver2.GameEndedAction = (System.Action)Delegate.Combine(gameObserver2.GameEndedAction, new System.Action(onGameEnded));
			DanceBattleMmoItemObserver gameObserver3 = GameObserver;
			gameObserver3.TurnDataUpdatedAction = (Action<DanceBattleTurnData>)Delegate.Combine(gameObserver3.TurnDataUpdatedAction, new Action<DanceBattleTurnData>(onTurnStarted));
			DanceBattleMmoItemObserver gameObserver4 = GameObserver;
			gameObserver4.ScoresUpdatedAction = (Action<DanceBattleScoreData>)Delegate.Combine(gameObserver4.ScoresUpdatedAction, new Action<DanceBattleScoreData>(onScoresUpdated));
			DanceBattleAnimationEventHandler animEventHandler = AnimEventHandler;
			animEventHandler.OnStartTurnSequence = (System.Action)Delegate.Combine(animEventHandler.OnStartTurnSequence, new System.Action(startTurnSequence));
			dispatcher.AddListener<DanceBattleEvents.TurnInputSent>(onTurnInputSent);
			setState(DanceBattleVisualsState.Off);
			if (!DisableWhenNoGameServer.IsGameServerAvailable())
			{
				Screens[7].SetActive(false);
			}
			musicTargetObject = GameObject.Find(MusicTargetObjectPath);
			Content.LoadAsync(onMoveIconLoaded, MoveIconKey);
		}

		public void setPlayerIsInGame(bool isPlayerInGame)
		{
			localPlayerIsInGame = isPlayerInGame;
		}

		public void RemoveTopHud()
		{
			if (topHud != null)
			{
				UnityEngine.Object.Destroy(topHud.gameObject);
				dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			}
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<DanceBattleEvents.TurnInputSent>(onTurnInputSent);
			DanceBattleLobbyMmoItemObserver lobbyObserver = LobbyObserver;
			lobbyObserver.LobbyStartedAction = (Action<long>)Delegate.Remove(lobbyObserver.LobbyStartedAction, new Action<long>(onLobbyStarted));
			DanceBattleLobbyMmoItemObserver lobbyObserver2 = LobbyObserver;
			lobbyObserver2.LobbyEndedAction = (System.Action)Delegate.Remove(lobbyObserver2.LobbyEndedAction, new System.Action(onLobbyEnded));
			DanceBattleLobbyMmoItemObserver lobbyObserver3 = LobbyObserver;
			lobbyObserver3.LobbyPlayersUpdatedAction = (Action<PartyGamePlayerCollection>)Delegate.Remove(lobbyObserver3.LobbyPlayersUpdatedAction, new Action<PartyGamePlayerCollection>(onLobbyPlayersUpdated));
			DanceBattleMmoItemObserver gameObserver = GameObserver;
			gameObserver.GameStartedAction = (System.Action)Delegate.Remove(gameObserver.GameStartedAction, new System.Action(onGameStarted));
			DanceBattleMmoItemObserver gameObserver2 = GameObserver;
			gameObserver2.GameEndedAction = (System.Action)Delegate.Remove(gameObserver2.GameEndedAction, new System.Action(onGameEnded));
			DanceBattleMmoItemObserver gameObserver3 = GameObserver;
			gameObserver3.TurnDataUpdatedAction = (Action<DanceBattleTurnData>)Delegate.Remove(gameObserver3.TurnDataUpdatedAction, new Action<DanceBattleTurnData>(onTurnStarted));
			DanceBattleMmoItemObserver gameObserver4 = GameObserver;
			gameObserver4.ScoresUpdatedAction = (Action<DanceBattleScoreData>)Delegate.Remove(gameObserver4.ScoresUpdatedAction, new Action<DanceBattleScoreData>(onScoresUpdated));
		}

		private void onMoveIconLoaded(string path, GameObject prefab)
		{
			moveIconPrefab = prefab;
		}

		private void onLobbyStarted(long gameStartTime)
		{
			setState(DanceBattleVisualsState.Lobby);
			long num = Service.Get<INetworkServicesManager>().GameTimeMilliseconds / 1000;
			long num2 = gameStartTime - num;
			LobbyTimer.StartTimer(new TimeSpan(0, 0, (int)num2));
			Content.LoadAsync(onLobbyPrefabLoaded, LobbyPrefabKey);
			turnOnEffectsForLobby();
			EventManager.Instance.PostEvent("MUS/Town/DJCadence", EventAction.SetSwitch, "CadenceIntro", musicTargetObject);
		}

		private void onLobbyPrefabLoaded(string key, GameObject lobbyPrefab)
		{
			if (LobbyObserver.MmoItemExists)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(lobbyPrefab, base.transform);
				gameObject.GetComponentInChildren<NetworkObjectController>().ItemId = LobbyObserver.MmoItemId;
				spawnedLobby = gameObject.GetComponent<DanceBattleLobby>();
				spawnedLobby.Init(partyGameDefinition.MaxPlayerCount / partyGameDefinition.NumTeams);
			}
		}

		private void onLobbyEnded()
		{
			if (spawnedLobby != null)
			{
				UnityEngine.Object.Destroy(spawnedLobby.gameObject);
			}
			if (currentState == DanceBattleVisualsState.Lobby)
			{
				turnOffEffects();
				if (localPlayerIsInGame && (numPlayersInLobby < partyGameDefinition.MinPlayerCount || redTeamLobbyCount == 0 || blueTeamLobbyCount == 0))
				{
					Service.Get<PromptManager>().ShowPrompt("PartyGameNotEnoughPlayersPrompt", null);
					DanceBattleUtils.LogNotEnoughPlayersBI();
					PartyGameUtils.EnableMainNavigation();
				}
				localPlayerIsInGame = false;
				EventManager.Instance.PostEvent("MUS/Town/DJCadence", EventAction.SetSwitch, "Evergreen", musicTargetObject);
			}
			if (topHud != null)
			{
				RemoveTopHud();
			}
			numPlayersInLobby = 0;
			LobbyCamera.gameObject.SetActive(false);
		}

		private string formatTimer(TimeSpan countdownTime)
		{
			return default(DateTime).Add(countdownTime).ToString("m:ss");
		}

		private void onLobbyPlayersUpdated(PartyGamePlayerCollection players)
		{
			bool flag = false;
			redTeamLobbyCount = 0;
			blueTeamLobbyCount = 0;
			for (int i = 0; i < players.Players.Count; i++)
			{
				PartyGamePlayer partyGamePlayer = players.Players[i];
				if (partyGamePlayer.UserSessionId == localPlayerSessionId)
				{
					flag = true;
					localPlayerTeamId = partyGamePlayer.TeamId;
					if (!localPlayerIsInGame)
					{
						moveLocalPlayerToPosition(spawnedLobby.GetPlayerPosition(partyGamePlayer.TeamId, partyGamePlayer.RoleId));
						CoroutineRunner.Start(createTopHud(), this, "createTopHud");
						DanceBattleUtils.LogPlayerJoinDanceBI(players.Players.Count);
					}
				}
				if (partyGamePlayer.TeamId == 1)
				{
					redTeamLobbyCount++;
				}
				else
				{
					blueTeamLobbyCount++;
				}
			}
			if (localPlayerIsInGame && !flag)
			{
				if (topHud != null)
				{
					RemoveTopHud();
				}
				PartyGameUtils.EnableMainNavigation();
			}
			else if (!localPlayerIsInGame && flag)
			{
				PartyGameUtils.DisableMainNavigation();
			}
			numPlayersInLobby = players.Players.Count;
			localPlayerIsInGame = flag;
			LobbyCamera.gameObject.SetActive(localPlayerIsInGame);
		}

		private void moveLocalPlayerToPosition(Transform position)
		{
			DirectionalPlayerMover directionalPlayerMover = new DirectionalPlayerMover();
			directionalPlayerMover.StartPlayerMovement(position.position, position.forward);
		}

		private void onGameStarted()
		{
			dispatcher.DispatchEvent(new DanceBattleEvents.DanceBattleStart(this));
			isFirstTurn = true;
			setState(DanceBattleVisualsState.Intro);
			turnOnEffectsForGame();
			if (localPlayerIsInGame)
			{
				DanceBattleUtils.LogDanceStartBI(numPlayersInLobby);
			}
			EventManager.Instance.PostEvent("MUS/Town/DJCadence/IntroAdvance", EventAction.PlaySound, null, musicTargetObject);
			if (topHud != null)
			{
				RemoveTopHud();
			}
		}

		private IEnumerator playCadenceRiseAnim()
		{
			yield return new WaitForEndOfFrame();
			CadenceAnimator.SetTrigger("CadenceRise");
			yield return new WaitForSeconds(6f);
		}

		private void onGameEnded()
		{
			turnOffEffects();
			if (topHud != null)
			{
				RemoveTopHud();
			}
			moveIconList.Clear();
			setChatVisible(true);
			localPlayerIsInGame = false;
			EventManager.Instance.PostEvent("MUS/Town/DJCadence", EventAction.SetSwitch, "Evergreen", musicTargetObject);
		}

		private void turnOnEffectsForLobby()
		{
			FloorController.SetFloorAnim(true, true);
			StageAnimator.SetTrigger("WarmUp");
		}

		private void turnOnEffectsForGame()
		{
			BlueScoreBar.StartBounceAnim();
			RedScoreBar.StartBounceAnim();
			FloorController.SetFloorAnim(true, true);
			StageAnimator.ResetTrigger("GameOver");
			CadenceAnimator.ResetTrigger("CadenceGoodbye");
			StageAnimator.ResetTrigger("WinBlue");
			StageAnimator.ResetTrigger("WinRed");
			StageAnimator.ResetTrigger("WinTie");
			StageAnimator.SetTrigger("CadenceRises");
			CoroutineRunner.Start(playCadenceRiseAnim(), this, "PlayCadenceRise");
		}

		private void turnOffEffects()
		{
			setState(DanceBattleVisualsState.Off);
			FloorController.SetFloorAnim(false, false);
			FloorController.SetFloorHighlight(false, false);
			RedScoreBar.TurnBarOff();
			BlueScoreBar.TurnBarOff();
			StageAnimator.ResetTrigger("CadenceRises");
			StageAnimator.ResetTrigger("WarmUp");
			CadenceAnimator.ResetTrigger("CadenceRise");
			CadenceAnimator.SetTrigger("CadenceGoodbye");
			StageAnimator.SetTrigger("GameOver");
			StageAnimator.SetTrigger("StrobesLow");
		}

		private void setState(DanceBattleVisualsState newState)
		{
			Screens[(int)currentState].SetActive(false);
			if (newState == DanceBattleVisualsState.Off || newState == DanceBattleVisualsState.Lobby || localPlayerIsInGame)
			{
				Screens[(int)newState].SetActive(true);
				RemotePenguinScreen.SetActive(false);
			}
			else
			{
				RemotePenguinScreen.SetActive(true);
			}
			switch (newState)
			{
			case DanceBattleVisualsState.Off:
				startTimerForNextLobbyStart();
				break;
			case DanceBattleVisualsState.Game:
				RedScoreBar.StopBounceAnim();
				BlueScoreBar.StopBounceAnim();
				MoveIconContainer.SetActive(true);
				setChatVisible(false);
				if (isFirstTurn)
				{
					CoroutineRunner.Start(playDanceMusicAfterDelay(DanceMusicStartDelay), this, "");
					isFirstTurn = false;
				}
				CoroutineRunner.Start(switchStateAfterDelay(currentSequenceSet.SequenceDisplayTimeInSeconds, DanceBattleVisualsState.TurnTimer), this, "DanceBattleTimerWait");
				break;
			case DanceBattleVisualsState.TurnTimer:
			{
				destroyMoveIcons();
				MoveIconInputContainer.SetActive(true);
				for (int i = 0; i < currentSequenceSet.SequenceLength; i++)
				{
					DanceBattleMoveIcon component = UnityEngine.Object.Instantiate(moveIconPrefab, MoveIconInputContainer.transform, false).GetComponent<DanceBattleMoveIcon>();
					component.SetState((localPlayerTeamId != 1) ? DanceBattleMoveIcon.MoveIconState.BlankBlue : DanceBattleMoveIcon.MoveIconState.BlankRed);
					moveIconList.Add(component);
				}
				dispatcher.DispatchEvent(new DanceBattleEvents.SequenceDisplayEnd(this));
				TurnTimer.StartCountdown(currentSequenceSet.TurnTimeInSeconds);
				CoroutineRunner.Start(switchStateAfterDelay(currentSequenceSet.TurnTimeInSeconds, DanceBattleVisualsState.PerformingMoves), this, "DanceBattleResultsWait");
				break;
			}
			case DanceBattleVisualsState.PerformingMoves:
			{
				destroyMoveIcons();
				setChatVisible(true);
				StageAnimator.SetTrigger("StrobesHigh");
				string parameter = string.Format("High{0}", currentTurnData.RoundNum);
				EventManager.Instance.PostEvent("MUS/Town/DJCadence/DanceGame", EventAction.SetSwitch, parameter, musicTargetObject);
				dispatcher.DispatchEvent(default(DanceBattleEvents.TurnTimerComplete));
				break;
			}
			case DanceBattleVisualsState.RoundResults:
			{
				StageAnimator.SetTrigger("StrobesLow");
				string trigger = "ResultLow";
				if (lastScoreBlueDelta > lastScoreRedDelta)
				{
					trigger = "ResultPerfectBlue";
				}
				else if (lastScoreBlueDelta < lastScoreRedDelta)
				{
					trigger = "ResultPerfectRed";
				}
				else if (lastScoreRedDelta > 0.5f)
				{
					trigger = "ResultMed";
				}
				StageAnimator.SetTrigger(trigger);
				if (currentTurnData.RoundNum == danceBattleDefinition.NumberOfRounds - 1)
				{
					CoroutineRunner.Start(switchStateAfterDelay(danceBattleDefinition.TurnStartDelayInSeconds, DanceBattleVisualsState.FinalResults), this, "DanceBattleResultsWait");
				}
				EventManager.Instance.PostEvent("MUS/Town/DJCadence/DanceGame", EventAction.SetSwitch, "Low", musicTargetObject);
				break;
			}
			case DanceBattleVisualsState.FinalResults:
				if (currentScoreData != null)
				{
					int num = (!(currentScoreData.Team1Score > currentScoreData.Team2Score)) ? 1 : 0;
					bool flag = currentScoreData.Team1Score == currentScoreData.Team2Score;
					if (flag)
					{
						StageAnimator.SetTrigger("WinTie");
						WinnerText.text = localizer.GetTokenTranslation("PartyGames.FindFour.Draw");
						WinnerBG.SelectColor(2);
						FloorController.SetFloorHighlight(true, true);
						FloorController.SetFloorAnim(true, true);
					}
					else if (num == 0)
					{
						StageAnimator.SetTrigger("WinBlue");
						WinnerText.text = localizer.GetTokenTranslation("Activity.DanceBattle.TeamBlueWin");
						WinnerBG.SelectColor(0);
						FloorController.SetFloorHighlight(false, true);
						FloorController.SetFloorAnim(false, true);
					}
					else
					{
						StageAnimator.SetTrigger("WinRed");
						WinnerText.text = localizer.GetTokenTranslation("Activity.DanceBattle.TeamRedWin");
						WinnerBG.SelectColor(1);
						FloorController.SetFloorHighlight(true, false);
						FloorController.SetFloorAnim(true, false);
					}
					if (flag || localPlayerTeamId == num)
					{
						EventManager.Instance.PostEvent("MUS/Town/DJCadence/DanceGame", EventAction.SetSwitch, "Win", musicTargetObject);
					}
					else
					{
						EventManager.Instance.PostEvent("MUS/Town/DJCadence/DanceGame", EventAction.SetSwitch, "Lose", musicTargetObject);
					}
					dispatcher.DispatchEvent(new DanceBattleEvents.FinalResultsShown(flag, num));
					if (EndGameParticleSelector != null)
					{
						EndGameParticleSelector.SelectGameObject(num);
					}
				}
				break;
			}
			currentState = newState;
		}

		private IEnumerator switchStateAfterDelay(float delay, DanceBattleVisualsState newState)
		{
			yield return new WaitForSeconds(delay);
			if (currentState != DanceBattleVisualsState.Off)
			{
				setState(newState);
			}
		}

		private IEnumerator playDanceMusicAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			EventManager.Instance.PostEvent("MUS/Town/DJCadence", EventAction.SetSwitch, "DanceGame", musicTargetObject);
		}

		private void startTimerForNextLobbyStart()
		{
			long timeInSeconds = Service.Get<ContentSchedulerService>().PresentTime().GetTimeInSeconds();
			DateTime dateTime = TimeUtils.calculateNextMultipleAfterTheHour(Service.Get<ContentSchedulerService>().PresentTime(), launcherDefinition.EveryXMinutesAfterTheHour);
			long num = dateTime.GetTimeInSeconds() + lobbyDefinition.LobbyLengthInSeconds - timeInSeconds;
			PreLobbyTimer.StartTimer(new TimeSpan(0, 0, (int)num));
		}

		private void onTurnStarted(DanceBattleTurnData turnData)
		{
			currentTurnData = turnData;
			currentSequenceSet = getDanceSequenceSetForRoundNumber(turnData.RoundNum);
			setState(DanceBattleVisualsState.Game);
			RoundText.text = string.Format(localizer.GetTokenTranslation("Activity.DanceBattle.Round"), turnData.RoundNum);
			dispatcher.DispatchEvent(new DanceBattleEvents.TurnStart(this, currentTurnData));
		}

		private DanceBattleDefinition.DanceSequenceSet getDanceSequenceSetForRoundNumber(int roundNumber)
		{
			DanceBattleDefinition.DanceSequenceSet result = null;
			for (int i = 0; i < danceBattleDefinition.DanceSequenceSets.Length; i++)
			{
				if (roundNumber >= danceBattleDefinition.DanceSequenceSets[i].MinRound && roundNumber <= danceBattleDefinition.DanceSequenceSets[i].MaxRound)
				{
					result = danceBattleDefinition.DanceSequenceSets[i];
					break;
				}
			}
			return result;
		}

		private void startTurnSequence()
		{
			CoroutineRunner.Start(showTurnSequence(), this, "ShowDanceBattleTurnSequence");
		}

		private void onScoresUpdated(DanceBattleScoreData scoreData)
		{
			if (currentState == DanceBattleVisualsState.PerformingMoves)
			{
				currentScoreData = scoreData;
				lastScoreRedDelta = scoreData.Team2Score - lastScoreRed;
				lastScoreRed = scoreData.Team2Score;
				lastScoreBlueDelta = scoreData.Team1Score - lastScoreBlue;
				lastScoreBlue = scoreData.Team1Score;
				if (localPlayerIsInGame)
				{
					DanceBattleUtils.LogRoundEndBI(currentTurnData.RoundNum, (localPlayerTeamId == 0) ? lastScoreBlueDelta : lastScoreRedDelta, localPlayerTeamId);
				}
				RedScoreBar.SetBarValue(scoreData.Team2Score / (float)danceBattleDefinition.NumberOfRounds);
				BlueScoreBar.SetBarValue(scoreData.Team1Score / (float)danceBattleDefinition.NumberOfRounds);
				setState(DanceBattleVisualsState.RoundResults);
				dispatcher.DispatchEvent(new DanceBattleEvents.TurnEnd(this, currentTurnData, currentScoreData));
			}
		}

		private IEnumerator showTurnSequence()
		{
			dispatcher.DispatchEvent(new DanceBattleEvents.SequenceDisplayStart(this, currentTurnData));
			destroyMoveIcons();
			for (int i = 0; i < currentTurnData.Moves.Count; i++)
			{
				DanceBattleMoveIcon moveIcon = UnityEngine.Object.Instantiate(moveIconPrefab, MoveIconContainer.transform, false).GetComponent<DanceBattleMoveIcon>();
				switch (currentTurnData.Moves[i])
				{
				case 1:
					moveIcon.SetState(DanceBattleMoveIcon.MoveIconState.Icon1);
					break;
				case 2:
					moveIcon.SetState(DanceBattleMoveIcon.MoveIconState.Icon2);
					break;
				case 3:
					moveIcon.SetState(DanceBattleMoveIcon.MoveIconState.Icon3);
					break;
				}
				moveIcon.GetComponent<Animator>().SetTrigger("Intro");
				moveIconList.Add(moveIcon);
				EventManager.Instance.PostEvent("SFX/UI/DanceGame/NoteAppear", EventAction.PlaySound, null, null);
				yield return new WaitForSeconds(1f);
			}
		}

		private void destroyMoveIcons()
		{
			for (int num = MoveIconContainer.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(MoveIconContainer.transform.GetChild(num).gameObject);
			}
			for (int num2 = MoveIconInputContainer.transform.childCount - 1; num2 >= 0; num2--)
			{
				UnityEngine.Object.Destroy(MoveIconInputContainer.transform.GetChild(num2).gameObject);
			}
			moveIconList.Clear();
		}

		private bool onTurnInputSent(DanceBattleEvents.TurnInputSent evt)
		{
			if (currentState == DanceBattleVisualsState.TurnTimer && evt.PlayerId == localPlayerSessionId)
			{
				switch (evt.Input)
				{
				case 1:
					moveIconList[evt.InputCount].SetState(DanceBattleMoveIcon.MoveIconState.Icon1);
					break;
				case 2:
					moveIconList[evt.InputCount].SetState(DanceBattleMoveIcon.MoveIconState.Icon2);
					break;
				case 3:
					moveIconList[evt.InputCount].SetState(DanceBattleMoveIcon.MoveIconState.Icon3);
					break;
				}
			}
			return false;
		}

		private void getDefinitions()
		{
			Dictionary<int, PartyGameDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, PartyGameDefinition>>();
			if (dictionary.ContainsKey(3))
			{
				partyGameDefinition = dictionary[3];
				danceBattleDefinition = (DanceBattleDefinition)partyGameDefinition.GameData;
				lobbyDefinition = (PartyGameLobbyMmoItemTeamDefinition)partyGameDefinition.LobbyData;
				launcherDefinition = PartyGameUtils.GetPartyGameLauncherForPartyGameId(partyGameDefinition.Id);
			}
		}

		private IEnumerator createTopHud()
		{
			AssetRequest<GameObject> request = Content.LoadAsync(TopHudPrefabKey);
			yield return request;
			GameObject topHudGameObject = UnityEngine.Object.Instantiate(request.Asset);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(topHudGameObject));
			topHud = topHudGameObject.GetComponent<PartyGameTopHud>();
			string token = (localPlayerTeamId == 0) ? "Activity.DanceBattle.PickTeam1" : "Activity.DanceBattle.PickTeam2";
			topHud.ShowHudState(PartyGameTopHud.HudStates.TextOnly, token, localPlayerTeamId);
			dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
			dispatcher.DispatchEvent(default(PlayerCardEvents.DismissPlayerCard));
			yield return new WaitForSeconds(3f);
			if (topHud != null)
			{
				topHud.ShowHudState(PartyGameTopHud.HudStates.TextAboveLoader, "Activity.DanceBattle.Waiting", localPlayerTeamId);
			}
		}

		private void setChatVisible(bool visible)
		{
			if (localPlayerIsInGame)
			{
				if (visible)
				{
					CinematicSpeechState.ShowChatUI();
					CameraCullingMaskHelper.ShowLayer(Camera.main, "AllPlayerInteractibles");
				}
				else
				{
					CinematicSpeechState.HideChatUI();
					CameraCullingMaskHelper.HideLayer(Camera.main, "AllPlayerInteractibles");
				}
			}
		}

		private bool isCurrentLobbyFull()
		{
			bool result = false;
			if (numPlayersInLobby >= partyGameDefinition.MaxPlayerCount)
			{
				result = true;
			}
			return result;
		}
	}
}
