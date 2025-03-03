using ClubPenguin.PartyGames;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceBanner : MonoBehaviour
	{
		private enum BannerState
		{
			Wait,
			RaceIn,
			Join,
			Ready,
			Set,
			Go
		}

		private const float WAIT_START_TIME = 1f;

		public TubeRaceLobbyMmoItemObserver MmoItemObserver;

		public CountdownTimer Timer;

		public PartyGameDefinition GameDefinition;

		public string StartPlatformAnimatorTrigger = "PlatformGODown";

		public GameObject WaitPanel;

		public GameObject RaceInPanel;

		public GameObject JoinRacePanel;

		public GameObject ReadyPanel;

		public GameObject SetPanel;

		public GameObject GoPanel;

		public Animator StartPlatformAnimator;

		public int SecondsForReadyState = 0;

		public int SecondsForJoinState = 30;

		public int SecondsForRaceInState = 60;

		private BannerState currentState;

		private void Awake()
		{
			Timer.Format = formatTimer;
			TubeRaceLobbyMmoItemObserver mmoItemObserver = MmoItemObserver;
			mmoItemObserver.LobbyStartedAction = (Action<long>)Delegate.Combine(mmoItemObserver.LobbyStartedAction, new Action<long>(onLobbyStarted));
			setState(BannerState.Wait);
		}

		private void setState(BannerState newState)
		{
			currentState = newState;
			WaitPanel.SetActive(newState == BannerState.Wait);
			RaceInPanel.SetActive(newState == BannerState.RaceIn);
			JoinRacePanel.SetActive(newState == BannerState.Join);
			ReadyPanel.SetActive(newState == BannerState.Ready);
			SetPanel.SetActive(newState == BannerState.Set);
			GoPanel.SetActive(newState == BannerState.Go);
			switch (newState)
			{
			case BannerState.RaceIn:
			case BannerState.Join:
				showCountdown();
				break;
			default:
				hideCountdown();
				break;
			}
		}

		private void showCountdown()
		{
			Timer.Text.enabled = true;
		}

		private void hideCountdown()
		{
			Timer.Text.enabled = false;
		}

		private void onLobbyStarted(long startTimestampInSeconds)
		{
			int seconds = (int)(startTimestampInSeconds - Service.Get<ContentSchedulerService>().PresentTime().GetTimeInSeconds());
			Timer.StopTimer(true);
			Timer.StartTimer(new TimeSpan(0, 0, seconds));
		}

		private string formatTimer(TimeSpan countdownTime)
		{
			handleTimerUpdate((int)countdownTime.TotalSeconds);
			return default(DateTime).Add(countdownTime).ToString("m:ss");
		}

		private void handleTimerUpdate(int totalSeconds)
		{
			if (totalSeconds <= SecondsForReadyState && currentState == BannerState.Join)
			{
				Timer.StopTimer();
				Service.Get<EventDispatcher>().DispatchEvent(default(TubeRaceEvents.CloseLobby));
				CoroutineRunner.Start(showRaceStart(), this, "showRaceStart");
			}
			else if (totalSeconds <= SecondsForJoinState && currentState == BannerState.RaceIn)
			{
				setState(BannerState.Join);
			}
			else if (totalSeconds <= SecondsForRaceInState && currentState == BannerState.Wait)
			{
				setState(BannerState.RaceIn);
			}
		}

		private IEnumerator showRaceStart()
		{
			setState(BannerState.Ready);
			yield return new WaitForSeconds(1f + (float)MmoItemObserver.BufferLobbyStartTime);
			WaitForSeconds wait = new WaitForSeconds(1f);
			setState(BannerState.Set);
			yield return wait;
			setState(BannerState.Go);
			StartPlatformAnimator.SetTrigger(StartPlatformAnimatorTrigger);
			yield return wait;
			setState(BannerState.Wait);
		}

		private void onDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			Timer.StopTimer();
			TubeRaceLobbyMmoItemObserver mmoItemObserver = MmoItemObserver;
			mmoItemObserver.LobbyStartedAction = (Action<long>)Delegate.Remove(mmoItemObserver.LobbyStartedAction, new Action<long>(onLobbyStarted));
		}
	}
}
