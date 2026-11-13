using ClubPenguin.Core;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneClaimableRewardActivityDefinition : CellPhoneActivityDefinition, ICellPhoneScheduledActivityDefinition
	{
		public ClaimableRewardDefinition ClaimableReward;

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
