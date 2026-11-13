using ClubPenguin.Core;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneScheduledLocationActivityDefinition : CellPhoneLocationActivityDefinition, ICellPhoneScheduledActivityDefinition
	{
		public ScheduledEventDateDefinition AvailableDates;

		public DateUnityWrapper GetStartingDate()
		{
			return AvailableDates.Dates.StartDate;
		}

		public DateUnityWrapper GetEndingDate()
		{
			return AvailableDates.Dates.EndDate;
		}
	}
}
