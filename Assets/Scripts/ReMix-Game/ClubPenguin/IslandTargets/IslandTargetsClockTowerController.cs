using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.ScheduledWorldObjects;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.IslandTargets
{
	public class IslandTargetsClockTowerController : MonoBehaviour
	{
		private enum TargetsGameState
		{
			Idle,
			InGame,
			FinalRound
		}

		public const string TARGET_REWARD_SOURCE_ID = "Targets";

		private const string START_GAME_ANIM_TRIGGER = "StartGame";

		private const string END_GAME_ANIM_TRIGGER = "EndGame";

		private const string IN_GAME_ANIM_BOOL = "InGame";

		private const string START_BIG_TARGET_ANIM_TRIGGER = "StartBigTarget";

		private const string TIME_OUT_ANIM_TRIGGER = "TimeOut";

		private const string ROOF_OPEN_ANIM_NAME = "CrateCoRoofOpen";

		private const float ROOF_OPEN_ANIM_TIME = 0.31f;

		private const string CULLING_GROUP_NAME = "SkiVillageLeft";

		public IslandTargetPlaygroundController playgroundController;

		public IslandTargetsPlayground islandTargetsPlayground;

		public Animator ClockTowerAnimator;

		public AnalogClockController ClockController;

		public Transform RewardEffectsPosition;

		public GameObject FinishCamera;

		public GameObject ClockTimer;

		public GameObject ClockLargeHand;

		public GameObject ClockSmallHand;

		public ColorUtils.ColorAtPercent[] ClockTimerColors;

		public IslandTargetsSideScoreboard[] SideScoreboards;

		[Header(" ")]
		public string ClockTriggerFlyOut;

		public string ClockTriggerFlyBack;

		public string ParticipationPlayerPrefName;

		public string NormalScarecrowName;

		public string ScarecrowAppearTrigger;

		public GameObject ClockCountdown;

		public float scaleCountdownText = 0.0125f;

		public float scaleReadyText = 0.007f;

		public int BestEverStreak;

		[Header(" ")]
		public Image ClockTimerImage;

		public InWorldText WinStreakText;

		public InWorldText DailyRecordText;

		public TextMesh GameStartCountdownTimerText;

		public InWorldText[] TimerTexts;

		public InWorldText[] ScoreTexts;

		public InWorldText[] WaveTexts;

		[Range(0.25f, 10f)]
		public float RoofOpenSpeed = 1f;

		[Range(0f, 10f)]
		public float RewardDelay = 4f;

		[Range(0f, 10f)]
		public float TimeoutDelay = 5f;

		public float RoundFailResetDelay = 5f;

		public float RoundWinResetDelay = 5f;

		public GameObject RewardEffectsPrefab;

		[Header("Audio")]
		public string MusicEventName = "MUS/MtBlizzard/Village";

		public string EvergreenMusicTargetPath = "Music/WorldMusic/Village";

		public string CrateCoMusicTargetPath = "Music/WorldMusic/Village";

		private GameObject evergreenMusicTarget;

		private GameObject createCoMusicTarget;

		public string[] RoundMusicNames;

		public string WinMusicName = "CrateCo_Win";

		public string LoseMusicName = "CrateCo_Lose";

		public string[] RoundSFXTriggers;

		public string WinSFXTrigger = "";

		public string LoseSFXTrigger = "";

		public string LowTimeSFXTrigger = "SFX/AO/CrateCo/Clock/TimerCountdown";

		public string GameStartingWarningSFXTrigger = "SFX/AO/CrateCo/Clock/TimerCountdown";

		public string RoofOpenSFXTrigger = "SFX/AO/CrateCo/Clock/RoofOpen";

		public float ReturnToEvergreenMusicWait = 4f;

		[Tooltip("Tier1 name to log the BI under")]
		[Header("BI Logging")]
		public string BI_Tier1Name = "crate_co_game";

		private TargetsGameState gameState = TargetsGameState.Idle;

		private float currentGameDuration;

		private float currentGameTime;

		private float currentRoofOpenAmount = 0f;

		private float targetRoofOpenAmount = 0f;

		private bool isPlayingLowTimeSFX = false;

		private bool isPlayingLoseSFXTrigger = false;

		private bool isPlayingLoseMusic = false;

		private bool isZoneTransitioning = false;

		private Timer gameTimer;

		private EventDispatcher dispatcher;

		private EventChannel eventChannel;

		private CPDataEntityCollection dataEntityCollection;

		private ContentSchedulerService contentSchedulerService;

		private DateTime nextGameStartTime;

		private bool gameStartCountdownTimerOn;

		private Localizer localizer;

		private Animator animatorFloatingClock;

		private GameObject normalScarecrowObj;

		private void Awake()
		{
			dispatcher = playgroundController.EventDispatcher;
			eventChannel = new EventChannel(dispatcher);
			contentSchedulerService = Service.Get<ContentSchedulerService>();
			localizer = Service.Get<Localizer>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			evergreenMusicTarget = GameObject.Find(EvergreenMusicTargetPath);
			EventManager.Instance.PostEvent(MusicEventName, EventAction.PlaySound, evergreenMusicTarget);
			createCoMusicTarget = GameObject.Find(CrateCoMusicTargetPath);
			Service.Get<EventDispatcher>().AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.RewardsEarned>(onRewardsEarned);
			Service.Get<EventDispatcher>().AddListener<IslandTargetsEvents.ClockTowerStateChanged>(onClockTowerStateChange);
			if (DisableWhenNoGameServer.IsGameServerAvailable())
			{
				gameTimer = new Timer(1f, true, delegate
				{
					onGameTimerTick();
				});
				animatorFloatingClock = ClockTimer.GetComponentInChildren<Animator>();
				CoroutineRunner.Start(setScarecrowVisible(true), this, "setScarecrowVisible");
				CoroutineRunner.Start(setClockTimerVisible(false), this, "setClockTimerVisible");
				eventChannel.AddListener<IslandTargetsEvents.TargetsRemainingUpdated>(onTargetsRemainingUpdated);
				eventChannel.AddListener<IslandTargetsEvents.GameRoundStarted>(onRoundStarted);
				eventChannel.AddListener<IslandTargetsEvents.GameRoundEnded>(onRoundEnded);
				eventChannel.AddListener<IslandTargetsEvents.TargetGameTimeOut>(onGameTimeOut);
				eventChannel.AddListener<IslandTargetsEvents.StatsUpdated>(onStatsUpdated);
				if (TimeUtils.isMultipleOfXMinutesAfterTheHour(contentSchedulerService.PresentTime(), islandTargetsPlayground.EveryXMinutesAfterTheHour))
				{
					gotoFiveSecsGameStartMarkState();
				}
				else
				{
					InvokeRepeating("displayGameStartCountdown", 0f, 1f);
				}
			}
			else
			{
				ClockTimer.SetActive(false);
				WinStreakText.SetText(BestEverStreak.ToString());
				DailyRecordText.transform.parent.gameObject.SetActive(false);
			}
			if (string.IsNullOrEmpty(BI_Tier1Name))
			{
				BI_Tier1Name = "crate_co_game";
				Log.LogError(this, string.Format("Error: Tier1 name for BI is not set on '{0}'", base.gameObject.GetPath()));
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			if (gameState == TargetsGameState.FinalRound)
			{
				dispatcher.RemoveListener<IslandTargetsEvents.TargetHit>(onTargetHit);
			}
			Service.Get<EventDispatcher>().RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.RewardsEarned>(onRewardsEarned);
			Service.Get<EventDispatcher>().RemoveListener<IslandTargetsEvents.ClockTowerStateChanged>(onClockTowerStateChange);
			CoroutineRunner.StopAllForOwner(this);
		}

		private void Update()
		{
			if (gameState == TargetsGameState.Idle)
			{
				return;
			}
			currentGameTime += Time.deltaTime;
			float num = currentGameTime / currentGameDuration;
			ClockTimerImage.fillAmount = 1f - num;
			Color color = Color.green;
			for (int i = 0; i < ClockTimerColors.Length; i++)
			{
				if (num > ClockTimerColors[i].Percent)
				{
					color = ClockTimerColors[i].Color;
					if (!isPlayingLowTimeSFX && i == ClockTimerColors.Length - 1)
					{
						EventManager.Instance.PostEvent(LowTimeSFXTrigger, EventAction.PlaySound, base.gameObject);
						isPlayingLowTimeSFX = true;
					}
				}
			}
			ClockTimerImage.color = color;
			ClockSmallHand.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -360f * num));
			if (gameState == TargetsGameState.FinalRound)
			{
				animateRoofOpen();
			}
		}

		private void displayGameStartCountdown()
		{
			if (!gameStartCountdownTimerOn)
			{
				nextGameStartTime = TimeUtils.calculateNextMultipleAfterTheHour(contentSchedulerService.PresentTime(), islandTargetsPlayground.EveryXMinutesAfterTheHour);
			}
			gameStartCountdownTimerOn = true;
			DateTime value = contentSchedulerService.PresentTime();
			float num = (float)nextGameStartTime.Subtract(value).TotalSeconds;
			if (num <= 30f && num > 5f)
			{
				if (SideScoreboards[0].CurrentState != IslandTargetsSideScoreboard.SideScoreboardState.ThirtySecsGameStartMark)
				{
					gotoThirtySecsGameStartMarkState();
				}
			}
			else if (num <= 5f && num > 0f)
			{
				gotoFiveSecsGameStartMarkState();
				stopGameStartCountdown(false);
				return;
			}
			int num2 = (int)num / 60;
			int num3 = (int)num % 60;
			GameStartCountdownTimerText.characterSize = scaleCountdownText;
			GameStartCountdownTimerText.text = string.Format("{0}:{1:00}", num2, num3);
		}

		private void gotoThirtySecsGameStartMarkState()
		{
			for (int i = 0; i < SideScoreboards.Length; i++)
			{
				SideScoreboards[i].SetState(IslandTargetsSideScoreboard.SideScoreboardState.ThirtySecsGameStartMark);
			}
		}

		private void gotoFiveSecsGameStartMarkState()
		{
			EventManager.Instance.PostEvent(GameStartingWarningSFXTrigger, EventAction.PlaySound, base.gameObject);
			for (int i = 0; i < SideScoreboards.Length; i++)
			{
				SideScoreboards[i].SetState(IslandTargetsSideScoreboard.SideScoreboardState.FiveSecsGameStartMark);
			}
			GameStartCountdownTimerText.characterSize = scaleReadyText;
			GameStartCountdownTimerText.text = localizer.GetTokenTranslation("Zone.CrateCo.Ready");
		}

		private void stopGameStartCountdown(bool clearCountdownText = true)
		{
			if (clearCountdownText)
			{
				GameStartCountdownTimerText.text = "";
			}
			CancelInvoke("displayGameStartCountdown");
			gameStartCountdownTimerOn = false;
		}

		private void onGameTimerTick()
		{
			UpdateTimers();
		}

		private void UpdateTimers()
		{
			string text = new DateTime(0L).AddSeconds(currentGameTime).ToString("m:ss");
			for (int i = 0; i < TimerTexts.Length; i++)
			{
				TimerTexts[i].SetText(text);
			}
		}

		private void UpdateScores(int currentScore, int totalScore)
		{
			string text = CreateScoreString(currentScore, totalScore);
			for (int i = 0; i < ScoreTexts.Length; i++)
			{
				ScoreTexts[i].SetText(text);
			}
		}

		private string CreateScoreString(int current, int total)
		{
			if (current > total)
			{
				current = total;
			}
			bool flag = current >= 0 && total >= 0;
			if (!flag)
			{
			}
			return flag ? string.Format("{0}/{1}", current, total) : "";
		}

		private void UpdateRounds(int currentRound, int totalRounds)
		{
			string text = CreateScoreString(currentRound, totalRounds);
			for (int i = 0; i < WaveTexts.Length; i++)
			{
				WaveTexts[i].SetText(text);
			}
		}

		private bool onTargetsRemainingUpdated(IslandTargetsEvents.TargetsRemainingUpdated evt)
		{
			if (gameState != 0)
			{
				UpdateScores(evt.TotalTargets - evt.TargetsRemaining, evt.TotalTargets);
			}
			return false;
		}

		private bool onClockTowerStateChange(IslandTargetsEvents.ClockTowerStateChanged evt)
		{
			if (evt.IsEnabled)
			{
				ClockTowerAnimator.ResetTrigger("StartGame");
				ClockTowerAnimator.ResetTrigger("StartBigTarget");
				ClockTowerAnimator.ResetTrigger("TimeOut");
				switch (gameState)
				{
				case TargetsGameState.InGame:
					ClockTowerAnimator.SetTrigger("StartGame");
					break;
				case TargetsGameState.FinalRound:
					ClockTowerAnimator.Play("CrateCoRoofOpen", 0, 0f);
					ClockTowerAnimator.speed = 0f;
					break;
				}
				UpdateScores(playgroundController.TotalTargets - playgroundController.TargetsRemaining, playgroundController.TotalTargets);
			}
			return false;
		}

		private bool onRoundStarted(IslandTargetsEvents.GameRoundStarted evt)
		{
			DateTime value = evt.StartTime.MsToDateTime();
			DateTime dateTime = evt.EndTime.MsToDateTime();
			DateTime dateTime2 = contentSchedulerService.PresentTime();
			currentGameDuration = (float)dateTime.Subtract(value).TotalSeconds;
			currentGameTime = (float)dateTime2.Subtract(value).TotalSeconds;
			IslandTargetsSideScoreboard.SideScoreboardState state = IslandTargetsSideScoreboard.SideScoreboardState.InGame;
			if (gameState == TargetsGameState.Idle)
			{
				stopGameStartCountdown();
				ClockTimer.SetActive(true);
				if (animatorFloatingClock != null && !string.IsNullOrEmpty(ClockTriggerFlyOut))
				{
					animatorFloatingClock.SetTrigger(ClockTriggerFlyOut);
					animatorFloatingClock.ResetTrigger(ClockTriggerFlyBack);
				}
				CoroutineRunner.Start(setScarecrowVisible(false), this, "setScarecrowVisible");
				currentRoofOpenAmount = 0f;
				targetRoofOpenAmount = 0f;
				if (ClockController != null)
				{
					ClockController.StopClock();
				}
				iTween.RotateTo(ClockLargeHand, iTween.Hash("z", 0f, "time", 0.8f));
				ClockTowerAnimator.SetTrigger("StartGame");
				CoroutineRunner.Start(gameTimer.Start(), this, "TargetsScoreboardsTimer");
			}
			UpdateRounds(playgroundController.CurrentRound, playgroundController.TotalRounds);
			gameState = ((!evt.IsFinalRound) ? TargetsGameState.InGame : TargetsGameState.FinalRound);
			if (gameState == TargetsGameState.FinalRound)
			{
				dispatcher.AddListener<IslandTargetsEvents.TargetHit>(onTargetHit);
				ClockTowerAnimator.Play("CrateCoRoofOpen", 0, 0f);
				ClockTowerAnimator.speed = 0f;
				state = IslandTargetsSideScoreboard.SideScoreboardState.FinalRound;
			}
			UpdateScores(playgroundController.TotalTargets - playgroundController.TargetsRemaining, playgroundController.TotalTargets);
			for (int i = 0; i < SideScoreboards.Length; i++)
			{
				SideScoreboards[i].SetState(state);
			}
			int num = playgroundController.CurrentRound;
			if (num == 0)
			{
				num = RoundSFXTriggers.Length - 1;
			}
			num--;
			if (num < RoundSFXTriggers.Length && !string.IsNullOrEmpty(RoundSFXTriggers[num]))
			{
				EventManager.Instance.PostEvent(RoundSFXTriggers[num], EventAction.PlaySound, base.gameObject);
			}
			if (num < RoundMusicNames.Length && !string.IsNullOrEmpty(RoundMusicNames[num]))
			{
				EventManager.Instance.PostEvent(MusicEventName, EventAction.SetSwitch, RoundMusicNames[num], createCoMusicTarget);
			}
			return false;
		}

		private bool onTargetHit(IslandTargetsEvents.TargetHit evt)
		{
			if (gameState == TargetsGameState.FinalRound)
			{
				targetRoofOpenAmount = evt.DamageCount / evt.DamageCapacity;
				EventManager.Instance.PostEvent(RoofOpenSFXTrigger, EventAction.PlaySound, base.gameObject);
			}
			return false;
		}

		private void onClockTimerColorUpdate(Color value)
		{
			ClockTimerImage.color = value;
		}

		private bool onRoundEnded(IslandTargetsEvents.GameRoundEnded evt)
		{
			if (gameState == TargetsGameState.FinalRound)
			{
				gameState = TargetsGameState.Idle;
				ClockTowerAnimator.speed = 1f;
				gameState = TargetsGameState.Idle;
				dispatcher.RemoveListener<IslandTargetsEvents.TargetHit>(onTargetHit);
				CoroutineRunner.Start(setClockTimerVisible(false, RewardDelay), this, "setClockTimerVisible");
				ClockSmallHand.transform.localRotation = Quaternion.identity;
				if (animatorFloatingClock != null && !string.IsNullOrEmpty(ClockTriggerFlyBack))
				{
					animatorFloatingClock.SetTrigger(ClockTriggerFlyBack);
					animatorFloatingClock.ResetTrigger(ClockTriggerFlyOut);
				}
				CoroutineRunner.Start(setScarecrowVisible(true, RewardDelay), this, "setScarecrowVisible");
				FinishCamera.SetActive(true);
				if (ClockController != null)
				{
					ClockController.StartClock();
				}
				for (int i = 0; i < SideScoreboards.Length; i++)
				{
					SideScoreboards[i].SetState(IslandTargetsSideScoreboard.SideScoreboardState.WonGame);
				}
				CoroutineRunner.Start(resetScoreboardState(RoundWinResetDelay), this, "ResetScoreboardState");
				CoroutineRunner.Start(showRewards(), this, "ShowTargetGameRewards");
				if (!string.IsNullOrEmpty(WinMusicName))
				{
					EventManager.Instance.PostEvent(MusicEventName, EventAction.SetSwitch, WinMusicName, createCoMusicTarget);
				}
				EventManager.Instance.PostEvent(WinSFXTrigger, EventAction.PlaySound, base.gameObject);
				LogBIDataForWin();
				CoroutineRunner.Start(SwitchBackToDefautMusic(ReturnToEvergreenMusicWait), this, "SwitchToEvergreenMusic");
			}
			else
			{
				for (int i = 0; i < SideScoreboards.Length; i++)
				{
					SideScoreboards[i].SetState(IslandTargetsSideScoreboard.SideScoreboardState.BetweenRounds);
				}
			}
			if (isPlayingLowTimeSFX)
			{
				EventManager.Instance.PostEvent(LowTimeSFXTrigger, EventAction.StopSound, base.gameObject);
				isPlayingLowTimeSFX = false;
			}
			InvokeRepeating("displayGameStartCountdown", 0f, 1f);
			return false;
		}

		private void LogBIDataForWin()
		{
			Service.Get<ICPSwrveService>().Action(BI_Tier1Name, "game_success");
		}

		private bool onGameTimeOut(IslandTargetsEvents.TargetGameTimeOut evt)
		{
			gameState = TargetsGameState.Idle;
			gameTimer.Stop();
			CoroutineRunner.Start(setClockTimerVisible(false, TimeoutDelay), this, "setClockTimerVisible");
			ClockTowerAnimator.speed = 1f;
			if (ClockController != null)
			{
				ClockController.StartClock();
			}
			ClockTowerAnimator.SetTrigger("TimeOut");
			if (animatorFloatingClock != null && !string.IsNullOrEmpty(ClockTriggerFlyBack))
			{
				animatorFloatingClock.ResetTrigger(ClockTriggerFlyOut);
				animatorFloatingClock.SetTrigger(ClockTriggerFlyBack);
			}
			CoroutineRunner.Start(setScarecrowVisible(true, TimeoutDelay), this, "setScarecrowVisible");
			for (int i = 0; i < SideScoreboards.Length; i++)
			{
				SideScoreboards[i].SetState(IslandTargetsSideScoreboard.SideScoreboardState.RoundFail);
			}
			CoroutineRunner.Start(resetScoreboardState(RoundFailResetDelay), this, "ResetScoreboardState");
			if (!string.IsNullOrEmpty(LoseMusicName) && !isZoneTransitioning)
			{
				EventManager.Instance.PostEvent(MusicEventName, EventAction.SetSwitch, LoseMusicName, createCoMusicTarget);
				isPlayingLoseMusic = true;
			}
			if (!isZoneTransitioning)
			{
				EventManager.Instance.PostEvent(LoseSFXTrigger, EventAction.PlaySound, base.gameObject);
				isPlayingLoseSFXTrigger = true;
			}
			CoroutineRunner.Start(SwitchBackToDefautMusic(ReturnToEvergreenMusicWait), this, "SwitchToEvergreenMusic");
			if (isPlayingLowTimeSFX)
			{
				EventManager.Instance.PostEvent(LowTimeSFXTrigger, EventAction.StopSound, base.gameObject);
				isPlayingLowTimeSFX = false;
			}
			InvokeRepeating("displayGameStartCountdown", 0f, 1f);
			return false;
		}

		private IEnumerator SwitchBackToDefautMusic(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			EventManager.Instance.PostEvent(MusicEventName, EventAction.SetSwitch, "Evergreen", evergreenMusicTarget);
		}

		private IEnumerator resetScoreboardState(float delay)
		{
			yield return new WaitForSeconds(delay);
			for (int i = 0; i < SideScoreboards.Length; i++)
			{
				SideScoreboards[i].SetState(IslandTargetsSideScoreboard.SideScoreboardState.Off);
			}
		}

		private void animateRoofOpen()
		{
			if (currentRoofOpenAmount < targetRoofOpenAmount)
			{
				currentRoofOpenAmount += Time.deltaTime * RoofOpenSpeed;
				if (currentRoofOpenAmount > targetRoofOpenAmount)
				{
					currentRoofOpenAmount = targetRoofOpenAmount;
				}
				ClockTowerAnimator.Play("CrateCoRoofOpen", 0, currentRoofOpenAmount * 0.31f);
			}
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin)
			{
				isZoneTransitioning = true;
				if (isPlayingLoseMusic)
				{
					EventManager.Instance.PostEvent(MusicEventName, EventAction.StopSound, LoseMusicName, createCoMusicTarget);
				}
				if (isPlayingLoseSFXTrigger)
				{
					EventManager.Instance.PostEvent(LoseSFXTrigger, EventAction.StopSound, base.gameObject);
				}
			}
			return false;
		}

		private bool onStatsUpdated(IslandTargetsEvents.StatsUpdated evt)
		{
			WinStreakText.SetText(evt.CurrentWinStreak.ToString());
			DailyRecordText.SetText(evt.BestWinStreak.ToString());
			return false;
		}

		private IEnumerator showRewards()
		{
			yield return new WaitForSeconds(RewardDelay);
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimDelayedReward(RewardSource.MINI_GAME, "Targets"));
			FinishCamera.SetActive(false);
		}

		private bool onRewardsEarned(RewardServiceEvents.RewardsEarned evt)
		{
			if (evt.RewardedUsers != null && evt.RewardedUsers.source == RewardSource.MINI_GAME && evt.RewardedUsers.sourceId == "Targets")
			{
				List<long> list = new List<long>();
				foreach (KeyValuePair<long, Reward> reward in evt.RewardedUsers.rewards)
				{
					long key = reward.Key;
					list.Add(reward.Key);
					if (!string.IsNullOrEmpty(ParticipationPlayerPrefName) && !isQuestActive())
					{
						DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(key);
						if (!dataEntityHandle.IsNull && key == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId)
						{
							RewardedForLocalParticipation();
						}
					}
				}
				SceneRefs.CelebrationRunner.PlayCelebrationAnimation(list);
				UnityEngine.Object.Instantiate(RewardEffectsPrefab, RewardEffectsPosition.position, Quaternion.identity);
			}
			return false;
		}

		private void RewardedForLocalParticipation()
		{
			if (!string.IsNullOrEmpty(ParticipationPlayerPrefName))
			{
				PlayerPrefs.SetInt(ParticipationPlayerPrefName, 1);
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(IslandTargetsEvents.LocalPlayerParticipated));
		}

		private IEnumerator setScarecrowVisible(bool isVisible, float waitTime = 0f)
		{
			if (!string.IsNullOrEmpty(NormalScarecrowName))
			{
				yield return new WaitForSeconds(waitTime);
				if (normalScarecrowObj == null)
				{
					normalScarecrowObj = GameObject.Find(NormalScarecrowName);
				}
				if (normalScarecrowObj != null)
				{
					normalScarecrowObj.SetActive(isVisible);
					if (isVisible && !string.IsNullOrEmpty(ScarecrowAppearTrigger))
					{
						Animator component = normalScarecrowObj.GetComponent<Animator>();
						if (component != null)
						{
							component.ResetTrigger(ScarecrowAppearTrigger);
							component.SetTrigger(ScarecrowAppearTrigger);
						}
					}
				}
			}
			if (ClockCountdown != null)
			{
				ClockCountdown.SetActive(isVisible);
			}
		}

		private IEnumerator setClockTimerVisible(bool isVisible, float waitTime = 0f)
		{
			yield return new WaitForSeconds(waitTime);
			ClockTimer.SetActive(isVisible);
		}

		private bool isQuestActive()
		{
			return Service.Get<QuestService>().ActiveQuest != null;
		}
	}
}
