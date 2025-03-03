using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class ContentSchedulerService
	{
		private bool lockDate = false;

		private DateTime currentDate;

		private List<DateTime> days;

		private ScheduledEventDateDefinition supported;

		public int FallbackTimeOffsetHours
		{
			get;
			private set;
		}

		public bool QADateOverride
		{
			get
			{
				return lockDate;
			}
		}

		public ContentSchedulerService(ICollection<DateTime> days, int fallbackTimeOffsetHours, ScheduledEventDateDefinition supported)
		{
			this.days = new List<DateTime>(days);
			FallbackTimeOffsetHours = fallbackTimeOffsetHours;
			this.supported = supported;
			currentDate = today();
		}

		public DateTime RefreshContentDate()
		{
			if (!lockDate)
			{
				currentDate = today();
			}
			return currentDate;
		}

		public DateTime CurrentContentDate()
		{
			return currentDate;
		}

		public DateTime ScheduledEventDate()
		{
			if (QADateOverride)
			{
				return CurrentContentDate() + PresentTime().TimeOfDay;
			}
			return PresentTime();
		}

		public bool IsDuringScheduleEventDates(ScheduledEventDateDefinitionKey scheduledEventDateDefinitionKey)
		{
			ScheduledEventDateDefinition definitionById = Service.Get<IGameData>().GetDefinitionById(scheduledEventDateDefinitionKey);
			return IsDuringScheduleEventDates(definitionById);
		}

		public bool IsAfterScheduleEventDates(ScheduledEventDateDefinitionKey scheduledEventDateDefinitionKey)
		{
			ScheduledEventDateDefinition definitionById = Service.Get<IGameData>().GetDefinitionById(scheduledEventDateDefinitionKey);
			return IsAfterScheduleEventDates(definitionById);
		}

		public bool IsDuringScheduleEventDates(ScheduledEventDateDefinition scheduledEventDatedDef)
		{
			if (scheduledEventDatedDef != null)
			{
				DateTime target = ScheduledEventDate();
				DateUnityWrapper startDate = scheduledEventDatedDef.Dates.StartDate;
				DateUnityWrapper endDate = scheduledEventDatedDef.Dates.EndDate;
				if (DateTimeUtils.DoesDateFallBetween(target, startDate, endDate))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsBeforeScheduleEventDates(ScheduledEventDateDefinition scheduledEventDatedDef)
		{
			if (scheduledEventDatedDef != null)
			{
				DateTime target = ScheduledEventDate();
				DateUnityWrapper startDate = scheduledEventDatedDef.Dates.StartDate;
				if (DateTimeUtils.IsBefore(target, startDate))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAfterScheduleEventDates(ScheduledEventDateDefinition scheduledEventDatedDef)
		{
			if (scheduledEventDatedDef != null)
			{
				DateTime target = ScheduledEventDate();
				DateUnityWrapper endDate = scheduledEventDatedDef.Dates.EndDate;
				if (DateTimeUtils.IsAfter(target, endDate))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasSupportEndded()
		{
			return IsAfterScheduleEventDates(supported);
		}

		private DateTime today()
		{
			if (days.Count == 0)
			{
				return default(DateTime);
			}
			DateTime date = PresentTime().Date;
			if (days.Contains(date))
			{
				return date;
			}
			days.Sort();
			DateTime d = days[0];
			int index = Mathf.Abs((date - d).Days) % days.Count;
			return days[index];
		}

		public DateTime PresentTime()
		{
			if (Service.IsSet<INetworkServicesManager>())
			{
				INetworkServicesManager networkServicesManager = Service.Get<INetworkServicesManager>();
				if (networkServicesManager.GameTimeMilliseconds > 0)
				{
					return networkServicesManager.ServerDateTime;
				}
			}
			if (DateTime.Now.IsDaylightSavingTime())
			{
				return DateTime.UtcNow.AddHours(FallbackTimeOffsetHours + 1);
			}
			return DateTime.UtcNow.AddHours(FallbackTimeOffsetHours);
		}
	}
}
