namespace ClubPenguin.UI
{
	public class EditableItemEvents
	{
		public struct EditStateChanged
		{
			public readonly bool IsEditStateActive;

			public EditStateChanged(bool isEditStateActive)
			{
				IsEditStateActive = isEditStateActive;
			}
		}

		public struct ActionButtonClicked
		{
			public readonly EditableItem Item;

			public readonly EditableItem.ActionType Action;

			public ActionButtonClicked(EditableItem item, EditableItem.ActionType action)
			{
				Item = item;
				Action = action;
			}
		}

		public struct ItemReady
		{
			public readonly EditableItem Item;

			public readonly int Index;

			public ItemReady(EditableItem item, int index)
			{
				Item = item;
				Index = index;
			}
		}

		public struct ItemDisappeared
		{
			public readonly EditableItem Item;

			public ItemDisappeared(EditableItem item)
			{
				Item = item;
			}
		}
	}
}
