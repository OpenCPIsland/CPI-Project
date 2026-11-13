namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class InventoryIconModel<T>
	{
		public readonly long Id;

		public readonly T Data;

		public readonly bool IsMemberItem;

		public bool IsEquipped;

		public bool IsHidden;

		public InventoryIconModel(long id, T data, bool isEquipped, bool isMemberItem, bool isHidden = false)
		{
			Id = id;
			Data = data;
			IsMemberItem = isMemberItem;
			IsEquipped = isEquipped;
			IsHidden = isHidden;
		}
	}
}
