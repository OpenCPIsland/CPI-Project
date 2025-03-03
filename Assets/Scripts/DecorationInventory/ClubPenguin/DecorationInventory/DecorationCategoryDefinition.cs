using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	[CreateAssetMenu(menuName = "Definition/Igloo/DecorationCategory")]
	public class DecorationCategoryDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Id;

		[LocalizationToken]
		public string DisplayName;

		public int SortOrder;
	}
}
