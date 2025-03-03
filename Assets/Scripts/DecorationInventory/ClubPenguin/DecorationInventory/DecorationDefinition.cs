using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	[CreateAssetMenu(menuName = "Definition/Igloo/Decoration")]
	public class DecorationDefinition : IglooAssetDefinition<int>
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public int Cost;

		[LocalizationToken]
		[Tooltip("The full description as presented to users")]
		public string Description;

		public TagDefinitionKey[] Tags;

		[Header("The prefab containing the colliders and other unity properties for this item when it's placed in a scene")]
		public PrefabContentKey Prefab;

		[Tooltip("The rules for manipulating this item")]
		public CollisionRuleSetDefinitionKey RuleSet;

		[Header("Category used for sorting")]
		public DecorationCategoryDefinitionDefinitionKey CategoryKey;

		public DecorationRenderDataContentKey RenderData;

		public float MaxScale = 1f;

		public float MinScale = 1f;

		public int MaxRotation = 0;

		public int MinRotation = 0;

		[Tooltip("The number of items considered a group. Default is 1, a pair is 2, triplets 3, etc.")]
		public int GroupSize = 1;

		public override int GetId()
		{
			return Id;
		}
	}
}
