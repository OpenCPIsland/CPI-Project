using ClubPenguin.PartyGames;
using Newtonsoft.Json;
using System;

namespace ClubPenguin.Game.PartyGames
{
	[Serializable]
	[JsonConverter(typeof(PartyGameDataDefinitionJsonConverter))]
	public class FishBucketDefinition : PartyGameDataDefinition
	{
		[Serializable]
		public struct FishBucketDeck
		{
			[Serializable]
			public struct FishBucketCard
			{
				public int Quantity;

				public int FishDelta;
			}

			public FishBucketCard[] Cards;

			public int MinPlayerCount;

			public int MaxPlayerCount;
		}

		public int IntroTimeInSeconds;

		public int TurnTimeInSeconds;

		public int NextTurnDelayFishInSeconds;

		public int NextTurnDelaySquidInSeconds;

		public int PlayerProp;

		public FishBucketDeck[] CardDecks;

		public float MaxGameStartDistance;
	}
}
