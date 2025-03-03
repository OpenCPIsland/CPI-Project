using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	[CreateAssetMenu(menuName = "Definition/Igloo/Structure")]
	public class StructureDefinition : IglooAssetDefinition<int>
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public int Cost;

		[Range(1f, 3f)]
		public int SizeUnits;

		[LocalizationToken]
		[Tooltip("The full description as presented to users")]
		public string Description;

		public TagDefinitionKey[] Tags;

		public StructureRenderDataContentKey RenderData;

		[Header("The prefab containing the colliders and other unity properties for this item when it's placed in a scene")]
		public PrefabContentKey Prefab;

		[Tooltip("The rules for manipulating this item")]
		public CollisionRuleSetDefinitionKey RuleSet;

		public override int GetId()
		{
			return Id;
		}
	}
}
