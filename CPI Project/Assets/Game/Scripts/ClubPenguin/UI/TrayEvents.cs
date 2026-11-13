using System.Runtime.InteropServices;

namespace ClubPenguin.UI
{
	public static class TrayEvents
	{
		public struct OpenTray
		{
			public bool IsPersistent;

			public OpenTray(bool isPersistent = true)
			{
				IsPersistent = isPersistent;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct RestoreTray
		{
		}

		public struct CloseTray
		{
			public bool IsControlsVisible;

			public bool IsPersistent;

			public CloseTray(bool isControlsVisible = false, bool isPersistent = false)
			{
				IsControlsVisible = isControlsVisible;
				IsPersistent = isPersistent;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct TrayOpened
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct TrayClosed
		{
		}

		public struct SelectTrayScreen
		{
			public const string SUBSCENE_PLAYERPREFS_KEY = "SelectTrayScreen_SubScene";

			public readonly string ScreenName;

			public readonly string SubScreenName;

			public readonly bool JumpToScreen;

			public SelectTrayScreen(string screenName, string subScreenName = "", bool jumpToScreen = true)
			{
				ScreenName = screenName;
				SubScreenName = subScreenName;
				JumpToScreen = jumpToScreen;
			}
		}

		public struct ScreenLoadComplete
		{
			public readonly string ScreenName;

			public ScreenLoadComplete(string screenName)
			{
				ScreenName = screenName;
			}
		}

		public struct TrayResized
		{
			public float Size;

			public TrayResized(float size)
			{
				Size = size;
			}
		}

		public struct TrayHeightAdjust
		{
			public float NewHeight;

			public TrayHeightAdjust(float height)
			{
				NewHeight = height;
			}
		}
	}
}
