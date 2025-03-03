using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class AnalogClockController : MonoBehaviour
	{
		private const float UPDATE_TIME = 1f;

		private const float ROTATE_TIME = 0.8f;

		public GameObject MinuteHand;

		public GameObject HourHand;

		public bool InvertRotation;

		public bool AutoStart = true;

		private bool isRunning;

		private DateTime currentClockTime;

		private float timeSinceUpdate;

		private Vector3 MinuteHandStartRotation;

		private Vector3 HourHandStartRotation;

		public bool IsRunning
		{
			get
			{
				return isRunning;
			}
		}

		public DateTime CurrentClockTime
		{
			get
			{
				return currentClockTime;
			}
		}

		private void Awake()
		{
			MinuteHandStartRotation = MinuteHand.transform.rotation.eulerAngles;
			HourHandStartRotation = HourHand.transform.rotation.eulerAngles;
			if (AutoStart)
			{
				StartClock(false);
			}
		}

		private void OnEnable()
		{
			if (isRunning)
			{
				StartClock(false);
			}
		}

		private void Update()
		{
			if (isRunning)
			{
				timeSinceUpdate += Time.deltaTime;
				if (timeSinceUpdate > 1f)
				{
					currentClockTime = currentClockTime.AddSeconds(timeSinceUpdate);
					timeSinceUpdate = 0f;
					SetClockTime(currentClockTime);
				}
			}
		}

		public void StartClock(bool animateToTime = true)
		{
			StartClock(Service.Get<ContentSchedulerService>().PresentTime(), animateToTime);
		}

		public void StartClock(DateTime startTime, bool animateToTime = true)
		{
			currentClockTime = startTime;
			if (!animateToTime)
			{
				SetClockTime(currentClockTime);
			}
			else
			{
				iTween.RotateTo(MinuteHand, iTween.Hash("rotation", getMinuteRotation(currentClockTime).eulerAngles, "time", 0.8f, "oncomplete", "onRotateAnimComplete", "onompletetarget", base.gameObject));
				iTween.RotateTo(HourHand, iTween.Hash("rotation", getHourRotation(currentClockTime).eulerAngles, "time", 0.8f));
			}
			isRunning = true;
		}

		private void onRotateAnimComplete()
		{
			isRunning = true;
		}

		public void StopClock()
		{
			isRunning = false;
		}

		public void SetClockTime(DateTime time)
		{
			MinuteHand.transform.rotation = getMinuteRotation(time);
			HourHand.transform.rotation = getHourRotation(time);
		}

		private Quaternion getMinuteRotation(DateTime time)
		{
			float num = (float)(time.Minute * 60 + time.Second) / 3600f;
			if (InvertRotation)
			{
				num = 0f - num;
			}
			return Quaternion.Euler(MinuteHandStartRotation.x, MinuteHandStartRotation.y, num * 360f);
		}

		private Quaternion getHourRotation(DateTime time)
		{
			float num = (float)(time.Hour * 3600 + time.Minute * 60 + time.Second) / 43200f;
			if (InvertRotation)
			{
				num = 0f - num;
			}
			return Quaternion.Euler(HourHandStartRotation.x, HourHandStartRotation.y, num * 360f);
		}
	}
}
