using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(CountdownTimer))]
	public class AllAccessCountdownTimer : MonoBehaviour
	{
		public string FormatToken;

		private string formatString;

		private CountdownTimer countdownTimer;

		private void OnEnable()
		{
			AllAccessService allAccessService = Service.Get<AllAccessService>();
			if (allAccessService.IsAllAccessActive())
			{
				formatString = Service.Get<Localizer>().GetTokenTranslation(FormatToken);
				TimeSpan remainingTime = allAccessService.GetRemainingTime();
				countdownTimer = GetComponent<CountdownTimer>();
				countdownTimer.Format = format;
				countdownTimer.StartTimer(remainingTime);
			}
		}

		public void OnDisable()
		{
			if (countdownTimer != null)
			{
				countdownTimer.StopTimer();
			}
		}

		private string format(TimeSpan countdownTime)
		{
			return string.Format(formatString, countdownTime.Days, countdownTime.Hours, countdownTime.Minutes, countdownTime.Seconds);
		}
	}
}
