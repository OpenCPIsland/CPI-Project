using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class RectUtil
	{
		public static bool ContainsHorizontal(Rect rect, Vector2 point)
		{
			return point.x >= rect.x && point.x <= rect.x + rect.width;
		}

		public static bool ContainsVertical(Rect rect, Vector2 point)
		{
			return point.y >= rect.y && point.y <= rect.y + rect.height;
		}
	}
}
