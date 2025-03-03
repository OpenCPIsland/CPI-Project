using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneRecurringLocationActivityDefinition : CellPhoneScheduledLocationActivityDefinition
	{
		public string ActivityStartScheduleCron;

		public string ShowWidgetScheduleCron;
	}
}
