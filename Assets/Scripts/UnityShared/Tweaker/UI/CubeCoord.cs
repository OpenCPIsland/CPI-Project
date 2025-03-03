using System;

namespace Tweaker.UI
{
	public struct CubeCoord
	{
		public static CubeCoord[] Directions = new CubeCoord[6]
		{
			new CubeCoord(1, -1, 0),
			new CubeCoord(1, 0, -1),
			new CubeCoord(0, 1, -1),
			new CubeCoord(-1, 1, 0),
			new CubeCoord(-1, 0, 1),
			new CubeCoord(0, -1, 1)
		};

		public static CubeCoord Origin = new CubeCoord(0, 0, 0);

		public int x;

		public int y;

		public int z;

		public CubeCoord(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static CubeCoord FromFraction(double x, double y, double z)
		{
			double num = Math.Round(x);
			double num2 = Math.Round(y);
			double num3 = Math.Round(z);
			double num4 = Math.Abs(num - x);
			double num5 = Math.Abs(num2 - y);
			double num6 = Math.Abs(num3 - z);
			if (num4 > num5 && num4 > num6)
			{
				num = 0.0 - num2 - num3;
			}
			else if (num5 > num6)
			{
				num2 = 0.0 - num - num3;
			}
			else
			{
				num3 = 0.0 - num - num2;
			}
			return new CubeCoord((int)Math.Round((decimal)num), (int)Math.Round((decimal)num2), (int)Math.Round((decimal)num3));
		}

		public int Distance(CubeCoord other)
		{
			return (Math.Abs(x - other.x) + Math.Abs(y - other.y) + Math.Abs(z - other.z)) / 2;
		}

		public override string ToString()
		{
			return string.Format("({0},{1},{2})", x, y, z);
		}

		public static CubeCoord operator +(CubeCoord a, CubeCoord b)
		{
			return new CubeCoord(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static CubeCoord operator -(CubeCoord a, CubeCoord b)
		{
			return new CubeCoord(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static CubeCoord operator *(CubeCoord a, CubeCoord b)
		{
			return new CubeCoord(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static CubeCoord operator *(CubeCoord coord, int scalar)
		{
			return new CubeCoord(coord.x * scalar, coord.y * scalar, coord.z * scalar);
		}

		public static CubeCoord operator *(int scalar, CubeCoord coord)
		{
			return new CubeCoord(coord.x * scalar, coord.y * scalar, coord.z * scalar);
		}

		public static CubeCoord operator /(CubeCoord a, CubeCoord b)
		{
			return FromFraction((double)a.x / (double)b.x, (double)a.y / (double)b.y, (double)a.z / (double)b.z);
		}

		public static CubeCoord operator /(CubeCoord coord, int scalar)
		{
			return FromFraction((double)coord.x / (double)scalar, (double)coord.y / (double)scalar, (double)coord.z / (double)scalar);
		}

		public static CubeCoord operator /(int scalar, CubeCoord coord)
		{
			return FromFraction((double)coord.x / (double)scalar, (double)coord.y / (double)scalar, (double)coord.z / (double)scalar);
		}
	}
}
