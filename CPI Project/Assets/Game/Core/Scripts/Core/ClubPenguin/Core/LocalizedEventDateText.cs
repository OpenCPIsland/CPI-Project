using DevonLocalization;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ClubPenguin.Core
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocalizedEventDateText : LocalizedTextMeshProUGUI
	{
		private static int forcedEventId = -1;

		public static void SetForcedEventId(int eventId)
		{
			forcedEventId = eventId;
		}

		[Serializable]
		private struct DateOptions
		{
			public bool UseYear;

			public bool UseMonth;

			public bool UseDay;

			public bool SubtractDay;
		}

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		public DateType DateType;

		[SerializeField]
		private DateOptions startDateOptions;

		[SerializeField]
		private DateOptions endDateOptions;

		protected override void setText(string text)
		{
			Dictionary<int, ScheduledEventDateDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>();
			ScheduledEventDateDefinition value;
			if (dictionary.TryGetValue(DateDefinitionKey.Id, out value))
			{
				if (DateType == DateType.Both && forcedEventId == DateDefinitionKey.Id)
				{
					DateTime forcedEndDate = adjustedDate(value.Dates.EndDate.Date, endDateOptions);
					string endDateFormat = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Prompts.EndDate");
					base.setText(string.Format(endDateFormat, forcedEndDate.GetLocalizedMonth(), forcedEndDate.Day.ToString()));
					return;
				}
				if (DateType == DateType.Both && (token == "GlobalUI.Prompts.1MonthDate" || token == "GlobalUI.Prompts.2MonthDate"))
				{
					DateTime startDate = adjustedDate(value.Dates.StartDate.Date, startDateOptions);
					DateTime endDate = adjustedDate(value.Dates.EndDate.Date, endDateOptions);
					bool spansMonths = startDate.Month != endDate.Month || startDate.Year != endDate.Year;
					string dateToken = spansMonths ? "GlobalUI.Prompts.2MonthDate" : "GlobalUI.Prompts.1MonthDate";
					string dateFormat = Service.Get<Localizer>().GetTokenTranslation(dateToken);
					string[] dateParts = spansMonths
						? new string[] { startDate.GetLocalizedMonth(), startDate.Day.ToString(), endDate.GetLocalizedMonth(), endDate.Day.ToString() }
						: new string[] { startDate.GetLocalizedMonth(), startDate.Day.ToString(), endDate.Day.ToString() };
					base.setText(string.Format(dateFormat, dateParts));
					return;
				}
				List<string> list = new List<string>();
				switch (DateType)
				{
				case DateType.StartDate:
					addDateParts(value.Dates.StartDate, list, startDateOptions);
					break;
				case DateType.EndDate:
					addDateParts(value.Dates.EndDate, list, endDateOptions);
					break;
				case DateType.Both:
					addDateParts(value.Dates.StartDate, list, startDateOptions);
					addDateParts(value.Dates.EndDate, list, endDateOptions);
					break;
				}
				string text2 = string.Format(text, list.ToArray());
				base.setText(text2);
			}
			else
			{
				base.setText(string.Empty);
				Log.LogErrorFormatted(this, "Could not find ScheduledEventDateDefinition for id {0}", DateDefinitionKey.Id);
			}
		}

		private DateTime adjustedDate(DateTime date, DateOptions options)
		{
			return options.SubtractDay ? date.AddDays(-1.0) : date;
		}

		private void addDateParts(DateUnityWrapper date, List<string> dateParts, DateOptions dateOptions)
		{
			if (dateOptions.UseYear)
			{
				dateParts.Add(date.Date.Year.ToString());
			}
			if (dateOptions.UseMonth)
			{
				dateParts.Add(date.Date.GetLocalizedMonth());
			}
			if (dateOptions.UseDay)
			{
				string item = (!dateOptions.SubtractDay) ? date.Date.Day.ToString() : date.Date.AddDays(-1.0).Day.ToString();
				dateParts.Add(item);
			}
		}
	}
}
