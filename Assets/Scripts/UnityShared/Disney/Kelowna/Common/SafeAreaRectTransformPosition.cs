using UnityEngine;

namespace Disney.Kelowna.Common
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class SafeAreaRectTransformPosition : AbstractSafeAreaComponent
	{
		private void Start()
		{
			RectTransform component = GetComponent<RectTransform>();
			Canvas componentInParent = GetComponentInParent<Canvas>();
			RectOffset safeAreaOffset = safeAreaService.GetSafeAreaOffset();
			float normalizedVerticalOffset = safeAreaService.GetNormalizedVerticalOffset(safeAreaOffset.top);
			float num = 1f - normalizedVerticalOffset;
			Rect normalizedRect = RectTransformUtil.GetNormalizedRect(component, componentInParent);
			if (normalizedRect.yMax > num)
			{
				float num2 = normalizedRect.yMax - num;
				RectTransform rectTransform = componentInParent.transform as RectTransform;
				float num3 = num2 * rectTransform.rect.height;
				Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.y -= num3;
				component.anchoredPosition = anchoredPosition;
			}
		}
	}
}
