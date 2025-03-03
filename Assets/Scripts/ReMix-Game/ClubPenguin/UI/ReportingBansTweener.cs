using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ReportingBansTweener : OpenCloseTweener
	{
		public RectTransform Content;

		private void Start()
		{
			CoroutineRunner.Start(waitForLayout(), this, "waitForLayout");
		}

		private IEnumerator waitForLayout()
		{
			while (Math.Abs(((RectTransform)base.transform).sizeDelta.x) < 0.05f)
			{
				yield return null;
			}
			float openPosition = 0f - ((RectTransform)base.transform).sizeDelta.x;
			float closedPosition = 0f;
			Init(openPosition, closedPosition);
		}

		protected override void setPosition(float value)
		{
			Vector2 anchoredPosition = Content.anchoredPosition;
			anchoredPosition.x = value;
			Content.anchoredPosition = anchoredPosition;
		}
	}
}
