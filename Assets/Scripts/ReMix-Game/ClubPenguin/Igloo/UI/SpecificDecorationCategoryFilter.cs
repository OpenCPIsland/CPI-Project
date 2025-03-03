using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.SceneManipulation;
using System.Collections.Generic;

namespace ClubPenguin.Igloo.UI
{
	internal class SpecificDecorationCategoryFilter : AbstractDecorationCategoryFilter
	{
		private readonly DecorationCategoryDefinition categoryDefinition;

		public SpecificDecorationCategoryFilter(Dictionary<int, DecorationDefinition> dictionaryOfDecorationDefinitions, DecorationInventoryService decorationInventoryService, DecorationCategoryDefinition categoryDefinition)
			: base(dictionaryOfDecorationDefinitions, decorationInventoryService)
		{
			this.categoryDefinition = categoryDefinition;
		}

		internal override List<KeyValuePair<DecorationDefinition, int>> GetDefinitionsToDisplay()
		{
			List<KeyValuePair<DecorationDefinition, int>> list = new List<KeyValuePair<DecorationDefinition, int>>();
			List<KeyValuePair<DecorationDefinition, int>> availableDecorations = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().GetAvailableDecorations();
			for (int i = 0; i < availableDecorations.Count; i++)
			{
				if (availableDecorations[i].Key.CategoryKey.Id == categoryDefinition.Id)
				{
					list.Add(availableDecorations[i]);
				}
			}
			return list;
		}
	}
}
