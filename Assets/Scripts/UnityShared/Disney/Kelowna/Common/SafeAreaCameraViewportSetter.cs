using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class SafeAreaCameraViewportSetter : AbstractSafeAreaComponent
	{
		private void Start()
		{
			RectOffset safeAreaOffset = safeAreaService.GetSafeAreaOffset();
			float normalizedVerticalOffset = safeAreaService.GetNormalizedVerticalOffset(safeAreaOffset.top);
			Camera component = GetComponent<Camera>();
			Rect rect = component.rect;
			rect.height = 1f - normalizedVerticalOffset;
			component.rect = rect;
		}
	}
}
