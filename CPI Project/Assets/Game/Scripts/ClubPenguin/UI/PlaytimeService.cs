using ClubPenguin.Net.Client;
using ClubPenguin.Net.Offline;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PlaytimeService : MonoBehaviour
	{
		private const float SAVE_INTERVAL_SECONDS = 1f;

		private static PlaytimeService instance;

		private string accountToken;
		private PlayTimeData playTimeData;
		private float sessionSeconds;
		private float lastSaveTime;
		private float lastSampleTime;
		private bool isTracking;

		public static PlaytimeService Instance
		{
			get { return instance; }
		}

		private void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			EnsureAccountBinding();
			if (!isTracking)
			{
				return;
			}

			AccumulateElapsedTime();
			if (Time.unscaledTime - lastSaveTime >= SAVE_INTERVAL_SECONDS)
			{
				Flush();
			}
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				Flush();
			}
			else
			{
				lastSaveTime = Time.unscaledTime;
			}
		}

		private void OnApplicationQuit()
		{
			Flush();
		}

		public long GetCurrentTotalSeconds()
		{
			EnsureAccountBinding();
			if (!isTracking)
			{
				return 0L;
			}

			AccumulateElapsedTime();
			return playTimeData.TotalSeconds + (long)sessionSeconds;
		}

		public static string FormatPlaytime(long totalSeconds)
		{
			System.TimeSpan playtime = System.TimeSpan.FromSeconds(totalSeconds);
			long remainingDays = (long)playtime.TotalDays;
			long years = remainingDays / 365L;
			remainingDays %= 365L;
			long months = remainingDays / 30L;
			remainingDays %= 30L;

			System.Text.StringBuilder result = new System.Text.StringBuilder();
			AppendPlaytimeUnit(result, years, "y");
			AppendPlaytimeUnit(result, months, "mo");
			AppendPlaytimeUnit(result, remainingDays, "d");
			AppendPlaytimeUnit(result, playtime.Hours, "h");
			AppendPlaytimeUnit(result, playtime.Minutes, "m");
			AppendPlaytimeUnit(result, playtime.Seconds, "s");
			return result.Length > 0 ? result.ToString() : "0s";
		}

		private static void AppendPlaytimeUnit(System.Text.StringBuilder result, long value, string unit)
		{
			if (value <= 0L)
			{
				return;
			}

			if (result.Length > 0)
			{
				result.Append(' ');
			}
			result.Append(value).Append(unit);
		}

		public void Flush()
		{
			if (!isTracking || string.IsNullOrEmpty(accountToken))
			{
				return;
			}

			long elapsedSeconds = (long)sessionSeconds;
			if (elapsedSeconds <= 0L)
			{
				return;
			}

			playTimeData.TotalSeconds += elapsedSeconds;
			sessionSeconds -= elapsedSeconds;
			OfflineDatabase.Write(playTimeData, accountToken);
			lastSaveTime = Time.unscaledTime;
		}

		private void EnsureAccountBinding()
		{
			if (!Service.IsSet<OfflineDatabase>())
			{
				return;
			}

			string currentAccountToken = Service.Get<OfflineDatabase>().AccessToken;
			if (string.IsNullOrEmpty(currentAccountToken))
			{
				if (isTracking)
				{
					Flush();
					isTracking = false;
					accountToken = null;
					sessionSeconds = 0f;
				}
				return;
			}

			if (isTracking && currentAccountToken == accountToken)
			{
				return;
			}

			if (isTracking)
			{
				Flush();
			}

			accountToken = currentAccountToken;
			playTimeData = OfflineDatabase.Read<PlayTimeData>(accountToken);
			sessionSeconds = 0f;
			lastSampleTime = Time.realtimeSinceStartup;
			lastSaveTime = Time.unscaledTime;
			isTracking = true;
		}

		private void AccumulateElapsedTime()
		{
			float now = Time.realtimeSinceStartup;
			sessionSeconds += Mathf.Max(0f, now - lastSampleTime);
			lastSampleTime = now;
		}
	}
}
