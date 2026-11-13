using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public static class UIExtensions
	{
		public static void CenterOnElement(this ScrollRect scrollRect, int elementIndex, RectTransform[] elementTransforms, Vector2 elementSpacing)
		{
			Vector2 zero = Vector2.zero;
			if (!scrollRect.horizontal)
			{
				zero.x = scrollRect.content.anchoredPosition.x;
			}
			if (!scrollRect.vertical)
			{
				zero.y = scrollRect.content.anchoredPosition.y;
			}
			for (int i = 0; i < elementIndex; i++)
			{
				if (scrollRect.horizontal)
				{
					zero.x += elementTransforms[i].rect.width + elementSpacing.x;
				}
				if (scrollRect.vertical)
				{
					zero.y += elementTransforms[i].rect.height + elementSpacing.y;
				}
			}
			RectTransform component = scrollRect.GetComponent<RectTransform>();
			if (scrollRect.horizontal)
			{
				zero.x += elementTransforms[elementIndex].sizeDelta.x * 0.5f + elementSpacing.x;
				zero.x -= component.rect.width * 0.5f;
				if (zero.x < 0f)
				{
					zero.x = 0f;
				}
			}
			if (scrollRect.vertical)
			{
				zero.y += elementTransforms[elementIndex].sizeDelta.y * 0.5f + elementSpacing.y;
				zero.y -= component.rect.height * 0.5f;
				if (zero.y < 0f)
				{
					zero.y = 0f;
				}
			}
			scrollRect.content.anchoredPosition = -zero;
		}
	}
}
