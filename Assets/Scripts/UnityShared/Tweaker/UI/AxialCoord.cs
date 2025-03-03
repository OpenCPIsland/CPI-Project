namespace Tweaker.UI
{
	public struct AxialCoord
	{
		public static AxialCoord[] Directions = new AxialCoord[6]
		{
			new AxialCoord(1, 0),
			new AxialCoord(1, -1),
			new AxialCoord(0, -1),
			new AxialCoord(-1, 0),
			new AxialCoord(-1, 1),
			new AxialCoord(0, 1)
		};

		public int q;

		public int r;

		public AxialCoord(int q, int r)
		{
			this.q = q;
			this.r = r;
		}

		public int Distance(AxialCoord other)
		{
			CubeCoord outCoord;
			HexCoord.AxialToCube(ref this, out outCoord);
			CubeCoord outCoord2;
			HexCoord.AxialToCube(ref other, out outCoord2);
			return outCoord.Distance(outCoord2);
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", q, r);
		}

		public static AxialCoord operator +(AxialCoord a, AxialCoord b)
		{
			return new AxialCoord(a.q + b.q, a.r + b.r);
		}

		public static AxialCoord operator -(AxialCoord a, AxialCoord b)
		{
			return new AxialCoord(a.q - b.q, a.r - b.r);
		}
	}
}
