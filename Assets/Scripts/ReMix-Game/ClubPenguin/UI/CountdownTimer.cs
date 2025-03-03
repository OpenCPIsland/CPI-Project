using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CountdownTimer : MonoBehaviour
	{
		public delegate string FormatCountdownTime(TimeSpan countdownTime);

		public FormatCountdownTime Format;

		public Text Text;

		public TextMesh TextMesh;

		private TimeSpan countdownTime;

		public void StartTimer(TimeSpan countdownTime)
		{
			this.countdownTime = countdownTime;
			setText(Format(countdownTime));
			StartCoroutine(runCountdown());
		}

		public void StopTimer(bool reset = false)
		{
			StopAllCoroutines();
			if (reset)
			{
				setText(Format(default(TimeSpan)));
			}
		}

		private IEnumerator runCountdown()
		{
			TimeSpan oneSecond = TimeSpan.FromSeconds(1.0);
			while (countdownTime > TimeSpan.Zero)
			{
				yield return new WaitForSecondsRealtime(1f);
				countdownTime = countdownTime.Subtract(oneSecond);
				setText(Format(countdownTime));
			}
		}

		private void setText(string text)
		{
			if (Text != null)
			{
				Text.text = text;
			}
			else if (TextMesh != null)
			{
				TextMesh.text = text;
			}
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}
	}
}
