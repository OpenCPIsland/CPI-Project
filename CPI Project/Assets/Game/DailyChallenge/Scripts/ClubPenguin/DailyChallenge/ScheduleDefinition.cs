using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.DailyChallenge
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Schedule")]
	public class ScheduleDefinition : StaticGameDataDefinition
	{
		[TextArea]
		public string Description;

		public DailyChallengeDefinitionContentKey[] Assets;
	}
}
