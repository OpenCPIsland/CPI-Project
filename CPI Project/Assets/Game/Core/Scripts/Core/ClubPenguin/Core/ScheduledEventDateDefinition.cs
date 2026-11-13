using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/ScheduledEventDate")]
	public class ScheduledEventDateDefinition : StaticGameDataDefinition
	{
		[Serializable]
		public struct DateRange
		{
			public DateUnityWrapper StartDate;

			public DateUnityWrapper EndDate;
		}

		[StaticGameDataDefinitionId]
		public int Id;

		[Tooltip("Both dates are set to 00:00. If you want something available on the 1st, set the end date to the 2nd.")]
		[Header("Both dates are set to 00:00. If you want something available on the 1st, set the end date to the 2nd.")]
		public DateRange Dates;
	}
}
