using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.DailyChallenge
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/DailyChallengeSchedule")]
	public class DailyChallengeScheduleDefinition : StaticGameDataDefinition
	{
		public const string SCHEDULE_ASSET_FILENAME = "Schedule";

		[TextArea]
		public string Description;

		public DailyChallengeDefinitionContentKey[] Assets;

		public string CreationSettings;
	}
}
