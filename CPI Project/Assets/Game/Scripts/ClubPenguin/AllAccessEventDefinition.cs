using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/AllAccessEventDefinition")]
	public class AllAccessEventDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		public bool ApplyToGoogleUsers;

		public bool ApplyToAppleUsers;

		public override string ToString()
		{
			ScheduledEventDateDefinition scheduledEventDateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[DateDefinitionKey.Id];
			return string.Format("[AllAccessEventDefinition] Id: {0}, Start: {1}, End: {2}, Google={3}, Apple={4}", Id, scheduledEventDateDefinition.Dates.StartDate, scheduledEventDateDefinition.Dates.EndDate, ApplyToGoogleUsers, ApplyToAppleUsers);
		}
	}
}
