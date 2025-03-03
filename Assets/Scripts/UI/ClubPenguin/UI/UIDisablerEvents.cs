using System.Runtime.InteropServices;

namespace ClubPenguin.UI
{
	public static class UIDisablerEvents
	{
		public struct DisableUIElement
		{
			public string ElementID;

			public bool HideElement;

			public DisableUIElement(string elementID, bool hideElement = false)
			{
				ElementID = elementID;
				HideElement = hideElement;
			}
		}

		public struct EnableUIElement
		{
			public string ElementID;

			public EnableUIElement(string elementID)
			{
				ElementID = elementID;
			}
		}

		public struct DisableUIElementGroup
		{
			public string ElementGroupID;

			public bool HideElements;

			public DisableUIElementGroup(string elementGroupID, bool hideElements = false)
			{
				ElementGroupID = elementGroupID;
				HideElements = hideElements;
			}
		}

		public struct EnableUIElementGroup
		{
			public string ElementGroupID;

			public EnableUIElementGroup(string elementGroupID)
			{
				ElementGroupID = elementGroupID;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DisableAllUIElements
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct EnableAllUIElements
		{
		}

		public struct CameraCullingStateChanged
		{
			public readonly bool IsRenderingEnabled;

			public CameraCullingStateChanged(bool isRenderingEnabled)
			{
				IsRenderingEnabled = isRenderingEnabled;
			}
		}
	}
}
