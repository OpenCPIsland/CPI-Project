using DevonLocalization;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class LocalizedEventDateText : LocalizedText
	{
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
