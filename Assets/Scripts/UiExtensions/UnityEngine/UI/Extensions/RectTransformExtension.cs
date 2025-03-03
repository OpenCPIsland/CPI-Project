namespace UnityEngine.UI.Extensions
{
	public static class RectTransformExtension
	{
		public static Vector2 switchToRectTransform(this RectTransform from, RectTransform to)
		{
			float width = from.rect.width;
			Vector2 pivot = from.pivot;
			float x = width * pivot.x + from.rect.xMin;
			float height = from.rect.height;
			Vector2 pivot2 = from.pivot;
			Vector2 vector = new Vector2(x, height * pivot2.y + from.rect.yMin);
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, from.position);
			screenPoint += vector;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenPoint, null, out localPoint);
			float width2 = to.rect.width;
			Vector2 pivot3 = to.pivot;
			float x2 = width2 * pivot3.x + to.rect.xMin;
			float height2 = to.rect.height;
			Vector2 pivot4 = to.pivot;
			Vector2 b = new Vector2(x2, height2 * pivot4.y + to.rect.yMin);
			return to.anchoredPosition + localPoint - b;
		}
	}
}
