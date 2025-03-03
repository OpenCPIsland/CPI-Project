using ClubPenguin.Avatar;
using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public static class InventoryModelEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CurrentAvatarEquipmentChanged
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EquipmentItemVisibilityChanged
		{
		}

		public struct EquipmentAddedToAvatar
		{
			public readonly DCustomEquipment Data;

			public EquipmentAddedToAvatar(DCustomEquipment data)
			{
				Data = data;
			}
		}

		public struct EquipmentRemovedFromAvatar
		{
			public readonly long EquipmentId;

			public EquipmentRemovedFromAvatar(long equipmentId)
			{
				EquipmentId = equipmentId;
			}
		}

		public struct DisplayedInventoryUpdated
		{
			public readonly int DisplayCount;

			public DisplayedInventoryUpdated(int displayCount)
			{
				DisplayCount = displayCount;
			}
		}

		public struct SelectEquipment
		{
			public readonly long Id;

			public SelectEquipment(long id)
			{
				Id = id;
			}
		}

		public struct DeselectEquipment
		{
			public readonly long Id;

			public DeselectEquipment(long id)
			{
				Id = id;
			}
		}
	}
}
