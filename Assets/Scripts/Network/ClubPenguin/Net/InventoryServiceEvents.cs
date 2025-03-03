using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public static class InventoryServiceEvents
	{
		public struct EquipmentCreated
		{
			public readonly long EquipmentId;

			public EquipmentCreated(long equipmentId)
			{
				EquipmentId = equipmentId;
			}
		}

		public struct InventoryLoaded
		{
			public readonly List<CustomEquipment> Inventory;

			public InventoryLoaded(List<CustomEquipment> inventory)
			{
				Inventory = inventory;
			}
		}

		public struct EquipmentDeleted
		{
			public readonly long EquipmentId;

			public EquipmentDeleted(long equipmentId)
			{
				EquipmentId = equipmentId;
			}
		}
	}
}
