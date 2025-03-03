using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(RectTransform))]
	public class PhysicalSizeBitmapFontScaler : PhysicalSizeScaler
	{
		public float TargetScale_Iphone5 = 0f;

		public float TargetScale_Ipad = 0f;

		private RectTransform rectTransform;

		private void Start()
		{
			rectTransform = GetComponent<RectTransform>();
			ApplyScale();
		}

		private void ApplyScale()
		{
			Vector3 targetDimensions = GetTargetDimensions();
			rectTransform.localScale = targetDimensions;
		}

		private Vector3 GetTargetDimensions()
		{
			Vector3 result = default(Vector3);
			float deviceSize = GetDeviceSize();
			float num = NormalizeScaleProperty(TargetScale_Ipad, TargetScale_Iphone5);
			result.x = TargetScale_Iphone5 + (deviceSize - 4f) * num;
			result.y = result.x;
			result.z = result.x;
			return result;
		}
	}
}
