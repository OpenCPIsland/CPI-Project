using Mix.Native;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class KeyboardEvents : MonoBehaviour
	{
		public struct KeyboardResized
		{
			public readonly int Height;

			public KeyboardResized(int height)
			{
				Height = height;
			}
		}

		public struct KeyboardShown
		{
			public readonly int Height;

			public KeyboardShown(int height)
			{
				Height = height;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct KeyboardHidden
		{
		}

		public struct KeyPressed
		{
			public readonly string Text;

			public KeyPressed(string text)
			{
				Text = text;
			}
		}

		public struct ReturnKeyPressed
		{
			public readonly NativeKeyboardReturnKey ReturnKey;

			public ReturnKeyPressed(NativeKeyboardReturnKey returnKey)
			{
				ReturnKey = returnKey;
			}
		}
	}
}
