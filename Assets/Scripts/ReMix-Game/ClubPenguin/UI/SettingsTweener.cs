using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SettingsTweener : OpenCloseTweener
	{
		private RectTransform rect;

		private void Start()
		{
			rect = GetComponent<RectTransform>();
			adjustContentSize();
			CalculateTweenPosition();
		}

		public void CalculateTweenPosition()
		{
			CoroutineRunner.Start(initTweener(), this, "initTweener");
		}

		private void adjustContentSize()
		{
			rect.anchorMax = new Vector2(2f, 1f);
			rect.offsetMax = Vector2.zero;
		}

		private IEnumerator initTweener()
		{
			yield return new WaitForEndOfFrame();
			float openPosition = rect.rect.width / -2f;
			float closedPosition = 0f;
			Init(openPosition, closedPosition);
		}

		protected override void setPosition(float value)
		{
			Vector2 anchoredPosition = rect.anchoredPosition;
			anchoredPosition.x = value;
			rect.anchoredPosition = anchoredPosition;
		}
	}
}
