using ClubPenguin.SledRacer;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SledRaceLeaderboard : MonoBehaviour
	{
		public enum ScreenState
		{
			Default,
			Finish
		}

		public GameObject DefaultScreen;

		public GameObject FinishScreen;

		public TextMesh TimeText;

		public TextMesh TimeTextShadow;

		public GameObject[] TrophyIcons;

		public float ScoreboardDuration = 20f;

		private ScreenState currentState;

		private Timer scoreboardTimer;

		private void Start()
		{
			scoreboardTimer = new Timer(ScoreboardDuration, false, delegate
			{
				onTimerComplete();
			});
			setState(ScreenState.Default);
		}

		private void setState(ScreenState state)
		{
			switch (state)
			{
			case ScreenState.Default:
				TimeText.gameObject.SetActive(false);
				TimeTextShadow.gameObject.SetActive(false);
				DefaultScreen.SetActive(true);
				FinishScreen.SetActive(false);
				break;
			case ScreenState.Finish:
				TimeText.gameObject.SetActive(true);
				TimeTextShadow.gameObject.SetActive(true);
				DefaultScreen.SetActive(false);
				FinishScreen.SetActive(true);
				break;
			}
		}

		public void ShowFinishScreen(RaceResults raceResults)
		{
			setTime(raceResults.CompletionTime);
			setState(ScreenState.Finish);
			long num = raceResults.CompletionTime - raceResults.StartTime;
			string text = default(DateTime).AddMilliseconds(num).ToString("m:ss.ff");
			TimeText.text = text;
			TimeTextShadow.text = text;
			int num2 = (int)(raceResults.raceResultsCategory - 1);
			for (int i = 0; i < TrophyIcons.Length; i++)
			{
				if (i == num2)
				{
					TrophyIcons[i].SetActive(true);
				}
				else
				{
					TrophyIcons[i].SetActive(false);
				}
			}
			CoroutineRunner.Start(scoreboardTimer.Start(), this, "ScoreboardTimer");
		}

		private void setTime(long time)
		{
			TimeSpan time2 = new TimeSpan(time);
			TimeText.text = TimeFormatUtils.FormatTimeString(time2);
		}

		private void onTimerComplete()
		{
			setState(ScreenState.Default);
		}
	}
}
