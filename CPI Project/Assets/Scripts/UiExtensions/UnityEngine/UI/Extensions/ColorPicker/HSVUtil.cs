using System;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public static class HSVUtil
	{
		public static HsvColor ConvertRgbToHsv(Color color)
		{
			return ConvertRgbToHsv((int)(color.r * 255f), (int)(color.g * 255f), (int)(color.b * 255f));
		}

		public static HsvColor ConvertRgbToHsv(double r, double b, double g)
		{
			double num = 0.0;
			double num2 = Math.Min(Math.Min(r, g), b);
			double num3 = Math.Max(Math.Max(r, g), b);
			double num4 = num3 - num2;
			double num5 = (num3 != 0.0) ? (num4 / num3) : 0.0;
			if (num5 == 0.0)
			{
				num = 360.0;
			}
			else
			{
				if (r == num3)
				{
					num = (g - b) / num4;
				}
				else if (g == num3)
				{
					num = 2.0 + (b - r) / num4;
				}
				else if (b == num3)
				{
					num = 4.0 + (r - g) / num4;
				}
				num *= 60.0;
				if (num <= 0.0)
				{
					num += 360.0;
				}
			}
			HsvColor result = default(HsvColor);
			result.H = 360.0 - num;
			result.S = num5;
			result.V = num3 / 255.0;
			return result;
		}

		public static Color ConvertHsvToRgb(double h, double s, double v, float alpha)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			if (s == 0.0)
			{
				num = v;
				num2 = v;
				num3 = v;
			}
			else
			{
				h = ((h != 360.0) ? (h / 60.0) : 0.0);
				int num4 = (int)h;
				double num5 = h - (double)num4;
				double num6 = v * (1.0 - s);
				double num7 = v * (1.0 - s * num5);
				double num8 = v * (1.0 - s * (1.0 - num5));
				switch (num4)
				{
				case 0:
					num = v;
					num2 = num8;
					num3 = num6;
					break;
				case 1:
					num = num7;
					num2 = v;
					num3 = num6;
					break;
				case 2:
					num = num6;
					num2 = v;
					num3 = num8;
					break;
				case 3:
					num = num6;
					num2 = num7;
					num3 = v;
					break;
				case 4:
					num = num8;
					num2 = num6;
					num3 = v;
					break;
				default:
					num = v;
					num2 = num6;
					num3 = num7;
					break;
				}
			}
			return new Color((float)num, (float)num2, (float)num3, alpha);
		}
	}
}
