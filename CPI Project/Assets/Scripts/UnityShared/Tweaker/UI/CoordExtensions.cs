using UnityEngine;

namespace Tweaker.UI
{
	public static class CoordExtensions
	{
		public static Vector2 ToVector(this PixelCoord coord)
		{
			return new Vector2(coord.x, coord.y);
		}
	}
}
