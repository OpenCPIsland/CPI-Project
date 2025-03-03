using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class InventoryCategoryController
	{
		private enum CategoryType
		{
			All,
			Equipped,
			Hidden,
			Other
		}

		private InventoryEquipmentModel model;

		private EventChannel externalEventChannel;

		private CategoryManager categoryManager;

		private bool isInEditMode;

		private CategoryType currentCategory;

		private string otherCategory;

		public void Init(InventoryEquipmentModel model, CategoryManager categoryManager)
		{
			this.model = model;
			this.categoryManager = categoryManager;
			categoryManager.InitAndDisableButtons(model.InventoryData, new InventoryCategoryManagerEventProxy());
			categoryManager.SetAllItemCount(model.InventoryData.Inventory.Count - model.HiddenItems.Count);
			categoryManager.EnableEquippedButton(model.InventoryData.CurrentAvatarEquipment.Count > 0);
			categoryManager.ShowHiddenButton(false);
			setupListeners();
		}

		private void setupListeners()
		{
			externalEventChannel = new EventChannel(InventoryContext.EventBus);
			externalEventChannel.AddListener<ClothingDesignerUIEvents.ShowAllTemplates>(onShowAllTemplates);
			externalEventChannel.AddListener<ClothingDesignerUIEvents.ShowEquippedItems>(onShowEquippedItems);
			externalEventChannel.AddListener<ClothingDesignerUIEvents.ShowHiddenItems>(onShowHiddenItems);
			externalEventChannel.AddListener<ClothingDesignerUIEvents.CategoryChange>(onChangeCategory);
			externalEventChannel.AddListener<InventoryModelEvents.CurrentAvatarEquipmentChanged>(onCurrentAvatarEquipmentChanged);
			externalEventChannel.AddListener<InventoryModelEvents.EquipmentItemVisibilityChanged>(onEquipmentItemVisibilityChanged);
			Service.Get<EventDispatcher>().AddListener<EditableItemEvents.EditStateChanged>(onEditStateChanged);
		}

		private bool onEditStateChanged(EditableItemEvents.EditStateChanged evt)
		{
			isInEditMode = evt.IsEditStateActive;
			categoryManager.ShowHiddenButton(evt.IsEditStateActive);
			if (evt.IsEditStateActive)
			{
				categoryManager.SetAllItemCount(model.InventoryData.Inventory.Count);
				categoryManager.EnableHiddenButton(model.HiddenItems.Count > 0);
			}
			else
			{
				categoryManager.SetAllItemCount(model.InventoryData.Inventory.Count - model.HiddenItems.Count);
			}
			switch (currentCategory)
			{
			case CategoryType.All:
				model.SetDisplayedInventoryToAll(!isInEditMode);
				break;
			case CategoryType.Hidden:
				if (!isInEditMode)
				{
					categoryManager.ClickAllButton();
				}
				break;
			case CategoryType.Other:
				model.SetDisplayedInventoryToCategory(otherCategory, !isInEditMode);
				break;
			}
			return false;
		}

		private bool onShowAllTemplates(ClothingDesignerUIEvents.ShowAllTemplates evt)
		{
			currentCategory = CategoryType.All;
			model.SetDisplayedInventoryToAll(!isInEditMode);
			return false;
		}

		private bool onShowEquippedItems(ClothingDesignerUIEvents.ShowEquippedItems evt)
		{
			currentCategory = CategoryType.Equipped;
			model.SetDisplayedInventoryToEquipped();
			return false;
		}

		private bool onShowHiddenItems(ClothingDesignerUIEvents.ShowHiddenItems evt)
		{
			currentCategory = CategoryType.Hidden;
			model.SetDisplayedInventoryToHidden();
			return false;
		}

		private bool onChangeCategory(ClothingDesignerUIEvents.CategoryChange evt)
		{
			currentCategory = CategoryType.Other;
			otherCategory = evt.Category;
			model.SetDisplayedInventoryToCategory(evt.Category, !isInEditMode);
			return false;
		}

		private bool onCurrentAvatarEquipmentChanged(InventoryModelEvents.CurrentAvatarEquipmentChanged evt)
		{
			if (currentCategory == CategoryType.Equipped)
			{
				model.SetDisplayedInventoryToEquipped();
			}
			else
			{
				categoryManager.EnableEquippedButton(model.InventoryData.CurrentAvatarEquipment.Count > 0);
			}
			return false;
		}

		private bool onEquipmentItemVisibilityChanged(InventoryModelEvents.EquipmentItemVisibilityChanged evt)
		{
			if (currentCategory == CategoryType.Hidden)
			{
				model.SetDisplayedInventoryToHidden();
			}
			else
			{
				categoryManager.EnableHiddenButton(model.HiddenItems.Count > 0);
			}
			return false;
		}

		public void Destroy()
		{
			externalEventChannel.RemoveAllListeners();
			Service.Get<EventDispatcher>().RemoveListener<EditableItemEvents.EditStateChanged>(onEditStateChanged);
		}
	}
}
