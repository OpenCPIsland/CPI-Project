using System.Collections.Generic;

namespace Tweaker.UI
{
	public class HexGrid<TCellValue> where TCellValue : class
	{
		private Dictionary<string, HexGridCell<TCellValue>> cells;

		public HexGrid(uint width, uint height)
		{
			cells = HexGridFactory.MakeRectangleGrid<TCellValue>(width, height);
		}

		public void ClearCells()
		{
			foreach (HexGridCell<TCellValue> value in cells.Values)
			{
				value.Value = null;
			}
		}

		public HexGridCell<TCellValue> GetCell(AxialCoord axialCoord)
		{
			HexGridCell<TCellValue> value = null;
			cells.TryGetValue(axialCoord.ToString(), out value);
			return value;
		}

		public HexGridCell<TCellValue> GetCell(CubeCoord coord)
		{
			AxialCoord outCoord;
			HexCoord.CubeToAxial(ref coord, out outCoord);
			return GetCell(outCoord);
		}

		public TCellValue GetCellValue(AxialCoord coord)
		{
			return GetCell(coord).Value;
		}

		public TCellValue GetCellValue(CubeCoord coord)
		{
			return GetCell(coord).Value;
		}

		public void SetCellValue(TCellValue value, CubeCoord coord)
		{
			HexGridCell<TCellValue> cell = GetCell(coord);
			cell.Value = value;
		}

		public void SetCellValue(TCellValue value, AxialCoord coord)
		{
			HexGridCell<TCellValue> cell = GetCell(coord);
			cell.Value = value;
		}

		public IEnumerable<HexGridCell<TCellValue>> GetRingCells(CubeCoord center, uint radius)
		{
			CubeCoord direction = CubeCoord.Directions[4] * (int)radius;
			CubeCoord cube = center + direction;
			for (uint i = 0u; i < 6; i++)
			{
				for (uint j = 0u; j < radius; j++)
				{
					HexGridCell<TCellValue> cell = GetCell(cube);
					if (cell != null)
					{
						yield return cell;
					}
					cube = HexCoord.GetNeighbour(cube, i);
				}
			}
		}

		public IEnumerable<HexGridCell<TCellValue>> GetSpiralCells(CubeCoord center, uint radius)
		{
			HexGridCell<TCellValue> centerCell = GetCell(center);
			if (centerCell != null)
			{
				yield return centerCell;
			}
			for (uint i = 1u; i <= radius; i++)
			{
				foreach (HexGridCell<TCellValue> cell in GetRingCells(center, i))
				{
					if (cell != null)
					{
						yield return cell;
					}
				}
			}
		}

		public IEnumerable<TCellValue> GetRingValues(CubeCoord center, uint radius)
		{
			foreach (HexGridCell<TCellValue> cell in GetRingCells(center, radius))
			{
				yield return cell.Value;
			}
		}

		public IEnumerable<TCellValue> GetSpiralValues(CubeCoord center, uint radius)
		{
			foreach (HexGridCell<TCellValue> cell in GetSpiralCells(center, radius))
			{
				yield return cell.Value;
			}
		}
	}
}
