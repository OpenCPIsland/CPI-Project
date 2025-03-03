namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class InventoryCategoryManagerEventProxy : CategoryManagerEventProxy
	{
		public override void OnAllButton()
		{
			InventoryContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowAllTemplates));
		}

		public override void OnEquippedButton()
		{
			InventoryContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowEquippedItems));
		}

		public override void OnButtonPressed(string category)
		{
			InventoryContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.CategoryChange(category));
		}

		public override void OnHiddenButton()
		{
			InventoryContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowHiddenItems));
		}
	}
}
