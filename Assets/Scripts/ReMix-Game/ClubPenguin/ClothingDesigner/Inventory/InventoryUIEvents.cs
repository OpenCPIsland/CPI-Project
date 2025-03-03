using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public static class InventoryUIEvents
	{
		public struct ModelCreated
		{
			public readonly InventoryEquipmentModel EquipmentModel;

			public ModelCreated(InventoryEquipmentModel equipmentModel)
			{
				EquipmentModel = equipmentModel;
			}
		}

		public struct EquipEquipment
		{
			public readonly long EquipmentId;

			public EquipEquipment(long equipmentId)
			{
				EquipmentId = equipmentId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StartedLoadingEquipment
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AllEquipmentLoaded
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShrinkClicked
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClearAllEquippedEquipment
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableScrollRect
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableScrollRect
		{
		}
	}
}
