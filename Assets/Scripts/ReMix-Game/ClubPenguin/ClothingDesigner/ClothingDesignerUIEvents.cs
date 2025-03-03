using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerUIEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ChangeStateInventory
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ChangeStateCustomizer
		{
		}

		public struct ShowSubmittedInCatalog
		{
			public long ClothingCatalogItemSubmittedId;

			public ShowSubmittedInCatalog(long clothingCatalogItemSubmittedId)
			{
				ClothingCatalogItemSubmittedId = clothingCatalogItemSubmittedId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowCatalog
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideCatalog
		{
		}

		public struct UpdateCameraState
		{
			public readonly ClothingDesignerCameraState CameraState;

			public readonly bool AnimateCamera;

			public UpdateCameraState(ClothingDesignerCameraState cameraState, bool animateCamera)
			{
				CameraState = cameraState;
				AnimateCamera = animateCamera;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CameraPositionChangeComplete
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CloseButton
		{
		}

		public struct ShowCoinWidget
		{
			public readonly int CoinCount;

			public ShowCoinWidget(int coinCount)
			{
				CoinCount = coinCount;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideCoinWidget
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowAllTemplates
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowEquippedItems
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowHiddenItems
		{
		}

		public struct CategoryChange
		{
			public readonly string Category;

			public CategoryChange(string category)
			{
				Category = category;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomizerTemplateState
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomizerChosenState
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomizerEditingState
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowCloseButton
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideCloseButton
		{
		}

		public struct ShowMemberFlow
		{
			public readonly string Trigger;

			public ShowMemberFlow(string trigger)
			{
				Trigger = trigger;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableDragAreaControllerUpdates
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableDragAreaControllerUpdates
		{
		}
	}
}
