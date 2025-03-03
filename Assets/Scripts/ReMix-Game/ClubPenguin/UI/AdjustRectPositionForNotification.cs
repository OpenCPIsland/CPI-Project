using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class AdjustRectPositionForNotification : AbstractAdjustForNotification
	{
		private RectTransform rectTransform;

		private Vector2 currentPosition;

		protected override void start()
		{
			rectTransform = GetComponent<RectTransform>();
			currentPosition = rectTransform.anchoredPosition;
		}

		private void Update()
		{
			if (rectTransform != null && isDown && rectTransform.anchoredPosition != currentPosition)
			{
				rectTransform.anchoredPosition = currentPosition;
			}
		}

		protected override void doMoveUp(float height)
		{
			currentPosition = rectTransform.anchoredPosition + new Vector2(0f, height);
			rectTransform.anchoredPosition = currentPosition;
		}

		protected override void doMoveDown(float height)
		{
			currentPosition = rectTransform.anchoredPosition - new Vector2(0f, height);
			rectTransform.anchoredPosition = currentPosition;
		}
	}
}
