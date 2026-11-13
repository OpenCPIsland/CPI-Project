using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.MiniGames.Fishing
{
	[Serializable]
	public class LootTableRewardDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		[LocalizationToken]
		public string DisplayName;

		public LootTableBucketDefinition Bucket;

		public int Weight;

		public float TimingWindow;

		public PrefabContentKey ModelAsset;

		public PrefabContentKey ShadowAsset;

		public RewardDefinition Reward;

		public SpriteContentKey RewardImage;
	}
}
