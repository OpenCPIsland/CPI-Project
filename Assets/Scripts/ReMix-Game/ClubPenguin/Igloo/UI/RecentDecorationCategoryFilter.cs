using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.SceneManipulation;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin.Igloo.UI
{
	internal class RecentDecorationCategoryFilter : AbstractDecorationCategoryFilter
	{
		public RecentDecorationCategoryFilter(Dictionary<int, DecorationDefinition> dictionaryOfDecorationDefinitions, DecorationInventoryService decorationInventoryService)
			: base(dictionaryOfDecorationDefinitions, decorationInventoryService)
		{
		}

		internal override List<KeyValuePair<DecorationDefinition, int>> GetDefinitionsToDisplay()
		{
			List<DecorationDefinition> list = new List<DecorationDefinition>(Service.Get<RecentDecorationsService>().DefinitionsIDsList.Count);
			foreach (int definitionsIDs in Service.Get<RecentDecorationsService>().DefinitionsIDsList)
			{
				if (dictionaryOfDecorationDefinitions.ContainsKey(definitionsIDs))
				{
					list.Add(dictionaryOfDecorationDefinitions[definitionsIDs]);
				}
			}
			List<KeyValuePair<DecorationDefinition, int>> list2 = new List<KeyValuePair<DecorationDefinition, int>>();
			foreach (DecorationDefinition item in list)
			{
				int availableDecorationCount = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().GetAvailableDecorationCount(item.Id);
				list2.Add(new KeyValuePair<DecorationDefinition, int>(item, availableDecorationCount));
			}
			return list2;
		}
	}
}
