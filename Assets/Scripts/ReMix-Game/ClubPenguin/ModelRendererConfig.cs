using UnityEngine;

namespace ClubPenguin
{
	public class ModelRendererConfig
	{
		public Transform ObjectToRenderTransform;

		public Vector3 ObjectCameraOffset;

		public Vector2 TextureDimensions;

		public bool IsOrthographic;

		public bool FrameObjectInCamera;

		public float FieldOfView;

		public bool UseSolidBackground;

		public Color CameraBackgroundColor;

		public bool AutoDestroyObjectToRender;

		public bool UseOcclusionCulling;

		public ModelRendererConfig(Transform objectToRenderTransform, Vector3 objectCameraOffset, Vector2 textureDimensions)
		{
			ObjectToRenderTransform = objectToRenderTransform;
			ObjectCameraOffset = objectCameraOffset;
			TextureDimensions = textureDimensions;
			IsOrthographic = false;
			FrameObjectInCamera = false;
			FieldOfView = 60f;
			CameraBackgroundColor = Color.white;
			AutoDestroyObjectToRender = true;
			UseOcclusionCulling = true;
		}
	}
}
