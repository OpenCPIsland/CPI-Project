using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class RectTransformUtil
	{
		private delegate Vector2 toPoint(Vector3 worldPoint, Canvas canvas);

		public static Rect GetCanvasRect(RectTransform rectTransform, Canvas canvas)
		{
			return getRect(rectTransform, canvas, toCanvasPoint);
		}

		public static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas)
		{
			return getRect(rectTransform, canvas, toScreenPoint);
		}

		public static Rect GetNormalizedRect(RectTransform rectTransform, Canvas canvas)
		{
			Rect result = default(Rect);
			Rect rect = getRect(rectTransform, canvas, toScreenPoint);
			float num = Screen.width;
			float num2 = Screen.height;
			result.size = new Vector2(rect.width / num, rect.height / num2);
			result.position = new Vector2(rect.position.x / num, rect.position.y / num2);
			return result;
		}

		private static Rect getRect(RectTransform rectTransform, Canvas canvas, toPoint toPointDelegate)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Vector2 vector;
			Vector2 vector2;
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				vector = array[0];
				vector2 = array[2];
			}
			else
			{
				vector = toPointDelegate(array[0], canvas);
				vector2 = toPointDelegate(array[2], canvas);
			}
			return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		}

		public static Vector2[] GetCanvasCorners(RectTransform rectTransform, Canvas canvas)
		{
			return getCorners(rectTransform, canvas, toCanvasPoint);
		}

		public static Vector2[] GetScreenCorners(RectTransform rectTransform, Canvas canvas)
		{
			return getCorners(rectTransform, canvas, toScreenPoint);
		}

		private static Vector2[] getCorners(RectTransform rectTransform, Canvas canvas, toPoint toPointDelegate)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Vector2[] array2 = new Vector2[4];
			for (int i = 0; i < 4; i++)
			{
				if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					array2[i] = array[i];
				}
				else
				{
					array2[i] = toPointDelegate(array[i], canvas);
				}
			}
			return array2;
		}

		private static Vector2 toCanvasPoint(Vector3 worldPoint, Canvas canvas)
		{
			Vector3 a = canvas.worldCamera.WorldToScreenPoint(worldPoint);
			Vector3 v = a * (1f / canvas.scaleFactor);
			return v;
		}

		private static Vector2 toScreenPoint(Vector3 worldPoint, Canvas canvas)
		{
			return canvas.worldCamera.WorldToScreenPoint(worldPoint);
		}
	}
}
