using UnityEngine;

namespace Disney.Native
{
	public static class Util
	{
		public static Rect GetRectInPhysicalScreenSpace(RectTransform aRectTransform)
		{
			Vector3[] array = new Vector3[4];
			aRectTransform.GetWorldCorners(array);
			Canvas parentCanvas = GetParentCanvas(aRectTransform.gameObject);
			if (parentCanvas == null)
			{
				return new Rect(0f, 0f, 0f, 0f);
			}
			Camera worldCamera = parentCanvas.worldCamera;
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(worldCamera, array[0]);
			Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(worldCamera, array[1]);
			Vector2 vector3 = RectTransformUtility.WorldToScreenPoint(worldCamera, array[2]);
			float num = (float)Display.displays[0].systemWidth / (float)Screen.width;
			float num2 = (float)Display.displays[0].systemHeight / (float)Screen.height;
			int num3 = Mathf.CeilToInt((vector3.x - vector2.x) * num);
			int num4 = Mathf.CeilToInt((vector2.y - vector.y) * num2);
			int num5 = (int)(vector2.x * num);
			int num6 = (int)(vector2.y * num2);
			num6 = Display.displays[0].systemHeight - num6;
			return new Rect(num5, num6, num3, num4);
		}

		public static Rect GetRectInScreenSpace(RectTransform aRectTransform)
		{
			Vector3[] array = new Vector3[4];
			aRectTransform.GetWorldCorners(array);
			Canvas parentCanvas = GetParentCanvas(aRectTransform.gameObject);
			if (parentCanvas == null)
			{
				return new Rect(0f, 0f, 0f, 0f);
			}
			Camera worldCamera = parentCanvas.worldCamera;
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(worldCamera, array[0]);
			Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(worldCamera, array[1]);
			Vector2 vector3 = RectTransformUtility.WorldToScreenPoint(worldCamera, array[2]);
			int num = (int)(vector3.x - vector2.x);
			int num2 = (int)(vector2.y - vector.y);
			int num3 = (int)vector.x;
			int num4 = (int)vector.y;
			return new Rect(num3, num4, num, num2);
		}

		public static Canvas GetParentCanvas(GameObject aGameObject)
		{
			Canvas canvas = null;
			Transform parent = aGameObject.transform.parent;
			while (canvas == null && parent != null)
			{
				canvas = parent.GetComponent<Canvas>();
				parent = parent.parent;
			}
			return canvas;
		}
	}
}
