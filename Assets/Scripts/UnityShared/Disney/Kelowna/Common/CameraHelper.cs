using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class CameraHelper
	{
		public static Camera GetCameraByTag(string tag)
		{
			GameObject gameObject = GameObject.FindWithTag(tag);
			if (gameObject != null)
			{
				return gameObject.GetComponent<Camera>();
			}
			return null;
		}

		public static Rect WorldBoundsToScreenRect(string cameraTag, Bounds bounds)
		{
			Camera cameraByTag = GetCameraByTag(cameraTag);
			return WorldBoundsToScreenRect(cameraByTag, bounds);
		}

		public static Rect WorldBoundsToScreenRect(Camera camera, Bounds bounds)
		{
			Vector3 vector = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
			Vector3 vector2 = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));
			return new Rect(vector.x, (float)Screen.height - vector.y, vector2.x - vector.x, vector.y - vector2.y);
		}
	}
}
