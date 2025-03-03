using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.Progression;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class DecorationScreenController : AbstractIglooObjectSpawnerScreenController<DecorationDefinition>
	{
		private IglooCustomizationButton catalogButton;

		protected override void Start()
		{
			base.Start();
			decorationInventoryService.InventoryUpdated += onDecorationInventoryService;
		}

		private void onDecorationInventoryService()
		{
			Reload();
		}

		protected override List<ProgressionUtils.ParsedProgression<DecorationDefinition>> GetProgressionList()
		{
			return ProgressionUtils.RetrieveProgressionLockedItems<DecorationDefinition, DecorationRewardDefinition>(ProgressionUnlockCategory.decorationPurchaseRights, AbstractStaticGameDataRewardDefinition<DecorationDefinition>.ToDefinitionArray);
		}

		protected override void dispatchAddNewObjectEvent(DecorationDefinition definition, Vector2 finalTouchPoint)
		{
			eventDispatcher.DispatchEvent(new IglooUIEvents.AddNewDecoration(definition, finalTouchPoint));
		}

		protected override List<KeyValuePair<DecorationDefinition, int>> GetAvailableItems()
		{
			return decorationCategoryManager.GetDefinitionsToDisplay();
		}

		protected override void objectAddedOrRemoved(int itemIndex, ManipulatableObject manipulatableObject)
		{
			base.objectAddedOrRemoved(itemIndex, manipulatableObject);
			if (decorationCategoryManager != null && decorationCategoryManager.GetCurrentFilter() is InMyIglooDecorationCategoryFilter)
			{
				int totalInventoryCountForDecoration = decorationInventoryService.GetTotalInventoryCountForDecoration(manipulatableObject.DefinitionId);
				if (totalInventoryCountForDecoration == inventoryCountPairs[itemIndex].Value)
				{
					decorationCategoryManager.RefreshDecorationsDisplayed();
				}
			}
		}

		protected override void manipulatedObjectNotFound(ManipulatableObject manipulatableObject)
		{
			base.manipulatedObjectNotFound(manipulatableObject);
			if (decorationCategoryManager != null && decorationCategoryManager.GetCurrentFilter() is InMyIglooDecorationCategoryFilter)
			{
				decorationCategoryManager.RefreshDecorationsDisplayed();
			}
		}

		protected override void onCategoryUpdated(int newCategory)
		{
			base.onCategoryUpdated(newCategory);
			setCatalogButtonCategory(newCategory);
		}

		private void setCatalogButtonCategory(int category)
		{
			if (catalogButton != null)
			{
				if (category < 0)
				{
					catalogButton.SetupCatalogButton();
				}
				else
				{
					catalogButton.SetCatalogFilterCategory(category);
				}
			}
		}

		protected override void onObjectAdded(RectTransform item, int index)
		{
			base.onObjectAdded(item, index);
			if (index == 0)
			{
				catalogButton = item.GetComponent<IglooCustomizationButton>();
				int currentCategoryId = decorationCategoryManager.GetCurrentCategoryId();
				setCatalogButtonCategory(currentCategoryId);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			decorationInventoryService.InventoryUpdated -= onDecorationInventoryService;
		}
	}
}
