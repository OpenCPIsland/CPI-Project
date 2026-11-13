using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public static class InventoryDragEvents
	{
		public struct DragInventoryButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public DragInventoryButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct RotatePenguinPreview
		{
			public readonly CustomizerGestureModel GestureModel;

			public RotatePenguinPreview(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct GestureComplete
		{
		}
	}
}
