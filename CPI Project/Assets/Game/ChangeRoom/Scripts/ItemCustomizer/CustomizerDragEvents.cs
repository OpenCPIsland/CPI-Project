using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public static class CustomizerDragEvents
	{
		public struct StartDragFabricButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public StartDragFabricButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct EndDragFabricButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public EndDragFabricButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct DragFabricButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public DragFabricButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct DragOffChannel
		{
			public readonly CustomizerGestureModel GestureModel;

			public DragOffChannel(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct StartDragDecalButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public StartDragDecalButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct EndDragDecalButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public EndDragDecalButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct DragDecalButton
		{
			public readonly CustomizerGestureModel GestureModel;

			public DragDecalButton(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		public struct DragTemplate
		{
			public readonly CustomizerGestureModel GestureModel;

			public DragTemplate(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct GestureComplete
		{
		}

		public struct RotatePenguin
		{
			public readonly CustomizerGestureModel GestureModel;

			public RotatePenguin(CustomizerGestureModel gestureModel)
			{
				GestureModel = gestureModel;
			}
		}
	}
}
