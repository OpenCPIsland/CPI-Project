using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/TagCategory")]
	public class TagCategoryDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string CategoryName;

		[TextArea]
		public string Description;
	}
}
