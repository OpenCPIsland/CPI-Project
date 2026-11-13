using System;

namespace Tweaker.UI
{
	public static class HexCoord
	{
		public static void CubeToAxial(ref CubeCoord coord, out AxialCoord outCoord)
		{
			outCoord = new AxialCoord(coord.x, coord.y);
		}

		public static AxialCoord CubeToAxial(CubeCoord coord)
		{
			return new AxialCoord(coord.x, coord.y);
		}

		public static void AxialToCube(ref AxialCoord coord, out CubeCoord outCoord)
		{
			outCoord = new CubeCoord(coord.q, coord.r, -coord.q - coord.r);
		}

		public static CubeCoord AxialToCube(AxialCoord coord)
		{
			return new CubeCoord(coord.q, coord.r, -coord.q - coord.r);
		}

		public static PixelCoord AxialToPixel(CubeCoord coord, float size)
		{
			AxialCoord coord2 = CubeToAxial(coord);
			return AxialToPixel(coord2, size);
		}

		public static PixelCoord AxialToPixel(AxialCoord coord, float size)
		{
			float x = size * 3f / 2f * (float)coord.q;
			float y = size * (float)Math.Sqrt(3.0) * ((float)coord.r + (float)coord.q / 2f);
			return new PixelCoord(x, y);
		}

		public static CubeCoord GetNeighbour(CubeCoord coord, uint direction)
		{
			return coord + CubeCoord.Directions[direction];
		}

		public static AxialCoord GetNeighbour(AxialCoord coord, uint direction)
		{
			return coord + AxialCoord.Directions[direction];
		}
	}
}
