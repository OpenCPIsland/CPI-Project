using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Catalog;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.ClothingDesigner.ItemCustomizer;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerController
	{
		private enum ClothingDesignerState
		{
			Initializing,
			Inventory,
			Customizer,
			Editing,
			Catalog
		}

		private const string CUSTOMIZER_SCREEN_NAME = "CustomizerScreen";

		private const string EQUIPMENT_SCREEN_NAME = "InventoryEquipmentScreen";

		private const string PENGUIN_LIGHT_SOURCE_NAME = "Realtime Directional Light";

		private static readonly DPrompt promptData = new DPrompt("GlobalUI.Prompts.loseProgressTitle", "GlobalUI.Prompts.newItemNoSave", DPrompt.ButtonFlags.CANCEL | DPrompt.ButtonFlags.OK);

		private readonly PrefabContentKey screenPrefabKey = new PrefabContentKey("Prefabs/ClothingDesigner/Screens/*");

		private ClothingDesignerState currentState;

		private Transform currentLoadedScreen;

		private ClothingDesignerDependencies dependencies;

		private EventChannel eventChannel;

		private EventChannel customizerEventChannel;

		private EventChannel catalogEventChannel;

		private EventChannel inventoryEventChannel;

		private bool cameraTransitionComplete;

		private bool isNewScreenInitialized;

		private bool isFromCatalog;

		private InventoryData inventoryData;

		private GameObject penguinLighting;

		private string currentInventoryCategory;

		private CatalogThemeDefinition challengeTheme;

		private bool showCatalogCustomizer;

		public event System.Action ControllerIsInitializing;

		public void Init(ClothingDesignerDependencies dependencies)
		{
			this.dependencies = dependencies;
			eventChannel = new EventChannel(ClothingDesignerContext.EventBus);
			eventChannel.AddListener<ClothingDesignerUIEvents.CloseButton>(onCloseButton);
			eventChannel.AddListener<ClothingDesignerUIEvents.ChangeStateInventory>(onStateChangeInventory);
			eventChannel.AddListener<ClothingDesignerUIEvents.ChangeStateCustomizer>(onStateChangeCustomizer);
			eventChannel.AddListener<ClothingDesignerUIEvents.ShowCatalog>(onShowCatalog);
			eventChannel.AddListener<ClothingDesignerUIEvents.HideCatalog>(onHideCatalog);
			eventChannel.AddListener<ClothingDesignerUIEvents.CameraPositionChangeComplete>(onCameraTransitionComplete);
			eventChannel.AddListener<ClothingDesignerUIEvents.ShowMemberFlow>(onShowMemberFlow);
			eventChannel.AddListener<ClothingDesignerUIEvents.ShowSubmittedInCatalog>(onShowSubmittedInCatalog);
			customizerEventChannel = new EventChannel(CustomizationContext.EventBus);
			customizerEventChannel.AddListener<ClothingDesignerUIEvents.CustomizerEditingState>(onCustomizerEditState);
			customizerEventChannel.AddListener<ClothingDesignerUIEvents.CustomizerTemplateState>(onCustomizerTemplateState);
			customizerEventChannel.AddListener<CustomizerUIEvents.SelectTemplate>(onCustomizerTemplateSelected);
			inventoryEventChannel = new EventChannel(InventoryContext.EventBus);
			inventoryEventChannel.AddListener<ClothingDesignerUIEvents.ShowAllTemplates>(onShowAllTemplates);
			inventoryEventChannel.AddListener<ClothingDesignerUIEvents.ShowEquippedItems>(onShowEquippedItems);
			inventoryEventChannel.AddListener<ClothingDesignerUIEvents.ShowHiddenItems>(onShowHiddenItems);
			inventoryEventChannel.AddListener<ClothingDesignerUIEvents.CategoryChange>(onInventoryChangeCategory);
			catalogEventChannel = new EventChannel(CatalogContext.EventBus);
			catalogEventChannel.AddListener<CatalogUIEvents.AcceptChallengeClickedEvent>(onAcceptChallengeClickEvent);
			penguinLighting = GameObject.Find("Realtime Directional Light");
			hideCatalog();
			loadInventory();
			Service.Get<ICPSwrveService>().Action("view.my_style");
			showCatalogCustomizer = false;
		}

		private void loadInventory()
		{
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				inventoryData = Service.Get<CPDataEntityCollection>().GetComponent<InventoryData>(localPlayerHandle);
				if (inventoryData == null)
				{
					Service.Get<EventDispatcher>().AddListener<InventoryServiceEvents.InventoryLoaded>(onInventoryRetrieved);
					Service.Get<INetworkServicesManager>().InventoryService.GetEquipmentInventory();
				}
				else
				{
					setDefaultScreen();
				}
			}
			else
			{
				setDefaultScreen();
			}
		}

		private bool onInventoryRetrieved(InventoryServiceEvents.InventoryLoaded evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<InventoryServiceEvents.InventoryLoaded>(onInventoryRetrieved);
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				inventoryData = Service.Get<CPDataEntityCollection>().AddComponent<InventoryData>(localPlayerHandle);
				inventoryData.Inventory = new Dictionary<long, InventoryIconModel<DCustomEquipment>>();
				inventoryData.CurrentAvatarEquipment = new List<long>();
				setTemplateData();
				for (int i = 0; i < evt.Inventory.Count; i++)
				{
					try
					{
						DCustomEquipment data = CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(evt.Inventory[i]);
						InventoryIconModel<DCustomEquipment> value = new InventoryIconModel<DCustomEquipment>(data.Id, data, false, true);
						inventoryData.Inventory.Add(data.Id, value);
					}
					catch (KeyNotFoundException)
					{
					}
				}
				AvatarDetailsData component;
				if (Service.Get<CPDataEntityCollection>().TryGetComponent(localPlayerHandle, out component) && component.Outfit != null)
				{
					DCustomOutfit currentAvatarEquipment = default(DCustomOutfit);
					currentAvatarEquipment.Equipment = component.Outfit;
					setCurrentAvatarEquipment(currentAvatarEquipment);
				}
			}
			else
			{
				Log.LogError(this, "Unable to find the LocalPlayerHandle.");
			}
			setDefaultScreen();
			string key = SceneTransitionService.SceneArgs.ShowCatalogOnEntry.ToString();
			if (Service.Get<SceneTransitionService>().HasSceneArg(key) && (bool)Service.Get<SceneTransitionService>().GetSceneArg(key))
			{
				showCatalog();
			}
			return false;
		}

		private void setTemplateData()
		{
			List<EquipmentCategoryDefinitionContentKey> list = new List<EquipmentCategoryDefinitionContentKey>();
			Dictionary<string, List<TemplateDefinition>> dictionary = new Dictionary<string, List<TemplateDefinition>>();
			Dictionary<int, TemplateDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			foreach (KeyValuePair<int, TemplateDefinition> item in dictionary2)
			{
				TemplateDefinition value = item.Value;
				if (value.CategoryKey != null)
				{
					if (dictionary.ContainsKey(value.CategoryKey.Key))
					{
						dictionary[value.CategoryKey.Key].Add(value);
					}
					else
					{
						List<TemplateDefinition> list2 = new List<TemplateDefinition>();
						list2.Add(value);
						dictionary.Add(value.CategoryKey.Key, list2);
						list.Add(value.CategoryKey);
					}
				}
			}
			inventoryData.TemplateData = dictionary;
			inventoryData.CategoryKeys = list;
		}

		private void setDefaultScreen()
		{
			currentState = ClothingDesignerState.Initializing;
			if (this.ControllerIsInitializing != null)
			{
				this.ControllerIsInitializing();
			}
			updateScreen(ClothingDesignerState.Inventory, "InventoryEquipmentScreen", true);
		}

		private void setCurrentAvatarEquipment(DCustomOutfit outfit)
		{
			for (int i = 0; i < outfit.Equipment.Length; i++)
			{
				long id = outfit.Equipment[i].Id;
				if (inventoryData.Inventory.ContainsKey(id))
				{
					inventoryData.CurrentAvatarEquipment.Add(id);
					inventoryData.Inventory[id].IsEquipped = true;
				}
				else
				{
					Log.LogErrorFormatted(this, "The item {0} exists on a users outfit that does not exist in there inventory.", id);
				}
			}
		}

		private bool onStateChangeInventory(ClothingDesignerUIEvents.ChangeStateInventory evt)
		{
			if (isFromCatalog)
			{
				showCatalog();
			}
			else
			{
				updateScreen(ClothingDesignerState.Inventory, "InventoryEquipmentScreen");
				ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Inventory, true));
			}
			return false;
		}

		private bool onStateChangeCustomizer(ClothingDesignerUIEvents.ChangeStateCustomizer evt)
		{
			updateScreen(ClothingDesignerState.Customizer, "CustomizerScreen");
			if (showCatalogCustomizer)
			{
				ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.CatalogCustomizer, false));
				showCatalogCustomizer = false;
			}
			else
			{
				ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Customizer, true));
			}
			return false;
		}

		private bool onShowCatalog(ClothingDesignerUIEvents.ShowCatalog evt)
		{
			isFromCatalog = true;
			Service.Get<ICPSwrveService>().Action("game.clothing_catalog", "view");
			showCatalog();
			return false;
		}

		private bool onHideCatalog(ClothingDesignerUIEvents.HideCatalog evt)
		{
			isFromCatalog = false;
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerEvents.ResetClothingDesignerTheme));
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.UpdateCameraState(ClothingDesignerCameraState.Inventory, false));
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ChangeStateInventory));
			hideCatalog();
			return false;
		}

		private bool onCloseButton(ClothingDesignerUIEvents.CloseButton evt)
		{
			if (currentState == ClothingDesignerState.Editing)
			{
				Service.Get<PromptManager>().ShowPrompt(promptData, onLoseProgressPromptDone);
			}
			else
			{
				Service.Get<CatalogServiceProxy>().cache.ClearCache();
				Service.Get<GameStateController>().ReturnToZoneScene();
			}
			return false;
		}

		private void onLoseProgressPromptDone(DPrompt.ButtonFlags button)
		{
			if (button == DPrompt.ButtonFlags.OK)
			{
				Service.Get<CatalogServiceProxy>().cache.ClearCache();
				Service.Get<GameStateController>().ReturnToZoneScene();
			}
		}

		private bool onShowSubmittedInCatalog(ClothingDesignerUIEvents.ShowSubmittedInCatalog evt)
		{
			dependencies.ScreenContent.SetActive(false);
			dependencies.CatalogContainer.SetActive(true);
			CatalogContext.EventBus.DispatchEvent(new CatalogModelEvents.CatalogStateChanged(CatalogState.ItemsView));
			CurrentThemeData currentThemeData = Service.Get<CatalogServiceProxy>().GetCurrentThemeData();
			CatalogContext.EventBus.DispatchEvent(new CatalogUIEvents.ShowItemsForThemeEvent(currentThemeData, evt.ClothingCatalogItemSubmittedId));
			return false;
		}

		private void showCatalog()
		{
			isFromCatalog = true;
			currentState = ClothingDesignerState.Catalog;
			dependencies.ScreenContent.SetActive(false);
			dependencies.CatalogContainer.SetActive(true);
			penguinLighting.SetActive(false);
		}

		private void hideCatalog()
		{
			dependencies.ScreenContent.SetActive(true);
			dependencies.CatalogContainer.SetActive(false);
			penguinLighting.SetActive(true);
		}

		private void updateScreen(ClothingDesignerState newState, string screenName, bool initialScreen = false)
		{
			if (newState != currentState)
			{
				cameraTransitionComplete = initialScreen;
				isNewScreenInitialized = false;
				currentState = newState;
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.EnableDragAreaControllerUpdates));
				CoroutineRunner.Start(loadScreenContent(screenName, newState), this, "loadScreenContent");
			}
		}

		private IEnumerator loadScreenContent(string screenName, ClothingDesignerState transitionState)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(screenPrefabKey, screenName);
			yield return assetRequest;
			if (currentLoadedScreen != null)
			{
				UnityEngine.Object.Destroy(currentLoadedScreen.gameObject);
				currentLoadedScreen = null;
				dependencies.LoadingIndicator.SetActive(true);
			}
			currentLoadedScreen = UnityEngine.Object.Instantiate(assetRequest.Asset).transform;
			currentLoadedScreen.SetParent(dependencies.ScreenContainer, false);
			currentLoadedScreen.gameObject.SetActive(true);
			while (!cameraTransitionComplete)
			{
				yield return null;
			}
			dependencies.LoadingIndicator.SetActive(false);
			if (currentState == transitionState && !isNewScreenInitialized)
			{
				initializeNewState();
			}
		}

		private bool onCameraTransitionComplete(ClothingDesignerUIEvents.CameraPositionChangeComplete evt)
		{
			cameraTransitionComplete = true;
			return false;
		}

		private void initializeNewState()
		{
			if (currentState != ClothingDesignerState.Customizer || isFromCatalog)
			{
				currentInventoryCategory = "";
			}
			switch (currentState)
			{
			case ClothingDesignerState.Inventory:
				setupInventory();
				break;
			case ClothingDesignerState.Customizer:
				setupCustomization();
				break;
			}
			isNewScreenInitialized = true;
		}

		private void setupCustomization()
		{
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.ShowCoinCountWidget));
			CustomizerAvatarController componentInChildren = dependencies.CustomizerAvatarPreview.GetComponentInChildren<CustomizerAvatarController>();
			CustomizationContext component = currentLoadedScreen.GetComponent<CustomizationContext>();
			component.Init(componentInChildren, dependencies.DefaultChannelTextures, currentInventoryCategory);
		}

		private void setupInventory()
		{
			Service.Get<CatalogServiceProxy>().DeactivateCatalogTheme();
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerWidgetEvents.HideCoinCountWidget));
			InventoryContext component = currentLoadedScreen.GetComponent<InventoryContext>();
			component.Init();
		}

		private bool onCustomizerEditState(ClothingDesignerUIEvents.CustomizerEditingState evt)
		{
			currentState = ClothingDesignerState.Editing;
			return false;
		}

		private bool onCustomizerTemplateState(ClothingDesignerUIEvents.CustomizerTemplateState evt)
		{
			currentState = ClothingDesignerState.Customizer;
			return false;
		}

		private bool onCustomizerTemplateSelected(CustomizerUIEvents.SelectTemplate evt)
		{
			if (challengeTheme != null)
			{
				Service.Get<ICPSwrveService>().Action("clothing_catalog_challenge", "template_chosen", evt.TemplateData.Name, challengeTheme.Title);
			}
			return false;
		}

		private bool onShowAllTemplates(ClothingDesignerUIEvents.ShowAllTemplates evt)
		{
			currentInventoryCategory = "";
			return false;
		}

		private bool onShowEquippedItems(ClothingDesignerUIEvents.ShowEquippedItems evt)
		{
			currentInventoryCategory = "";
			return false;
		}

		private bool onShowHiddenItems(ClothingDesignerUIEvents.ShowHiddenItems evt)
		{
			currentInventoryCategory = "";
			return false;
		}

		private bool onInventoryChangeCategory(ClothingDesignerUIEvents.CategoryChange evt)
		{
			currentInventoryCategory = evt.Category;
			return false;
		}

		private bool onShowMemberFlow(ClothingDesignerUIEvents.ShowMemberFlow evt)
		{
			if (!Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				Service.Get<GameStateController>().ShowAccountSystemMembership(evt.Trigger);
			}
			return false;
		}

		private bool onAcceptChallengeClickEvent(CatalogUIEvents.AcceptChallengeClickedEvent evt)
		{
			challengeTheme = evt.Theme;
			showCatalogCustomizer = true;
			ClothingDesignerContext.EventBus.DispatchEvent(new ClothingDesignerEvents.UpdateClothingDesignerTheme(evt.ThemeColors));
			ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ChangeStateCustomizer));
			hideCatalog();
			return false;
		}

		public void Destroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (customizerEventChannel != null)
			{
				customizerEventChannel.RemoveAllListeners();
			}
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (catalogEventChannel != null)
			{
				catalogEventChannel.RemoveAllListeners();
			}
			if (inventoryEventChannel != null)
			{
				inventoryEventChannel.RemoveAllListeners();
			}
		}
	}
}
