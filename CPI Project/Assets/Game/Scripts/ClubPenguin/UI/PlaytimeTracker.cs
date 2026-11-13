using DevonLocalization;
using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PlaytimeTracker : MonoBehaviour
	{
		public TMP_Text PlaytimeText;

		private LocalizedTextMeshProUGUI localizedText;

		private void OnEnable()
		{
			if (PlaytimeText == null)
			{
				PlaytimeText = GetComponentInChildren<TMP_Text>();
			}
			localizedText = PlaytimeText != null ? PlaytimeText.GetComponent<LocalizedTextMeshProUGUI>() : null;
			if (localizedText != null)
			{
				localizedText.OnUpdateToken += OnLocalizedTextUpdated;
			}

			if (PlaytimeText != null)
			{
				UpdateDisplay();
				return;
			}

			UpdateDisplay();
		}

		private void Update()
		{
			UpdateDisplay();
		}

		private void OnDisable()
		{
			if (localizedText != null)
			{
				localizedText.OnUpdateToken -= OnLocalizedTextUpdated;
			}
		}


		private void UpdateDisplay()
		{
			if (PlaytimeText == null)
			{
				return;
			}

			long totalSeconds = PlaytimeService.Instance != null ? PlaytimeService.Instance.GetCurrentTotalSeconds() : 0L;
			string data = FormatPlaytime(totalSeconds);
			if (localizedText != null && !string.IsNullOrEmpty(localizedText.TranslatedText))
			{
				PlaytimeText.text = localizedText.TranslatedText.Replace("{Data}", data);
			}
		}

		private void OnLocalizedTextUpdated(string translatedText)
		{
			long totalSeconds = PlaytimeService.Instance != null ? PlaytimeService.Instance.GetCurrentTotalSeconds() : 0L;
			PlaytimeText.text = translatedText.Replace("{Data}", FormatPlaytime(totalSeconds));
		}

		private static string FormatPlaytime(long totalSeconds)
		{
			TimeSpan playtime = TimeSpan.FromSeconds(totalSeconds);
			long remainingDays = (long)playtime.TotalDays;
			long years = remainingDays / 365L;
			remainingDays %= 365L;
			long months = remainingDays / 30L;
			remainingDays %= 30L;

			StringBuilder result = new StringBuilder();
			AppendUnit(result, years, "y");
			AppendUnit(result, months, "mo");
			AppendUnit(result, remainingDays, "d");
			AppendUnit(result, playtime.Hours, "h");
			AppendUnit(result, playtime.Minutes, "m");
			AppendUnit(result, playtime.Seconds, "s");
			return result.Length > 0 ? result.ToString() : "0s";
		}

		private static void AppendUnit(StringBuilder result, long value, string unit)
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
	}
}
