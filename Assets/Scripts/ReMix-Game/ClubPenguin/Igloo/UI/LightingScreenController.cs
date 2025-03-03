using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class LightingScreenController : AbstractIglooScreenController<LightingDefinition, int>
	{
		private IglooCustomizationButton lastSelected;

		private IglooCameraOverrideStateData iglooLightingCameraStateData;

		protected override void OnEnable()
		{
			base.OnEnable();
			sceneDataHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			if (!dataEntityCollection.TryGetComponent(sceneDataHandle, out iglooLightingCameraStateData))
			{
				iglooLightingCameraStateData = dataEntityCollection.AddComponent<IglooCameraOverrideStateData>(sceneDataHandle);
			}
			iglooLightingCameraStateData.OverrideToLightingRail = true;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (iglooLightingCameraStateData != null)
			{
				iglooLightingCameraStateData.OverrideToLightingRail = false;
				dataEntityCollection.RemoveComponent<IglooCameraOverrideStateData>(sceneDataHandle);
			}
		}

		protected override List<ProgressionUtils.ParsedProgression<LightingDefinition>> GetProgressionList()
		{
			return ProgressionUtils.RetrieveProgressionLockedItems<LightingDefinition, LightingRewardDefinition>(ProgressionUnlockCategory.lighting, AbstractStaticGameDataRewardDefinition<LightingDefinition>.ToDefinitionArray);
		}

		protected override List<KeyValuePair<LightingDefinition, int>> GetAvailableItems()
		{
			List<ProgressionUtils.ParsedProgression<LightingDefinition>> progressionList = GetProgressionList();
			List<KeyValuePair<LightingDefinition, int>> list = new List<KeyValuePair<LightingDefinition, int>>();
			for (int i = 0; i < progressionList.Count; i++)
			{
				list.Add(new KeyValuePair<LightingDefinition, int>(progressionList[i].Definition, 0));
			}
			return list;
		}

		protected override void SetupIglooCustomizationButton(IglooCustomizationButton iglooCustomizationButton, int index)
		{
			base.SetupIglooCustomizationButton(iglooCustomizationButton, index);
			int index2 = index - numberOfStaticButtons;
			LightingDefinition key = inventoryCountPairs[index2].Key;
			iglooCustomizationButton.IglooButtonClicked += onLightingButton;
			if (sceneLayoutData != null)
			{
				iglooCustomizationButton.SetSelected(sceneLayoutData.LightingId == key.Id);
				if (lastSelected == null && sceneLayoutData.LightingId == key.Id)
				{
					lastSelected = iglooCustomizationButton;
				}
			}
		}

		protected override void onObjectRemoved(RectTransform item, int index)
		{
			item.GetComponent<IglooCustomizationButton>().IglooButtonClicked -= onLightingButton;
		}

		protected override int GetIntegerDefinitionId(LightingDefinition definition)
		{
			return definition.Id;
		}

		private void onLightingButton(IglooCustomizationButton customizationButton)
		{
			customizationButton.RemoveBreadcrumb();
			Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(MenuBreadcrumbType);
			if (lastSelected.DefinitionId == customizationButton.DefinitionId)
			{
				return;
			}
			LightingDefinition key = inventoryCountPairs[customizationButton.Index - numberOfStaticButtons].Key;
			if (key != null)
			{
				if (sceneLayoutData != null)
				{
					sceneLayoutData.LightingId = key.Id;
					Service.Get<ICPSwrveService>().Action("igloo", "lighting_selection", key.InternalName);
				}
				lastSelected.SetSelected(false);
				customizationButton.SetSelected(true);
				lastSelected = customizationButton;
			}
		}
	}
}
