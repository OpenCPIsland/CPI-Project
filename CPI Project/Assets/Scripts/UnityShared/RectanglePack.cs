using System.Collections.Generic;
using UnityEngine;

public class RectanglePack
{
	public static int Pack(Rect[] rects, int padding)
	{
		int num = rects.Length;
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = rects[i].width;
		}
		List<int> list = new List<int>();
		for (int j = 0; j < num; j++)
		{
			float num2 = 0f;
			int item = 0;
			for (int k = 0; k < num; k++)
			{
				if (!list.Contains(k))
				{
					float num3 = array[k];
					if (num3 > num2)
					{
						num2 = num3;
						item = k;
					}
				}
			}
			list.Add(item);
		}
		int num4 = 0;
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		for (int l = 0; l < num; l++)
		{
			Rect rect = rects[list[l]];
			int num5 = (int)rect.width;
			int num6 = (int)rect.height;
			bool flag = false;
			int count = list2.Count;
			int num7 = 0;
			for (int m = 0; m < count; m++)
			{
				if (num6 + list3[m] + padding * 2 < num4 && num5 + padding * 2 <= list2[m])
				{
					rect.x = num7 + padding;
					rect.y = list3[m] + padding;
					if (num5 != list2[m])
					{
						list2.Insert(m + 1, list2[m] - (num5 + padding * 2));
						list3.Insert(m + 1, list3[m]);
						list2[m] = num5 + padding * 2;
						count++;
					}
					list3[m] += num6 + padding * 2;
					flag = true;
					break;
				}
				num7 += list2[m];
			}
			if (!flag)
			{
				rect.x = num4 + padding;
				rect.y = padding;
				list2.Add(num5 + padding * 2);
				list3.Add(num6 + padding * 2);
				num4 += num5 + padding * 2;
			}
			rects[list[l]] = rect;
		}
		return num4;
	}
}
