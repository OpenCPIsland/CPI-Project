using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[CreateAssetMenu(menuName = "Definition/LoginZoneDefinition")]
	public class LoginZoneDefinition : StaticGameDataDefinition
	{
		public enum ConditionType
		{
			PlayerPrefExist = 1,
			PlayerPrefDoesNotExist
		}

		[Serializable]
		public struct LoginZoneCondition
		{
			public ConditionType TypeOfCondition;

			public string PlayerPrefsKey;

			public bool AddPlayerNameToKey;

			public ScheduledEventDateDefinitionKey DateDefinitionKey;
		}

		[Serializable]
		public struct CompositeLoginZoneCondition
		{
			public LoginZoneCondition[] ANDConditions;
		}

		public ZoneDefinitionKey Zone;

		public ScheduledEventDateDefinitionKey ScheduledEventDateKey;

		[Header("If ANY of each element are met, then player will be sent to the specified zone.")]
		[Header("The AND Conditions means all conditions in that group must be met for that group to be met.")]
		public CompositeLoginZoneCondition[] ANYConditions;
	}
}
