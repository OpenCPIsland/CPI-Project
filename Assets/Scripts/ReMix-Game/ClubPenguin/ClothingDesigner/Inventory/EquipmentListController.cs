using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class EquipmentListController : MonoBehaviour
	{
		private const int ADDITIONAL_ITEM_COUNT = 2;

		private PrefabContentKey inventoryPrefabContentKey = new PrefabContentKey("Prefabs/EquipmentIconPrefab");

		[SerializeField]
		private GameObject noInventoryPanel;

		[SerializeField]
		private EditableItemController editableItemController;

		[SerializeField]
		private CategoryManager categoryManager;

		private InventoryEquipmentModel model;

		private ItemImageBuilder itemImageBuilder;

		private InventoryCategoryController categoryController;

		private EventChannel mainEventChannel;

		private EventChannel eventChannel;

		private ScrollRect scrollRect;

		private int indexToRemove;

		private bool backButtonClicked;

		private Dictionary<int, TemplateDefinition> templateDefinitions;

		public bool IsInitialized
		{
			get;
			private set;
		}

		private void Awake()
		{
			IsInitialized = false;
			backButtonClicked = false;
		}

		public void Init(InventoryEquipmentModel model)
		{
			this.model = model;
			templateDefinitions = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			scrollRect = editableItemController.GetComponentInChildren<ScrollRect>();
			itemImageBuilder = ItemImageBuilder.acquire();
			categoryController = new InventoryCategoryController();
			categoryController.Init(model, categoryManager);
			InventoryContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowAllTemplates));
			setupListeners();
			editableItemController.gameObject.SetActive(true);
			int size = model.DisplayedInventory.Count + 2;
			editableItemController.CreateEditableItemList(inventoryPrefabContentKey, size);
			IsInitialized = true;
		}

		private void OnDisable()
		{
			AvatarDetailsData component;
			if (model != null && !model.LocalPlayerInfo.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(model.LocalPlayerInfo, out component))
			{
				DCustomEquipment[] currentAvatarEquipment = model.GetCurrentAvatarEquipment();
				if (component.Outfit != currentAvatarEquipment)
				{
					new SaveOutfitToWearCMD(model.InventoryData.CurrentAvatarEquipment.ToArray(), currentAvatarEquipment).Execute();
				}
			}
			if (IsInitialized)
			{
				mainEventChannel.RemoveAllListeners();
			}
		}

		private void OnEnable()
		{
			if (IsInitialized)
			{
				setupMainEventChannelListeners();
			}
			backButtonClicked = false;
		}

		private void setupListeners()
		{
			mainEventChannel = new EventChannel(Service.Get<EventDispatcher>());
			setupMainEventChannelListeners();
			eventChannel = new EventChannel(InventoryContext.EventBus);
			eventChannel.AddListener<InventoryModelEvents.DisplayedInventoryUpdated>(orderedInventoryUpdated);
			eventChannel.AddListener<InventoryUIEvents.EnableScrollRect>(onEnableScrollRect);
			eventChannel.AddListener<InventoryUIEvents.DisableScrollRect>(onDisableScrollRect);
		}

		private void setupMainEventChannelListeners()
		{
			mainEventChannel.AddListener<EditableItemEvents.ItemReady>(onItemReady);
			mainEventChannel.AddListener<EditableItemEvents.ActionButtonClicked>(onEditableItemActionButtonClicked);
			mainEventChannel.AddListener<InventoryServiceEvents.EquipmentDeleted>(onInventoryItemDeleted);
			mainEventChannel.AddListener<EditableItemEvents.ItemDisappeared>(onItemDisappeared);
		}

		private bool onItemReady(EditableItemEvents.ItemReady evt)
		{
			int index = evt.Index;
			EditableItem item = evt.Item;
			EquipmentIcon componentInChildren = item.GetComponentInChildren<EquipmentIcon>();
			switch (index)
			{
			case 0:
				item.ShowActionButton = false;
				componentInChildren.SetupCreateButton();
				break;
			case 1:
				item.ShowActionButton = false;
				componentInChildren.SetupCatalogButton();
				break;
			default:
			{
				componentInChildren.SetupEquipmentButton();
				long key = model.DisplayedInventory[index - 2];
				InventoryIconModel<DCustomEquipment> inventoryIconModel = model.InventoryData.Inventory[key];
				item.ShowActionButton = true;
				item.Action = EditableItem.ActionType.Delete;
				TemplateDefinition value;
				if (templateDefinitions.TryGetValue(inventoryIconModel.Data.DefinitionId, out value) && !value.IsEditable)
				{
					if (inventoryIconModel.IsHidden)
					{
						item.Action = EditableItem.ActionType.Hide;
					}
					else
					{
						item.Action = EditableItem.ActionType.Show;
					}
				}
				AccessibilitySettings component = componentInChildren.GetComponent<AccessibilitySettings>();
				if (component != null)
				{
					component.CustomToken = value.Name;
				}
				setItemView(componentInChildren, inventoryIconModel);
				break;
			}
			}
			return false;
		}

		private void setItemView(EquipmentIcon iconItem, InventoryIconModel<DCustomEquipment> dataModel)
		{
			bool isPlayerMember = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember() || !model.InventoryData.IsEquipmentMemberOnly(dataModel.Data);
			iconItem.SetupView(dataModel, isPlayerMember);
			itemImageBuilder.RequestImage(dataModel.Data, iconItem.SetIcon);
		}

		private bool orderedInventoryUpdated(InventoryModelEvents.DisplayedInventoryUpdated evt)
		{
			int size = evt.DisplayCount + 2;
			editableItemController.ResetContent();
			editableItemController.Refresh(size);
			return false;
		}

		private bool onEditableItemActionButtonClicked(EditableItemEvents.ActionButtonClicked evt)
		{
			EditableItem item = evt.Item;
			if (evt.Action == EditableItem.ActionType.Delete)
			{
				removeItem(item);
			}
			else if (evt.Action == EditableItem.ActionType.Hide)
			{
				EquipmentIcon componentInChildren = item.GetComponentInChildren<EquipmentIcon>();
				model.HideItem(componentInChildren.EquipmentId);
			}
			else if (evt.Action == EditableItem.ActionType.Show)
			{
				EquipmentIcon componentInChildren = item.GetComponentInChildren<EquipmentIcon>();
				model.ShowItem(componentInChildren.EquipmentId);
			}
			return false;
		}

		private void removeItem(EditableItem editableItem)
		{
			EquipmentIcon iconItem = editableItem.GetComponentInChildren<EquipmentIcon>();
			indexToRemove = editableItem.GetCurrentIndex();
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("DeleteEquipmentPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, delegate(PromptLoaderCMD loader)
			{
				showRemoveItemPrompt(loader, iconItem);
			});
			promptLoaderCMD.Execute();
		}

		private void showRemoveItemPrompt(PromptLoaderCMD promptLoader, EquipmentIcon iconItem)
		{
			Texture2D texture2D = (Texture2D)iconItem.GetIcon();
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			promptLoader.PromptData.SetImage(DPrompt.PROMPT_IMAGE_DEFAULT, sprite);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, delegate(DPrompt.ButtonFlags pressed)
			{
				deleteConfirmationButton(pressed, iconItem.EquipmentId);
			}, promptLoader.Prefab);
		}

		private void deleteConfirmationButton(DPrompt.ButtonFlags pressed, long equipmentIdToDelete)
		{
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				Service.Get<INetworkServicesManager>().InventoryService.DeleteCustomEquipment(equipmentIdToDelete);
			}
		}

		private bool onInventoryItemDeleted(InventoryServiceEvents.EquipmentDeleted evt)
		{
			Service.Get<ICPSwrveService>().Action("game.item_deletion", model.InventoryData.Inventory[evt.EquipmentId].Data.Name);
			itemImageBuilder.RemoveImageFromCache(model.InventoryData.Inventory[evt.EquipmentId].Data);
			model.DeleteEquipment(evt.EquipmentId);
			editableItemController.Remove(indexToRemove);
			return false;
		}

		private bool onItemDisappeared(EditableItemEvents.ItemDisappeared evt)
		{
			int size = model.DisplayedInventory.Count + 2;
			editableItemController.Refresh(size);
			return false;
		}

		private bool onEnableScrollRect(InventoryUIEvents.EnableScrollRect evt)
		{
			scrollRect.enabled = true;
			return false;
		}

		private bool onDisableScrollRect(InventoryUIEvents.DisableScrollRect evt)
		{
			scrollRect.enabled = false;
			return false;
		}

		public void BackToHomeButton()
		{
			if (!backButtonClicked)
			{
				backButtonClicked = true;
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ChangeStateCustomizer));
			}
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (mainEventChannel != null)
			{
				mainEventChannel.RemoveAllListeners();
			}
			if (model != null)
			{
				model.Destroy();
			}
			if (itemImageBuilder != null)
			{
				ItemImageBuilder.release();
			}
			if (categoryController != null)
			{
				categoryController.Destroy();
			}
		}
	}
}
