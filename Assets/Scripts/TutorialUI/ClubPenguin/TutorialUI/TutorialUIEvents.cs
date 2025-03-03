using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	public static class TutorialUIEvents
	{
		public struct ShowHighlightOverlay
		{
			public DTutorialOverlay TutorialOverlayData;

			public ShowHighlightOverlay(DTutorialOverlay tutorialOverlayData)
			{
				TutorialOverlayData = tutorialOverlayData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideHighlightOverlay
		{
		}

		public struct ShowTooltip
		{
			public GameObject Tooltip;

			public RectTransform Target;

			public Vector2 Offset;

			public bool FullScreenClose;

			public ShowTooltip(GameObject tooltip, RectTransform target, Vector2 offset, bool fullScreenClose)
			{
				Tooltip = tooltip;
				Target = target;
				Offset = offset;
				FullScreenClose = fullScreenClose;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideTooltip
		{
		}

		internal struct OnTooltipCreated
		{
			public TutorialTooltip Tooltip;

			public OnTooltipCreated(TutorialTooltip tooltip)
			{
				Tooltip = tooltip;
			}
		}

		public struct ShowTutorialPopup
		{
			public string PopupID;

			public GameObject Popup;

			public RectTransform Target;

			public Vector2 Offset;

			public float Scale;

			public ShowTutorialPopup(string popupID, GameObject popup, RectTransform target, Vector2 offset, float scale)
			{
				PopupID = popupID;
				Popup = popup;
				Target = target;
				Offset = offset;
				Scale = scale;
			}
		}

		public struct ShowTutorialPopupAtPosition
		{
			public string PopupID;

			public GameObject Popup;

			public Vector2 Position;

			public float Scale;

			public ShowTutorialPopupAtPosition(string popupID, GameObject popup, Vector2 position, float scale)
			{
				PopupID = popupID;
				Popup = popup;
				Position = position;
				Scale = scale;
			}
		}

		public struct HideTutorialPopup
		{
			public string PopupID;

			public HideTutorialPopup(string popupID)
			{
				PopupID = popupID;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideAllTutorialPopups
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SortTutorialUIToTop
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetTutorialUISorting
		{
		}
	}
}
