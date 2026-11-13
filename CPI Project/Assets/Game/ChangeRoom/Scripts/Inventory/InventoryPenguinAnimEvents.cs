using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class InventoryPenguinAnimEvents
	{
		public struct EquipmentEquipped
		{
			public readonly int DefinitionId;

			public EquipmentEquipped(int definitionId)
			{
				DefinitionId = definitionId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct OutfitCreatedAnimation
		{
		}
	}
}
