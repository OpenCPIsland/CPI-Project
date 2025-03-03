using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.Progression;
using ClubPenguin.SceneManipulation;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class StructureScreenController : AbstractIglooObjectSpawnerScreenController<StructureDefinition>
	{
		protected override void Start()
		{
			base.Start();
			decorationInventoryService.InventoryUpdated += onInventoryUpdated;
		}

		private void onInventoryUpdated()
		{
			Reload();
		}

		protected override List<ProgressionUtils.ParsedProgression<StructureDefinition>> GetProgressionList()
		{
			return ProgressionUtils.RetrieveProgressionLockedItems<StructureDefinition, StructureRewardDefinition>(ProgressionUnlockCategory.structurePurchaseRights, AbstractStaticGameDataRewardDefinition<StructureDefinition>.ToDefinitionArray);
		}

		protected override bool SetLockableButtonLockedStatus(ILockableButton button, StructureDefinition definition, ProgressionUtils.ParsedProgression<StructureDefinition> progressData)
		{
			bool result;
			if (definition.SizeUnits > Service.Get<ObjectManipulationService>().StructurePlotManager.LargestPlotSize)
			{
				result = true;
				(button as IglooCustomizationButton).SetSizeLocked();
			}
			else
			{
				result = base.SetLockableButtonLockedStatus(button, definition, progressData);
			}
			return result;
		}

		protected override void SetupIglooCustomizationButton(IglooCustomizationButton iglooCustomizationButton, int index)
		{
			if (index == 0)
			{
				iglooCustomizationButton.SetupCatalogButton(IglooCustomizationButton.CatalogFilterType.STRUCTURES);
				return;
			}
			base.SetupIglooCustomizationButton(iglooCustomizationButton, index);
			int index2 = index - numberOfStaticButtons;
			StructureDefinition key = inventoryCountPairs[index2].Key;
			iglooCustomizationButton.SetSizeIconSprite(key.SizeUnits - 1);
		}

		protected override void dispatchAddNewObjectEvent(StructureDefinition definition, Vector2 finalTouchPoint)
		{
			eventDispatcher.DispatchEvent(new IglooUIEvents.AddNewStructure(definition, finalTouchPoint));
		}

		protected override List<KeyValuePair<StructureDefinition, int>> GetAvailableItems()
		{
			return ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().GetAvailableStructures();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			decorationInventoryService.InventoryUpdated -= onInventoryUpdated;
		}
	}
}
