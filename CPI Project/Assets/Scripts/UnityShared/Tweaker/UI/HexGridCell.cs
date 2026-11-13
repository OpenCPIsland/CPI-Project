namespace Tweaker.UI
{
	public class HexGridCell<TCellValue> where TCellValue : class
	{
		private TCellValue value;

		public CubeCoord[] cubeNeighbours;

		public AxialCoord[] axialNeighbours;

		public TCellValue Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		public CubeCoord CubeCoord
		{
			get;
			private set;
		}

		public AxialCoord AxialCoord
		{
			get;
			private set;
		}

		public HexGridCell(AxialCoord axialCoord)
		{
			AxialCoord = axialCoord;
			CubeCoord = HexCoord.AxialToCube(axialCoord);
			CacheNeighbourCoords();
		}

		public HexGridCell(AxialCoord axialCoord, TCellValue value)
		{
			AxialCoord = axialCoord;
			CubeCoord = HexCoord.AxialToCube(axialCoord);
			Value = value;
			CacheNeighbourCoords();
		}

		private void CacheNeighbourCoords()
		{
			cubeNeighbours = new CubeCoord[6];
			axialNeighbours = new AxialCoord[6];
			for (int i = 0; i < 6; i++)
			{
				cubeNeighbours[i] = CubeCoord + CubeCoord.Directions[i];
				axialNeighbours[i] = AxialCoord + AxialCoord.Directions[i];
			}
		}
	}
}
