using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class ContentScroller : MonoBehaviour
	{
		public RectTransform Content;

		public float PixelsPerSecond;

		private bool isScrolling;

		public float NormalizedPosition
		{
			get
			{
				return Content.anchoredPosition.y / Content.rect.height;
			}
		}

		public event Action ScrollComplete;

		public event Action<float> PositionUpdated;

		private void OnEnable()
		{
			isScrolling = true;
		}

		private void Update()
		{
			if (isScrolling)
			{
				Vector2 anchoredPosition = Content.anchoredPosition;
				anchoredPosition.y += PixelsPerSecond * Time.deltaTime;
				if (anchoredPosition.y > Content.rect.height)
				{
					isScrolling = false;
					anchoredPosition.y = Content.rect.height;
				}
				Content.anchoredPosition = anchoredPosition;
				this.PositionUpdated.InvokeSafe(NormalizedPosition);
				if (!isScrolling)
				{
					this.ScrollComplete.InvokeSafe();
				}
			}
		}
	}
}
