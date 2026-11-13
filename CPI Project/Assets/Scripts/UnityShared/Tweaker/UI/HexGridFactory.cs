using System;
using System.Collections.Generic;

namespace Tweaker.UI
{
	public static class HexGridFactory
	{
		public static Dictionary<string, HexGridCell<TCellValue>> MakeRectangleGrid<TCellValue>(uint width, uint height) where TCellValue : class
		{
			Dictionary<string, HexGridCell<TCellValue>> dictionary = new Dictionary<string, HexGridCell<TCellValue>>();
			int num = (int)width / 2;
			int num2 = (int)height / 2;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int num3 = j - num;
					int num4 = -1 * (i - num2);
					int q = num3;
					int r = num4 - (num3 - (num3 & 1)) / 2;
					HexGridCell<TCellValue> hexGridCell = new HexGridCell<TCellValue>(new AxialCoord(q, r), null);
					dictionary.Add(hexGridCell.AxialCoord.ToString(), hexGridCell);
				}
			}
			return dictionary;
		}

		public static Dictionary<string, HexGridCell<TCellValue>> MakeHexagonGrid<TCellValue>(uint radius) where TCellValue : class
		{
			throw new NotImplementedException();
		}

		public static Dictionary<string, HexGridCell<TCellValue>> MakeTriangleGrid<TCellValue>(uint sideLength) where TCellValue : class
		{
			throw new NotImplementedException();
		}

		public static Dictionary<string, HexGridCell<TCellValue>> MakeRhombusGrid<TCellValue>(uint sideLength) where TCellValue : class
		{
			throw new NotImplementedException();
		}
	}
}
