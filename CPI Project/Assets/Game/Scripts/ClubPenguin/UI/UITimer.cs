using Fabric;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class UITimer : MonoBehaviour
	{
		public Image TimerBar;

		public Text TimerText;

		public int LowTimerSeconds;

		public Color LowTimeColor;

		[Space(10f)]
		public string LowTimeSFXTrigger;

		private Color defaultColor;

		private float totalTime;

		private bool isRunning;

		private int prevSeconds;

		private DateTime timer;

		private void Awake()
		{
			if (TimerBar != null)
			{
				defaultColor = TimerBar.color;
			}
		}

		private void Update()
		{
			if (!isRunning)
			{
				return;
			}
			float num = (float)timer.Second + (float)timer.Millisecond / 1000f;
			if (timer.Second < LowTimerSeconds && timer.Second != prevSeconds)
			{
				if (!string.IsNullOrEmpty(LowTimeSFXTrigger))
				{
					EventManager.Instance.PostEvent(LowTimeSFXTrigger, EventAction.PlaySound);
				}
				if (TimerBar != null)
				{
					TimerBar.color = LowTimeColor;
				}
			}
			if (num > 0f)
			{
				float fillAmount = num / totalTime;
				if (TimerBar != null)
				{
					TimerBar.fillAmount = fillAmount;
				}
				if (TimerText != null)
				{
					DateTime dateTime = timer.AddSeconds(1.0);
					TimerText.text = dateTime.ToString("m:ss");
				}
			}
			else
			{
				if (TimerBar != null)
				{
					TimerBar.fillAmount = 0f;
				}
				if (TimerText != null)
				{
					TimerText.text = "0:00";
				}
				isRunning = false;
			}
			if (num > Time.deltaTime)
			{
				prevSeconds = timer.Second;
				timer = timer.AddSeconds(0f - Time.deltaTime);
			}
			else
			{
				timer = default(DateTime);
			}
		}

		public void StartCountdown(float seconds)
		{
			totalTime = seconds;
			isRunning = true;
			timer = default(DateTime);
			timer = timer.AddSeconds(seconds);
			if (TimerText != null)
			{
				TimerText.text = timer.ToString("m:ss");
			}
			if (TimerBar != null)
			{
				TimerBar.color = defaultColor;
			}
		}

		public void StopCountdown()
		{
			isRunning = false;
		}
	}
}
