using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class SafeAreaRectTransformAnchors : AbstractSafeAreaComponent
	{
		public bool CheckTop = true;

		public bool CheckBottom;

		private void Start()
		{
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchorMin = component.anchorMin;
			Vector2 anchorMax = component.anchorMax;
			RectOffset safeAreaOffset = safeAreaService.GetSafeAreaOffset();
			float normalizedVerticalOffset = safeAreaService.GetNormalizedVerticalOffset(safeAreaOffset.top);
			float num = 1f - normalizedVerticalOffset;
			float normalizedVerticalOffset2 = safeAreaService.GetNormalizedVerticalOffset(safeAreaOffset.bottom);
			if (CheckTop && anchorMax.y > num)
			{
				float num2 = anchorMax.y - num;
				anchorMax.y -= num2;
				float num3 = CheckBottom ? normalizedVerticalOffset2 : 0f;
				if (anchorMin.y > num3)
				{
					anchorMin.y = Math.Max(anchorMin.y - num2, num3);
				}
			}
			if (CheckBottom && anchorMin.y < normalizedVerticalOffset2)
			{
				float num2 = normalizedVerticalOffset2 - anchorMin.y;
				anchorMin.y += num2;
				float num4 = CheckTop ? num : 1f;
				if (anchorMax.y < num4)
				{
					anchorMax.y = Math.Min(anchorMax.y + num2, num4);
				}
			}
			component.anchorMin = anchorMin;
			component.anchorMax = anchorMax;
		}
	}
}
