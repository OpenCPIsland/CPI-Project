using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class LotsScreenController : AbstractIglooScreenController<LotDefinition, string>
	{
		public LotDefinition[] AvailableLots;

		private IglooCameraOverrideStateData iglooLotCameraStateData;

		private IglooCustomizationButton lastSelected;

		private Dictionary<string, ZoneDefinition> zoneDefinitions;

		protected override void Start()
		{
			base.Start();
			zoneDefinitions = Service.Get<IGameData>().Get<Dictionary<string, ZoneDefinition>>();
			sceneLayoutData.LotZoneName = Service.Get<SceneTransitionService>().CurrentScene;
			iglooLotCameraStateData.UpdateTargetAndRail = true;
			iglooLotCameraStateData.OverrideState = SceneStateData.SceneState.StructurePlacement;
			eventDispatcher.DispatchEvent(default(IglooUIEvents.LotScreenReady));
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (!dataEntityCollection.TryGetComponent(sceneDataHandle, out iglooLotCameraStateData))
			{
				iglooLotCameraStateData = dataEntityCollection.AddComponent<IglooCameraOverrideStateData>(sceneDataHandle);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (iglooLotCameraStateData != null)
			{
				dataEntityCollection.RemoveComponent<IglooCameraOverrideStateData>(sceneDataHandle);
			}
		}

		public void OnBackButton()
		{
			eventDispatcher.DispatchEvent(default(IglooUIEvents.LotBackButtonPressed));
		}

		public void OnLotNextButton()
		{
			if (!Service.Get<SceneTransitionService>().IsTransitioning)
			{
				eventDispatcher.DispatchEvent(default(IglooUIEvents.LotNextButtonPressed));
			}
		}

		protected override List<ProgressionUtils.ParsedProgression<LotDefinition>> GetProgressionList()
		{
			return ProgressionUtils.RetrieveProgressionLockedItems<LotDefinition, LotRewardDefinition>(ProgressionUnlockCategory.lots, AbstractStaticGameDataRewardDefinition<LotDefinition>.ToDefinitionArray);
		}

		protected override List<KeyValuePair<LotDefinition, int>> GetAvailableItems()
		{
			List<ProgressionUtils.ParsedProgression<LotDefinition>> progressionList = GetProgressionList();
			List<KeyValuePair<LotDefinition, int>> list = new List<KeyValuePair<LotDefinition, int>>();
			for (int i = 0; i < progressionList.Count; i++)
			{
				list.Add(new KeyValuePair<LotDefinition, int>(progressionList[i].Definition, 0));
			}
			return list;
		}

		protected override void SetupIglooCustomizationButton(IglooCustomizationButton iglooCustomizationButton, int index)
		{
			base.SetupIglooCustomizationButton(iglooCustomizationButton, index);
			iglooCustomizationButton.IglooButtonClicked += onLotButton;
			iglooCustomizationButton.DefinitionId = index;
			LotDefinition key = inventoryCountPairs[index].Key;
			iglooCustomizationButton.SetSelected(sceneLayoutData.LotZoneName == key.LotName);
			if (lastSelected == null && sceneLayoutData.LotZoneName == key.LotName)
			{
				lastSelected = iglooCustomizationButton;
			}
		}

		protected override void onObjectRemoved(RectTransform item, int index)
		{
			base.onObjectRemoved(item, index);
			IglooCustomizationButton component = item.GetComponent<IglooCustomizationButton>();
			component.IglooButtonClicked -= onLotButton;
		}

		private void onLotButton(IglooCustomizationButton customizationButton)
		{
			LotDefinition key = inventoryCountPairs[customizationButton.DefinitionId].Key;
			ZoneDefinition zoneDefinition = zoneDefinitions[key.ZoneDefintion.Id];
			if (!Service.Get<SceneTransitionService>().IsTransitioning && Service.Get<SceneTransitionService>().CurrentScene != zoneDefinition.SceneName)
			{
				if (sceneLayoutData != null)
				{
					sceneLayoutData.LotZoneName = zoneDefinition.ZoneName;
					Service.Get<ICPSwrveService>().Action("igloo", "lot_selection", sceneLayoutData.LotZoneName);
				}
				lastSelected.SetSelected(false);
				customizationButton.SetSelected(true);
				lastSelected = customizationButton;
				eventDispatcher.DispatchEvent(new IglooUIEvents.SwapScene(zoneDefinition.SceneName));
			}
		}
	}
}
