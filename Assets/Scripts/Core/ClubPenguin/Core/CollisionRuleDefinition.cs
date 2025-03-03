using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Igloo/CollisionRule")]
	public class CollisionRuleDefinition : StaticGameDataDefinition
	{
		public CollisionRuleSetDefinitionKey InstalledItem;

		public CollisionRuleResult Result;

		public override string ToString()
		{
			return string.Concat("InstalledItem: ", InstalledItem, ", Result: ", Result);
		}
	}
}
