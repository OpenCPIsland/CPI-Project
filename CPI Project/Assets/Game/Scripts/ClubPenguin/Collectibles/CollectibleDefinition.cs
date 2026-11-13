using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Collectibles
{
	[Serializable]
	public class CollectibleDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string CollectibleType;

		public SpawnCategory SpawnCategory;

		public float ExchangeRate;

		public int RespawnSeconds;

		public SpriteContentKey SpriteAsset;

		public string NameToken;
	}
}
