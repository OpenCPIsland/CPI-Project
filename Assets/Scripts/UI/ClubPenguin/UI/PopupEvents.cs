using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.UI
{
	public static class PopupEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowingPopup
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HidingPopup
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct PopupManagerReady
		{
		}

		public struct ShowPopup
		{
			public readonly GameObject Popup;

			public readonly bool ScaleToFit;

			public readonly bool DestroyPopupOnBackPressed;

			public readonly string AccessibilityTitleToken;

			public ShowPopup(GameObject popup, bool destroyPopupOnBackPressed = false, bool scaleToFit = true, string accessibilityTitleToken = null)
			{
				Popup = popup;
				ScaleToFit = scaleToFit;
				DestroyPopupOnBackPressed = destroyPopupOnBackPressed;
				AccessibilityTitleToken = accessibilityTitleToken;
			}
		}

		public struct ShowFullScreenPopup
		{
			public readonly GameObject Popup;

			public readonly bool DestroyPopupOnBackPressed;

			public readonly string AccessibilityTitleToken;

			public ShowFullScreenPopup(GameObject popup, bool destroyPopupOnBackPressed = false, string accessibilityTitleToken = null)
			{
				Popup = popup;
				DestroyPopupOnBackPressed = destroyPopupOnBackPressed;
				AccessibilityTitleToken = accessibilityTitleToken;
			}
		}

		public struct ShowCameraSpacePopup
		{
			public readonly GameObject Popup;

			public readonly bool ScaleToFit;

			public readonly bool DestroyPopupOnBackPressed;

			public readonly string AccessibilityTitleToken;

			public readonly string NewCameraTag;

			public readonly float PlaneDistance;

			public readonly int OrderInLayer;

			public ShowCameraSpacePopup(GameObject popup, bool destroyPopupOnBackPressed = false, bool scaleToFit = true, string accessibilityTitleToken = null, string newCameraTag = "", float planeDistance = 150f, int orderInLayer = 5)
			{
				Popup = popup;
				ScaleToFit = scaleToFit;
				DestroyPopupOnBackPressed = destroyPopupOnBackPressed;
				AccessibilityTitleToken = accessibilityTitleToken;
				NewCameraTag = newCameraTag;
				PlaneDistance = planeDistance;
				OrderInLayer = orderInLayer;
			}
		}

		public struct ShowTopPopup
		{
			public readonly GameObject Popup;

			public readonly bool ScaleToFit;

			public readonly bool DestroyPopupOnBackPressed;

			public readonly string AccessibilityTitleToken;

			public ShowTopPopup(GameObject popup, bool destroyPopupOnBackPressed = false, bool scaleToFit = true, string accessibilityTitleToken = null)
			{
				Popup = popup;
				ScaleToFit = scaleToFit;
				DestroyPopupOnBackPressed = destroyPopupOnBackPressed;
				AccessibilityTitleToken = accessibilityTitleToken;
			}
		}
	}
}
