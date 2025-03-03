using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.SceneManipulation;
using System.Collections.Generic;

namespace ClubPenguin.Igloo.UI
{
	internal class AllMyIglooDecorationCategoryFilter : AbstractDecorationCategoryFilter
	{
		public AllMyIglooDecorationCategoryFilter(Dictionary<int, DecorationDefinition> dictionaryOfDecorationDefinitions, DecorationInventoryService decorationInventoryService)
			: base(dictionaryOfDecorationDefinitions, decorationInventoryService)
		{
		}

		internal override List<KeyValuePair<DecorationDefinition, int>> GetDefinitionsToDisplay()
		{
			return ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().GetAvailableDecorations();
		}
	}
}
