using ClubPenguin;
using ClubPenguin.Adventure;
using Disney.Kelowna.Common.Utils;
using Disney.MobileNetwork;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Scheduled Events")]
	[Tooltip("Test if date matches a cron expression. >> Do not use this in a loop <<")]
	public class ScheduleEventCommand : FsmStateAction
	{
		public enum ClockTimeSource
		{
			Client,
			Server,
			ScheduledEvent
		}

		public enum TimeZoneAbbreviations
		{
			PST,
			EST,
			UTC
		}

		[Note("Use a cron expression")]
		[RequiredField]
		public FsmString Schedule;

		public TimeZoneAbbreviations ClockTimeZone;

		public ClockTimeSource ClockSource;

		public FsmEvent FinishEventTrue;

		public FsmEvent FinishEventFalse;

		public bool hideWhenQuestActive = true;

		public override void Reset()
		{
			Schedule = null;
			FinishEventTrue = null;
			FinishEventFalse = null;
		}

		public override void OnUpdate()
		{
			CheckDateAgainstSchedule();
		}

		public void CheckDateAgainstSchedule()
		{
			DateTime presentTime = getPresentTime();
			string value = Schedule.Value;
			if (!string.IsNullOrEmpty(value))
			{
				if (shouldBeVisible() && CronExpressionEvaluator.EvaluatesTrue(presentTime, value))
				{
					if (FinishEventTrue != null)
					{
						base.Fsm.Event(FinishEventTrue);
					}
				}
				else if (FinishEventFalse != null)
				{
					base.Fsm.Event(FinishEventFalse);
				}
			}
			if (FinishEventTrue == null && FinishEventFalse == null)
			{
			}
			Finish();
		}

		private DateTime getPresentTime()
		{
			DateTime dateTime = DateTime.UtcNow;
			if (ClockSource == ClockTimeSource.Server)
			{
				dateTime = Service.Get<ContentSchedulerService>().PresentTime();
			}
			else if (ClockSource == ClockTimeSource.ScheduledEvent)
			{
				dateTime = Service.Get<ContentSchedulerService>().ScheduledEventDate();
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

		private bool shouldBeVisible()
		{
			if (hideWhenQuestActive)
			{
				return Service.Get<QuestService>().ActiveQuest == null;
			}
			return true;
		}
	}
}
