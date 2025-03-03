using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetClothingDesignerTheme
		{
		}

		public struct UpdateClothingDesignerTheme
		{
			public readonly Color[] ThemeColors;

			public UpdateClothingDesignerTheme(Color[] themeColors)
			{
				ThemeColors = themeColors;
			}
		}
	}
}
