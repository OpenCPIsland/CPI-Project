using System.Collections.Generic;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_Constants
	{
		public static void Get90DegreeColors(mg_bc_EJellyColors _in, out mg_bc_EJellyColors _one, out mg_bc_EJellyColors _two)
		{
			int num = (int)(_in + 3);
			if (num >= 12)
			{
				num -= 12;
			}
			_one = (mg_bc_EJellyColors)num;
			int num2 = (int)(_in - 3);
			if (num2 < 0)
			{
				num2 += 12;
			}
			_two = (mg_bc_EJellyColors)num2;
		}

		public static List<mg_bc_EJellyColors> GetRandomColors(int _count)
		{
			List<mg_bc_EJellyColors> list = new List<mg_bc_EJellyColors>();
			int num = Random.Range(0, 12);
			list.Add((mg_bc_EJellyColors)num);
			switch (_count)
			{
			case 2:
				num += 6;
				if (num >= 12)
				{
					num -= 12;
				}
				list.Add((mg_bc_EJellyColors)num);
				break;
			case 3:
				num += 4;
				if (num >= 12)
				{
					num -= 12;
				}
				list.Add((mg_bc_EJellyColors)num);
				num += 4;
				if (num >= 12)
				{
					num -= 12;
				}
				list.Add((mg_bc_EJellyColors)num);
				break;
			}
			return list;
		}

		public static Color GetColorForJelly(mg_bc_EJellyColors _jelly)
		{
			Color result = new Color(0f, 0f, 0f);
			switch (_jelly)
			{
			case mg_bc_EJellyColors.BLUE:
				result = new Color(0f, 0f, 1f);
				break;
			case mg_bc_EJellyColors.BLUE_GREEN:
				result = new Color(0f, 0.686f, 0.686f);
				break;
			case mg_bc_EJellyColors.GREEN:
				result = new Color(0f, 0.475f, 0f);
				break;
			case mg_bc_EJellyColors.GREEN_YELLOW:
				result = new Color(0f, 1f, 0f);
				break;
			case mg_bc_EJellyColors.YELLOW:
				result = new Color(1f, 1f, 0f);
				break;
			case mg_bc_EJellyColors.YELLOW_ORANGE:
				result = new Color(1f, 0.702f, 0f);
				break;
			case mg_bc_EJellyColors.ORANGE:
				result = new Color(1f, 0.5f, 0f);
				break;
			case mg_bc_EJellyColors.ORANGE_RED:
				result = new Color(1f, 0.275f, 0f);
				break;
			case mg_bc_EJellyColors.RED:
				result = new Color(1f, 0f, 0f);
				break;
			case mg_bc_EJellyColors.RED_VIOLET:
				result = new Color(0.8f, 0f, 0.686f);
				break;
			case mg_bc_EJellyColors.VIOLET:
				result = new Color(0.729f, 0f, 1f);
				break;
			case mg_bc_EJellyColors.VIOLET_BLUE:
				result = new Color(0.451f, 0.03f, 0.647f);
				break;
			}
			return result;
		}
	}
}
