using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectScrollWheelElasticity : MonoBehaviour, IScrollHandler, IEventSystemHandler
	{
		private ScrollRect scrollRect;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
		}

		public void OnScroll(PointerEventData eventData)
		{
			if (scrollRect.content == null)
			{
				return;
			}
			Rect rect = (scrollRect.viewport != null) ? scrollRect.viewport.rect : ((RectTransform)scrollRect.transform).rect;
			if (scrollRect.vertical)
			{
				if (rect.height >= scrollRect.content.rect.height)
				{
					scrollRect.verticalNormalizedPosition = (scrollRect.verticalNormalizedPosition + 1f) % 2f;
				}
				else if (scrollRect.verticalNormalizedPosition > 1f)
				{
					scrollRect.verticalNormalizedPosition = 1f;
				}
				else if (scrollRect.verticalNormalizedPosition < 0f)
				{
					scrollRect.verticalNormalizedPosition = 0f;
				}
			}
			if (scrollRect.horizontal)
			{
				if (rect.width >= scrollRect.content.rect.width)
				{
					scrollRect.horizontalNormalizedPosition = (scrollRect.horizontalNormalizedPosition + 1f) % 2f;
				}
				else if (scrollRect.horizontalNormalizedPosition > 1f)
				{
					scrollRect.horizontalNormalizedPosition = 1f;
				}
				else if (scrollRect.horizontalNormalizedPosition < 0f)
				{
					scrollRect.horizontalNormalizedPosition = 0f;
				}
			}
		}
	}
}
