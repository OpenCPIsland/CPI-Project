using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Tag")]
	public class TagDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Tag;

		public TagCategoryDefinition Category;

		[TextArea]
		public string Description;
	}
}
