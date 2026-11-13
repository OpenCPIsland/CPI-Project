using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.SceneManipulation;
using Disney.LaunchPadFramework;
using System.Collections.Generic;

namespace ClubPenguin.Igloo.UI
{
	internal class InMyIglooDecorationCategoryFilter : AbstractDecorationCategoryFilter
	{
		private readonly SceneLayoutData sceneLayoutData;

		public InMyIglooDecorationCategoryFilter(Dictionary<int, DecorationDefinition> dictionaryOfDecorationDefinitions, DecorationInventoryService decorationInventoryService, SceneLayoutData sceneLayoutData)
			: base(dictionaryOfDecorationDefinitions, decorationInventoryService)
		{
			this.sceneLayoutData = sceneLayoutData;
			if (this.sceneLayoutData == null)
			{
				Log.LogErrorFormatted(this, "The layout data is null");
			}
		}

		internal override List<KeyValuePair<DecorationDefinition, int>> GetDefinitionsToDisplay()
		{
			List<KeyValuePair<DecorationDefinition, int>> list = new List<KeyValuePair<DecorationDefinition, int>>();
			if (sceneLayoutData != null)
			{
				foreach (DecorationLayoutData item2 in sceneLayoutData.GetLayoutEnumerator())
				{
					if (item2.Type == DecorationLayoutData.DefinitionType.Decoration && dictionaryOfDecorationDefinitions.ContainsKey(item2.DefinitionId))
					{
						DecorationDefinition decorationDefinition = dictionaryOfDecorationDefinitions[item2.DefinitionId];
						int availableDecorationCount = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().GetAvailableDecorationCount(decorationDefinition.Id);
						KeyValuePair<DecorationDefinition, int> item = new KeyValuePair<DecorationDefinition, int>(decorationDefinition, availableDecorationCount);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				SceneManipulationService sceneManipulationService = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>();
				if (sceneManipulationService.IsObjectSelectedForAdd)
				{
					ObjectManipulator currentObjectManipulator = sceneManipulationService.ObjectManipulationInputController.CurrentObjectManipulator;
					if (currentObjectManipulator != null)
					{
						int definitionId = currentObjectManipulator.GetComponent<ManipulatableObject>().DefinitionId;
						DecorationDefinition decorationDefinition = dictionaryOfDecorationDefinitions[definitionId];
						int availableDecorationCount = sceneManipulationService.GetAvailableDecorationCount(decorationDefinition.Id);
						KeyValuePair<DecorationDefinition, int> item = new KeyValuePair<DecorationDefinition, int>(decorationDefinition, availableDecorationCount);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}
	}
}
