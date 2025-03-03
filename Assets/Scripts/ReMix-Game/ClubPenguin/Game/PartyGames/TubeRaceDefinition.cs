using ClubPenguin.PartyGames;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[Serializable]
	[JsonConverter(typeof(PartyGameDataDefinitionJsonConverter))]
	public class TubeRaceDefinition : PartyGameDataDefinition
	{
		public float MinimumCompletionTimeInSeconds;

		public int MaximumCompletionTimeInSeconds;

		public float MaxValidationRadius;

		public Vector3 RaceEndPosition;

		[Tooltip("Score given to players caught warping to finish, or have not finished within MaximumCompletionTimeInSeconds.")]
		public int DefaultPoints;

		public int ShowRewardsDelayInSeconds;

		public PartyGameEndPlacement SinglePlayerPlacement;

		public int StartingPoints;

		public int PointsDeductedPerSecond;

		public Vector3 RaceRestartPosition;

		public string QuestEventIdentifier;
	}
}
