using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class HorizontalOpenCloseTweener : OpenCloseTweener
	{
		public bool StartsOpen;

		private RectTransform Content;

		private void Start()
		{
			Content = (base.transform as RectTransform);
			Content.gameObject.SetActive(false);
			CoroutineRunner.Start(waitForLayout(), this, "waitForLayout");
		}

		private IEnumerator waitForLayout()
		{
			while (Math.Abs(Content.rect.width) < float.Epsilon)
			{
				yield return null;
			}
			float openPosition = 0f;
			float closedPosition = 0f - Content.rect.width;
			Init(openPosition, closedPosition);
			if (StartsOpen)
			{
				SetOpen();
			}
			else
			{
				SetClosed();
			}
			Content.gameObject.SetActive(true);
		}

		protected override void setPosition(float value)
		{
			Vector2 anchoredPosition = Content.anchoredPosition;
			anchoredPosition.x = value;
			Content.anchoredPosition = anchoredPosition;
		}
	}
}
