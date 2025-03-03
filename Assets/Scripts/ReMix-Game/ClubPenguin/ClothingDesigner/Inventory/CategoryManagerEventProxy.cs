namespace ClubPenguin.ClothingDesigner.Inventory
{
	public abstract class CategoryManagerEventProxy
	{
		public abstract void OnAllButton();

		public abstract void OnButtonPressed(string category);

		public virtual void OnEquippedButton()
		{
		}

		public virtual void OnHiddenButton()
		{
		}
	}
}
