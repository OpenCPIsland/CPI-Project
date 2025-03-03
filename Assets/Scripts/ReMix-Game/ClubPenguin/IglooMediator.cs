using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Igloo;
using ClubPenguin.Igloo.UI;
using ClubPenguin.Net;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.SceneLayoutSync;
using ClubPenguin.SceneManipulation;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class IglooMediator
	{
		public const long DEFAULT_LAYOUT_ID = 0L;

		private const float ITEM_LIMIT_WARNING_THRESHOLD = 0.85f;

		private const string CONFIRM_CLEAR_LAYOUT_PROMPT = "IglooConfirmClearLayoutPrompt";

		private CPDataEntityCollection dataEntityCollection;

		private SceneLayoutDataManager layoutManager;

		private SceneLayoutSyncService layoutSyncService;

		private INetworkServicesManager networkServicesManager;

		private DataEventListener stateListener;

		public IglooMediator(EventDispatcher eventDispatcher, CPDataEntityCollection dataEntityCollection, SceneLayoutDataManager sceneLayoutDataManager, SceneLayoutSyncService sceneLayoutSyncService, INetworkServicesManager networkServicesManager)
		{
			this.dataEntityCollection = dataEntityCollection;
			this.networkServicesManager = networkServicesManager;
			eventDispatcher.AddListener<PlayerStateServiceEvents.LocalPlayerDataReceived>(onLocalPlayerDataReceived);
			eventDispatcher.AddListener<IglooUIEvents.AddNewDecoration>(onAddNewDecoration);
			eventDispatcher.AddListener<IglooUIEvents.AddNewStructure>(onAddNewStructure);
			eventDispatcher.AddListener<IglooUIEvents.DuplicateSelectedObject>(onDuplicateSelectedObject);
			eventDispatcher.AddListener<IglooUIEvents.ClearCurrentLayout>(onClearCurrentLayout);
			layoutManager = sceneLayoutDataManager;
			layoutSyncService = sceneLayoutSyncService;
			layoutSyncService.AddExtraLayoutInfoLoader(loadSittingLocations);
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.AddListener<WorldServiceEvents.SelfRoomJoinedEvent>(onIglooJoined);
			eventDispatcher.AddListener<IglooServiceEvents.ForceLeave>(onForceLeave, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<IglooServiceEvents.IglooLayoutCreated>(onIglooCreated, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<IglooServiceEvents.IglooDataUpdated>(onIglooDataUpdated, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<IglooServiceEvents.IglooLayoutUpdated>(onIglooLayoutUpdated, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<IglooServiceEvents.IglooLayoutDeleted>(onIglooLayoutDeleted, EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<ObjectManipulationEvents.RemovingStructureWithItemsEvent>(onRemovingStructureWithItems);
			eventDispatcher.AddListener<IglooEvents.ChangeZone>(onChangeZone);
			eventDispatcher.AddListener<IglooEvents.ReloadLayout>(onReloadLayout);
			eventDispatcher.AddListener<IglooEvents.PlayIglooSound>(onPlayPlacedDecorationSoundEffect);
			eventDispatcher.AddListener<SceneLayoutEvents.SceneMaxItemsEvent>(onSceneMaxItemsEncountered);
			eventDispatcher.AddListener<IglooUIEvents.MaxItemsLimitReached>(onMaxLayoutItemsReached);
			DataEntityHandle activeHandle = layoutManager.GetActiveHandle();
			dataEntityCollection.Whenever<SceneLayoutData>(activeHandle, onLayoutAdded, onLayoutRemoved);
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin && stateListener != null)
			{
				stateListener.StopListening();
				stateListener = null;
			}
			return false;
		}

		private SavedIgloosMetaData getSavedIgloosMetaData()
		{
			SavedIgloosMetaData component;
			if (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				return dataEntityCollection.AddComponent<SavedIgloosMetaData>(dataEntityCollection.LocalPlayerHandle);
			}
			return component;
		}

		private bool onLocalPlayerDataReceived(PlayerStateServiceEvents.LocalPlayerDataReceived evt)
		{
			if (evt.Data.iglooLayouts != null)
			{
				UpdateSavedIgloosMetaDataFromSavedgloosLayoutSummary(evt.Data.iglooLayouts);
			}
			return false;
		}

		private void UpdateSavedIgloosMetaDataFromSavedgloosLayoutSummary(SavedIglooLayoutsSummary savedIglooLayoutsSummary)
		{
			SavedIgloosMetaData savedIgloosMetaData = getSavedIgloosMetaData();
			savedIgloosMetaData.ActiveIglooId = savedIglooLayoutsSummary.activeLayoutId;
			savedIgloosMetaData.IglooVisibility = savedIglooLayoutsSummary.visibility;
			List<SavedIglooLayoutSummary> layoutsSummary = savedIglooLayoutsSummary.layouts;
			if (layoutsSummary != null)
			{
				List<SavedIglooMetaData> list = new List<SavedIglooMetaData>(savedIgloosMetaData.SavedIgloos);
				int i;
				for (i = 0; i < layoutsSummary.Count; i++)
				{
					int num = list.FindIndex((SavedIglooMetaData layout) => layout.LayoutId == layoutsSummary[i].layoutId);
					if (num >= 0)
					{
						list[num] = SavedIglooLayoutSummaryToSavedIglooMetaData(layoutsSummary[i]);
					}
					else
					{
						list.Add(SavedIglooLayoutSummaryToSavedIglooMetaData(layoutsSummary[i]));
					}
				}
				savedIgloosMetaData.SavedIgloos = list;
			}
			savedIgloosMetaData.IsDirty = false;
		}

		private SavedIglooMetaData SavedIglooLayoutSummaryToSavedIglooMetaData(SavedIglooLayoutSummary savedIglooLayoutSummary)
		{
			SavedIglooMetaData savedIglooMetaData = new SavedIglooMetaData();
			savedIglooMetaData.Name = savedIglooLayoutSummary.name;
			savedIglooMetaData.LayoutId = savedIglooLayoutSummary.layoutId;
			savedIglooMetaData.LotZoneName = savedIglooLayoutSummary.lot;
			savedIglooMetaData.CreatedDate = savedIglooLayoutSummary.createdDate;
			savedIglooMetaData.LastModifiedDate = savedIglooLayoutSummary.lastUpdatedDate;
			savedIglooMetaData.MemberOnly = savedIglooLayoutSummary.memberOnly;
			return savedIglooMetaData;
		}

		private bool onIglooDataUpdated(IglooServiceEvents.IglooDataUpdated evt)
		{
			if (evt.SignedIglooData.Data.activeLayout == null)
			{
				return false;
			}
			SavedIgloosMetaData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				IglooData data = evt.SignedIglooData.Data;
				UpdateSavedIgloosMetaDataFromIglooData(data, component);
				foreach (SavedIglooMetaData savedIgloo in component.SavedIgloos)
				{
					if (savedIgloo.LayoutId == data.activeLayoutId)
					{
						SceneLayoutToSavedIglooMetaData(data.activeLayoutId.Value, data.activeLayout, savedIgloo);
						savedIgloo.SceneLayout = data.activeLayout;
						break;
					}
				}
				component.IsDirty = false;
			}
			if (evt.SignedIglooData.Data.activeLayout != null)
			{
				dataEntityCollection.GetComponent<ProfileData>(dataEntityCollection.LocalPlayerHandle).ZoneId.name = evt.SignedIglooData.Data.activeLayout.zoneId;
			}
			return false;
		}

		private bool onIglooCreated(IglooServiceEvents.IglooLayoutCreated evt)
		{
			updateSavedIglooMetaData(evt.SavedSceneLayout.layoutId, evt.SavedSceneLayout);
			setSceneLayoutDataItemLimit(layoutManager.GetActiveSceneLayoutData());
			return false;
		}

		private bool onIglooLayoutUpdated(IglooServiceEvents.IglooLayoutUpdated evt)
		{
			if (evt.SavedSceneLayout == null)
			{
				return false;
			}
			updateSavedIglooMetaData(evt.SavedSceneLayout.layoutId, evt.SavedSceneLayout);
			return false;
		}

		private bool onIglooLayoutDeleted(IglooServiceEvents.IglooLayoutDeleted evt)
		{
			SavedIgloosMetaData savedIgloosMetaData = getSavedIgloosMetaData();
			List<SavedIglooMetaData> list = new List<SavedIglooMetaData>(savedIgloosMetaData.SavedIgloos);
			foreach (SavedIglooMetaData item in list)
			{
				if (evt.LayoutId == item.LayoutId)
				{
					savedIgloosMetaData.SavedIgloos.Remove(item);
					break;
				}
			}
			return false;
		}

		private void updateSavedIglooMetaData(long layoutId, SceneLayout savedSceneLayout)
		{
			SavedIgloosMetaData savedIgloosMetaData = getSavedIgloosMetaData();
			List<SavedIglooMetaData> list = new List<SavedIglooMetaData>(savedIgloosMetaData.SavedIgloos);
			bool flag = false;
			foreach (SavedIglooMetaData item in list)
			{
				if (layoutId == item.LayoutId)
				{
					SceneLayoutToSavedIglooMetaData(layoutId, savedSceneLayout, item);
					item.SceneLayout = savedSceneLayout;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				SavedIglooMetaData current = CreateSavedIglooMetaDataFromSceneLayout(layoutId, savedSceneLayout);
				current.SceneLayout = savedSceneLayout;
				list.Add(current);
				if (!savedIgloosMetaData.ActiveIglooId.HasValue)
				{
					savedIgloosMetaData.ActiveIglooId = layoutId;
				}
			}
			savedIgloosMetaData.SavedIgloos = list;
			savedIgloosMetaData.IsDirty = false;
		}

		private bool onForceLeave(IglooServiceEvents.ForceLeave evt)
		{
			HeldObjectsData component = dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle);
			if (component.HeldObject.ObjectType == HeldObjectType.PARTYGAME)
			{
				component.HeldObject = null;
			}
			return false;
		}

		private bool onIglooJoined(WorldServiceEvents.SelfRoomJoinedEvent evt)
		{
			if (!string.IsNullOrEmpty(evt.Room.zoneId.instanceId))
			{
				DataEntityHandle activeHandle = layoutManager.GetActiveHandle();
				SceneOwnerData component;
				if (!dataEntityCollection.TryGetComponent(activeHandle, out component))
				{
					component = dataEntityCollection.AddComponent<SceneOwnerData>(activeHandle);
				}
				component.Name = evt.RoomOwnerName;
				component.IsOwner = evt.IsRoomOwner;
				if (layoutManager.IsInOwnIgloo())
				{
					SavedIgloosMetaData savedIgloosMetaData = getSavedIgloosMetaData();
					if (savedIgloosMetaData.ActiveIglooId.HasValue)
					{
						updateSavedIglooMetaData(savedIgloosMetaData.ActiveIglooId.Value, evt.ExtraLayoutData);
					}
					if (savedIgloosMetaData.IsFirstIglooLoad())
					{
						layoutManager.AddNewActiveLayout();
						savedIgloosMetaData.IglooVisibility = IglooVisibility.PUBLIC;
						LocomotionActionEvent locomotionActionEvent = default(LocomotionActionEvent);
						locomotionActionEvent.Type = LocomotionAction.Move;
						locomotionActionEvent.Position = Vector3.zero;
						locomotionActionEvent.Direction = Vector3.zero;
						LocomotionActionEvent action = locomotionActionEvent;
						networkServicesManager.PlayerActionService.LocomotionAction(action);
					}
					else
					{
						layoutManager.CacheLayoutFromSceneLayout(savedIgloosMetaData.ActiveIglooId.Value, evt.ExtraLayoutData);
					}
				}
				else
				{
					layoutManager.CacheLayoutFromSceneLayout(0L, evt.ExtraLayoutData);
				}
				DataEntityHandle activeHandle2 = layoutManager.GetActiveHandle();
				stateListener = dataEntityCollection.Whenever<SceneStateData>(activeHandle2, onStateAdded, onStateRemoved);
			}
			return false;
		}

		private void initActiveIglooLayoutData(SceneStateData.SceneState state)
		{
			if (layoutManager.HasCachedLayoutData())
			{
				SceneLayoutData cachedSceneLayoutData = layoutManager.GetCachedSceneLayoutData();
				layoutManager.AddNewSceneLayoutData(cachedSceneLayoutData);
				layoutManager.RemoveCachedSceneLayout();
			}
			else
			{
				loadIglooInCreateState();
			}
		}

		public void loadIglooInCreateState()
		{
			SceneStateData.SceneState sceneState = SceneStateData.SceneState.Create;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			ProfileData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				Service.Get<ZoneTransitionService>().LoadIgloo(component.ZoneId, Service.Get<Localizer>().Language, sceneState);
			}
			else
			{
				Log.LogError(this, "Unable to find profileData to load into local players igloo.");
			}
		}

		private void onLayoutAdded(SceneLayoutData sceneLayoutData)
		{
			sceneLayoutData.DecorationAdded += onDecorationAdded;
			sceneLayoutData.DecorationRemoved += onDecorationRemoved;
			sceneLayoutData.MaxLayoutItemsReached += onMaxLayoutItemsReached;
			setSceneLayoutDataItemLimit(sceneLayoutData);
		}

		private void onLayoutRemoved(SceneLayoutData sceneLayoutData)
		{
			sceneLayoutData.DecorationAdded -= onDecorationAdded;
			sceneLayoutData.DecorationRemoved -= onDecorationRemoved;
			sceneLayoutData.MaxLayoutItemsReached -= onMaxLayoutItemsReached;
		}

		private void onMaxLayoutItemsReached()
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooItemLimitReachedPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooItemLimitReachedPromptLoaded);
			promptLoaderCMD.Execute();
			if (ClubPenguin.Core.SceneRefs.IsSet<ObjectManipulationInputController>())
			{
				ClubPenguin.Core.SceneRefs.Get<ObjectManipulationInputController>().Reset();
			}
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.EndDragInventoryItem));
		}

		private bool onMaxLayoutItemsReached(IglooUIEvents.MaxItemsLimitReached evt)
		{
			onMaxLayoutItemsReached();
			return false;
		}

		private void onIglooItemLimitReachedPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, null, promptLoader.Prefab);
		}

		private bool onRemovingStructureWithItems(ObjectManipulationEvents.RemovingStructureWithItemsEvent evt)
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooConfirmStructureRemovalPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooConfirmStructureRemovalPromptLoaded);
			promptLoaderCMD.Execute();
			return false;
		}

		private void onIglooConfirmStructureRemovalPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onIglooConfirmStructureRemovalPromptButtonClicked, promptLoader.Prefab);
		}

		private void onIglooConfirmStructureRemovalPromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			if (flags == DPrompt.ButtonFlags.YES)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ObjectManipulationEvents.DeleteSelectedItemEvent(true));
			}
		}

		private void onDecorationAdded(SceneLayoutData sceneLayoutData, DecorationLayoutData decoration)
		{
			if (!sceneLayoutData.ItemLimitWarningShown && sceneLayoutData.LayoutCount < sceneLayoutData.MaxLayoutItems)
			{
				float num = (float)sceneLayoutData.LayoutCount / ((float)sceneLayoutData.MaxLayoutItems * 1f);
				if (num >= 0.85f)
				{
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Igloos.ItemLimit.Banner");
					Service.Get<EventDispatcher>().DispatchEvent(new IglooUIEvents.ShowNotification(tokenTranslation, 5f, null, true, false));
					sceneLayoutData.ItemLimitWarningShown = true;
				}
			}
		}

		private void onDecorationRemoved(SceneLayoutData sceneLayoutData)
		{
			if (sceneLayoutData.ItemLimitWarningShown)
			{
				float num = (float)sceneLayoutData.LayoutCount / ((float)sceneLayoutData.MaxLayoutItems * 1f);
				if (num < 0.85f)
				{
					sceneLayoutData.ItemLimitWarningShown = false;
				}
			}
		}

		private void setSceneLayoutDataItemLimit(SceneLayoutData sceneLayoutData)
		{
			sceneLayoutData.ItemLimitWarningShown = false;
			LotDefinition lotDefinitionFromZoneName = GetLotDefinitionFromZoneName(sceneLayoutData.LotZoneName);
			if (lotDefinitionFromZoneName != null)
			{
				sceneLayoutData.MaxLayoutItems = lotDefinitionFromZoneName.MaxItems;
			}
		}

		private ExtraLayoutInfo loadSittingLocations()
		{
			RuntimeSittingLocationsExporter runtimeSittingLocationsExporter = new RuntimeSittingLocationsExporter();
			IList<ExportedSittingLocation> objectToSerialize = runtimeSittingLocationsExporter.ExportCurrentScene();
			string value = Service.Get<JsonService>().Serialize(objectToSerialize);
			ExtraLayoutInfo extraLayoutInfo = new ExtraLayoutInfo();
			extraLayoutInfo.Key = "SittingLocations";
			extraLayoutInfo.Value = value;
			return extraLayoutInfo;
		}

		private bool onReloadLayout(IglooEvents.ReloadLayout evt)
		{
			layoutManager.CacheLayoutFromSceneLayout(layoutManager.GetActiveSceneLayoutData().LayoutId, evt.NewLayout);
			layoutManager.RemoveActiveSceneLayout();
			if (ClubPenguin.Core.SceneRefs.IsSet<IglooUIStateController>())
			{
				ClubPenguin.Core.SceneRefs.Get<IglooUIStateController>().StopListeningToStateChange();
			}
			SceneTransitionService sceneTransitionService = Service.Get<SceneTransitionService>();
			sceneTransitionService.LoadScene(sceneTransitionService.CurrentScene, "Loading");
			return false;
		}

		private void onStateAdded(SceneStateData sceneStateData)
		{
			sceneStateData.OnStateChanged += onSceneStateStateChanged;
			if (sceneStateData.State != SceneStateData.SceneState.Create)
			{
				initActiveIglooLayoutData(sceneStateData.State);
			}
		}

		private void onStateRemoved(SceneStateData sceneStateData)
		{
			sceneStateData.OnStateChanged -= onSceneStateStateChanged;
		}

		private void onSceneStateStateChanged(SceneStateData.SceneState state)
		{
			if (!ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				return;
			}
			if (ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController != null)
			{
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ObjectManipulationInputController.Reset();
			}
			ISceneModifier[] sceneModifiers = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().SceneModifiers;
			for (int i = 0; i < sceneModifiers.Length; i++)
			{
				if (sceneModifiers[i] is EditModeModifier)
				{
					if (state == SceneStateData.SceneState.StructurePlacement)
					{
						(sceneModifiers[i] as EditModeModifier).EnabledStructures();
					}
					else
					{
						(sceneModifiers[i] as EditModeModifier).DisableStructures();
					}
				}
			}
		}

		private bool onChangeZone(IglooEvents.ChangeZone evt)
		{
			if (string.IsNullOrEmpty(evt.ZoneId.instanceId))
			{
				Service.Get<ZoneTransitionService>().LoadZone(evt.ZoneId.name);
			}
			else
			{
				Service.Get<ZoneTransitionService>().LoadIgloo(evt.ZoneId, Service.Get<Localizer>().Language, SceneStateData.SceneState.Play);
			}
			return false;
		}

		private bool onAddNewDecoration(IglooUIEvents.AddNewDecoration evt)
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				DecorationLayoutData data = default(DecorationLayoutData);
				data.DefinitionId = evt.Definition.Id;
				data.Type = DecorationLayoutData.DefinitionType.Decoration;
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().AddNewObject(evt.Definition.Prefab, evt.FinalTouchPoint, data);
			}
			return false;
		}

		private bool onAddNewStructure(IglooUIEvents.AddNewStructure evt)
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				DecorationLayoutData data = default(DecorationLayoutData);
				data.DefinitionId = evt.Definition.Id;
				data.Type = DecorationLayoutData.DefinitionType.Structure;
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().AddNewObject(evt.Definition.Prefab, evt.FinalTouchPoint, data);
			}
			return false;
		}

		private bool onDuplicateSelectedObject(IglooUIEvents.DuplicateSelectedObject evt)
		{
			SceneLayoutData activeSceneLayoutData = layoutManager.GetActiveSceneLayoutData();
			if (activeSceneLayoutData.IsLayoutAtMaxItemLimit())
			{
				Log.LogError(this, "Attempting to duplicate when max items already met");
				return false;
			}
			DecorationLayoutData data = default(DecorationLayoutData);
			bool flag = false;
			Vector3 zero = Vector3.zero;
			UnityEngine.Quaternion identity = UnityEngine.Quaternion.identity;
			Vector3 one = Vector3.one;
			if (!ClubPenguin.Core.SceneRefs.IsSet<ObjectManipulationInputController>())
			{
				Log.LogError(this, "ObjectManipulationInputController not set when attempting to duplicate.");
				return false;
			}
			ObjectManipulationInputController objectManipulationInputController = ClubPenguin.Core.SceneRefs.Get<ObjectManipulationInputController>();
			if (objectManipulationInputController.CurrentlySelectedObject == null)
			{
				Log.LogError(this, "Currently selected object was null when attempting to duplicate.");
				return false;
			}
			ManipulatableObject component = objectManipulationInputController.CurrentlySelectedObject.GetComponent<ManipulatableObject>();
			zero = component.transform.position;
			identity = component.transform.rotation;
			Transform parent = component.transform.parent;
			component.transform.parent = null;
			one = component.transform.localScale;
			component.transform.parent = parent;
			data.DefinitionId = component.DefinitionId;
			data.Type = component.Type;
			if (true && ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				SceneManipulationService sceneManipulationService = ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>();
				if (data.Type == DecorationLayoutData.DefinitionType.Decoration)
				{
					Vector3 b = Vector3.right;
					if (objectManipulationInputController.CurrentlySelectedObject.GetComponent<PartneredObject>() != null || objectManipulationInputController.CurrentlySelectedObject.GetComponent<SplittableObject>() != null)
					{
						b = Vector3.right * 3f;
					}
					DecorationDefinition decorationDefinition = Service.Get<DecorationInventoryService>().GetDecorationDefinition(data.DefinitionId);
					int availableDecorationCount = sceneManipulationService.GetAvailableDecorationCount(data.DefinitionId);
					if (decorationDefinition != null && availableDecorationCount > 0)
					{
						sceneManipulationService.AddNewObject(decorationDefinition.Prefab, zero + b, identity, one, data, false);
					}
				}
				else
				{
					StructureDefinition structureDefinition = Service.Get<DecorationInventoryService>().GetStructureDefinition(data.DefinitionId);
					int availableDecorationCount = sceneManipulationService.GetAvailableStructureCount(data.DefinitionId);
					if (structureDefinition != null && availableDecorationCount > 0)
					{
						sceneManipulationService.AddNewObject(structureDefinition.Prefab, zero + Vector3.right, identity, one, data, false);
					}
				}
			}
			return false;
		}

		private bool onClearCurrentLayout(IglooUIEvents.ClearCurrentLayout evt)
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooConfirmClearLayoutPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onConfirmClearLayoutPromptLoaded);
			promptLoaderCMD.Execute();
			return false;
		}

		private void onConfirmClearLayoutPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onConfirmClearLayoutPromptButtonClicked, promptLoader.Prefab);
		}

		private void onConfirmClearLayoutPromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			if (DPrompt.IsConfirmation(flags))
			{
				clearCurrentLayout();
			}
		}

		private void clearCurrentLayout()
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<SceneManipulationService>())
			{
				ClubPenguin.Core.SceneRefs.Get<SceneManipulationService>().ClearCurrentLayout();
			}
		}

		public static SavedIglooMetaData CreateSavedIglooMetaDataFromSceneLayout(long layoutId, SceneLayout sceneLayout)
		{
			SavedIglooMetaData savedIglooMetaData = new SavedIglooMetaData();
			SceneLayoutToSavedIglooMetaData(layoutId, sceneLayout, savedIglooMetaData);
			return savedIglooMetaData;
		}

		public static void SceneLayoutToSavedIglooMetaData(long layoutId, SceneLayout sceneLayout, SavedIglooMetaData savedIglooMetaData)
		{
			savedIglooMetaData.LayoutId = layoutId;
			savedIglooMetaData.CreatedDate = (sceneLayout.createdDate.HasValue ? sceneLayout.createdDate.Value : 0);
			savedIglooMetaData.LastModifiedDate = (sceneLayout.lastModifiedDate.HasValue ? sceneLayout.lastModifiedDate.Value : 0);
			savedIglooMetaData.MemberOnly = sceneLayout.memberOnly;
			savedIglooMetaData.LotZoneName = sceneLayout.zoneId;
		}

		public static void UpdateSavedIgloosMetaDataFromIglooData(IglooData iglooData, SavedIgloosMetaData savedIgloosMetaData)
		{
			savedIgloosMetaData.ActiveIglooId = (iglooData.activeLayoutId.HasValue ? iglooData.activeLayoutId.Value : 0);
			savedIgloosMetaData.IglooVisibility = (iglooData.visibility.HasValue ? iglooData.visibility.Value : IglooVisibility.PRIVATE);
		}

		public static string GetSceneNameFromZoneName(string zoneName)
		{
			Dictionary<string, ZoneDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<string, ZoneDefinition>>();
			ZoneDefinition zoneDefinition = dictionary[zoneName];
			if (zoneDefinition != null)
			{
				return zoneDefinition.SceneName;
			}
			return null;
		}

		public static string GetZoneNameFromSceneName(string sceneName)
		{
			Dictionary<string, ZoneDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<string, ZoneDefinition>>();
			foreach (KeyValuePair<string, ZoneDefinition> item in dictionary)
			{
				if (item.Value.SceneName == sceneName)
				{
					return item.Value.ZoneName;
				}
			}
			return null;
		}

		public static LotDefinition GetLotDefinitionFromZoneName(string zoneId)
		{
			Dictionary<string, LotDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<string, LotDefinition>>();
			foreach (KeyValuePair<string, LotDefinition> item in dictionary)
			{
				if (item.Value.ZoneDefintion.Id == zoneId)
				{
					return item.Value;
				}
			}
			return null;
		}

		private bool onPlayPlacedDecorationSoundEffect(IglooEvents.PlayIglooSound evt)
		{
			EventManager.Instance.PostEvent(evt.Sound, EventAction.PlaySound);
			return false;
		}

		private bool onSceneMaxItemsEncountered(SceneLayoutEvents.SceneMaxItemsEvent evt)
		{
			onMaxLayoutItemsReached();
			return false;
		}
	}
}
