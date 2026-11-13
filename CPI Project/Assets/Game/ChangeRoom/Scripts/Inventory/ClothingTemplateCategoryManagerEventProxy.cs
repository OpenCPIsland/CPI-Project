using ClubPenguin.ClothingDesigner.ItemCustomizer;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class ClothingTemplateCategoryManagerEventProxy : CategoryManagerEventProxy
	{
		public override void OnAllButton()
		{
			CustomizationContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.ShowAllTemplates));
		}

		public override void OnButtonPressed(string category)
		{
			CustomizationContext.EventBus.DispatchEvent(new ClothingDesignerUIEvents.CategoryChange(category));
		}
	}
}
