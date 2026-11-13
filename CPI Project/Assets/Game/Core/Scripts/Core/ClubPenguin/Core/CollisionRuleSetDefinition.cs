using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Igloo/CollisionRuleSet")]
	public class CollisionRuleSetDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		[Tooltip("Collection of rules controlling how this type collides with all other types")]
		public CollisionRuleDefinition[] InstalledItemRules = new CollisionRuleDefinition[0];
	}
}
