using ClubPenguin.Core.StaticGameData;
using System;

namespace ClubPenguin.MiniGames.Fishing
{
	[Serializable]
	public class LootTableBucketDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string BucketName;

		public int Weight;
	}
}
