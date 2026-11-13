using ClubPenguin.PartyGames;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[Serializable]
	[JsonConverter(typeof(PartyGameDataDefinitionJsonConverter))]
	public class DanceBattleDefinition : PartyGameDataDefinition
	{
		[Serializable]
		public class DanceSequenceSet
		{
			public int SequenceDisplayTimeInSeconds;

			public int TurnTimeInSeconds;

			public int MinRound;

			public int MaxRound;

			public int[] DanceMoveIds;

			public int[] FillerDanceMoveIds;

			public int SequenceLength;

			public int TurnOutcomeSequenceDelayInSeconds;
		}

		public int NumberOfRounds;

		public int GameProp;

		public int TauntProp;

		public int IntroTimeInSeconds;

		public int OutroTimeInSeconds;

		public Vector3 GameLocation;

		public int TurnStartDelayInSeconds;

		public int TurnOutcomeStartDelayInSeconds;

		public int TurnOutcomeEndDelayInSeconds;

		public int FailureMoveId;

		public int SuccessMoveId;

		public DanceSequenceSet[] DanceSequenceSets;
	}
}
