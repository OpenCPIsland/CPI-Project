using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class SafeAreaService
	{
		private const string DEVICE_OVERRIDE_KEY = "SafeAreaService.DeviceOverride";

		private RectOffset safeAreaOffset;

		public SafeAreaService(RectOffset safeAreaOffset)
		{
			this.safeAreaOffset = safeAreaOffset;
		}

		public RectOffset GetSafeAreaOffset()
		{
			return safeAreaOffset;
		}

		public float GetNormalizedHorizontalOffset(float offset)
		{
			return offset / (float)Screen.width;
		}

		public float GetNormalizedVerticalOffset(float offset)
		{
			return offset / (float)Screen.height;
		}
	}
}
