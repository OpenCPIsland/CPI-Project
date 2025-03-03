using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace ClubPenguin
{
	public class ColorUtils
	{
		[Serializable]
		public struct ColorAtPercent
		{
			[Range(0f, 1f)]
			public float Percent;

			public Color Color;

			public ColorAtPercent(float percent, Color color)
			{
				Percent = percent;
				Color = color;
			}
		}

		public static string ColorToHex(Color32 color)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(color.r.ToString("X2"));
			stringBuilder.Append(color.g.ToString("X2"));
			stringBuilder.Append(color.b.ToString("X2"));
			return stringBuilder.ToString();
		}

		public static Color HexToColor(string colorHex)
		{
			byte r = byte.Parse(colorHex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(colorHex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(colorHex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, byte.MaxValue);
		}

		public static Color GetColorAtPercent(ColorAtPercent[] colorData, float percent, bool transitionBetweenColors = false)
		{
			Color white = Color.white;
			int num = 0;
			for (int i = 1; i < colorData.Length; i++)
			{
				if (percent > colorData[i].Percent)
				{
					num = i;
				}
			}
			if (transitionBetweenColors)
			{
				ColorAtPercent colorAtPercent = colorData[num];
				ColorAtPercent colorAtPercent2 = (num < colorData.Length - 1) ? colorData[num + 1] : colorData[num];
				float t = (percent - colorAtPercent.Percent) / (colorAtPercent2.Percent - colorAtPercent.Percent);
				return Color.Lerp(colorAtPercent.Color, colorAtPercent2.Color, t);
			}
			return colorData[num].Color;
		}
	}
}
