using System.Runtime.InteropServices;

namespace ClubPenguin.ObjectManipulation
{
	public static class ObjectManipulationEvents
	{
		public struct DeleteSelectedItemEvent
		{
			public readonly bool DeleteChildren;

			public DeleteSelectedItemEvent(bool deleteChildren)
			{
				DeleteChildren = deleteChildren;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ConfirmPlacementSelectedItemEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct BeginDragInventoryItem
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RemovingStructureWithItemsEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EndDragInventoryItem
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetSelectedItem
		{
		}
	}
}
