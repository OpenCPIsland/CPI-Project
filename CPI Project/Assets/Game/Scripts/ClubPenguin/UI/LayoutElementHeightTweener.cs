using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(LayoutElement))]
	public class LayoutElementHeightTweener : OpenCloseTweener
	{
		public RectTransform Content;

		private LayoutElement layoutElement;

		private void Start()
		{
			layoutElement = GetComponent<LayoutElement>();
			if (!isInit)
			{
				float preferredHeight = layoutElement.preferredHeight;
				float closedPosition = 0f;
				Init(preferredHeight, closedPosition);
			}
		}

		protected override void setPosition(float value)
		{
			layoutElement.preferredHeight = value;
			if (Content != null)
			{
				Vector2 sizeDelta = Content.sizeDelta;
				sizeDelta.y = openPosition - value;
				Content.sizeDelta = sizeDelta;
			}
		}
	}
}
