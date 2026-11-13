using ClubPenguin;
using Disney.Kelowna.Common.Utils;
using Disney.MobileNetwork;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Misc")]
	[Tooltip("Waits for the specific time to trigger an event")]
	public class WaitForTimeCommand : FsmStateAction
	{
		public enum ClockTimeSource
		{
			Client,
			Server
		}

		public enum TimeZoneAbbreviations
		{
			PST,
			EST,
			UTC
		}

		[RequiredField]
		[Note("Use a cron expression")]
		public FsmString ClockTime;

		public TimeZoneAbbreviations ClockTimeZone;

		public ClockTimeSource ClockSource;

		public FsmEvent FinishEvent;

		public FsmInt NextTime;

		public FsmString CountdownText;

		private long serverTime;

		private DateTime nextDate;

		private string currentSchedule;

		private DateTime lastCheckTime;

		public override void Reset()
		{
			ClockTime = null;
			FinishEvent = null;
		}

		public override void OnEnter()
		{
			lastCheckTime = getPresentTime();
			if (isEventTime())
			{
				base.Fsm.Event(FinishEvent);
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (isEventTime())
			{
				Finish();
				if (FinishEvent != null)
				{
					base.Fsm.Event(FinishEvent);
				}
			}
		}

		public bool isEventTime()
		{
			bool flag = false;
			DateTime presentTime = getPresentTime();
			string value = ClockTime.Value;
			if (currentSchedule != value || nextDate <= presentTime)
			{
				flag = (lastCheckTime <= nextDate && nextDate <= presentTime);
				if (!flag)
				{
					flag = CronExpressionEvaluator.EvaluatesTrue(presentTime, value, out nextDate);
				}
				currentSchedule = value;
			}
			lastCheckTime = presentTime;
			TimeSpan timeSpan = nextDate.Subtract(presentTime);
			if (NextTime.Value != (int)timeSpan.TotalSeconds)
			{
				NextTime.Value = (int)timeSpan.TotalSeconds;
				CountdownText.Value = ((int)Math.Floor(timeSpan.TotalMinutes)).ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");
			}
			return flag;
		}

		private DateTime getPresentTime()
		{
			DateTime dateTime = DateTime.UtcNow;
			if (ClockSource == ClockTimeSource.Server)
			{
				dateTime = Service.Get<ContentSchedulerService>().PresentTime();
			}
			if (ClockTimeZone == TimeZoneAbbreviations.PST)
			{
				TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
				dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, timeZoneInfo.Id);
			}
			else if (ClockTimeZone == TimeZoneAbbreviations.EST)
			{
				TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
				dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, timeZoneInfo.Id);
			}
			return dateTime;
		}
	}
}
