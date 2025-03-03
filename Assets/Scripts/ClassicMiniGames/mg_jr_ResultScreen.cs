using ClubPenguin.Classic;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using JetpackReboot;
using MinigameFramework;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class mg_jr_ResultScreen : MinigameScreen
{
	private enum ResultScreenState
	{
		WAITING_TO_SHOW = 1,
		FIRST_VISIBLE,
		PENGUIN_FLYBY_START,
		PENGUIN_FLYBY_AND_DISTANCE_COUNT,
		GARY_SPEECH,
		COINS_COLLECTED_FLYIN,
		COIN_COUNT,
		BONUS_ROBOT_SPEECH,
		WAITING_TO_CLOSE,
		MAX
	}

	private mg_jr_GameLogic m_gameLogic = null;

	private Button m_retryButton = null;

	private Button m_quitButton = null;

	private GameObject m_resultdistanceContainer = null;

	private GameObject m_DistanceSparkleContainer = null;

	private GameObject m_runCoinBar = null;

	private GameObject m_bonusRobotBig = null;

	private GameObject m_bonusRobotSmall = null;

	private Animator m_runCoinBarAnimator = null;

	private mg_jr_FlyByPenguin m_flybyPenguin = null;

	private mg_jr_ResultGary m_gary = null;

	private mg_jr_ResultCounter m_distanceCounter = null;

	private bool m_isDistanceCountCompleted = false;

	private mg_jr_ResultCounter m_coinCounter = null;

	private Text m_coinsCollectedSession = null;

	private Text m_runCoinbarLabel;

	private Animator m_coinTransfer = null;

	private Localizer localizer;

	private ResultScreenState m_currentState = ResultScreenState.WAITING_TO_SHOW;

	protected override void Awake()
	{
		base.Awake();
		if (Service.IsSet<Localizer>())
		{
			localizer = Service.Get<Localizer>();
		}
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	private void Start()
	{
		mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
		m_gameLogic = active.GameLogic;
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_jr_RetryButton")
			{
				m_retryButton = button;
			}
			else if (button.gameObject.name == "mg_jr_QuitButton")
			{
				m_quitButton = button;
			}
		}
		m_retryButton.onClick.AddListener(OnRetryClicked);
		m_quitButton.onClick.AddListener(OnQuitClicked);
		ClassicMiniGames.PushBackButtonHandler(OnQuitClicked);
		Text[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Text>(true);
		Text[] array2 = componentsInChildren2;
		foreach (Text text in array2)
		{
			if (text.gameObject.name == "mg_jr_TotalCoinCount")
			{
				m_coinsCollectedSession = text;
				m_coinsCollectedSession.text = string.Concat(m_gameLogic.Player.CoinsCollectedSession - m_gameLogic.Player.CoinsCollected);
			}
			else if (text.gameObject.name == "mg_jr_RunCoinCount")
			{
				text.text = string.Concat(m_gameLogic.Player.CoinsCollected);
			}
			else if (text.gameObject.name == "mg_jr_RunCoinsLabel")
			{
				m_runCoinbarLabel = text;
				if (localizer != null)
				{
					m_runCoinbarLabel.text = localizer.GetTokenTranslation("Activity.MiniGames.CollectedCoins");
				}
				else
				{
					m_runCoinbarLabel.text = "Collected Coins";
				}
			}
		}
		Transform[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<Transform>(true);
		Transform[] array3 = componentsInChildren3;
		foreach (Transform transform in array3)
		{
			if (transform.gameObject.name == "mg_jr_ResultDistanceContainer")
			{
				m_resultdistanceContainer = transform.gameObject;
			}
			else if (transform.gameObject.name == "mg_jr_DistanceSparkle")
			{
				m_DistanceSparkleContainer = transform.gameObject;
			}
			else if (transform.gameObject.name == "mg_jr_RunCoinBar")
			{
				m_runCoinBar = transform.gameObject;
				m_runCoinBarAnimator = m_runCoinBar.GetComponent<Animator>();
			}
			else if (transform.gameObject.name == "mg_jr_CoinTransfer")
			{
				m_coinTransfer = transform.GetComponent<Animator>();
			}
			else if (transform.gameObject.name == "mg_jr_BigBonusRobot")
			{
				m_bonusRobotBig = transform.gameObject;
			}
			else if (transform.gameObject.name == "mg_jr_LittleBonusRobot")
			{
				m_bonusRobotSmall = transform.gameObject;
			}
		}
		mg_jr_ResultCounter[] componentsInChildren4 = GetComponentsInChildren<mg_jr_ResultCounter>(true);
		mg_jr_ResultCounter[] array4 = componentsInChildren4;
		foreach (mg_jr_ResultCounter mg_jr_ResultCounter in array4)
		{
			if (mg_jr_ResultCounter.gameObject.name == "mg_jr_ResultDistance")
			{
				m_distanceCounter = mg_jr_ResultCounter;
				Assert.NotNull(m_distanceCounter, "distance conuter needed");
				m_distanceCounter.SfxLoopName = mg_jr_Sound.UI_POINTS_COUNTER_LOOP.ClipName();
				m_distanceCounter.PostFix = "m";
			}
			else if (mg_jr_ResultCounter.gameObject.name == "mg_jr_TotalCoinCount")
			{
				m_coinCounter = mg_jr_ResultCounter;
				Assert.NotNull(m_coinCounter, "coin conuter needed");
				m_coinCounter.SfxLoopName = mg_jr_Sound.COIN_COLLECT_LOOP.ClipName();
			}
		}
		m_flybyPenguin = GetComponentsInChildren<mg_jr_FlyByPenguin>(true)[0];
		Assert.NotNull(m_flybyPenguin, "fly by penguin needed");
		m_flybyPenguin.PassingMiddleOfScreen += OnFlyByPenguinPassingMiddle;
		m_gary = GetComponentInChildren<mg_jr_ResultGary>();
		Assert.NotNull(m_gary, "Gary is needed");
		active.MusicManager.SelectTrack(mg_jr_Sound.MENU_MUSIC_AMBIENT.ClipName());
		m_currentState = ResultScreenState.PENGUIN_FLYBY_START;
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_retryButton.onClick.RemoveListener(OnRetryClicked);
		m_quitButton.onClick.RemoveListener(OnQuitClicked);
		ClassicMiniGames.RemoveBackButtonHandler(OnQuitClicked);
		m_flybyPenguin.PassingMiddleOfScreen -= OnFlyByPenguinPassingMiddle;
	}

	private void Update()
	{
		switch (m_currentState)
		{
		case ResultScreenState.WAITING_TO_SHOW:
			break;
		case ResultScreenState.FIRST_VISIBLE:
			break;
		case ResultScreenState.GARY_SPEECH:
			break;
		case ResultScreenState.COINS_COLLECTED_FLYIN:
			break;
		case ResultScreenState.COIN_COUNT:
			break;
		case ResultScreenState.BONUS_ROBOT_SPEECH:
			break;
		case ResultScreenState.WAITING_TO_CLOSE:
			break;
		case ResultScreenState.PENGUIN_FLYBY_START:
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.UI_GAMEOVER_SCREEN.ClipName());
			m_currentState = ResultScreenState.PENGUIN_FLYBY_AND_DISTANCE_COUNT;
			m_flybyPenguin.PerformFlyBy();
			break;
		case ResultScreenState.PENGUIN_FLYBY_AND_DISTANCE_COUNT:
		{
			if (!m_isDistanceCountCompleted || m_flybyPenguin.CurrentState != mg_jr_FlyByPenguin.FlyByState.COMPLETED)
			{
				break;
			}
			Animator component = m_DistanceSparkleContainer.GetComponent<Animator>();
			int num = (int)m_gameLogic.Odometer.DistanceTravelledThisRun;
			int bestDistance = m_gameLogic.MiniGame.PlayerStats.BestDistance;
			if (num >= bestDistance)
			{
				component.SetBool("Sparkle", true);
				Transform[] componentsInChildren = m_resultdistanceContainer.GetComponentsInChildren<Transform>(true);
				Transform[] array = componentsInChildren;
				foreach (Transform transform in array)
				{
					if (transform.gameObject.name == "mg_jr_DistanceRibbon")
					{
						transform.gameObject.SetActive(true);
					}
				}
			}
			else
			{
				component.SetBool("Sparkle", false);
				component.SetTrigger("SparkleOnce");
			}
			string thingToSay = (localizer == null) ? "Keep practicing, you&apos;ll get it!" : localizer.GetTokenTranslation("Activity.MiniGames.ResultDialogue3");
			if (num >= bestDistance)
			{
				thingToSay = ((localizer == null) ? "Gadzooks! What a record!" : localizer.GetTokenTranslation("Activity.MiniGames.ResultDialogue1"));
			}
			else if (num >= 100)
			{
				thingToSay = ((localizer == null) ? "Jumping jellyfish! What a flight!" : localizer.GetTokenTranslation("Activity.MiniGames.ResultDialogue2"));
			}
			m_gary.PerformSpeech(thingToSay, OnGarysDistanceSpeechCompleted);
			m_currentState = ResultScreenState.GARY_SPEECH;
			break;
		}
		default:
			Assert.IsTrue(false, "Unhandled state while updating result screen");
			break;
		}
	}

	private void OnDistanceCountCompleted()
	{
		m_isDistanceCountCompleted = true;
	}

	private void OnFlyByPenguinPassingMiddle()
	{
		m_resultdistanceContainer.SetActive(true);
		m_distanceCounter.StartCount(OnDistanceCountCompleted, (int)m_gameLogic.Odometer.DistanceTravelledThisRun);
	}

	private void OnGarysDistanceSpeechCompleted()
	{
		MinigameManager.GetActive().PlaySFX(mg_jr_Sound.UI_GAMEOVER_SCREEN.ClipName());
		m_runCoinBarAnimator.SetTrigger("Slide");
		m_currentState = ResultScreenState.COINS_COLLECTED_FLYIN;
	}

	public void OnCoinDiplaySlideInComplete()
	{
		int coinsCollectedSession = m_gameLogic.Player.CoinsCollectedSession;
		if (m_gameLogic.Player.CoinsCollected > 0)
		{
			m_coinCounter.StartCount(OnCoinCountCompleted, coinsCollectedSession, coinsCollectedSession - m_gameLogic.Player.CoinsCollected);
			m_coinTransfer.gameObject.SetActive(true);
			m_coinTransfer.SetBool("Transfer", true);
		}
	}

	private void OnCoinCountCompleted()
	{
		m_coinTransfer.SetBool("Transfer", false);
		m_coinTransfer.gameObject.SetActive(false);
		int coinsCollected = m_gameLogic.Player.CoinsCollected;
		if (coinsCollected > 0)
		{
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.PICKUP_COIN_10.ClipName());
		}
		if ((float)coinsCollected >= m_gameLogic.GameBalance.CoinsForBonusRobot)
		{
			m_bonusRobotBig.SetActive(true);
			string text = "";
			if (localizer != null)
			{
				m_runCoinbarLabel.text = localizer.GetTokenTranslation("Activity.MiniGames.Robot");
				text = localizer.GetTokenTranslation("Activity.MiniGames.GarysBonusAward");
			}
			else
			{
				m_runCoinbarLabel.text = "Congratulations! You earned a bonus Robot Penguin! Restart now to use it!";
				text = "These little guys can take a hit for you!";
			}
			m_runCoinbarLabel.fontSize = 16;
			m_gary.PerformSpeech(text, OnGarysBonusSpeechCompleted);
		}
		else
		{
			m_currentState = ResultScreenState.WAITING_TO_CLOSE;
		}
	}

	private void OnGarysBonusSpeechCompleted()
	{
		m_bonusRobotSmall.SetActive(true);
		m_currentState = ResultScreenState.WAITING_TO_CLOSE;
	}

	private void OnRetryClicked()
	{
		UIManager.Instance.PopScreen();
		mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
		active.PlaySFX(mg_jr_Sound.UI_SELECT.ClipName());
		active.SaveState();
		bool startWithBonusRobot = (float)m_gameLogic.Player.CoinsCollected >= m_gameLogic.GameBalance.CoinsForBonusRobot;
		active.GameLogic.LoadInitialState();
		active.GameLogic.StartGame(startWithBonusRobot);
	}

	private void OnQuitClicked()
	{
		UIManager.Instance.PopScreen();
		MinigameManager.Instance.OnMinigameQuit();
		ClassicMiniGames.RemoveBackButtonHandler(OnQuitClicked);
	}
}
