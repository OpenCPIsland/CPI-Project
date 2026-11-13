using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class VerticalPooledScrollRect : AbstractPooledScrollRect
	{
		protected const int POOL_SIZE_PADDING_FACTOR = 6;

		private VerticalLayoutGroup verticalLayoutGroup;

		private float preferredHeight;

		protected override float rectSize
		{
			get
			{
				return base.rectTransform.rect.height;
			}
		}

		protected override float position
		{
			get
			{
				return scrollRect.content.anchoredPosition.y;
			}
		}

		protected override float sizeDelta
		{
			get
			{
				return scrollRect.content.sizeDelta.y;
			}
		}

		protected override void awake()
		{
			verticalLayoutGroup = scrollRect.content.GetComponent<VerticalLayoutGroup>();
			if (verticalLayoutGroup == null)
			{
				Log.LogError(this, "This script requires ScrollRect Content to have a VerticalLayoutGroup component.");
			}
		}

		protected override void setUpEmptyCell(GameObject cell)
		{
			LayoutElement layoutElement = cell.AddComponent<LayoutElement>();
			layoutElement.preferredHeight = preferredHeight;
		}

		protected override int getPoolSize(GameObject item)
		{
			preferredHeight = LayoutUtility.GetPreferredHeight(item.transform as RectTransform);
			float num = preferredHeight + verticalLayoutGroup.spacing;
			int num2 = (int)(base.rectTransform.rect.height / num) * 6;
			if (num2 < 1)
			{
				Log.LogErrorFormatted(this, "Calculated negative pool size ({0})\nPreferred Height = {1}\nElement Height = {2}\nRect. Height = {3}", num2, preferredHeight, num, base.rectTransform.rect.height);
				num2 = 1;
			}
			if (num2 > 1000)
			{
				Log.LogErrorFormatted(this, "Calculated excessive pool size ({0})\nPreferred Height = {1}\nElement Height = {2}\nRect. Height = {3}", num2, preferredHeight, num, base.rectTransform.rect.height);
				num2 = 1000;
			}
			return num2;
		}

		protected override int getScrollPosition(Vector2 position)
		{
			return (int)(position.y * 100f);
		}

		protected override float getPosition(RectTransform item)
		{
			return item.localPosition.y + scrollRect.content.localPosition.y;
		}
	}
}
