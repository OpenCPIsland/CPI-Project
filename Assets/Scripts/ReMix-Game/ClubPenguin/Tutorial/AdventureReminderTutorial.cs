using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Tutorial
{
	public static class AdventureReminderTutorial
	{
		private const int TWENTY_FOUR_HOURS_IN_SECONDS = 86400;

		private const int MIN_NUM_PLAYING_DAYS_FOR_REMINDER = 3;

		private const string ADVENTURE_REMINDER_TUTORIAL_SHOWN_KEY_PREFIX = "adventure_reminder_tutorial_shown_";

		private const string AVAILABLE_ADVENTURE_EPOCH_TIMESTAMP_KEY_PREFIX = "available_adventure_timestamp_";

		private const string NUM_PLAYING_DAYS_WITH_AVAILABLE_ADVENTURE_KEY_PREFIX = "num_playing_days_with_available_adventure_";

		public static void SetAvailableAdventureTimestamp(string mascotName)
		{
			if (Service.Get<GameStateController>().IsFTUEComplete && !PlayerPrefs.HasKey("available_adventure_timestamp_" + mascotName) && PlayerPrefs.GetInt("adventure_reminder_tutorial_shown_" + mascotName) != 1)
			{
				PlayerPrefs.SetString("available_adventure_timestamp_" + mascotName, GetCurrentTimeInEpoch().ToString());
			}
		}

		public static void SetNumPlayingDaysWithAvailableAdventure(string mascotName)
		{
			long result;
			if (Service.Get<GameStateController>().IsFTUEComplete && PlayerPrefs.GetInt("adventure_reminder_tutorial_shown_" + mascotName) != 1 && PlayerPrefs.HasKey("available_adventure_timestamp_" + mascotName) && long.TryParse(PlayerPrefs.GetString("available_adventure_timestamp_" + mascotName), out result))
			{
				float value = (float)(GetCurrentTimeInEpoch() - result) / 86400f;
				PlayerPrefs.SetFloat("num_playing_days_with_available_adventure_" + mascotName, value);
			}
		}

		public static void SetReminderTutorialShown(string mascotName)
		{
			PlayerPrefs.SetInt("adventure_reminder_tutorial_shown_" + mascotName, 1);
		}

		public static bool IsReminderTutorialShown(string mascotName)
		{
			return PlayerPrefs.GetInt("adventure_reminder_tutorial_shown_" + mascotName) == 1;
		}

		public static bool IsReminderOn(string mascotName)
		{
			return PlayerPrefs.GetFloat("num_playing_days_with_available_adventure_" + mascotName) >= 3f;
		}

		public static void ClearReminderCount(string mascotName)
		{
			PlayerPrefs.DeleteKey("available_adventure_timestamp_" + mascotName);
			PlayerPrefs.DeleteKey("num_playing_days_with_available_adventure_" + mascotName);
		}

		public static long GetCurrentTimeInEpoch()
		{
			return Service.Get<ContentSchedulerService>().ScheduledEventDate().GetTimeInSeconds();
		}
	}
}
